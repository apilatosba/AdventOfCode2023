using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day23 {
   class Program {
      static void Main() {
         Part2();
      }

      static void Part1() {
         Tile[][] map = File.ReadAllLines("input.txt").Select(line => line.Select(c => (Tile)c).ToArray()).ToArray();

         Int2 start = new Int2();
         Int2 goal = new Int2();

         for (int x = 0; x < map[0].Length; x++) {
            if (map[0][x] == Tile.Path) {
               start = new Int2(0, x);
            }
         }

         for (int x = 0; x < map[map.Length - 1].Length; x++) {
            if (map[map.Length - 1][x] == Tile.Path) {
               goal = new Int2(map.Length - 1, x);
            }
         }

         // Dictionary<Int2, Int2> previous = AStarReversed(map, start, goal);

         // List<Int2> path = ReconstructPath(previous, goal);
         // Console.WriteLine(path.Count - 1);
         // foreach (Int2 position in path) {
         //    Console.WriteLine(position);
         // }

         List<List<Int2>> paths = BFS(map, start, goal);

         foreach (List<Int2> path in paths) {
            foreach (Int2 position in path) {
               Console.WriteLine(position);
            }
            Console.WriteLine("***********************************");
            Console.WriteLine("***********************************");
         }

         Console.WriteLine(paths.Count);
         foreach (List<Int2> path in paths) {
            Console.WriteLine(path.Count - 1);
         }

         Console.WriteLine($"Longest: {paths.Max(path => path.Count - 1)}");
      }

      static void Part2() {
         Tile[][] map = File.ReadAllLines("sample input.txt").Select(line => line.Select(c => {
            return c switch {
               '#' => Tile.Forest,
               _ => Tile.Path
            };
         }).ToArray()).ToArray();

         Int2 start = new Int2();
         Int2 goal = new Int2();

         for (int x = 0; x < map[0].Length; x++) {
            if (map[0][x] == Tile.Path) {
               start = new Int2(0, x);
            }
         }

         for (int x = 0; x < map[map.Length - 1].Length; x++) {
            if (map[map.Length - 1][x] == Tile.Path) {
               goal = new Int2(map.Length - 1, x);
            }
         }

         List<List<Int2>> paths = BFS(map, start, goal);

         foreach (List<Int2> path in paths) {
            foreach (Int2 position in path) {
               Console.WriteLine(position);
            }
            Console.WriteLine("***********************************");
            Console.WriteLine("***********************************");
         }

         Console.WriteLine(paths.Count);
         foreach (List<Int2> path in paths) {
            Console.WriteLine(path.Count - 1);
         }

         Console.WriteLine($"Longest: {paths.Max(path => path.Count - 1)}");
      }

      static List<List<Int2>> BFS(Tile[][] map, Int2 start, Int2 goal) {
         List<List<Int2>> paths = new List<List<Int2>>();
         Queue<List<Int2>> queue = new Queue<List<Int2>>();

         queue.Enqueue(new List<Int2>() { start });

         while (queue.Count > 0) {
            List<Int2> currentPath = queue.Dequeue();
            Int2 current = currentPath[currentPath.Count - 1];

            if (current == goal) {
               paths.Add(currentPath);
               continue;
            }

            foreach (Int2 neighbour in GetNeighbours(map, current)) {
               if (currentPath.Contains(neighbour)) {
                  continue;
               }

               List<Int2> newPath = new List<Int2>(currentPath);
               newPath.Add(neighbour);
               queue.Enqueue(newPath);
            }
         }         

         return paths;
      }

      static List<Int2> ReconstructPath(Dictionary<Int2, Int2> cameFrom, Int2 goal) {
         List<Int2> path = new List<Int2>();
         path.Add(goal);

         while (cameFrom.TryGetValue(goal, out Int2 previous)) {
            path.Add(previous);
            goal = previous;
         }

         path.Reverse();
         return path;
      }

      // Fails to find the longest path. No surprises there.
      static Dictionary<Int2, Int2> AStarReversed(Tile[][] map, Int2 start, Int2 goal) {
         HashSet<Int2> visited = new HashSet<Int2>();
         // SortedList<int, Int2> frontier = new SortedList<int, Int2>();
         SortedList<float, Int2> frontier = new SortedList<float, Int2>();
         Dictionary<Int2, Int2> cameFrom = new Dictionary<Int2, Int2>();
         Dictionary<Int2, int> gScore = new Dictionary<Int2, int>();

         gScore.Add(start, 0);
         frontier.Add(gScore[start] + HScore(start), start);

         float lidl = -0.001f;
         while (frontier.Count > 0) {
            // Get the highset
            Int2 current = frontier.Values[frontier.Count - 1];
            frontier.RemoveAt(frontier.Count - 1);

            if (current == goal) {
               return cameFrom;
            }

            visited.Add(current);

            foreach (Int2 neighbour in GetNeighbours(map, current)) {
               if (visited.Contains(neighbour)) {
                  continue;
               }

               int tentativeGScore = gScore[current] + 1;

               if (gScore.TryGetValue(neighbour, out int neighbourGScore)) {
                  if (tentativeGScore == neighbourGScore) {
                     Console.WriteLine("tentativeGScore == neighbourGScore. I am not sure which path to take.");
                  } else if (tentativeGScore > neighbourGScore) {
                     gScore[neighbour] = tentativeGScore;
                     cameFrom[neighbour] = current;
                  }
               } else {
                  gScore.Add(neighbour, tentativeGScore);
                  cameFrom.Add(neighbour, current);
               }

               if (!frontier.TryAdd(gScore[neighbour] + HScore(neighbour), neighbour)) {
                  frontier.Add(gScore[neighbour] + HScore(neighbour) + (lidl *= 2), neighbour);
               }
            }
         }

         throw new Exception("No path found");

         // Manhattan
         int HScore(Int2 position) {
            return Math.Abs(position.y - goal.y) + Math.Abs(position.x - goal.x);
            // return 0;
         }
      }

      static HashSet<Int2> GetNeighbours(Tile[][] map, Int2 position) {
         HashSet<Int2> neighbours = new HashSet<Int2>();

         Tile currentTile = map[position.y][position.x];

         switch (currentTile) {
            case Tile.Up: {
               CheckAndAddUp();
               break;
            }

            case Tile.Down: {
               CheckAndAddDown();
               break;
            }

            case Tile.Left: {
               CheckAndAddLeft();
               break;
            }

            case Tile.Right: {
               CheckAndAddRight();
               break;
            }

            case Tile.Path: {
               CheckAndAddUp();
               CheckAndAddDown();
               CheckAndAddLeft();
               CheckAndAddRight();
               break;
            }

            default: {
               throw new Exception($"Invalid tile type {currentTile}");
            }
         }

         return neighbours;

         void CheckAndAddUp() {
            if (position.y > 0 && map[position.y - 1][position.x] != Tile.Forest) {
               neighbours.Add(new Int2(position.y - 1, position.x));
            }
         }

         void CheckAndAddDown() {
            if (position.y < map.Length - 1 && map[position.y + 1][position.x] != Tile.Forest) {
               neighbours.Add(new Int2(position.y + 1, position.x));
            }
         }

         void CheckAndAddLeft() {
            if (position.x > 0 && map[position.y][position.x - 1] != Tile.Forest) {
               neighbours.Add(new Int2(position.y, position.x - 1));
            }
         }

         void CheckAndAddRight() {
            if (position.x < map[position.y].Length - 1 && map[position.y][position.x + 1] != Tile.Forest) {
               neighbours.Add(new Int2(position.y, position.x + 1));
            }
         }
      }

      static void PrintMap(Tile[][] map) {
         foreach (Tile[] row in map) {
            foreach (Tile tile in row) {
               Console.Write((char)tile);
            }
            Console.WriteLine();
         }
      }
   }

   enum Tile {
      Path = '.',
      Forest = '#',
      Up = '^',
      Down = 'v',
      Left = '<',
      Right = '>'
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
         return obj is Int2 other && this == other;
      }

      public override int GetHashCode() {
         return y.GetHashCode() ^ x.GetHashCode();
      }
   }
}