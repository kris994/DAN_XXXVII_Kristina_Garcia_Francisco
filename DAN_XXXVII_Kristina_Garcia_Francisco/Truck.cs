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
        public int route { get; set; }
        private List<Thread> allTrucks = new List<Thread>();

        public Truck()
        {

        }

        public Truck(int route)
        {
            this.route = route;
        }

        public void TruckLoading()
        {

        }

        public void CreateTrucks()
        {
            for (int i = 1; i < 11; i++)
            {
                Thread truckThreads = new Thread(TruckLoading)
                {
                    Name = "Truck_" + i
                };
                Console.WriteLine(truckThreads.Name);
                allTrucks.Add(truckThreads);
            }

            foreach (var item in allTrucks)
            {
                item.Start();
            }
        }
    }
}
