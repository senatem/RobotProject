using System;
using System.IO;
namespace RobotProject
{
    public static class ReportLogger
    {
        static string docPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Vardiya Raporları");

        public static void Init()
        {
            if (!Directory.Exists(docPath))
            {
                Directory.CreateDirectory(docPath);
            }
        }

        public static void CreateTodaysDir()
        {
            var date = DateTime.Now.ToString("dd/MM/yyyy dddd");
            var path = Path.Combine(docPath, date);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
        
        public static void AddRecord(int yontem, int tip, int yukseklik, int uzunluk)
        {
            
        }

        public static void OpenReport()
        {
            
        }

        public static void CloseReport()
        {
            
        }
    }
}