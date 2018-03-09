using System;
namespace DataStorageService.Helpers
{
    public static class ImportedDatabaseHelpers
    {
        public static string GetSqliteAssociatedMetadataFileName(this string databaseName)
        {
            return $"{databaseName.Replace(".", "_")}_metadata.json";
        }
    }
}
