using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


struct Edge {
	private int begin;
	private int end;
	private int weight;

	public Edge(int begin, int end, int weight)
	{
		this.begin = begin;
		this.end = end;
		this.weight = weight;
	}

	public Edge(int begin, int end)
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

	public int GetWeight()
	{
		return weight;
	}

	public override string ToString()
	{
		return "(" + begin + ", " + end + ", weight=" + weight + ")";
	}

	public override bool Equals(object obj) 
	{
		if (!(obj is Edge))
			return false;

		Edge e = (Edge) obj;
		return e.begin == this.begin && e.end == this.end;
	}

	public override int GetHashCode()
	{
		return begin + end * 317;
	}
}


struct WeightedDiGraph
{
	private List<Edge> edges;

	public WeightedDiGraph(Edge[] edgesArr)
	{
		edges = new List<Edge>(edgesArr);
	}

	public List<Edge> GetEdges()
	{
		return edges;
	}

	public List<Edge> GetArrowsFrom(int i)
	{
		List<Edge> result = new List<Edge>();

		foreach (Edge e in edges)
		{
			if (e.GetBegin() == i)
				result.Add(e);
		}

		return result;
	}

	public List<Edge> GetArrowsTo(int i)
	{
		List<Edge> result = new List<Edge>();

		foreach (Edge e in edges)
		{
			if (e.GetEnd() == i)
				result.Add(e);
		}

		return result;
	}

	public bool IsAdjacent(int i, int j)
	{
		return edges.Contains(new Edge(i, j));
	}
}




class CPM
{
/*	public List<Edge> GetCP(WeightedDiGraph G)
	{
		List<Edge> earliestStart = new int[G.GetEdges().Count];
		int[] earliestFinish = new int[G.GetEdges().Count];

		int[] latestStart = new int[G.GetEdges().Count];
		int[] latestFinish = new int[G.GetEdges().Count];

		earliestStart[0] = 0;
	}*/
}




class CPMTester
{
	static void Main(string[] args)
	{
		WeightedDiGraph G_1 = new WeightedDiGraph(
			new Edge[] {
				new Edge(1, 2, 3),
				new Edge(1, 3, 2),
				new Edge(1, 5, 3),
				new Edge(1, 8, 4),

				new Edge(2, 3),
				new Edge(3, 4, 2),
				new Edge(4, 6, 2),

				new Edge(6, 7, 2),
				new Edge(5, 7, 1),

				new Edge(7, 8, 2),

				new Edge(8, 9, 4)
			}
		);

		G_1.GetEdges().Add(new Edge(1, 2, 4));

		System.Console.Write(G_1.GetEdges()[0]);
	}
}
