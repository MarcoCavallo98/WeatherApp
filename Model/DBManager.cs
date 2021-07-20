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
using System.ComponentModel;

namespace WeatherApp
{
    class DBManager 
    {
        #region fileds

        private string PATH = "./Resource/WeatherMapConfig.json";

        private string DBPath;

        private SQLiteConnection conn;

        private static DBManager manager;

        private IList<FavPlaces> _locations;

        private event EventHandler<FavPlaces> _locationRemoved;
        private event EventHandler<FavPlaces> _locationAdded;

        #endregion

        #region properties

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

        public event EventHandler<FavPlaces> LocationRemoved 
        { 
            add => _locationRemoved += value;

            remove => _locationRemoved -= value;
        }

        public event EventHandler<FavPlaces> LocationAdded
        {
            add => _locationAdded+= value;

            remove => _locationAdded -= value;
        }

        #endregion

        #region methods

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
                        command.CommandText = $"INSERT INTO Cities (name) VALUES (@city);";
                        command.Parameters.Add(new SQLiteParameter("@city", c));
                        await command.ExecuteNonQueryAsync();
                    }
                }
                catch (Exception)
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
                catch (Exception)
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
            try
            {
                SQLiteCommand cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT * FROM Cities;";
                DbDataReader reader = await cmd.ExecuteReaderAsync();

                while (reader.Read())
                {
                    FavPlaces to_add = new FavPlaces { Name = reader.GetString(1) };
                    Locations.Add(to_add);
                }
            }
            catch (Exception) 
            {
                throw;
            }

        }

        public async Task AddFavPlaces(string cityName)
        {
            try
            {
                SQLiteCommand command = conn.CreateCommand();
                command.CommandText = $"INSERT INTO Cities (name) VALUES (@cityName)";
                command.Parameters.Add(new SQLiteParameter("@cityName", cityName));
                int ins = await command.ExecuteNonQueryAsync();
                if (ins > 0) 
                {
                    FavPlaces toAdd = new FavPlaces { Name = cityName };
                    Locations.Add(toAdd);
                    OnLocationAdded(toAdd);
                }
            }
            catch (Exception) 
            {
                throw;
            }
        }

        public async Task RemoveFavPlaces(FavPlaces city)
        {
            try
            {
                SQLiteCommand command = conn.CreateCommand();
                command.CommandText = $"DELETE FROM Cities where name = @cityName";
                command.Parameters.Add(new SQLiteParameter("@cityName", city.Name));
                int ins = await command.ExecuteNonQueryAsync();
                if (ins > 0)
                {
                    Locations.Remove(city);
                    OnLocationRemoved(city);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void OnLocationRemoved(FavPlaces location) 
        {
            EventHandler<FavPlaces> copy = _locationRemoved;
            if (copy != null)
                _locationRemoved(this, location);
        }

        public void OnLocationAdded(FavPlaces location)
        {
            EventHandler<FavPlaces> copy = _locationAdded;
            if (copy != null)
                _locationAdded(this, location);
        }

        #endregion

    }
}
