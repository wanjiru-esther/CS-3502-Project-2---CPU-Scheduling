using System;
using System.Collections.Generic;
using System.Linq;

public class Process
{
    public int ProcessId { get; set; }
    public int ArrivalTime { get; set; }
    public int BurstTime { get; set; }
    public int RemainingTime { get; set; } // For preemptive algorithms
    public int Priority { get; set; }
    public int StartTime { get; set; }
    public int CompletionTime { get; set; }
    public int WaitingTime { get; set; }
    public int TurnaroundTime { get; set; }
    public int ResponseTime { get; set; }

    public Process(int id, int arrival, int burst, int priority = 0)
    {
        ProcessId = id;
        ArrivalTime = arrival;
        BurstTime = burst;
        RemainingTime = burst;
        Priority = priority;
        StartTime = -1;
        CompletionTime = -1;
        WaitingTime = 0;
        TurnaroundTime = 0;
        ResponseTime = -1;
    }

    public override string ToString()
    {
        return $"ID: {ProcessId}, Arrival: {ArrivalTime}, Burst: {BurstTime}, Priority: {Priority}";
    }
}// Class to hold the results of a scheduling algorithm.
public class SchedulingResults
{
    public List<Process> CompletedProcesses { get; set; }
    public double AverageWaitingTime { get; set; }
    public double AverageTurnaroundTime { get; set; }
    public double CPUUtilization { get; set; }
    public double Throughput { get; set; }
    public double AverageResponseTime { get; set; }

    public SchedulingResults()
    {
        CompletedProcesses = new List<Process>();
        AverageWaitingTime = 0;
        AverageTurnaroundTime = 0;
        CPUUtilization = 0;
        Throughput = 0;
        AverageResponseTime = 0;
    }
}
public class HRRNScheduler
{
    public SchedulingResults Schedule(List<Process> processes)
    {
        SchedulingResults results = new SchedulingResults();
        List<Process> readyQueue = new List<Process>();
        List<Process> completedProcesses = new List<Process>();
        int currentTime = 0;
        processes.Sort((p1, p2) => p1.ArrivalTime.CompareTo(p2.ArrivalTime));

        while (completedProcesses.Count != processes.Count)
        {
            foreach (Process process in processes)
            {
                if (process.ArrivalTime <= currentTime && !readyQueue.Contains(process) && !completedProcesses.Contains(process))
                {
                    readyQueue.Add(process);
                }
            }

            if (readyQueue.Count == 0)
            {
                currentTime++;
                continue;
            }

            double highestResponseRatio = -1;
            Process selectedProcess = null;

            foreach (Process process in readyQueue)
            {
                double responseRatio = (double)(currentTime - process.ArrivalTime + process.BurstTime) / process.BurstTime;
                if (responseRatio > highestResponseRatio)
                {
                    highestResponseRatio = responseRatio;
                    selectedProcess = process;
                }
            }

            if (selectedProcess != null)
            {
                if (selectedProcess.StartTime == -1)
                {
                    selectedProcess.StartTime = currentTime;
                }
                currentTime += selectedProcess.BurstTime;
                selectedProcess.CompletionTime = currentTime;
                CalculateMetrics(selectedProcess);
                completedProcesses.Add(selectedProcess);
                readyQueue.Remove(selectedProcess);
            }
        }
        results.CompletedProcesses = completedProcesses;
        CalculateOverallMetrics(results, processes.Count, currentTime);
        return results;
    }
    private void CalculateMetrics(Process process)
    {
        process.TurnaroundTime = process.CompletionTime - process.ArrivalTime;
        process.WaitingTime = process.TurnaroundTime - process.BurstTime;
        if (process.StartTime != -1 && process.ArrivalTime != -1)
        {
            process.ResponseTime = process.StartTime - process.ArrivalTime;
        }
    }

