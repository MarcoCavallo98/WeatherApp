using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.IO;
using System.Text.Json;
using System.Diagnostics;

namespace WeatherApp.Model
{
    class DBManager
    {

        private string PATH = "./Resource/WeatherMapConfig.json";

        private string DBPath;

        private SQLiteConnection conn;

        private static DBManager manager;

        private DBManager() { }


        public async void CreateDB() 
        {
            if (DBPath == null)
                await ApplyConfig();

            if (!File.Exists(DBPath))
            {
                try
                {
                    SQLiteConnection.CreateFile(DBPath);
                    DBConnect();
                    SQLiteCommand command = conn.CreateCommand();
                    command.CommandText = @"
                                            CREATE TABLE Cities(
                                                id INTEGER PRIMARY KEY AUTOINCREMENT,
                                                name VARCHAR(50)
                                            );
                                       ";
                    await command.ExecuteNonQueryAsync();
                }
                catch (Exception e) 
                {
                    throw;
                }
            }
        }

        public async void DBConnect() 
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

    }
}
