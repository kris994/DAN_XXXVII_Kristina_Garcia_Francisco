using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DAN_XXXVII_Kristina_Garcia_Francisco
{
    class Truck
    {
        private List<Thread> allTrucks = new List<Thread>();
        private Dictionary<int, string> allLoadingTime = new Dictionary<int, string>();
        private SemaphoreSlim semaphore = new SemaphoreSlim(2, 2);
        private readonly object lockTruck = new object();
        private readonly object lockRng = new object();
        private Random rng = new Random();
        private int counter = 0;
        private int EnterCounter = 0;
        private int arrivalTime = 0;

        public void TruckLoading()
        {
            int waitTime;

            semaphore.Wait();           
            Console.WriteLine("Truck {0} started loading.", Thread.CurrentThread.Name);
            lock (lockRng)
            {
                waitTime = rng.Next(500, 5001);
                allLoadingTime.Add(waitTime, Thread.CurrentThread.Name);
            }

            // Write the tiem it took to load
            if (allLoadingTime.Any(tr => tr.Value.Equals(Thread.CurrentThread.Name)))
            {
                Console.WriteLine("Truck {0} finished loading, it took {1} milliseconds."
                    , Thread.CurrentThread.Name, allLoadingTime.FirstOrDefault(x => x.Value == Thread.CurrentThread.Name).Key);
            }
            Thread.Sleep(waitTime);
                   
            semaphore.Release();

            counter++;
            
            // Wait for all trucks to finish loading
            while(counter != 10)
            {
                Thread.Sleep(0);
            }

            // Just to put a white line between loading and routing
            lock (lockTruck)
            {
                if (counter == 10)
                {
                    Console.WriteLine();
                    counter = 0;
                }
            }

            TruckRouting();
            counter++;

            // Just to put a white line between loading and routing
            lock (lockTruck)
            {
                if (counter == 10)
                {
                    Console.WriteLine();
                    counter = 0;
                }
            }
        
            // Wait for all trucks to finish getting a route
            while (counter != 10)
            {
                Thread.Sleep(0);
            }
            EnterCounter = 0;
            Arrival();
        }

        public void TruckRouting()
        {
            lock(lockTruck)
            {
                Console.WriteLine("Truck {0} received route {1}", Thread.CurrentThread.Name, Manager.bestRoutes[EnterCounter]);
                EnterCounter++;
            }
        }

        public void Arrival()
        {
            lock (lockTruck)
            {
                arrivalTime = rng.Next(500, 5000);

                Console.WriteLine("Truck {0} started coming, expected arrival time in {1} milliseconds."
                    , Thread.CurrentThread.Name, arrivalTime);
                EnterCounter++;
                Thread.Sleep(arrivalTime);

                if (arrivalTime > 3000)
                {
                    Console.WriteLine("Truck {0} is taking too long to arrive, delivery has been canceled. " +
                        "Expected return time is {1} milliseconds.", Thread.CurrentThread.Name, arrivalTime);
                    Thread.Sleep(arrivalTime);
                }
                else
                {
                    // Write the time it took to unload
                    if (allLoadingTime.Any(tr => tr.Value.Equals(Thread.CurrentThread.Name)))
                    {
                        int unloadingTime = allLoadingTime.FirstOrDefault(x => x.Value == Thread.CurrentThread.Name).Key / 2;
                        Console.WriteLine("Truck {0} arrived, the unloading time is {1} milliseconds."
                            , Thread.CurrentThread.Name, unloadingTime);
                        Thread.Sleep(arrivalTime);
                    }
                }
            }
            
        }

        public void CreateTrucks()
        {
            for (int i = 1; i < 11; i++)
            {
                Thread truckThreads = new Thread(TruckLoading)
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
