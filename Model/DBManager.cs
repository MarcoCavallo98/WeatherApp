using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.IO;
using System.Text.Json;
using System.Diagnostics;
using System.Data.Common;

namespace WeatherApp
{
    class DBManager
    {

        private string PATH = "./Resource/WeatherMapConfig.json";

        private string DBPath;

        private SQLiteConnection conn;

        private static DBManager manager;

        private IList<FavPlaces> _locations;


        public IList<FavPlaces> Locations 
        {
            get
            {
                if (_locations == null)
                {
                    _locations = new List<FavPlaces>();
                }
                return _locations;
            }
        }

        private DBManager() { }


        public async Task CreateDB()
        {
            if (DBPath == null)
                await ApplyConfig();

            if (!File.Exists(DBPath))
            {
                try
                {
                    SQLiteConnection.CreateFile(DBPath);
                    await DBConnect();
                    SQLiteCommand command = conn.CreateCommand();
                    command.CommandText = @"
                                            CREATE TABLE Cities(
                                                id INTEGER PRIMARY KEY AUTOINCREMENT,
                                                name VARCHAR(50)
                                            );
                                       ";
                    await command.ExecuteNonQueryAsync();

                    string[] cities = { "London", "Rome", "Berlin", "Paris" };

                    foreach (string c in cities)
                    {
                        command.CommandText = $"INSERT INTO Cities (name) VALUES ('{c}');";
                        await command.ExecuteNonQueryAsync();
                    }
                }
                catch (Exception e)
                {
                    throw;
                }
            }
        }

        public async Task DBConnect()
        {
            if (DBPath == null)
                await ApplyConfig();

            if (conn == null)
            {
                try
                {
                    conn = new SQLiteConnection($"Data Source={DBPath};");
                    conn.Open();
                }
                catch (Exception e)
                {
                    throw;
                }
            }
        }

        private async Task ApplyConfig()
        {
            FileStream fs = File.OpenRead(PATH);
            JsonElement data = await JsonSerializer.DeserializeAsync<JsonElement>(fs);
            string name = data.GetProperty("DBName").ToString().Trim();
            string path = data.GetProperty("DBPath").ToString().Trim();
            DBPath = $"./{path}/{name}.db";
        }

        public static DBManager getIstance()
        {
            if (manager == null)
                manager = new DBManager();

            return manager;
        }

        public async Task LoadFavPlaces() 
        {
            SQLiteCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM Cities;";
            DbDataReader reader = await cmd.ExecuteReaderAsync();

            while (reader.Read()) 
            {
                FavPlaces to_add = new FavPlaces { ID = reader.GetInt32(0), Name = reader.GetString(1) };
                Locations.Add(to_add);
            }

        }

    }
}
