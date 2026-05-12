using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using MAB_Spark.Models;

namespace MAB_Spark.Services
{
    public class DatabaseService
    {
        private readonly string _dbPath;
        private readonly string _connectionString;

        public DatabaseService()
        {
            var appDataPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "MAB_Spark");

            if (!Directory.Exists(appDataPath))
                Directory.CreateDirectory(appDataPath);

            _dbPath = Path.Combine(appDataPath, "shortcuts.db");
            _connectionString = $"Data Source={_dbPath};Version=3;";

            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            if (File.Exists(_dbPath))
                return;

            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        CREATE TABLE Shortcuts (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            ShortText TEXT NOT NULL UNIQUE,
                            ExpandedText TEXT NOT NULL,
                            IsEnabled BOOLEAN DEFAULT 1,
                            CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
                            UpdatedAt DATETIME DEFAULT CURRENT_TIMESTAMP
                        )";
                    command.ExecuteNonQuery();
                }
            }
        }

        public void AddShortcut(Shortcut shortcut)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        INSERT INTO Shortcuts (ShortText, ExpandedText, IsEnabled, CreatedAt, UpdatedAt)
                        VALUES (@short, @expanded, @enabled, @created, @updated)";

                    command.Parameters.AddWithValue("@short", shortcut.ShortText);
                    command.Parameters.AddWithValue("@expanded", shortcut.ExpandedText);
                    command.Parameters.AddWithValue("@enabled", shortcut.IsEnabled ? 1 : 0);
                    command.Parameters.AddWithValue("@created", shortcut.CreatedAt);
                    command.Parameters.AddWithValue("@updated", shortcut.UpdatedAt);

                    command.ExecuteNonQuery();
                }
            }
        }

        public List<Shortcut> GetAllShortcuts()
        {
            var shortcuts = new List<Shortcut>();

            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT Id, ShortText, ExpandedText, IsEnabled, CreatedAt, UpdatedAt FROM Shortcuts";
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            shortcuts.Add(new Shortcut
                            {
                                Id = Convert.ToInt32(reader[0]),
                                ShortText = reader[1].ToString() ?? "",
                                ExpandedText = reader[2].ToString() ?? "",
                                IsEnabled = Convert.ToBoolean(reader[3]),
                                CreatedAt = Convert.ToDateTime(reader[4]),
                                UpdatedAt = Convert.ToDateTime(reader[5])
                            });
                        }
                    }
                }
            }

            return shortcuts;
        }

        public Shortcut? GetShortcutByText(string shortText)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        SELECT Id, ShortText, ExpandedText, IsEnabled, CreatedAt, UpdatedAt 
                        FROM Shortcuts 
                        WHERE ShortText = @short AND IsEnabled = 1";

                    command.Parameters.AddWithValue("@short", shortText);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Shortcut
                            {
                                Id = Convert.ToInt32(reader[0]),
                                ShortText = reader[1].ToString() ?? "",
                                ExpandedText = reader[2].ToString() ?? "",
                                IsEnabled = Convert.ToBoolean(reader[3]),
                                CreatedAt = Convert.ToDateTime(reader[4]),
                                UpdatedAt = Convert.ToDateTime(reader[5])
                            };
                        }
                    }
                }
            }

            return null;
        }

        public void UpdateShortcut(Shortcut shortcut)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        UPDATE Shortcuts 
                        SET ExpandedText = @expanded, IsEnabled = @enabled, UpdatedAt = @updated
                        WHERE Id = @id";

                    command.Parameters.AddWithValue("@id", shortcut.Id);
                    command.Parameters.AddWithValue("@expanded", shortcut.ExpandedText);
                    command.Parameters.AddWithValue("@enabled", shortcut.IsEnabled ? 1 : 0);
                    command.Parameters.AddWithValue("@updated", DateTime.Now);

                    command.ExecuteNonQuery();
                }
            }
        }

        public void DeleteShortcut(int id)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "DELETE FROM Shortcuts WHERE Id = @id";
                    command.Parameters.AddWithValue("@id", id);
                    command.ExecuteNonQuery();
                }
            }
        }

        public string GetDatabasePath() => _dbPath;
    }
}
