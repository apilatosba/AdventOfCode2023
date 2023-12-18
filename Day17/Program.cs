using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day17 {
   class Program {
      //static Dictionary<Int2, ulong> distances = new Dictionary<Int2, ulong>();
      //static Dictionary<Int2, Int2> previouses = new Dictionary<Int2, Int2>();
      
      static void Main(string[] args) {
         Part1();
      }

      static void Part1() {
         int[][] input = File.ReadAllLines("sample input.txt").Select(line => {
            int[] row = new int[line.Length];
            for (int i = 0; i < line.Length; i++) {
               row[i] = int.Parse(line[i].ToString());
            }
            return row;
         }).ToArray();

         var result = Dijkstra(input);
         ;
         ulong lowestDistance = CalculateDistance(new Int2(input.Length - 1, input[input.Length - 1].Length - 2), result.previous, input);
         Console.WriteLine(lowestDistance);
      }

      static void Part2() {

      }

      static (ulong[][] distances, Int2?[][] previous) Dijkstra(int[][] map) {
         ulong[][] distances = new ulong[map.Length][];
         Int2?[][] previous = new Int2?[map.Length][];
         // SortedDictionary<ulong, List<Int2>> unvisited = new SortedDictionary<ulong, List<Int2>>();
         bool[][] visited = new bool[map.Length][];

         for (int i = 0; i < map.Length; i++) {
            distances[i] = new ulong[map[i].Length];
            previous[i] = new Int2?[map[i].Length];
            visited[i] = new bool[map[i].Length];
         }

         distances[0][0] = 0;
         previous[0][0] = null;
         visited[0][0] = true;

         for (int y = 0; y < map.Length; y++) {
            for (int x = 0; x < map[y].Length; x++) {
               if (x != 0 || y != 0) {
                  distances[y][x] = ulong.MaxValue;               
                  previous[y][x] = null;
                  visited[y][x] = false;
               }

               // AddToUnvisited(new Int2(y, x), distances[y][x]);
            }
         }

         while (GetVisitedCount() > 0) {
            Int2? current = GetLowestAndTagVisited();

            if (current == null) {
               break;
            }
            
            current = current.Value;
            
            List<Int2> neighbours = GetNeigbours(map, current.Value);

            if (neighbours.Any(n => n == new Int2(1, 0))) {
               ;
            }

            for (int i = 0; i < neighbours.Count; i++) {
               if (!IsVisited(neighbours[i])) {
                  ulong neighbourDistance = distances[current.Value.y][current.Value.x] + (ulong)map[neighbours[i].y][neighbours[i].x];

                  if (neighbourDistance < distances[neighbours[i].y][neighbours[i].x]) {
                     distances[neighbours[i].y][neighbours[i].x] = neighbourDistance;
                     previous[neighbours[i].y][neighbours[i].x] = current.Value;
                  }
               }
            }
         }

         // while (unvisited.Count > 0) {
         //    Int2 current = RemoveAndGetLowest();

         //    List<Int2> neighbours = GetNeigbours(map, current);

         //    for (int i = 0; i < neighbours.Count; i++) {
         //       if (IsVertexUnvisited(neighbours[i])) {
         //          ulong neighbourDistance = distances[current.y][current.x] + (ulong)map[neighbours[i].y][neighbours[i].x];

         //          if (neighbourDistance < distances[neighbours[i].y][neighbours[i].x]) {
         //             distances[neighbours[i].y][neighbours[i].x] = neighbourDistance;
         //             previous[neighbours[i].y][neighbours[i].x] = current;
         //          }
         //       }
         //    }
         // }

         return (distances, previous);

         // distances[0][0] = 0;
         // PriorityQueue<Int2, ulong> unvisited = new PriorityQueue<Int2, ulong>();

         // for (int y = 0; y < map.Length; y++) {
         //    for (int x = 0; x < map[y].Length; x++) {
         //       if (x != 0 || y != 0) {
         //          distances[y][x] = ulong.MaxValue;
         //          previous[y][x] = null;
         //       }

         //       unvisited.Enqueue(new Int2(y, x), distances[y][x]);
         //    }
         // }

         // while (unvisited.Count > 0) {
         //    Int2 current = unvisited.Dequeue();
         //    List<Int2> neighbours = GetNeigbours(map, current);

         //    for (int i = 0; i < neighbours.Count; i++) {
         //       ulong neigbourDistance = distances[current.y][current.x] + (ulong)map[neighbours[i].y][neighbours[i].x];

         //       if (neigbourDistance < distances[neighbours[i].y][neighbours[i].x]) {
         //          distances[neighbours[i].y][neighbours[i].x] = neigbourDistance;
         //          previous[neighbours[i].y][neighbours[i].x] = current;
         //          unvisited
         //       }
         //    }
         // }

         int GetVisitedCount() {
            int count = 0;

            for (int y = 0; y < visited.Length; y++) {
               for (int x = 0; x < visited[y].Length; x++) {
                  if (visited[y][x]) {
                     count++;
                  }
               }
            }

            return count;
         }

         bool IsVisited(Int2 vertex) {
            return visited[vertex.y][vertex.x];
         }

         Int2? GetLowestAndTagVisited() {
            ulong lowestDistance = ulong.MaxValue;
            Int2? lowest = null;

            for (int y = 0; y < distances.Length; y++) {
               for (int x = 0; x < distances[y].Length; x++) {
                  if (!visited[y][x] && distances[y][x] < lowestDistance) {
                     lowestDistance = distances[y][x];
                     lowest = new Int2(y, x);
                  }
               }
            }

            if (lowest == null) {
               return null;
            }

            visited[lowest.Value.y][lowest.Value.x] = true;
            return lowest;
         }

         // void AddToUnvisited(Int2 vertex, ulong distance) {
         //    if (unvisited.ContainsKey(distance)) {
         //       unvisited[distance].Add(vertex);
         //    } else {
         //       unvisited.Add(distance, new List<Int2>() { vertex });
         //    }
         // }

         // Int2 RemoveAndGetLowest() {
         //    ulong lowestKey = unvisited.Keys.First();
         //    Int2 lowest = unvisited[lowestKey][unvisited[lowestKey].Count - 1];
         //    unvisited[lowestKey].RemoveAt(unvisited[lowestKey].Count - 1);

         //    if (unvisited[lowestKey].Count == 0) {
         //       unvisited.Remove(lowestKey);
         //    }

         //    return lowest;  
         // }

         // // TODO only check the keys that are higher than the current distance
         // bool IsVertexUnvisited(Int2 vertex) {
         //    foreach (var kvp in unvisited) {
         //       if (kvp.Value.Contains(vertex)) {
         //          return true;
         //       }
         //    }

         //    return false;
         // }
      }

      static ulong CalculateDistance(Int2 destination, Int2?[][] previous, int[][] map) {
         ulong distance = 0;
         Int2? current = destination;
         
         while (current != null) {
            System.Console.WriteLine(current.Value);
            distance += (ulong)map[current.Value.y][current.Value.x];
            current = previous[current.Value.y][current.Value.x];
         }

         return distance;
      }

      static List<Int2> GetNeigbours(int[][] map, Int2 vertex) {
         List<Int2> neighbours = new List<Int2>();

         if (vertex.y > 0) {
            neighbours.Add(new Int2(vertex.y - 1, vertex.x));
         }

         if (vertex.y < map.Length - 1) {
            neighbours.Add(new Int2(vertex.y + 1, vertex.x));
         }

         if (vertex.x > 0) {
            neighbours.Add(new Int2(vertex.y, vertex.x - 1));
         }

         if (vertex.x < map[vertex.y].Length - 1) {
            neighbours.Add(new Int2(vertex.y, vertex.x + 1));
         }

         return neighbours;
      }
   }

   struct Int2 {
      public int y;
      public int x;

      public Int2(int y, int x) {
         this.y = y;
         this.x = x;
      }

      public static Int2 operator +(in Int2 a, in Int2 b) {
         return new Int2(a.y + b.y, a.x + b.x);
      }

      public static Int2 operator -(in Int2 a, in Int2 b) {
         return new Int2(a.y - b.y, a.x - b.x);
      }

      public static bool operator ==(in Int2 a, in Int2 b) {
         return a.y == b.y && a.x == b.x;
      }

      public static bool operator !=(in Int2 a, in Int2 b) {
         return a.y != b.y || a.x != b.x;
      }

      public override string ToString() {
         return $"({y}, {x})";
      }

      public override bool Equals(object obj) {
         return obj is Int2 i && i == this;
      }

      public override int GetHashCode() {
         return y.GetHashCode() ^ x.GetHashCode();
      }
   }
}