using System;
using System.Collections.Generic;
//using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace Text2Txt
{
    //public  class Database
    //{


    //    // Connection string for the SQLite database
    //    public static readonly string connectionString = "Data Source=mydatabase.db;Version=3;";

    //    public static bool CheckDatabaseExists()
    //    {
    //        bool exists = false;

    //        using (var connection = new SQLiteConnection(connectionString))
    //        {
    //            try
    //            {
    //                connection.Open();
    //                exists = true;
    //            }
    //            catch (SQLiteException)
    //            {
    //                exists = false;
    //            }
    //        }

    //        return exists;
    //    }
    //    // Check if the user table exists
    //    bool userTableExists = false; 


    //    public static void CreateDatabase()
    //    {
    //        using (var connection = new SQLiteConnection(connectionString))
    //        {
    //            connection.Open();

    //            var command = new SQLiteCommand("CREATE TABLE IF NOT EXISTS users (id INTEGER PRIMARY KEY, apiKey TEXT)", connection);
    //            command.ExecuteNonQuery();
    //        }
    //    }
    //    public static void InsertApiKey(string apiKey)
    //    {
    //        using (var connection = new SQLiteConnection(connectionString))
    //        {
    //            connection.Open();
    //            var command = new SQLiteCommand("INSERT INTO users (apiKey) VALUES (@apiKey)", connection);
    //            command.Parameters.AddWithValue("@apiKey", apiKey);
    //            command.ExecuteNonQuery();
    //        }
    //    }
    //    public static string GetApiKey() {
    //        using (var connection = new SQLiteConnection(connectionString))
    //        {
    //            connection.Open();

    //            var command = new SQLiteCommand("SELECT api_key FROM user WHERE id=@id", connection);
    //            command.Parameters.AddWithValue("@id", 1); // Replace 1 with the actual id you want to retrieve the API key for
    //            var reader = command.ExecuteReader();

    //            if (reader.Read())
    //            {
    //                return reader.GetString(0);
    //            }
    //            else
    //            {
    //                return "Place API Key Here";
    //            }
    //        }
    //    }
    //    public static void UpdateApiKey(string apiKey)
    //    {
    //        using (var connection = new SQLiteConnection(connectionString))
    //        {
    //            connection.Open();

    //            var command = new SQLiteCommand("UPDATE user SET api_key=@apiKey WHERE id=1", connection);
    //            command.Parameters.AddWithValue("@apiKey", apiKey);
    //            command.ExecuteNonQuery();
    //        }
    //    }
    //}
}


