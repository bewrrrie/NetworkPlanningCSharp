using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


struct Arrow {
	private int begin;
	private int end;
	private double weight;

	public Arrow(Arrow e)
	{
		this.begin = e.begin;
		this.end = e.end;
		this.weight = e.weight;
	}

	public Arrow(int begin, int end, double weight)
	{
		this.begin = begin;
		this.end = end;
		this.weight = weight;
	}

	public Arrow(int begin, int end)
	{
		this.begin = begin;
		this.end = end;
		this.weight = 0;
	}

	public int GetBegin()
	{
		return begin;
	}

	public int GetEnd()
	{
		return end;
	}

	public double GetWeight()
	{
		return weight;
	}

	public override string ToString()
	{
		return "(" + begin + ", " + end + ", weight=" + weight + ")";
	}

	public override bool Equals(object obj) 
	{
		if (!(obj is Arrow))
			return false;

		Arrow a = (Arrow) obj;
		return a.begin == this.begin && a.end == this.end;
	}

	public override int GetHashCode()
	{
		return begin + end * 317;
	}
}


struct WeightedDiGraph
{
	private List<Arrow> arrows;

	public WeightedDiGraph(Arrow[] arrowsArr)
	{
		arrows = new List<Arrow>();

		foreach (Arrow a in arrowsArr)
		{
			arrows.Add(new Arrow(a));
		}
	}

	public WeightedDiGraph(List<Arrow> arrows)
	{
		this.arrows = new List<Arrow>();

		foreach (Arrow a in arrows)
		{
			this.arrows.Add(new Arrow(a));
		}
	}

	public int GetEdgesCount()
	{
		return arrows.Count();
	}

	public List<int> GetVertices()
	{
		List<int> vertices = new List<int>();

		foreach (Arrow a in arrows)
		{
			if (!vertices.Contains(a.GetBegin()))
			{
				vertices.Add(a.GetBegin());
			}

			if (!vertices.Contains(a.GetEnd()))
			{
				vertices.Add(a.GetEnd());
			}
		}

		return vertices;
	}

	public List<int> GetVerticesSorted()
	{
		return Sorter.TopologicalSort(this);
	}

	public List<Arrow> GetArrows()
	{
		List<Arrow> result = new List<Arrow>();

		foreach (Arrow a in arrows)
		{
			result.Add(new Arrow(a));
		}

		return result;
	}

	public List<Arrow> GetArrowsFrom(int i)
	{
		List<Arrow> result = new List<Arrow>();

		foreach (Arrow a in arrows)
		{
			if (a.GetBegin() == i)
			{
				result.Add(new Arrow(a));
			}
		}

		return result;
	}

	public List<Arrow> GetArrowsTo(int i)
	{
		List<Arrow> result = new List<Arrow>();

		foreach (Arrow a in arrows)
		{
			if (a.GetEnd() == i)
			{
				result.Add(new Arrow(a));
			}
		}

		return result;
	}

	public bool IsAdjacent(int i, int j)
	{
		return arrows.Contains(new Arrow(i, j));
	}
}


class Sorter
{
	private static void DFS
	(
		int v,
		WeightedDiGraph G,
		Dictionary<int, bool> used,
		List<int> result
	)
	{
		used[v] = true;
		List<Arrow> outcomingArrows = G.GetArrowsFrom(v);

		foreach (Arrow a in outcomingArrows)
		{
			if (!used[a.GetEnd()])
			{
				DFS(a.GetEnd(), G, used, result);
			}
		}

		result.Add(v);
	}

	public static List<int> TopologicalSort(WeightedDiGraph G)
	{
		List<int> vertices = G.GetVertices();
		Dictionary<int, bool> used = new Dictionary<int, bool>();
		List<int> result = new List<int>();

		foreach (int v in vertices)
		{
			used[v] = false;
		}

		result.Clear();
		foreach (int v in vertices)
		{
			if (!used[v])
			{
				DFS(v, G, used, result);
			}
		}
		result.Reverse();

		return result;
	}

