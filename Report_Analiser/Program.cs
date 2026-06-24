using System;
using System.IO;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace Analiser
    {
    public enum ReportType
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
        static void ReadTxtFile(string path, string[] fileData)
        {
             
            if (!File.Exists(path))
            {
                Console.WriteLine($"Error file {Path.GetFileName(path)} not found");
            }

            string[] fileDataRead = File.ReadAllLines(path, Encoding.UTF8);
            if (fileDataRead.Length==0) 
            {
                Console.WriteLine($"Error file {Path.GetFileName(path)} is empty");
            }
            for(int i =0; i > fileDataRead.Length; i++)
            {
                if (i >= 100) break;//if file bigger then 100 lines

                fileData[i] = fileDataRead[i];
            }

            
        }
        static int ProcessReports(string[] stringDataArr, string[] unitName, string[] reportType, int[] priority, double[] score, string[] status)
        {   
            Array[] splitDataArr = new Array[100];
            string[] splitlineStr = new string[5];
            int linecount = 0;

            for (int i = 0 ; i < stringDataArr.Length; i++)//splits every line at ','
            {
                if (stringDataArr[i] == "") // checks that line is full
                {
                    continue; 
                }
                splitlineStr = stringDataArr[i].Split(",");
                for (int j = 0; j < 5; j++)//strips eny extras
                {
                    splitlineStr[j] = splitlineStr[j].Trim();
                }
                splitDataArr[i] = splitlineStr;//adds clean lines to arry 
            }

            FilterNotValid(splitDataArr);

            foreach (string[] line in splitDataArr)
            {
                if (line[0] =="") { continue; }

                unitName[linecount] = line[0];
                reportType[linecount] = line[1];
                priority[linecount] = int.Parse(line[2]);
                score[linecount] = double.Parse(line[3]);
                status[linecount] = line[4];
                linecount++;

            }
            return linecount;

        }

        static void FilterNotValid(Array[] splitDataArr) 
        {
            // checks all if have valid fealds else clears all
            foreach (string[] line in splitDataArr)
            {
                bool flag = false;
                flag = Enum.TryParse(line[1], true, out ReportType report);

                flag = int.TryParse(line[2], out int num);
                if (num > 5 | num < 0) { flag = true; }

                flag = double.TryParse(line[3], out double score);
                if (score > 100 | score < 0) { flag = true; }

                flag = Enum.TryParse(line[4], true, out Status stat);

                if (flag) { line[0] = ""; line[1] = ""; line[2] = ""; line[3] = ""; line[4] = ""; } 
            }
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
            string[] fileData = new string[100];
            int count;

            string[] unitName = new string[fileData.Length];
            string[] reportType = new string[fileData.Length];
            int[] priority = new int[fileData.Length];
            double[] score = new double[fileData.Length];
            string[] status = new string[fileData.Length];

            ReadTxtFile(filePath, fileData);
            count = ProcessReports(fileData, unitName, reportType, priority, score, status);






            }   
        }
    }