using System;

namespace ACO
{
    internal static class Program
    {
        private static void Main()
        {
            var algorithm = new Algorithm();

            if (!algorithm.ParseFile("Graphs\\yuzSHP95.aco"))
            {
                Console.WriteLine("File cannot be parsed");
                return;
            }

            algorithm.Run(50);
        }
    }
}
