using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using netplanlib;

namespace netplanmain {
	public class Start {
		static void Main(string[] args)
		{
			try
			{
				int n = int.Parse(args[0]);
				List<Arrow> arrows = new List<Arrow>();

				for (int i = 1; i <= n; i++)
					for (int j = 1; j <= n; j++)
					{
						double weight = Convert.ToDouble(args[(i - 1) * n + j]);

						if (weight >= 0)
							arrows.Add(new Arrow(i, j, weight));
					}

				List<Arrow> cp = CPM.GetCriticalPath(new WeightedDiGraph(arrows));

				Console.Write(":\nCritical path: " + cp[0].GetBegin());

				double summWeight = 0;
				foreach (Arrow a in cp)
				{
					summWeight += a.GetWeight();
					Console.Write(", " + a.GetEnd());
				}
				Console.Write(";\nSumm weight = " + summWeight + ".\n\n");
			}
			catch (IndexOutOfRangeException e)
			{}
		}
	}
}