using System;
using System.IO;
using DataStorageService.AppSettings;
namespace DataStorageService.Helpers
{
    public static class DirectoryHelpers
    {
        

        public static string GetSqliteStorageRoot() {
            var appSettings = new ApplicationSettings();
            var sqliteStorageFolderLocation = appSettings.SqliteStorageFolderName;
            
            return Path.Combine(GetDataStorageServiceProjectRoot(), sqliteStorageFolderLocation);
        }

        public static string GetDataStorageServiceProjectRoot() {

            return Path.Combine(GetSolutionRoot(), "DataStorageService");
        }
        public static string GetSolutionRoot() {
            var slashCharacter = OperatingSystemHelpers.SystemSlash;
            string SolutionName = $"{slashCharacter}DataStorageService";

            var currentFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            return currentFolder
                .Remove(currentFolder.LastIndexOf(SolutionName));
        } 

    }
}
