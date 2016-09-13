using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace ACO
{
    public class Algorithm
    {
        private int ProblemSize { get; set; }

        private List<Node> Nodes { get; } = new List<Node>();

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

                        var node = new Node();
                        for (var i = 0; i < matches.Count; i++)
                            node.Weights.Add(Convert.ToInt32(matches[i].Value));
                        Nodes.Add(node);
                    }
                }
            }

            return true;
        }
    }
}
