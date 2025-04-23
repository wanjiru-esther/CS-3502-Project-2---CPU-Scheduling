# CS-3502-Project-2---CPU-Scheduling
**CPU Scheduling Simulation**

This project implements various CPU scheduling algorithms, including the **Highest Response Ratio Next (HRRN)** and **Multi-Level Feedback Queue (MLFQ)**, in C#. The program simulates the scheduling of processes on a CPU, taking into account various metrics like turnaround time, waiting time, and CPU utilization.

**Introduction**

This project is a simulation of CPU scheduling algorithms, where we simulate the process execution and scheduling using the HRRN and MLFQ algorithms. The main goal of this simulation is to analyze and compare the performance of these scheduling strategies based on the following metrics:

- Average Waiting Time
- Average Turnaround Time
- CPU Utilization
- Throughput
- Average Response Time

**Features**

- **HRRN Scheduler**: This scheduling algorithm calculates the Highest Response Ratio for each process and selects the one with the highest ratio for execution.
- **MLFQ Scheduler**: This scheduler uses multiple queues with different time quanta and dynamically moves processes between them based on their remaining burst time.
- **Process Metrics**: The program calculates several key metrics for each process, including start time, completion time, waiting time, turnaround time, and response time.
- **Idle Time Calculation**: The system tracks and calculates CPU idle time to compute the overall CPU utilization.

**How to Run**

To run the program, follow these steps:

1. Clone the repository:

   ```bash
   git clone https://github.com/yourusername/cpu-scheduling-simulation.git
   ```

2. Navigate into the project directory:

   ```bash
   cd cpu-scheduling-simulation
   ```

3. Build the project using Visual Studio or a compatible C# IDE.

4. Once the project is built, run the program. The results of the scheduling simulation will be displayed in the console output.

   For example, you can run the `Main` method in the `Program.cs` file, which initializes a set of processes and schedules them using the selected algorithm (HRRN or MLFQ).

5. The output will display the results of the scheduling algorithm, including detailed metrics for each process.

**License**

This project is licensed under the MIT License.

**External Libraries**

This project uses no external libraries, as all functionality is implemented from scratch.
