using System;

namespace ACO
{
    public class Edge
    {
        //private int _weight;
        //public int Weight
        //{
        //    get { return _weight; }
        //    set
        //    {
        //        _weight = value;
        //        InvertedWeight = 1 / value;
        //    }
        //}

        public double InvertedWeight { get; set; }

        public double Pheromones { get; set; } = new Random().NextDouble();
    }
}
