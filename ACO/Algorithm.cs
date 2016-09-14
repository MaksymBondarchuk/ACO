using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace ACO
{
    public class Algorithm
    {
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
                                InvertedWeight = 1.0 / weight
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
            var random = new Random();
            for (var i = 0; i < colonySize; i++)
            {
                var ant = new Ant();
                ant.VisitedNodes.Add(random.Next(ProblemSize - 1));
                Ants.Add(ant);
            }
        }

        public void Run(int colonySize)
        {
            Init(colonySize);
        }
    }
}
