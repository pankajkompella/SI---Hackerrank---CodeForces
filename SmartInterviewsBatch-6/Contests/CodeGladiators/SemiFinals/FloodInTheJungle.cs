﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartInterviewsBatch_6.Contests.CodeGladiators.SemiFinals
{
    public class FloodInTheJungle : ISolution
    {
        class Node
        {
            public int ArtiVertex { get; set; }
            public int MonkeyCnt { get; set; }
            public double X { get; set; }
            public double Y { get; set; }
            public int Threshold { get; set; }
        }

        public static int V = 0;
        public static int[,] twodGraph = null;
        public static Dictionary<int, int> map;
        static Node[] graph = null;
        public override void Solve()
        {
            map = new Dictionary<int, int>();
            Dictionary<int, int> capacity = new Dictionary<int, int>();
            //var NandC = ReadArrString().Select(int.Parse).ToArray();
            var NandC1 = ReadArrString();
            var NandC = new int[1];
            NandC[0] = int.Parse(NandC1[0]);
            //V = NandC[0] * 2 + 2;
            V = NandC[0] * 2 + 2 + NandC[0];
            graph = new Node[NandC[0]];
            //twodGraph = new int[NandC[0] * 2 + 2, NandC[0] * 2 + 2];
            twodGraph = new int[NandC[0] * 2 + 2 + NandC[0], NandC[0] * 2 + 2 + NandC[0]];
            var computedGraph = new bool[NandC[0], NandC[0]];
            int totalMonkeyCnt = 0, nonZeroMonkeyTrees = 0, invalidTrees = 0;
            for (int i = 0; i < NandC[0]; i++)
            {
                var info = ReadArrString().Select(int.Parse).ToArray();
                totalMonkeyCnt += info[2];
                if (info[2] > 0)
                {
                    nonZeroMonkeyTrees++;
                }
                if (info[2] > info[3])
                    invalidTrees++;
                graph[i] = new Node
                {

                    X = info[0],
                    Y = info[1],
                    MonkeyCnt = info[2],
                    Threshold = info[3]
                };
            }
            int index = 0;
            for (int i = 0; i < NandC[0]; i++)
            {
                for (int j = 0; j < NandC[0]; j++)
                {
                    if (i != j)
                    {
                        double ydist = Math.Abs(graph[j].Y - graph[i].Y);
                        ydist *= ydist;
                        double xdist = Math.Abs(graph[j].X - graph[i].X);
                        xdist *= xdist;
                        double dist = Math.Sqrt(ydist + xdist);
                        //out vertex to out vertex
                        //if (dist <= Convert.ToDouble(NandC1[1]))
                        if (dist <= Convert.ToDouble(NandC1[1]) && (!computedGraph[i, j] || !computedGraph[j, i]))
                        {
                            int ArtiVertexJ = 0, ArtiVertexI = i;
                            if (map.ContainsKey(i))
                            {
                                ArtiVertexI = map[i];
                                twodGraph[ArtiVertexI, j] = capacity[ArtiVertexI];
                            }
                            else
                            {
                                twodGraph[ArtiVertexI, j] = graph[ArtiVertexI].Threshold;
                            }

                            if (map.ContainsKey(j))
                            {
                                ArtiVertexJ = map[j];
                            }
                            else
                            {
                                ArtiVertexJ = NandC[0] * 2 + 2 + index;
                                map.Add(j, ArtiVertexJ);
                                capacity.Add(ArtiVertexJ, graph[j].Threshold);
                                index++;
                            }

                            //twodGraph[ArtiVertexI, j] = graph[i].Threshold;
                            //twodGraph[j, i] = graph[j].Threshold;

                            twodGraph[j, ArtiVertexJ] = graph[j].Threshold;
                            twodGraph[ArtiVertexJ, ArtiVertexI] = graph[j].Threshold;
                            computedGraph[i, j] = computedGraph[j, i] = true;
                        }
                    }
                }
                //in vertex to out vertex
                twodGraph[NandC[0] + i, i] = graph[i].MonkeyCnt;
                //super source node to every in vertex
                twodGraph[NandC[0] * 2, NandC[0] + i] = int.MaxValue;
            }

            Check(NandC[0], totalMonkeyCnt);
            Console.Write(output);
        }

        private void Check(int N, int totalMonkeyCnt)
        {
            int totalMeetingPts = 0;
            //end point
            for (int i = 0; i < N; i++)
            {
                // create a super sink with infinite capacity
                twodGraph[map.ContainsKey(i) ? map[i] : i, N * 2 + 1] = int.MaxValue;
                // check from super source to super sink
                int maxCost = fordFulkerson(twodGraph, N * 2, N * 2 + 1);
                if (totalMonkeyCnt == maxCost)
                {
                    totalMeetingPts++;
                    if (i == N - 1)
                    {
                        output.Append(i);
                    }
                    else
                        output.Append(i + " ");
                }

                // set the capacity of super sink to 0 so that you can change the super sink
                twodGraph[map.ContainsKey(i) ? map[i] : i, N * 2 + 1] = 0;
            }
            if (totalMeetingPts == 0)
                output.Append("-1");
        }

        private bool bfs(int[,] rGraph, int s, int t, int[] parent)
        {
            // Create a visited array and mark all vertices as not visited
            bool[] visited = new bool[V];

            // Create a queue, enqueue source vertex and mark source vertex
            // as visited
            Queue<int> q = new Queue<int>(V);
            q.Enqueue(s);
            visited[s] = true;
            parent[s] = -1;

            // Standard BFS Loop
            while (!q.IsEmpty())
            {
                int u = q.Peek();
                q.Dequeue();

                for (int v = 0; v < V; v++)
                {
                    if (visited[v] == false && rGraph[u, v] > 0)
                    {
                        q.Enqueue(v);
                        parent[v] = u;
                        visited[v] = true;
                    }
                }
            }

            // If we reached sink in BFS starting from source, then return
            // true, else false
            return (visited[t] == true);
        }

        private int fordFulkerson(int[,] graph, int s, int t)
        {
            int u, v;

            // Create a residual graph and fill the residual graph with
            // given capacities in the original graph as residual capacities
            // in residual graph
            int[,] rGraph = new int[V, V]; // Residual graph where rGraph[i][j] indicates 
                                           // residual capacity of edge from i to j (if there
                                           // is an edge. If rGraph[i][j] is 0, then there is not)  
            for (u = 0; u < V; u++)
                for (v = 0; v < V; v++)
                    rGraph[u, v] = graph[u, v];

            int[] parent = new int[V];  // This array is filled by BFS and to store path

            int max_flow = 0;  // There is no flow initially

            // Augment the flow while tere is path from source to sink
            while (bfs(rGraph, s, t, parent))
            {
                // Find minimum residual capacity of the edges along the
                // path filled by BFS. Or we can say find the maximum flow
                // through the path found.
                int path_flow = int.MaxValue;
                for (v = t; v != s; v = parent[v])
                {
                    u = parent[v];
                    path_flow = Math.Min(path_flow, rGraph[u, v]);
                }

                // update residual capacities of the edges and reverse edges
                // along the path
                for (v = t; v != s; v = parent[v])
                {
                    u = parent[v];

                    //rGraph[u, v] -= path_flow;
                    for (int x = 0; x < V; x++) {
                        rGraph[u, x] -= path_flow;
                    }
                    rGraph[v, u] += path_flow;
                }

                // Add path flow to overall flow
                max_flow += path_flow;
            }

            // Return the overall flow
            return max_flow;
        }
    }
}
