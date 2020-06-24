using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace DAN_XXXVII_Kristina_Garcia_Francisco
{
    /// <summary>
    /// Creates trucks and represents all actions trucks have to do
    /// </summary>
    class Truck
    {
        #region Properties
        /// <summary>
        /// List of all active trucks
        /// </summary>
        private List<Thread> allTrucks = new List<Thread>();
        /// <summary>
        /// Saving each trucks time it took to load
        /// </summary>
        private Dictionary<int, string> allLoadingTime = new Dictionary<int, string>();
        /// <summary>
        /// Semaphore for controling the truck loading time
        /// </summary>
        private SemaphoreSlim semaphore = new SemaphoreSlim(2, 2);
        /// <summary>
        /// Locks the truck activities
        /// </summary>
        private readonly object lockTruck = new object();
        /// <summary>
        /// Locks the trucks arrival
        /// </summary>
        private readonly object lockArrival = new object();
        /// <summary>
        /// Generate random numbers when needed
        /// </summary>
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
        /// <summary>
        /// Counts the amount of routes that were given to threads
        /// </summary>
        private int routeCounter = 0;
        #endregion

        /// <summary>
        /// All actions that a truck needs to do
        /// </summary>
        public void TruckActions()
        {
            TruckLoading();
            PauseThreadUntilAllReady(10);
            TruckRouting();
            TruckArriving();
        }

        #region Loading
        /// <summary>
        /// Load 2 trucks at the same time, each loading takes a random amount of time
        /// </summary>
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
            // Write the time it took to load
            if (allLoadingTime.Any(tr => tr.Value.Equals(Thread.CurrentThread.Name)))
            {
                Console.WriteLine("Truck {0} finished loading, it took {1} milliseconds."
                    , Thread.CurrentThread.Name, allLoadingTime.FirstOrDefault(x => x.Value == Thread.CurrentThread.Name).Key);
            }
            semaphore.Release();

            // Let more threads enter the semaphore
            restartThreadCount--;
            if (restartThreadCount == 0)
            {
                enterCounter = 0;
            }
        }
        #endregion

        #region Routing
        /// <summary>
        /// Give routes to each thread
        /// </summary>
        public void TruckRouting()
        {
            lock (lockTruck)
            {
                Console.WriteLine("Truck {0} received route {1}", Thread.CurrentThread.Name, Manager.truckRoutes[routeCounter]);
                routeCounter++;
            }
        }
        #endregion

        #region Arrival
        /// <summary>
        /// Calculates the trucks arrival time
        /// </summary>
        public void TruckArriving()
        {
            int arrivalTime;
            lock (lockTruck)
            {
                arrivalTime = rng.Next(500, 5000);

                Console.WriteLine("Truck {0} started coming, expected arrival time in {1} milliseconds."
                    , Thread.CurrentThread.Name, arrivalTime);
                enterCounter++;

                Monitor.Wait(lockTruck, arrivalTime);
            }
            ArrivalActions(arrivalTime);
        }

        /// <summary>
        /// Depending on the time a truck took to arrive, different actions will be done
        /// </summary>
        /// <param name="arrivalTime">the time it took for the truck to arrive</param>
        public void ArrivalActions(int arrivalTime)
        {
            lock (lockArrival)
            {
                if (arrivalTime > 3000)
                {
                    Console.WriteLine("Truck {0} is taking too long to arrive, delivery has been canceled. " +
                    "Expected return time is {1} milliseconds.", Thread.CurrentThread.Name, arrivalTime);
                    Monitor.Wait(lockArrival, arrivalTime);
                    Console.WriteLine("\t\t\t\t\tTruck {0} successfully returned.", Thread.CurrentThread.Name);
                }
                else
                {
                    int unloadingTime = allLoadingTime.FirstOrDefault(x => x.Value == Thread.CurrentThread.Name).Key;
                    Console.WriteLine("Truck {0} arrived, the unloading time is {1} milliseconds."
                        , Thread.CurrentThread.Name, unloadingTime / 2);
                    Monitor.Wait(lockArrival, unloadingTime / 2);
                    Console.WriteLine("\t\t\t\t\tTruck {0} successfully unloaded.", Thread.CurrentThread.Name);
                }
            }
        }
        #endregion

        #region Manipulating truck threads
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
        #endregion

        /// <summary>
        /// Creates 10 different truck threads and starts them at the same time
        /// </summary>
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
