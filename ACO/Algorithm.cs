using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace ACO
{
    public class Algorithm
    {
        #region Constants: private
        private const int IterationsNumber = 1000;
        private const double Alpha = .5;
        private const double Beta = .5;
        private const double Q = 1;
        private const double Rho = .1;
        #endregion

        #region Properties: private
        private int ProblemSize { get; set; }

        private List<Vertex> Nodes { get; } = new List<Vertex>();

        private List<Edge> Edges { get; } = new List<Edge>();

        private List<Ant> Ants { get; } = new List<Ant>();
        #endregion

        public bool ParseFile(string fileName)
        {
            DirectoryInfo directoryInfo = Directory.GetParent(Directory.GetCurrentDirectory()).Parent;
            if (directoryInfo == null)
                return false;
            var filePath = Path.Combine(directoryInfo.FullName, fileName);
            using (var sr = new StreamReader(filePath))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.Length == 0)
                        continue;

                    if (line[0] == 'p')
                        ProblemSize = Convert.ToInt32(Regex.Match(line, @"\d+").Value);

                    if (line[0] == 'i')
                    {
                        MatchCollection matches = Regex.Matches(line, @"\d+");

                        if (matches.Count != ProblemSize)
                            return false;

                        var node = new Vertex();
                        for (var i = 0; i < matches.Count; i++)
                        {
                            var weight = Convert.ToInt32(matches[i].Value);
                            if (weight == 0)
                            {
                                node.EdgesIndexes.Add(-1);
                                continue;
                            }

                            Edges.Add(new Edge
                            {
                                Weight = weight
                            });
                            node.EdgesIndexes.Add(Edges.Count - 1);
                        }
                        Nodes.Add(node);
                    }
                }
            }

            return true;
        }

        private void Init(int colonySize)
        {
            for (var i = 0; i < colonySize; i++)
                Ants.Add(new Ant());
        }

        public void Run(int colonySize)
        {
            Init(colonySize);

            for (var iter = 0; iter < IterationsNumber; iter++)
            {
                // Evaporate pheromones
                foreach (var edge in Edges)
                    edge.Pheromones *= 1 - Rho;

                foreach (var ant in Ants)
                {
                    // Generate solutions
                    var n = Nodes[ant.VisitedVertices.Last()].EdgesIndexes
                        .Where(edgeIdx => edgeIdx != -1)
                        .Sum(edgeIdx => Edges[edgeIdx].InvertedWeight * Edges[edgeIdx].Pheromones);

                    var random = new Random();
                    for (var i = 0; i < ProblemSize; i++)
                    {
                        var edgeIdx = Nodes[ant.VisitedVertices.Last()].EdgesIndexes[i];
                        if (edgeIdx == -1)
                            continue;

                        var p = Math.Pow(Edges[edgeIdx].InvertedWeight, Alpha) *
                            Math.Pow(Edges[edgeIdx].Pheromones, Beta) / n;
                        if (!(random.NextDouble() <= p)) continue;
                        ant.VisitedVertices.Add(i);
                        ant.UsedEdges.Add(edgeIdx);
                        ant.PathWeight += Edges[edgeIdx].Weight;
                        break;
                    }

                    // Update pheromones
                    if (ant.VisitedVertices.Last() == ProblemSize - 1)
                    {
                        var delta = Q / ant.PathWeight;
                        foreach (var edgeIdx in ant.UsedEdges.Where(edgeIdx => edgeIdx != -1))
                            Edges[edgeIdx].Pheromones += delta;
                        ant.VisitedVertices.Clear();
                        ant.VisitedVertices.Add(0);
                        ant.UsedEdges.Clear();
                        ant.UsedEdges.Add(-1);
                    }
                }
            }
        }
    }
}
