using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace DAN_XXXVII_Kristina_Garcia_Francisco
{
    class Truck
    {
        #region Properties
        private List<Thread> allTrucks = new List<Thread>();
        private Dictionary<int, string> allLoadingTime = new Dictionary<int, string>();
        private Dictionary<int, string> allArrivalTime = new Dictionary<int, string>();
        private SemaphoreSlim semaphore = new SemaphoreSlim(2, 2);
        private readonly object lockTruck = new object();
        private Random rng = new Random();
        /// <summary>
        /// Counts the active amount of threads
        /// </summary>
        private int activeThreads = 0;
        /// <summary>
        /// Counter increases as threads enter
        /// </summary>
        private int enterCounter = 0;
        /// <summary>
        /// Restarts the thread counts
        /// </summary>
        private int restartThreadCount = 0;
        #endregion

        public void TruckActions()
        {
            TruckLoading();
            PauseThreadUntilAllReady(10);
            TruckRouting();
            PauseThreadUntilAllReady(10);
            TruckUnloading();
        }

        public void TruckLoading()
        {           
            int waitTime;

            // Amount fo threads that can enter the semaphore
            MultipleThreads(2);

            semaphore.Wait();           
            Console.WriteLine("Truck {0} started loading.", Thread.CurrentThread.Name);
            lock (lockTruck)
            {
                waitTime = rng.Next(500, 5001);
                // Ensuring to get a random waitTime
                Thread.Sleep(15);
                allLoadingTime.Add(waitTime, Thread.CurrentThread.Name);
            }
            Thread.Sleep(waitTime);
            // Write the tiem it took to load
            if (allLoadingTime.Any(tr => tr.Value.Equals(Thread.CurrentThread.Name)))
            {
                Console.WriteLine("Truck {0} finished loading, it took {1} milliseconds."
                    , Thread.CurrentThread.Name, allLoadingTime.FirstOrDefault(x => x.Value == Thread.CurrentThread.Name).Key);
            }
            semaphore.Release();

            // Let more threads enter the semaphore
            RestartMultipleThreads();
        }

        /// <summary>
        /// Only let a fixed amount of threads to enter
        /// </summary>
        /// <param name="amount">the fixed amount</param>
        public void MultipleThreads(int amount)
        {
            // Allow only 2 threads at the same time
            while (true)
            {
                lock (lockTruck)
                {
                    enterCounter++;

                    if (enterCounter > amount)
                    {
                        Thread.Sleep(0);
                    }
                    else
                    {
                        restartThreadCount++;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Restart the amount of threads
        /// </summary>
        public void RestartMultipleThreads()
        {
            // Open semaphore for more threads
            restartThreadCount--;
            if (restartThreadCount == 0)
            {
                enterCounter = 0;
            }
        }

        /// <summary>
        /// Pause all threads until all of them are ready to continue
        /// </summary>
        /// <param name="amount">amount of threads to wait to be ready</param>
        public void PauseThreadUntilAllReady(int amount)
        {
            activeThreads++;

            // Wait for all trucks to finish
            while (activeThreads != amount)
            {
                Thread.Sleep(0);
            }

            // Just to put a white line on console after setting all trucks up
            lock (lockTruck)
            {
                if (activeThreads == amount)
                {
                    Console.WriteLine();
                    activeThreads = 0;
                }
            }
        }

        /// <summary>
        /// Give routes to each thread
        /// </summary>
        public void TruckRouting()
        {
            lock(lockTruck)
            {
                Console.WriteLine("Truck {0} received route {1}", Thread.CurrentThread.Name, Manager.bestRoutes[enterCounter]);
                enterCounter++;
            }
        }

        public void TruckUnloading()
        {
            int arrivalTime;
            lock (lockTruck)
            {
                arrivalTime = rng.Next(500, 5000);

                Console.WriteLine("Truck {0} started coming, expected arrival time in {1} milliseconds."
                    , Thread.CurrentThread.Name, arrivalTime);
                enterCounter++;
                allArrivalTime.Add(arrivalTime, Thread.CurrentThread.Name);

                ArrivalNotification(arrivalTime);
            }           
        }

        public void ArrivalNotification(int arrivalTime)
        {
            Thread.Sleep(arrivalTime);

            if (arrivalTime > 3000)
            {
                if (allArrivalTime.Any(tr => tr.Value.Equals(Thread.CurrentThread.Name)))
                {
                    Console.WriteLine("Truck {0} is taking too long to arrive, delivery has been canceled. " +
                "Expected return time is {1} milliseconds.", Thread.CurrentThread.Name, arrivalTime);
                    Thread.Sleep(arrivalTime);
                    Console.WriteLine("Truck {0} successfully returned.", Thread.CurrentThread.Name);
                }
            }
            else
            {
                // Write the time it took to unload
                if (allLoadingTime.Any(tr => tr.Value.Equals(Thread.CurrentThread.Name)))
                {
                    int unloadingTime = allLoadingTime.FirstOrDefault(x => x.Value == Thread.CurrentThread.Name).Key / 2;
                    Console.WriteLine("Truck {0} arrived, the unloading time is {1} milliseconds."
                        , Thread.CurrentThread.Name, unloadingTime);
                    Thread.Sleep(unloadingTime);
                    Console.WriteLine("Truck {0} successfully unloaded.", Thread.CurrentThread.Name);
                }
            }
        }

        public void CreateTrucks()
        {
            for (int i = 1; i < 11; i++)
            {
                Thread truckThreads = new Thread(TruckActions)
                {
                    Name = "Truck_" + i
                };
                allTrucks.Add(truckThreads);
            }

            foreach (var item in allTrucks)
            {
                item.Start();
            }
        }
    }
}
