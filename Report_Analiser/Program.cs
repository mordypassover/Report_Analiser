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
        static int LoadFile(string path, string[] fileData, string[] unitName, ReportType[] reportType, int[] priority, double[] score, Status[] status)
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
                if (i >= 100) break; //if file bigger then 100 lines 

                fileData[i] = fileDataRead[i];
            }
            
            int count = ProcessReports(fileData, unitName, reportType, priority, score, status);
            Console.WriteLine($"Processing complete.\n" +
                              $"Valid records: {count}\n" +
                              $"Invalid records: {fileDataRead.Length - count}");
            return count;

        }
        static int ProcessReports(string[] stringDataArr, string[] unitName, ReportType[] reportType, int[] priority, double[] score, Status[] status)
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

                if (splitLineStr.Length != 5) //makes line with les then 5 feiles drop at validation
                {
                    splitLineStr[0] ="";
                    continue; 
                }
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
                Enum.TryParse(line[1], true, out ReportType type);
                Enum.TryParse(line[4], true, out Status stat);

                unitName[linecount] = line[0];
                reportType[linecount] = type;
                priority[linecount] = int.Parse(line[2]);
                score[linecount] = double.Parse(line[3]);
                status[linecount] = stat;
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
                    Console.Write("Report Type not valid ");
                }

                if (!int.TryParse(line[2], out int num) || !(num < 6 && num > 0) )
                {
                    flag = false;
                    Console.Write("priority not valid ");
                }

                if (!double.TryParse(line[3], out double score) || !(score <= 100 && score >= 0))
                {
                    flag = false;
                    Console.Write("score not valid ");
                }

                if (!Enum.TryParse(line[4], true, out Status stat))
                {
                    flag = false;
                    Console.Write("Status not valid");
                }

                if (!flag) { line[0] = ""; line[1] = ""; line[2] = ""; line[3] = ""; line[4] = ""; Console.WriteLine(); } 
            }
        }

        static double CalculateAverage(double[] score,int numOfLines) 
        {
            double sum = 0;
            if (numOfLines == 0)// protects from division by 0
            { 
                return 0;
            }
            for (int i = 0; i < numOfLines; i++)
            {
                sum += score[i];
            }
            return (double)sum / numOfLines;
        }

        static double FindMaxScore(double[] score, int count) 
        {
            double max = 0;
            for (int i = 0; i < count; i++)
            {
                if (max < score[i]) { max = score[i]; }
            }
            return max;
        }

        static double FindMinScore(double[] score,int count)
        {
            double min = score[0];
            for (int i = 1; i < count; i++)
            {
                if (min > score[i]) { min = score[i]; }
            }
            return min;
        }
        

        static int CountByStatus(Status[] status, Status statusEnom, int count)
        {
            int cnt = 0;
            for (int i =0; i < count; i++) 
            {
                if (status[i] == statusEnom) cnt += 1;
            }
            return cnt;
        }

        static int CountByType(ReportType[] reportType, ReportType type, int count)
        {
            int cnt = 0;
            for (int i = 0; i < count; i++)
            {
                if (reportType[i] == type) cnt += 1;
            }
            return cnt;
        }
        static void DisplayBasicStatistics(double[] score, int count) 
        {
            Console.WriteLine($"=== Report Statistics ===\n" +
                $"number of reports : {count}\n" +
                $"average : {CalculateAverage(score, count):F2}\n" +
                $"max score : {FindMaxScore(score, count)}\n" +
                $"min score : {FindMinScore(score, count)}");

        }

        static void DisplayStatusCounts(Status[] status, int count)
        {
            Console.WriteLine($"=== Reports by Status ===\n" +
                $"Pending : {CountByStatus(status, Status.Pending,count)}\n" +
                $"Approved : {CountByStatus(status, Status.Approved, count)}\n" +
                $"Rejected : {CountByStatus(status, Status.Rejected, count)}");
        }

        static void DisplayTypeCounts(ReportType[] reportType, int count) 
        {
            Console.WriteLine($"=== Reports by Type ===\n" +
                $"Collect : {CountByType(reportType,ReportType.Collect, count)}\n" +
                $"Analyze : {CountByType(reportType, ReportType.Analyze, count)}\n" +
                $"Recon : {CountByType(reportType, ReportType.Recon, count)}\n" +
                $"Intel : {CountByType(reportType, ReportType.Intel, count)}");
        }
        static void DisplayHighestPriorityApproved(string[] unitName, ReportType[] reportType,int[] priority,double[] score, Status[] status, int count) 
        {
            int? highestApprovedPriorityIndex = null ;
            for (int i = 0; i < count; i++)
            {
                if (status[i] == Status.Approved) 
                {
                    if (highestApprovedPriorityIndex == null || priority[highestApprovedPriorityIndex.Value] < priority[i]) 
                    {
                        highestApprovedPriorityIndex = i; 
                    }
                }
            }
            if (highestApprovedPriorityIndex != null)
            {
                Console.WriteLine($"=== Highest Priority Approved ===\n\n" +
                    $"unit Name: {unitName[highestApprovedPriorityIndex.Value]}, \n" +
                    $"report Type : {reportType[highestApprovedPriorityIndex.Value]}, \n" +
                    $"priority : {priority[highestApprovedPriorityIndex.Value]}, \n" +
                    $"score : {score[highestApprovedPriorityIndex.Value]}, \n" +
                    $"status : {status[highestApprovedPriorityIndex.Value]}, \n");
            }
            else { Console.WriteLine("no status aproved found"); }
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
            ReportType[] reportType = new ReportType[100];
            int[] priority = new int[100];
            double[] score = new double[100];
            Status[] status = new Status[100];

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