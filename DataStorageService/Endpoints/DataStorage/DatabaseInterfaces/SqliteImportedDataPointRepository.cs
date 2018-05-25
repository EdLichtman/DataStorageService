using System;
using System.IO;
using System.Collections.Generic;
using DataStorageService.AppSettings;
using Microsoft.Data.Sqlite;
using DataStorageService.Endpoints.DataStorage.AggregateData;


namespace DataStorageService.Endpoints.DataStorage.DatabaseInterfaces
{
    public class SqliteImportedDataPointRepository : IImportedDataPointRepository
    {
        private readonly IApplicationSettings _applicationSettings;
        private const string table = "vibrations";
        private const string column1 = "timestamp_in_utc";
        private const string column2 = "bits";

        public SqliteImportedDataPointRepository(IApplicationSettings applicationSettings)
        {
            _applicationSettings = applicationSettings;

        }

        public IList<ImportedDataPoint> ReadFromDatabase(string folderLocation, string fileName)
        {
            var fileLocation = Path.Combine(folderLocation,fileName);
            var sqliteConnectionString = new SqliteConnectionStringBuilder
            {
                DataSource = fileLocation
            }.ToString();
            

            var importedData = new List<ImportedDataPoint>();
            using (var connection = new SqliteConnection($"{sqliteConnectionString}"))
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {

                    var insertCommand = connection.CreateCommand();
                    insertCommand.Transaction = transaction;
                    insertCommand.CommandText = SelectCommand;
                    try
                    {
                        using (var dataReader = insertCommand.ExecuteReader())
                        {
                            while (dataReader.Read())
                            {
                                importedData.Add(new ImportedDataPoint
                                {
                                    TimeStampInUtc = Convert.ToDateTime(dataReader[column1].ToString()),
                                    Bits = Convert.ToInt32(dataReader[column2].ToString())
                                });
                            }
                        }
                    }
                    catch (SqliteException e)
                    {
                        insertCommand.CommandText = $"SELECT * FROM {table}";
                        //might be legacy column names. If so, try again with legacy columns
                        using (var dataReader = insertCommand.ExecuteReader())
                        {
                            while (dataReader.Read())
                            {
                                importedData.Add(new ImportedDataPoint
                                {
                                    TimeStampInUtc = Convert.ToDateTime(dataReader["timestamp"].ToString()),
                                    Bits = Convert.ToInt32(dataReader["raw_intensity"].ToString())
                                });
                            }
                        }
                    }
                   

                    transaction.Commit();
                }
                connection.Close();
            }
            return importedData;
        }


        public bool WriteRangeToDatabase(string fileName, IList<ImportedDataPoint> dataPoints)
        {
            var fileLocation = Path.Combine(_applicationSettings.SqliteStorageFolderLocation,fileName);
            var sqliteConnectionString = new SqliteConnectionStringBuilder
            {
                DataSource = fileLocation
            }.ToString();

            Directory.CreateDirectory(_applicationSettings.SqliteStorageFolderLocation);
            CreateDatabase(sqliteConnectionString);
            using (var connection = new SqliteConnection($"{sqliteConnectionString}")) {
                connection.Open();

                using (var transaction = connection.BeginTransaction()) {
                    var insertCommand = connection.CreateCommand();
                    insertCommand.Transaction = transaction;
                    insertCommand.CommandText = "";
                    for (var i = 0; i < dataPoints.Count; i++) {
                        var dataPoint = dataPoints[i];
                        insertCommand.CommandText += GetInsertCommand(i);
                        insertCommand.Parameters.AddWithValue($"${column1}{i}", dataPoint.TimeStampInUtc.ToString());
                        insertCommand.Parameters.AddWithValue($"${column2}{i}", dataPoint.Bits);
                    }
                    insertCommand.ExecuteNonQuery();
                    transaction.Commit();
                }
                connection.Close();
            }
            return true;
        }

        private void CreateDatabase(string sqliteConnectionString) {
            using (var connection = new SqliteConnection(sqliteConnectionString)) {
                connection.Open();
                using (var transaction = connection.BeginTransaction()) {
                    var createCommand = connection.CreateCommand();
                    createCommand.Transaction = transaction;
                    createCommand.CommandText = CreateCommand;
                    createCommand.ExecuteNonQuery();
                    transaction.Commit();
                }
                connection.Close();
            }
        }

        private string GetInsertCommand(int i)
        {
            return $"INSERT INTO {table} ({column1},{column2}) VALUES (${column1}{i},${column2}{i});";
        } 
        private string CreateCommand => $"CREATE TABLE IF NOT EXISTS {table}({column1},{column2});";

        private string SelectCommand => $"SELECT {column1},{column2} FROM {table}";

    }
}
