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
        private List<int> allRoutes = new List<int>();
        private List<int> bestRoutes = new List<int>();
        private Random rng = new Random();
        private readonly object lockRoutes = new object();

        public void CreateRoutes()
        {
            ReadWriteFile rwf = new ReadWriteFile();
            lock(lockRoutes)
            {
                rwf.WriteToFile();
                Monitor.Pulse(lockRoutes);
            }
        }

        public void BestRoutes()
        {
            List<int> distinctValues = allRoutes.Distinct().ToList();
            distinctValues.Sort();

            for (int i = 0; i < distinctValues.Count; i++)
            {
                if (distinctValues[i] % 3 == 0)
                {
                    bestRoutes.Add(distinctValues[i]);
                    if(bestRoutes.Count == 10)
                    {
                        break;
                    }
                }
            }

            Console.Write("\n\nRoutes were created, the best routes are: ");
            foreach (var item in bestRoutes)
            {
                Console.Write("{0} ", item);
            }
            Console.WriteLine("\nThe trucks can now be loaded.");
        }

        public void ChooseBestRoutes()
        {
            ReadWriteFile rwf = new ReadWriteFile();

            Console.WriteLine("Manager waiting for routes to be created...");
            // Sleep max 3sec
            int waited = rng.Next(1000, 3001);
            
            lock (lockRoutes)
            {
                Monitor.Wait(lockRoutes, waited);

                Console.WriteLine("Manager waited {0} milliseconds for the routes to be created.", waited);

                // Get all routes from the file
                rwf.ReadFile(allRoutes);

                // Get best 10 routes
                BestRoutes();
            }
        }

        public void CreateWorkers()
        {
            Thread getValues = new Thread(CreateRoutes);
            getValues.Start();

            Thread menager = new Thread(ChooseBestRoutes);
            menager.Start();

            getValues.Join();
            menager.Join();
        }
    }
}
