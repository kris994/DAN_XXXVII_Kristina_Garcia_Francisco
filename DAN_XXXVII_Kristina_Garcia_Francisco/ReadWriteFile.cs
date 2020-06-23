using System;
using System.Collections.Generic;
using System.IO;

namespace DAN_XXXVII_Kristina_Garcia_Francisco
{
    /// <summary>
    /// Reads and writes from routes file
    /// </summary>
    class ReadWriteFile
    {
        private readonly string routesFile = "routes.txt";
        private Random rng = new Random();

        /// <summary>
        /// Write routes to file
        /// </summary>
        public void WriteToFile()
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
        }

        /// <summary>
        /// Read routes from file
        /// </summary>
        public void ReadFile(List<int> list)
        {
            using (StreamReader streamReader = File.OpenText(routesFile))
            {
                Console.WriteLine("\nAll routes: ");
                string line;
                while ((line = streamReader.ReadLine()) != null)
                {
                    list.Add(int.Parse(line));
                    Console.Write(line + " ");
                }
            }
        }
    }
}
