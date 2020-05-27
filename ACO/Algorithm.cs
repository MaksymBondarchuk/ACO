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

		private List<Vertex> Vertices { get; } = new List<Vertex>();

		private List<Edge> Edges { get; } = new List<Edge>();

		private List<Ant> Ants { get; } = new List<Ant>();

		#endregion

		public bool ParseFile(string fileName)
		{
			DirectoryInfo directoryInfo = Directory.GetParent(Directory.GetCurrentDirectory()).Parent;
			if (directoryInfo == null)
			{
				return false;
			}

			string filePath = Path.Combine(directoryInfo.FullName, fileName);
			using (var sr = new StreamReader(filePath))
			{
				string line;
				var random = new Random();
				while ((line = sr.ReadLine()) != null)
				{
					if (line.Length == 0)
					{
						continue;
					}

					if (line[0] == 'p')
					{
						ProblemSize = Convert.ToInt32(Regex.Match(line, @"\d+").Value);
					}

					if (line[0] == 'i')
					{
						MatchCollection matches = Regex.Matches(line, @"\d+");

						if (matches.Count != ProblemSize)
						{
							return false;
						}

						var vertex = new Vertex();
						for (var i = 0; i < matches.Count; i++)
						{
							var weight = Convert.ToInt32(matches[i].Value);
							if (weight == 0)
							{
								vertex.EdgesIndexes.Add(-1);
								continue;
							}

							if (i < Vertices.Count)
							{
								vertex.EdgesIndexes.Add(Vertices[i].EdgesIndexes[Vertices.Count]);
								continue;
							}

							Edges.Add(new Edge
							{
								Weight = weight,
								Pheromones = random.NextDouble(),
								Vertices = new List<int> {i, Vertices.Count}
							});
							vertex.EdgesIndexes.Add(Edges.Count - 1);
						}

						Vertices.Add(vertex);
					}
				}
			}

			return true;
		}

		private void Init(int colonySize)
		{
			for (var i = 0; i < colonySize; i++)
			{
				Ants.Add(new Ant());
			}
		}

		public void Run(int colonySize)
		{
			Init(colonySize);

			var random = new Random();
			var bestWeight = double.MaxValue;
			var bestWay = string.Empty;
			var lastImprovementOn = 0;
			for (var iter = 0; iter < IterationsNumber; iter++)
			{
				// Evaporate pheromones
				foreach (Edge edge in Edges)
				{
					edge.Pheromones *= 1 - Rho;
				}

				foreach (Ant ant in Ants)
				{
					// Generate solutions
					var n = 0.0;

					for (var i = 0; i < ProblemSize; i++)
					{
						int edgeIdx = Vertices[ant.VisitedVertices.Last()].EdgesIndexes[i];
						if (edgeIdx == -1 || ant.VisitedVertices.Contains(Edges[edgeIdx].Vertices.First(t => t != ant.VisitedVertices.Last())))
						{
							continue;
						}

						n += Math.Pow(Edges[edgeIdx].InvertedWeight, Alpha) *
						     Math.Pow(Edges[edgeIdx].Pheromones, Beta);
					}

					for (var i = 0; i < ProblemSize; i++)
					{
						int edgeIdx = Vertices[ant.VisitedVertices.Last()].EdgesIndexes[i];
						if (edgeIdx == -1 || ant.VisitedVertices.Contains(Edges[edgeIdx].Vertices.First(t => t != ant.VisitedVertices.Last())))
						{
							continue;
						}

						double p = Math.Pow(Edges[edgeIdx].InvertedWeight, Alpha) *
						           Math.Pow(Edges[edgeIdx].Pheromones, Beta) / n;
						if (!(random.NextDouble() <= p))
						{
							continue;
						}

						ant.VisitedVertices.Add(i);
						ant.UsedEdges.Add(edgeIdx);
						ant.PathWeight += Edges[edgeIdx].Weight;
						break;
					}

					// Update pheromones
					if (ant.VisitedVertices.Last() == ProblemSize - 1)
					{
						double delta = Q / ant.PathWeight;
						foreach (int edgeIdx in ant.UsedEdges.Where(edgeIdx => edgeIdx != -1))
						{
							Edges[edgeIdx].Pheromones += delta;
						}

						if (ant.PathWeight < bestWeight)
						{
							bestWeight = ant.PathWeight;
							bestWay = string.Join(", ", ant.VisitedVertices.ToArray());
							lastImprovementOn = iter;
						}

						ant.VisitedVertices.Clear();
						ant.VisitedVertices.Add(0);
						ant.UsedEdges.Clear();
						ant.UsedEdges.Add(-1);
						ant.PathWeight = 0;
					}
				}

				Console.WriteLine($"#{iter,-4} Best weight: {bestWeight}");
			}

			Console.WriteLine($"\nBest way: {bestWay}\nIts weight: {bestWeight}" +
			                  $"\nLast improvement was on iteration #{lastImprovementOn}");
		}
	}
}