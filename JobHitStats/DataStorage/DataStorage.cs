using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;

namespace JobHitStats
{
    using JobHitStats.Common;

    class DataStorage : IDataStorage
    {
        private const string DefaultFileName = "JobHitStats.sqlite";

        private const string ConnectionString = @"Data Source=JobHitStats.sqlite;Version=3;UseUTF16Encoding=True;";

        private readonly SQLiteConnection _dbConnection;

        private IEnumerable<Technology> _technologyValues;

        public DataStorage()
        {
            _dbConnection = new SQLiteConnection(ConnectionString);

            _technologyValues = Enum.GetValues(typeof(Technology)).Cast<Technology>();
        }

        public void DeleteRow(JobPortal portal, int id)
        {
            string tableName = portal.ToString();
            string sql = string.Format("DELETE FROM {0} WHERE id={1}", tableName, id);

            _dbConnection.Open();

            SQLiteCommand command = new SQLiteCommand(sql, _dbConnection);
            command.ExecuteNonQuery();

            _dbConnection.Close();
        }

        public void WriteTableContentToConsole(JobPortal table)
        {
            string sql = string.Format("SELECT * FROM {0}", table);

            _dbConnection.Open();

            SQLiteCommand command = new SQLiteCommand(sql, _dbConnection);
            SQLiteDataReader reader = command.ExecuteReader();

            Console.WriteLine("TABLE: {0}", table);
            while (reader.Read())
            {
                Console.WriteLine("Id: {0}", reader["Id"]);
                Console.WriteLine("DateTime: {0}", reader["DateTime"]);

                foreach (Technology value in _technologyValues)
                {
                    string technology = value.ToString();
                    Console.WriteLine("{0}: {1}", technology, reader[technology]);
                }
                Console.WriteLine();
            }

            _dbConnection.Close();
        }

        // TODO: try catch
        public void StoreData(JobPortal portal, IDictionary<Technology, uint> jobOffers)
        {
            Logger.LogStart(Logger.MethodName);

            if (jobOffers == null || jobOffers.Count == 0)
            {
                Logger.LogWarning("No job offers were passed to method.");
                Logger.LogStop(Logger.MethodName);
                
                return;
            }

            if (!TableExists(portal))
            {
                Logger.LogInfo(string.Format("Tabel '{0}' does not exist, yet.", portal));
                CreateTable(portal);
            }

            string columns = CreateColumnsForInsert();
            string values = CreateColumnValues(jobOffers);

            string sql = string.Format("INSERT INTO {0} ({1}) VALUES({2})", portal, columns, values);

            _dbConnection.Open();

            SQLiteCommand command = new SQLiteCommand(sql, _dbConnection);
            command.ExecuteNonQuery();

            Logger.LogInfo(string.Format("Data stored into '{0}'.", portal));

            _dbConnection.Close();

            Logger.LogStop(Logger.MethodName);
        }

        private string CreateColumnsForInsert()
        {
            StringBuilder columnsToInsert = new StringBuilder("DateTime, ");
            foreach (Technology value in _technologyValues)
            {
                columnsToInsert.Append(value + ", ");
            }

            string columns = columnsToInsert.ToString().Trim();
            return columns.Substring(0, columns.Length - 1); // removes last colon
        }

        private string CreateColumnValues(IDictionary<Technology, uint> jobOffers)
        {
            string dateTimeNow = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.FFF");

            StringBuilder columnValues = new StringBuilder("'" + dateTimeNow + "', ");
            foreach (Technology value in _technologyValues)
            {
                if (jobOffers.ContainsKey(value))
                {
                    columnValues.Append(jobOffers[value] + ", ");
                }
            }

            string values = columnValues.ToString().Trim();
            return values.Substring(0, values.Length - 1); // removes last colon
        }

        // TODO: try catch
        private bool TableExists(JobPortal table)
        {
            string tableName = table.ToString();
            string sql = string.Format(
                "SELECT count(*) AS count FROM sqlite_master WHERE type='table' AND name='{0}'", tableName);

            _dbConnection.Open();

            SQLiteCommand command = new SQLiteCommand(sql, _dbConnection);
            SQLiteDataReader reader = command.ExecuteReader();

            long count = 0;
            while (reader.Read())
            {
                count = (long)reader["count"];
            }

            _dbConnection.Close();

            return count == 1;
        }

        // TODO: try catch
        private void CreateTable(JobPortal table)
        {
            Logger.LogStart(Logger.MethodName);

            string tableName = table.ToString();

            string columnsDefinition = CreateColumnsDefinition();

            string sql = string.Format(
                "CREATE TABLE {0} (" +
                    "Id INTEGER PRIMARY KEY NOT NULL, " +
                    "DateTime TEXT NOT NULL, " + 
                    "{1})", tableName, columnsDefinition);

            _dbConnection.Open();

            SQLiteCommand command = new SQLiteCommand(sql, _dbConnection);
            
            command.ExecuteNonQuery();

            Logger.LogInfo(string.Format("Tabel '{0}' created.", table));

            _dbConnection.Close();

            Logger.LogStop(Logger.MethodName);
        }

        private string CreateColumnsDefinition()
        {
            StringBuilder columnsDefinition = new StringBuilder();
            foreach (Technology value in _technologyValues)
            {
                columnsDefinition.Append(value + " INTEGER DEFAULT NULL, ");
            }

            string definition = columnsDefinition.ToString().Trim();

            return definition.Substring(0, definition.Length - 1); // removes last colon
        }
    }
}
