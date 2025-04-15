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
} // Class to hold the results of a scheduling algorithm.
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
