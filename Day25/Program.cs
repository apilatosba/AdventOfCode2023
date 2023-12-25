using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day25 {
   class Program {
      static void Main() {
         Part1();
      }

      static void Part1() {
         string[] input = File.ReadAllLines("input.txt");
         Dictionary<string, List<string>> graph = new Dictionary<string, List<string>>();

         // using graphviz neato gives perfect visualization for this problem
         string[][] pairs = new string[][] {
            new string[] { "bbp", "dvr"},
            new string[] { "jzv", "qvq"},
            new string[] { "gtj", "tzj"}
         };

         foreach (string line in input) {
            string[] parts = line.Split(": ");
            string main = parts[0];
            string[] connections = parts[1].Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            if (!graph.ContainsKey(main)) {
               graph.Add(main, new List<string>(connections));
            } else {
               graph[main].AddRange(connections);
            }

            foreach (string connection in connections) {
               if (!graph.ContainsKey(connection)) {
                  graph.Add(connection, new List<string>() { main });
               } else {
                  graph[connection].Add(main);
               }
            }
         }

         // File.WriteAllLines("graphviz.gv", ToGraphviz(graph));

         foreach (string[] pair in pairs) {
            graph[pair[0]].Remove(pair[1]);
            graph[pair[1]].Remove(pair[0]);
         }

         // File.WriteAllLines("graphviz.gv", ToGraphviz(graph));


         // Count one island
         HashSet<string> visited = new HashSet<string>();
         Queue<string> frontier = new Queue<string>();

         frontier.Enqueue(graph.Keys.First()); // start from any node
         while (frontier.Count > 0) {
            string current = frontier.Dequeue();
            visited.Add(current);

            foreach (string connection in graph[current]) {
               if (!visited.Contains(connection)) {
                  frontier.Enqueue(connection);
               }
            }
         }

         int total = graph.Keys.Count;
         Console.WriteLine((total - visited.Count) * visited.Count);
      }

      static void Part2() {
      }

      static string[] ToGraphviz(Dictionary<string, List<string>> graph) {
         List<string> lines = new List<string>();
         lines.Add("digraph {");

         foreach (KeyValuePair<string, List<string>> kvp in graph) {
            foreach (string connection in kvp.Value) {
               lines.Add($"   {kvp.Key} -> {connection};");
            }
         }

         lines.Add("}");
         return lines.ToArray();
      }
   }
}