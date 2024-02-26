using System;
using System.IO;
using Microsoft.Data.Sqlite;
using System.Diagnostics;
using System.Collections.Generic;

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

        public bool QRCodeExists(string value) 
        {
            using (var connection = new SqliteConnection("Data Source=QrCodeDatabase.db")) 
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = $"SELECT COUNT(*) FROM QrCodes WHERE file_path = '{value}'";

                // ExecuteScalar returns the count of rows that match the condition
                int rowCount = Convert.ToInt32(command.ExecuteScalar());
                if (rowCount > 0)
                {
                    return true;
                }
                else 
                {
                    return false;
                }
            }
        }

        public void DeleteQRCode(string value) 
        {
            using (var connection = new SqliteConnection("Data Source=QrCodeDatabase.db")) 
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = $"DELETE FROM QrCodes WHERE file_path = '{value}'";
                command.ExecuteNonQuery();
            }
        }

        public void DeleteQRCodes(List<string> values)
        {
            using (var connection = new SqliteConnection("Data Source=QrCodeDatabase.db"))
            {
                connection.Open();
                foreach (string value in values) 
                {
                    var command = connection.CreateCommand();
                    command.CommandText = $"DELETE FROM QrCodes WHERE file_path = '{value}'";
                    command.ExecuteNonQuery();
                }
            }
        }

        public void DeleteAllQRCodes() 
        {
            using (var connection = new SqliteConnection("Data Source=QrCodeDatabase.db"))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "DELETE FROM QrCodes";
                command.ExecuteNonQuery();
            }
        }

        //public SqliteDataReader QRCodeDataReader() 
        //{
        //    using (var connection = new SqliteConnection($"Data Source=QrCodeDatabase.db"))
        //    {
        //        connection.Open();

        //        var selectCommand = connection.CreateCommand();
        //        selectCommand.CommandText = $"SELECT * FROM QrCodes";

        //        return selectCommand.ExecuteReader();
        //    }
        //}

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
