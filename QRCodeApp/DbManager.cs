using System;
using System.IO;
using Microsoft.Data.Sqlite;
using System.Diagnostics;

namespace QRCodeApp
{
    class DbManager
    {
        public void InitializeDatabase()
        {
            string databasePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "QrCodeDatabase.db");

            using (var connection = new SqliteConnection($"Data Source={databasePath}"))
            {
                connection.Open();
            }
            CreateTable();
        }

        public void CreateTable()
        {
            using (var connection = new SqliteConnection("Data Source=QrCodeDatabase.db"))
            {
                connection.Open();

                var createTableCommand = connection.CreateCommand();
                createTableCommand.CommandText = "CREATE TABLE IF NOT EXISTS QrCodes (name TEXT, file_path TEXT UNIQUE, content TEXT, dt TEXT)";
                createTableCommand.ExecuteNonQuery();
            }
        }

        public void InsertQRCode(string name, string file_path, string content) 
        {
            using (var connection = new SqliteConnection($"Data Source=QrCodeDatabase.db"))
            {
                connection.Open();

                var insertCommand = connection.CreateCommand();
                insertCommand.CommandText = $"INSERT INTO QrCodes VALUES ('{name}', '{file_path}', '{content}', '{GetDt()}')";
                insertCommand.ExecuteNonQuery();
            }
        }

        public void PrintQRCodes() 
        {
            using (var connection = new SqliteConnection($"Data Source=QrCodeDatabase.db"))
            {
                connection.Open();

                var selectCommand = connection.CreateCommand();
                selectCommand.CommandText = $"SELECT * FROM QrCodes";

                using (var reader = selectCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Trace.WriteLine($"Name: {reader.GetString(0)}, Content: {reader.GetString(2)}, DateTime: {reader.GetString(3)}, FilePath: {reader.GetString(1)}\n");
                    }
                }
            }
        }

        public string GetDt()
        {
            // Get the current date and time
            DateTime currentDateTime = DateTime.Now;

            // Format the date and time as "hh:mm:ss YYYY/MM/DD"
            string formattedDateTime = currentDateTime.ToString("HH:mm:ss yyyy/MM/dd");

            return formattedDateTime;
        }

    }

}
