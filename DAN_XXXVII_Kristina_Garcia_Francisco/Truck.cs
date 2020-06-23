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
        private string routesFile = "routes.txt";
        private Random rng = new Random();
        private bool ready = false;
        private readonly object lockRoutes = new object();

        #region Read and Write from file
        /// <summary>
        /// Write routes to file
        /// </summary>
        public void CreateRoutes()
        {
            // Save all the routes to file
            using (StreamWriter streamWriter = new StreamWriter(routesFile))
            {
                for (int i = 0; i < 1000; i++)
                {
                    int route = rng.Next(1, 5001);
                    streamWriter.WriteLine(route);
                }
            }

            ready = true;
        }

        /// <summary>
        /// Read routes from file
        /// </summary>
        public void ReadFile()
        {
            using (StreamReader streamReader = File.OpenText(routesFile))
            {
                Console.WriteLine("\nAll routes: ");
                string line;
                while ((line = streamReader.ReadLine()) != null)
                {
                    allRoutes.Add(int.Parse(line));
                    Console.Write(line + " ");
                }
            }
        }
        #endregion

        /// <summary>
        /// Thread waits a given random amount of time
        /// </summary>
        /// <param name="startTime">minimum time to wait</param>
        /// <param name="maxTime">maximum time to wait</param>
        /// <returns>the random generated wait time</returns>
        public int WaitTime(int minTime, int maxTime)
        {
            int sleepTime = rng.Next(minTime, maxTime);
            Thread.Sleep(sleepTime);

            // Run this while in case the thread started before the routes were created
            while (ready == false)
            {
                // Thread cannot sleep longer than its maxTime
                maxTime = maxTime - sleepTime;
                sleepTime = rng.Next(minTime, maxTime);
                Thread.Sleep(sleepTime);
            }

            return sleepTime;
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
            Console.WriteLine("Manager waiting for routes to be created...");
            // Sleep max 3sec
            int waited = WaitTime(0, 3001);

            Console.WriteLine("Manager waited {0} milliseconds for the routes to be created.", waited);

            // Return the ready bool back to default for future use
            ready = false;

            // Get all routes from the file
            ReadFile();

            // Get best 10 routes
            BestRoutes();
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