	public static int GetTopologicalMin(WeightedDiGraph G)
	{
		return TopologicalSort(G)[0];
	}

	public static int GetTopologicalMax(WeightedDiGraph G)
	{
		List<int> sorted = TopologicalSort(G);
		return sorted[sorted.Count() - 1];
	}
}

class CPM
{
	public static List<Arrow> GetCriticalProcesses(WeightedDiGraph G)
	{
		Dictionary<int, double> earliestStart = new Dictionary<int, double>();
		List<int> vertices = G.GetVerticesSorted();

		int source = vertices.Min();
		earliestStart[source] = 0;

		foreach (int v in vertices)
		{
			earliestStart[v] = 0;
		}

		foreach (int v in vertices)
		{
			List<Arrow> incomingArrows = G.GetArrowsTo(v);

			foreach (Arrow a in incomingArrows)
			{
				double current = earliestStart[a.GetBegin()] + a.GetWeight();

				if (current > earliestStart[a.GetEnd()])
				{
					earliestStart[a.GetEnd()] = current;
				}
			}
		}


		int sinkVertex = vertices[vertices.Count() - 1];
		Dictionary<int, double> latestStart = new Dictionary<int, double>();

		foreach (int v in vertices)
		{
			latestStart[v] = double.MaxValue;
		}
		latestStart[sinkVertex] = earliestStart[sinkVertex];

		vertices.Reverse();

		foreach (int v in vertices)
		{
			List<Arrow> outcomingArrows = G.GetArrowsFrom(v);

			foreach (Arrow a in outcomingArrows)
			{
				double current = earliestStart[a.GetEnd()] - a.GetWeight();

				if (current < latestStart[v])
				{
					latestStart[v] = current;
				}
			}
		}


		List<Arrow> criticalProcesses = new List<Arrow>();

		List<Arrow> arrows = G.GetArrows();
		foreach (Arrow a in arrows)
		{
			if
			(
				earliestStart[a.GetBegin()] == latestStart[a.GetBegin()] &&
				earliestStart[a.GetEnd()] == latestStart[a.GetEnd()] &&
				earliestStart[a.GetEnd()] - earliestStart[a.GetBegin()] == a.GetWeight() &&
				latestStart[a.GetEnd()] - latestStart[a.GetBegin()] == a.GetWeight()
			)
			{
				criticalProcesses.Add(new Arrow(a));
			}
		}

		return criticalProcesses;
	}


	public static List<Arrow> GetCriticalPath(WeightedDiGraph G)
	{
		WeightedDiGraph subgraph = new WeightedDiGraph(GetCriticalProcesses(G));
		List<Arrow> outcomingArrows = subgraph.GetArrowsFrom(Sorter.GetTopologicalMin(G));
		List<Arrow> resultPath = new List<Arrow>();

		foreach (Arrow a in outcomingArrows)
		{
			DFS(subgraph, a, resultPath, Sorter.GetTopologicalMax(G));
		}

		resultPath.Reverse();
		return resultPath;
	}

	private static bool DFS
	(
		WeightedDiGraph G,
		Arrow current,
		List<Arrow> resultPath,
		int sink
	)
	{
		if (current.GetEnd() == sink)
		{
			resultPath.Add(new Arrow(current));
			return true;
		}

		List<Arrow> outcomingArrows = G.GetArrowsFrom(current.GetEnd());

		foreach (Arrow a in outcomingArrows)
		{
			if (DFS(G, a, resultPath, sink))
			{
				resultPath.Add(current);
				return true;
			}
		}

		return false;
	}
}


class GraphReader
{
	public static WeightedDiGraph read(string file)
	{
		string[] lines = System.IO.File.ReadAllLines(file);
		Arrow[] arrows = new Arrow[lines.Count()];

		for (int i = 0; i < lines.Count(); i++)
		{
			string[] words = lines[i].Split(' ');
			arrows[i] = new Arrow
			(
				Int32.Parse(words[0]),
				Int32.Parse(words[1]),
				double.Parse(words[2])
			);
		}

		return new WeightedDiGraph(arrows);
	}
}