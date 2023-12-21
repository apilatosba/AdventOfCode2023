using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day21 {
   class Program {
      static void Main() {
         Part2();
      }

      static void Part1() {
         char[][] input = File.ReadAllLines("input.txt").Select(line => line.ToCharArray()).ToArray();

         Int2 start = new Int2();
         for (int y = 0; y < input.Length; y++) {
            for (int x = 0; x < input[y].Length; x++) {
               if (input[y][x] == 'S') {
                  start.y = y;
                  start.x = x;
                  goto ok;
               }
            }
         }
      ok:


         List<int>[][] visitStepInfos = BFS(input, start, 64);
         List<Int2> nodesThatCanBeReachedInExactly64Steps = new List<Int2>();

         for (int y = 0; y < visitStepInfos.Length; y++) {
            for (int x = 0; x < visitStepInfos[y].Length; x++) {
               if (visitStepInfos[y][x].Any(depth => depth % 2 == 0)) {
                  nodesThatCanBeReachedInExactly64Steps.Add(new Int2(y, x));
               }
            }
         }

         Console.WriteLine(nodesThatCanBeReachedInExactly64Steps.Count);
      }

      static void Part2() {
         char[][] input = File.ReadAllLines("sample input.txt").Select(line => line.ToCharArray()).ToArray();

         Int2 start = new Int2();
         for (int y = 0; y < input.Length; y++) {
            for (int x = 0; x < input[y].Length; x++) {
               if (input[y][x] == 'S') {
                  start.y = y;
                  start.x = x;
               }
            }
         }

         int?[][] visited = BFS2(input, start);

         PrintDepthMap(visited);
      }

      static int?[][] BFS2(char[][] map, Int2 start) {
         int?[][] visited = new int?[map.Length][];

         for (int y = 0; y < map.Length; y++) {
            visited[y] = new int?[map[y].Length];
            for (int x = 0; x < map[y].Length; x++) {
               visited[y][x] = null;
            }
         }

         Queue<(Int2 position, int depth)> queue = new Queue<(Int2, int)>();
         queue.Enqueue((start, 0));

         while (queue.Count > 0) {
            (Int2 position, int depth) = queue.Dequeue();

            if (visited[position.y][position.x] != null) continue;

            visited[position.y][position.x] = depth;

            List<Int2> neighbours = GetNeighbours(position, map);

            foreach (Int2 neighbour in neighbours) {
               if (visited[neighbour.y][neighbour.x] != null) continue;
               // if (isOdd && depth % 2 == 0) continue;
               // if (!isOdd && depth % 2 == 1) continue;
               queue.Enqueue((neighbour, depth + 1));
            }
         }

         return visited;
      }

      // static int BFSSelfDepth

      static void PrintDepthMap(int?[][] depthMap) {
         for (int y = 0; y < depthMap.Length; y++) {
            for (int x = 0; x < depthMap[y].Length; x++) {
               if (depthMap[y][x] == null) {
                  Console.Write("## ");
               } else {
                  Console.Write($"{depthMap[y][x]:00} ");
               }
            }
            Console.WriteLine();
         }
      }

      static List<int>[][] BFS(char[][] map, Int2 start, int maxDepth) {
         List<int>[][] visitStepInfos = new List<int>[map.Length][];

         for (int y = 0; y < map.Length; y++) {
            visitStepInfos[y] = new List<int>[map[y].Length];
            for (int x = 0; x < map[y].Length; x++) {
               visitStepInfos[y][x] = new List<int>();
            }
         }

         Queue<(Int2 position, int depth)> queue = new Queue<(Int2, int)>();
         queue.Enqueue((start, 0));

         for (; ; ) {
            (Int2 position, int depth) = queue.Dequeue();

            Console.WriteLine($"({position.y}, {position.x}) - {depth}");

            if (depth > maxDepth) break;

            visitStepInfos[position.y][position.x].Add(depth);

            List<Int2> neighbours = GetNeighbours(position, map);

            foreach (Int2 neighbour in neighbours) {
               if (visitStepInfos[neighbour.y][neighbour.x].Contains(depth + 1)) continue;
               // if (visitStepInfos[neighbour.y][neighbour.x].Count > 0) continue;
               // if (visitStepInfos[neighbour.y][neighbour.x].Any(d => d % 2 == maxDepth % 2) && queue.Any((n) => n.position == neighbour)) continue;
               // if (visitStepInfos[neighbour.y][neighbour.x].Any(d => d % 2 == maxDepth % 2) && (depth + 1) % 2 == maxDepth % 2) continue;
               if (visitStepInfos[neighbour.y][neighbour.x].Any(d => (d % 2) == ((depth + 1) % 2))) continue;
               if (queue.Any(n => (n.position == neighbour) && ((n.depth % 2) == ((depth + 1) % 2)))) continue;
               queue.Enqueue((neighbour, depth + 1));
            }
         }

         return visitStepInfos;
      }

      static List<Int2> GetNeighbours(Int2 a, char[][] map) {
         List<Int2> neighbours = new List<Int2>();

         // Left
         if (a.x > 0 && map[a.y][a.x - 1] != '#') {
            neighbours.Add(new Int2(a.y, a.x - 1));
         }

         // Right
         if (a.x < map[a.y].Length - 1 && map[a.y][a.x + 1] != '#') {
            neighbours.Add(new Int2(a.y, a.x + 1));
         }

         // Up
         if (a.y > 0 && map[a.y - 1][a.x] != '#') {
            neighbours.Add(new Int2(a.y - 1, a.x));
         }

         // Down
         if (a.y < map.Length - 1 && map[a.y + 1][a.x] != '#') {
            neighbours.Add(new Int2(a.y + 1, a.x));
         }

         return neighbours;
      }

      static List<Int2> GetNeighbours2(Int2 a, char[][] map) {
         List<Int2> neighbours = new List<Int2>();

         // Left
         if (a.x == 0) {
            neighbours.Add(new Int2(a.y, map[a.y].Length - 1));
         } else if (map[a.y][a.x - 1] != '#') {
            neighbours.Add(new Int2(a.y, a.x - 1));
         }

         // Right
         if (a.x == map[a.y].Length - 1) {
            neighbours.Add(new Int2(a.y, 0));
         } else if (map[a.y][a.x + 1] != '#') {
            neighbours.Add(new Int2(a.y, a.x + 1));
         }

         // Up
         if (a.y == 0) {
            neighbours.Add(new Int2(map.Length - 1, a.x));
         } else if (map[a.y - 1][a.x] != '#') {
            neighbours.Add(new Int2(a.y - 1, a.x));
         }

         // Down
         if (a.y == map.Length - 1) {
            neighbours.Add(new Int2(0, a.x));
         } else if (map[a.y + 1][a.x] != '#') {
            neighbours.Add(new Int2(a.y + 1, a.x));
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

      public override string ToString() {
         return $"({y}, {x})";
      }

      public static bool operator ==(Int2 a, Int2 b) {
         return a.y == b.y && a.x == b.x;
      }

      public static bool operator !=(Int2 a, Int2 b) {
         return !(a == b);
      }

      public override bool Equals(object obj) {
         throw new NotImplementedException();
      }

      public override int GetHashCode() {
         throw new NotImplementedException();
      }
   }
}