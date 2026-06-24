using System;
using System.IO;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace Analiser
    {
    public enum reportType
        {
            Intel,
            Recon,
            Analyze,
            Collect
        }
    public enum Status
        {
        Rejected,
        Approved,
        Pending
        }
    class ReportAnaliser
        {
        static string[]? ReadTxtFile(string path )
        {
             
            if (!File.Exists(path)) { return null; }

            string[] fileData = File.ReadAllLines(path, Encoding.UTF8);
            return fileData;
        }
        static void ProcessReports(string[] stringDataArr, string[] unitName, string[] reportType, double[] score, string[] status)
        {
        
        }

        static void CalculateAverage() { }

        static void FindMaxScore() { }

        static void FindMinScore() { }

        static void CountByStatus() { }

        static void CountByType() { }

        static void DisplayBasicStatistics() { }

        static void DisplayStatusCounts() { }

        static void DisplayTypeCounts() { }
        static void DisplayHighestPriorityApproved() { }
        static void DisplayAverageByPriority() { }

        static void Main()
            {
            string filePath = "txt.reports";
            string[]? fileData = ReadTxtFile(filePath);
            if (fileData != null)
            {
                string[] unitName = new string[fileData.Length];
                string[] reportType = new string[fileData.Length];
                int[] priority = new int[fileData.Length];
                double[] score = new double[fileData.Length];
                string[] status = new string[fileData.Length];
            }
            }   
        }
    }