    private void CalculateOverallMetrics(SchedulingResults results, int totalProcesses, int totalTime)
    {
        double totalWaitingTime = 0;
        double totalTurnaroundTime = 0;
        double totalResponseTime = 0;

        foreach (Process process in results.CompletedProcesses)
        {
            totalWaitingTime += process.WaitingTime;
            totalTurnaroundTime += process.TurnaroundTime;
            totalResponseTime += process.ResponseTime;
        }

        results.AverageWaitingTime = totalWaitingTime / totalProcesses;
        results.AverageTurnaroundTime = totalTurnaroundTime / totalProcesses;
        results.CPUUtilization = (double)totalTime > 0 ? (double)(totalTime - CountIdleTime(results.CompletedProcesses)) / totalTime * 100 : 0;
        results.Throughput = (double)totalProcesses / totalTime;
        results.AverageResponseTime = results.CompletedProcesses.Any(p => p.ResponseTime != -1) ? totalResponseTime / results.CompletedProcesses.Count : 0;
    }
    private int CountIdleTime(List<Process> completedProcesses)
    {
        if (completedProcesses == null || completedProcesses.Count == 0)
        {
            return 0;
        }

        List<Process> sortedProcesses = completedProcesses.OrderBy(p => p.StartTime).ToList();
        int idleTime = 0;
        int previousCompletionTime = 0;

        for (int i = 0; i < sortedProcesses.Count; i++)
        {
            if (sortedProcesses[i].StartTime > previousCompletionTime)
            {
                idleTime += sortedProcesses[i].StartTime - previousCompletionTime;
            }
            previousCompletionTime = sortedProcesses[i].CompletionTime;
        }
        return idleTime;
    }
    public void DisplayResults(SchedulingResults results)
    {
        Console.WriteLine("HRRN Results:");
        Console.WriteLine("Process\tAT\tBT\tST\tCT\tWT\tTAT\tRT");
        foreach (Process p in results.CompletedProcesses)
        {
            Console.WriteLine($"{p.ProcessId}\t{p.ArrivalTime}\t{p.BurstTime}\t{p.StartTime}\t{p.CompletionTime}\t{p.WaitingTime}\t{p.TurnaroundTime}\t{p.ResponseTime}");
        }
        Console.WriteLine($"Average Waiting Time: {results.AverageWaitingTime}");
        Console.WriteLine($"Average Turnaround Time: {results.AverageTurnaroundTime}");
        Console.WriteLine($"CPU Utilization: {results.CPUUtilization}%");
        Console.WriteLine($"Throughput: {results.Throughput} processes/unit time");
        Console.WriteLine($"Average Response Time: {results.AverageResponseTime}");
    }
}
public class MLFQScheduler
{
    public SchedulingResults Schedule(List<Process> processes)
    {
        SchedulingResults results = new SchedulingResults();
        int numQueues = 3;
        int[] timeQuantums = { 4, 8, 16 };
        List<Queue<Process>> queues = new List<Queue<Process>>();
        for (int i = 0; i < numQueues; i++)
        {
            queues.Add(new Queue<Process>());
        }

        List<Process> completedProcesses = new List<Process>();
        int currentTime = 0;
        processes.Sort((p1, p2) => p1.ArrivalTime.CompareTo(p2.ArrivalTime));
        int currentProcessIndex = 0;

        while (completedProcesses.Count != processes.Count)
        {
            while (currentProcessIndex < processes.Count && processes[currentProcessIndex].ArrivalTime <= currentTime)
            {
                queues[0].Enqueue(processes[currentProcessIndex]);
                currentProcessIndex++;
            }
            bool processExecuted = false;

            for (int i = 0; i < numQueues; i++)
            {
                if (queues[i].Count > 0)
                {
                    Process currentProcess = queues[i].Dequeue();
                    processExecuted = true;

                    if (currentProcess.StartTime == -1)
                    {
                        currentProcess.StartTime = currentTime;
                    }

                    if (currentProcess.RemainingTime <= timeQuantums[i])
                    {
                        currentTime += currentProcess.RemainingTime;
                        currentProcess.RemainingTime = 0;
                        currentProcess.CompletionTime = currentTime;
                        CalculateMetrics(currentProcess);
                        completedProcesses.Add(currentProcess);
                    }
                    else
                    {
                        currentTime += timeQuantums[i];
                        currentProcess.RemainingTime -= timeQuantums[i];
                        if (currentProcess.RemainingTime > 0)
                        {
                            if (i < numQueues - 1)
                            {
                                queues[i + 1].Enqueue(currentProcess);
                            }
                            else
                            {
                                queues[i].Enqueue(currentProcess);
                            }
                        }
                        else
                        {
                            currentProcess.CompletionTime = currentTime;
                            CalculateMetrics(currentProcess);
                            completedProcesses.Add(currentProcess);
                        }
                    }
                    break;
                }
            }
            if (!processExecuted)
            {
                currentTime++;
            }
        }
        results.CompletedProcesses = completedProcesses;
        CalculateOverallMetrics(results, processes.Count, currentTime);
        return results;
    }
    private void CalculateMetrics(Process process)
    {
        process.TurnaroundTime = process.CompletionTime - process.ArrivalTime;
        process.WaitingTime = process.TurnaroundTime - process.BurstTime;
        if (process.StartTime != -1 && process.ArrivalTime != -1)
        {
            process.ResponseTime = process.StartTime - process.ArrivalTime;
        }
    }

