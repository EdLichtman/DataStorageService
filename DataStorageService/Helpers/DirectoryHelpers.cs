using System;
using DataStorageService.AppSettings;
namespace DataStorageService.Helpers
{
    public static class DirectoryHelpers
    {
        

        public static string GetSqliteStorageRoot() {
            var appSettings = new ApplicationSettings();
            var sqliteStorageFolderLocation = appSettings.SqliteStorageFolderName;
            return $"{GetDataStorageServiceProjectRoot()}/{sqliteStorageFolderLocation}";
        }

        public static string GetDataStorageServiceProjectRoot() {
            
            return $"{GetSolutionRoot()}/DataStorageService";
        }
        public static string GetSolutionRoot() {
            const string SolutionName = "/DataStorageServiceTests";

            var currentFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            return currentFolder
                .Remove(currentFolder.LastIndexOf(SolutionName));

        } 
    }
}
