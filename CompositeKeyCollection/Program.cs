using System;
using System.Collections.Generic;

namespace CompositeKeyCollection
{
    record Emploee(string Position, decimal Salary);

    class Program
    {
        static void Main(string[] args)
        {
            var collection = new CompositeKeyCollection<int, string, Emploee>();

            collection.Add(1, "Nikita", new Emploee("Java Developer", 340_000));
            collection.Add(1, "Valera", new Emploee("JS Developer", 345_000));
            collection.Add(2, "Nikita", new Emploee("C# Developer", 330_000));

            var id1 = collection.GetById(1);
            PrintValues(id1);

            Console.WriteLine();

            collection.Remove(1, "Valera");
            id1 = collection.GetById(1);
            PrintValues(id1);

            Console.WriteLine();

            var nikitas = collection.GetByName("Nikita");
            PrintValues(nikitas);

            Console.WriteLine();

            var nikita = collection[2, "Nikita"];
            Console.WriteLine(nikita);

            Console.WriteLine();

            collection.Add(2, "Vladimir", new Emploee("Manager", 450_000));
            var id2 = collection.GetById(2);
            PrintValues(id2);

            Console.WriteLine();

            collection[2, "Nikita"] = new Emploee("Senior C# Developer", 360_000);
            nikita = collection[2, "Nikita"];
            Console.WriteLine(nikita);
        }

        static void PrintValues(List<Emploee> values)
        {
            foreach (var value in values)
            {
                Console.WriteLine(value);
            }
        }
    }
}