    private void CalculateOverallMetrics(SchedulingResults results, int totalProcesses, int totalTime)
    {
        double totalWaitingTime = 0;
        double totalTurnaroundTime = 0;
        double totalResponseTime = 0;

        foreach (Process process in results.CompletedProcesses)
        {
            totalWaitingTime += process.WaitingTime;
            totalTurnaroundTime += process.TurnaroundTime;
            totalResponseTime += process.ResponseTime;
        }

        results.AverageWaitingTime = totalWaitingTime / totalProcesses;
        results.AverageTurnaroundTime = totalTurnaroundTime / totalProcesses;
        results.CPUUtilization = (double)totalTime > 0 ? (double)(totalTime - CountIdleTime(results.CompletedProcesses)) / totalTime * 100 : 0;
        results.Throughput = (double)totalProcesses / totalTime;
        results.AverageResponseTime = results.CompletedProcesses.Any(p => p.ResponseTime != -1) ? totalResponseTime / results.CompletedProcesses.Count : 0;
    }
    private int CountIdleTime(List<Process> completedProcesses)
    {
        if (completedProcesses == null || completedProcesses.Count == 0)
        {
            return 0;
        }

        List<Process> sortedProcesses = completedProcesses.OrderBy(p => p.StartTime).ToList();
        int idleTime = 0;
        int previousCompletionTime = 0;

        for (int i = 0; i < sortedProcesses.Count; i++)
        {
            if (sortedProcesses[i].StartTime > previousCompletionTime)
            {
                idleTime += sortedProcesses[i].StartTime - previousCompletionTime;
            }
            previousCompletionTime = sortedProcesses[i].CompletionTime;
        }
        return idleTime;
    }

    public void DisplayResults(SchedulingResults results)
    {
        Console.WriteLine("MLFQ Results:");
        Console.WriteLine("Process\tAT\tBT\tST\tCT\tWT\tTAT\tRT");
        foreach (Process p in results.CompletedProcesses)
        {
            Console.WriteLine($"{p.ProcessId}\t{p.ArrivalTime}\t{p.BurstTime}\t{p.StartTime}\t{p.CompletionTime}\t{p.WaitingTime}\t{p.TurnaroundTime}\t{p.ResponseTime}");
        }
        Console.WriteLine($"Average Waiting Time: {results.AverageWaitingTime}");
        Console.WriteLine($"Average Turnaround Time: {results.AverageTurnaroundTime}");
        Console.WriteLine($"CPU Utilization: {results.CPUUtilization}%");
        Console.WriteLine($"Throughput: {results.Throughput} processes/unit time");
        Console.WriteLine($"Average Response Time: {results.AverageResponseTime}");
    }
}
public class CPUSchedulerManager
{
    public static void Main(string[] args)
    {
        List<Process> processes = new List<Process>
        {
            new Process(1, 0, 8),
            new Process(2, 1, 4),
            new Process(3, 2, 9),
            new Process(4, 3, 5),
            new Process(5, 4, 7),
            new Process(6, 5, 2),
            new Process(7, 6, 6),
            new Process(8, 7, 3),
        };

        HRRNScheduler hrrnScheduler = new HRRNScheduler();
        MLFQScheduler mlfqScheduler = new MLFQScheduler();

        SchedulingResults hrrnResults = hrrnScheduler.Schedule(processes.ToList());
        SchedulingResults mlfqResults = mlfqScheduler.Schedule(processes.ToList());

        hrrnScheduler.DisplayResults(hrrnResults);
        mlfqScheduler.DisplayResults(mlfqResults);

        Console.ReadKey();
    }
}
//output 
/*Process AT      BT      ST      CT      WT      TAT     RT
1       0       8       0       33      25      33      0
2       1       4       8       8       3       7       7
6       5       2       12      22      15      17      7
8       7       3       14      29      19      22      7
4       3       5       17      39      31      36      14
7       6       6       22      44      32      38      16
5       4       7       28      42      31      38      24
3       2       9       35      38      27      36      33
Average Waiting Time: 13.5
Average Turnaround Time: 19
CPU Utilization: 100%
Throughput: 0.18181818181818182 processes/unit time
Average Response Time: 13.5
MLFQ Results:
Process AT      BT      ST      CT      WT      TAT     RT
2       1       4       8       8       3       7       7
6       5       2       12      22      15      17      7
8       7       3       14      29      19      22      7
1       0       8       0       33      25      33      0
3       2       9       35      38      27      36      33
4       3       5       17      39      31      36      14
5       4       7       28      42      31      38      24
7       6       6       22      44      32      38      16
Average Waiting Time: 22.875
Average Turnaround Time: 28.375
CPU Utilization: 90.9090909090909%
Throughput: 0.18181818181818182 processes/unit time
Average Response Time: 13.5
*/
