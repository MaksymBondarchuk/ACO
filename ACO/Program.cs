using System;

namespace ACO
{
    internal static class Program
    {
        private static void Main()
        {
            var algorithm = new Algorithm();

            var isFileParsed = algorithm.ParseFile("Graphs/yuzSHP55.aco");
            if (!isFileParsed)
            {
                Console.WriteLine("File cannot be parsed");
                return;
            }

            algorithm.Run(50);
        }
    }
}
