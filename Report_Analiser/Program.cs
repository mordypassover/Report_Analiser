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
             
            if (!File.Exists(path))
            {
                Console.WriteLine($"Error file {Path.GetFileName(path)} not found");
                return null;
            }

            string[] fileData = File.ReadAllLines(path, Encoding.UTF8);
            if (fileData.Length==0) 
            {
                Console.WriteLine($"Error file {Path.GetFileName(path)} is empty");
                return null;
            }
            return fileData;
        }
        static void ProcessReports(string[] stringDataArr, string[] unitName, string[] reportType, double[] score, string[] status)
        {
        }

        static void CalculateAverage() 
        {

        }

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
            string filePath = "reports.txt";
            string[]? fileData = ReadTxtFile(filePath);
            if (fileData != null | fileData.Length ==0)
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