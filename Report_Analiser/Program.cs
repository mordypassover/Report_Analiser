using System;
using System.IO;
using System.Net.NetworkInformation;
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
        static int LoadFile(string path, string[] fileData, string[] unitName, string[] reportType, int[] priority, double[] score, string[] status)
        {
             
            if (!File.Exists(path))
            {
                Console.WriteLine($"Error file {Path.GetFileName(path)} not found");
                return 0;
            }

            string[] fileDataRead = File.ReadAllLines(path, Encoding.UTF8);
            if (fileDataRead.Length==0) 
            {
                Console.WriteLine($"Error file {Path.GetFileName(path)} is empty");
                return 0;
            }
            Console.WriteLine($"File loaded: {fileDataRead.Length} lines found.");
            for(int i =0; i < fileDataRead.Length; i++)
            {
                if (i >= 100) break;//if file bigger then 100 lines

                fileData[i] = fileDataRead[i];
            }
            
            int count = ProcessReports(fileData, unitName, reportType, priority, score, status);
            Console.WriteLine($"Processing complete.\n" +
                              $"Valid records: {count}\n" +
                              $"Invalid records: {fileDataRead.Length - count}");
            return count;

        }
        static int ProcessReports(string[] stringDataArr, string[] unitName, string[] reportType, int[] priority, double[] score, string[] status)
        {   
            Array[] splitDataArr = new Array[100];
            int linecount = 0;

            for (int i = 0 ; i < stringDataArr.Length; i++)//splits every line at ','
            {
                string[] splitLineStr = new string[5];
                if ( stringDataArr[i] == null) // checks that line is full
                {
                    stringDataArr[i] = " , ";
                    continue; 
                }
                
                splitLineStr = stringDataArr[i].Split(",");
                for (int j = 0; j < 5; j++)//strips eny extras
                {
                    splitLineStr[j] = splitLineStr[j].Trim();
                }
                splitDataArr[i] = splitLineStr;//adds clean lines to arry 
            }

            FilterNotValid(splitDataArr);

            foreach (string[] line in splitDataArr)
            {
                if (line==null || line[0] =="")
                { 
                    continue;
                }
                unitName[linecount] = line[0];
                reportType[linecount] = line[1];
                priority[linecount] = int.Parse(line[2]);
                score[linecount] = double.Parse(line[3]);
                status[linecount] = line[4];
                linecount++;
                Console.WriteLine("Valid record processed.");
            }
            Console.WriteLine($"Stored {linecount} valid records for analysis.");
            return linecount;
        }

        static void FilterNotValid(Array[] splitDataArr) 
        {
            // checks all if have valid fealds else clears all
            foreach (string[] line in splitDataArr)
            {
                if ((line == null ) || line.Length != 5 ) 
                {
                    continue;
                }
                bool flag = true;
                if (!Enum.TryParse(line[1], true, out ReportType report)) 
                {
                    flag = false;
                    Console.WriteLine("Report Type not valid");
                }

                if (!int.TryParse(line[2], out int num) || !(num < 6 && num > 0) )
                {
                    flag = false;
                    Console.WriteLine("priority not valid");
                }

                if (!double.TryParse(line[3], out double score) || !(score <= 100 && score >= 0))
                {
                    flag = false;
                    Console.WriteLine("score not valid");
                }

                if (!Enum.TryParse(line[4], true, out Status stat))
                {
                    flag = false;
                    Console.WriteLine("Status not valid");
                }

                if (!flag) { line[0] = ""; line[1] = ""; line[2] = ""; line[3] = ""; line[4] = ""; } 
            }
        }

        static double CalculateAverage(double[] score,int numOfLines) 
        {
            double count = 0;
            if (numOfLines == 0) return 0;
            foreach (double num in score) { count = count + num; }
            return (double)count / numOfLines;
        }

        static double FindMaxScore(double[] score) 
        {
            double max = 0;
            foreach (double num in score) 
            {
                if (max < num) max = num;
            }
            return max;
        }

        static double FindMinScore(double[] score,int count)
        {
            double min = 100.0;
            foreach (double num in score)
            {
                if (min > num) min = num;
            }
            return min;
        }
        

        static int CountByStatus(string[] status, Status statusEnom, int count)
        {
            int cnt = 0;
            for (int i =0; i < count; i++) 
            {
                if (status[i].ToLower() == statusEnom.ToString().ToLower()) cnt += 1;
            }
            return cnt;
        }

        static int CountByType(string[] reportType, ReportType type, int count)
        {
            int cnt = 0;
            for (int i = 0; i < count; i++)
            {
                if (reportType[i].ToLower() == type.ToString().ToLower()) cnt += 1;
            }
            return cnt;
        }
        static void DisplayBasicStatistics(double[] score, int count) 
        {
            Console.WriteLine($"=== Report Statistics ===\n" +
                $"number of reports : {count}\n" +
                $"average : {CalculateAverage(score, count):F2}\n" +
                $"max score : {FindMaxScore(score)}\n" +
                $"min score : {FindMinScore(score, count)}");

        }

        static void DisplayStatusCounts(string[] status, int count)
        {
            Console.WriteLine($"=== Reports by Status ===\n" +
                $"Pending : {CountByStatus(status, Status.Pending,count)}\n" +
                $"Approved : {CountByStatus(status, Status.Approved, count)}\n" +
                $"Rejected : {CountByStatus(status, Status.Rejected, count)}");
        }

        static void DisplayTypeCounts(string[] reportType, int count) 
        {
            Console.WriteLine($"=== Reports by Type ===\n" +
                $"Collect : {CountByType(reportType,ReportType.Collect, count)}\n" +
                $"Analyze : {CountByType(reportType, ReportType.Analyze, count)}\n" +
                $"Recon : {CountByType(reportType, ReportType.Recon, count)}\n" +
                $"Intel : {CountByType(reportType, ReportType.Intel, count)}");
        }
        static void DisplayHighestPriorityApproved(string[] unitName, string[] reportType,int[] priority,double[] score,string[] status, int count) 
        {
            int highestApprovedPriorityIndex = 0;
            for (int i = 1; i < count; i++)
            {
                if (status[i].ToLower() == Status.Approved.ToString().ToLower()) 
                {
                    if (priority[highestApprovedPriorityIndex] < priority[i]) 
                    {
                        highestApprovedPriorityIndex = i; 
                    }
                }
            }
            Console.WriteLine($"=== Highest Priority Approved ===\n\n" +
                $"unit Name: {unitName[highestApprovedPriorityIndex]}, \n" +
                $"report Type : {reportType[highestApprovedPriorityIndex]}, \n" +
                $"priority : {priority[highestApprovedPriorityIndex]}, \n" +
                $"score : {score[highestApprovedPriorityIndex]}, \n" +
                $"status : {status[highestApprovedPriorityIndex]}, \n");
        }
        static void DisplayAverageByPriority(int[] priority, double[] score, int count)
        {
            double[] prioritysSums = new double[5];
            int[] prioritysCnts = new int[5];
            for (int i = 0; i < count; i++)
            {
                prioritysSums[priority[i] - 1] += score[i];
                prioritysCnts[priority[i] - 1]++;
            }
            for (int i = 0; i < prioritysCnts.Length; i++)
            {
                if (prioritysCnts[i] == 0) 
                {
                    Console.WriteLine($"priorety {i + 1} : no reports");
                    continue;
                }
                Console.WriteLine($"priorety {i + 1} : {(double)prioritysSums[i] / prioritysCnts[i]}"); 
            }
        }

        static void Main()
        {

            string filePath = "reports.txt";
            int count;

            string[] fileData = new string[100];
            string[] unitName = new string[100];
            string[] reportType = new string[100];
            int[] priority = new int[100];
            double[] score = new double[100];
            string[] status = new string[100];

            count = LoadFile(filePath, fileData, unitName, reportType, priority, score, status);
            
            if (count > 0)
            {
                DisplayBasicStatistics(score, count);
                DisplayStatusCounts(status, count);
                DisplayTypeCounts(reportType, count);
                DisplayHighestPriorityApproved(unitName, reportType, priority, score, status, count);
                DisplayAverageByPriority(priority, score, count);

                
            }
        }   
    }
}