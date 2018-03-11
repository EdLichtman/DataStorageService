using System;
using System.IO;
using System.Collections.Generic;
using DataStorageService.AppSettings;
using Microsoft.Data.Sqlite;


namespace DataStorageService.Endpoints.DataStorage.DatabaseInterfaces
{
    public class SqliteImportedDataPointRepository : IImportedDataPointRepository
    {
        private readonly IApplicationSettings _applicationSettings;
        private const string table = "datapoints";
        private const string column1 = "timestamp";
        private const string column2 = "raw_intensity";

        public SqliteImportedDataPointRepository(IApplicationSettings applicationSettings)
        {
            _applicationSettings = applicationSettings;

        }

        public IList<ImportedDataPoint> ReadFromDatabase(string fileName)
        {
            var fileLocation = Path.Combine(_applicationSettings.SqliteStorageFolderLocation,fileName);
            var sqliteConnectionString = new SqliteConnectionStringBuilder { DataSource = fileLocation };

            var importedData = new List<ImportedDataPoint>();
            using (var connection = new SqliteConnection($"{sqliteConnectionString}"))
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {

                    var insertCommand = connection.CreateCommand();
                    insertCommand.Transaction = transaction;
                    insertCommand.CommandText = SelectCommand;

                    using (var dataReader = insertCommand.ExecuteReader()) {
                        while (dataReader.Read()) {
                            importedData.Add(new ImportedDataPoint
                            {
                                TimeStamp = Convert.ToDateTime(dataReader[column1].ToString()),
                                RawIntensity = Convert.ToInt32(dataReader[column2].ToString())
                            });
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
            var sqliteConnectionString = new SqliteConnectionStringBuilder { DataSource = fileLocation };

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
                        insertCommand.Parameters.AddWithValue($"${column1}{i}", dataPoint.TimeStamp.ToString());
                        insertCommand.Parameters.AddWithValue($"${column2}{i}", dataPoint.RawIntensity);
                    }
                    insertCommand.ExecuteNonQuery();
                    transaction.Commit();
                }
                connection.Close();
            }
            return true;
        }

        private void CreateDatabase(SqliteConnectionStringBuilder sqliteConnectionString) {
            using (var connection = new SqliteConnection($"{sqliteConnectionString}")) {
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
