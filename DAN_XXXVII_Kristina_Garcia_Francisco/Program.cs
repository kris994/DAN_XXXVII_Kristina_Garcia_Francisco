using System;

namespace DAN_XXXVII_Kristina_Garcia_Francisco
{
    class Program
    {
        static void Main(string[] args)
        {
            Manager manager = new Manager();
            Truck truck = new Truck();

            manager.CreateWorkers();
            truck.CreateTrucks();

            Console.ReadKey();
        }
    }
}
