using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day16 {
   class Program {
      static HashSet<Int2> visited = new HashSet<Int2>();
      static HashSet<(Int2 previous, Int2 current)> cache = new HashSet<(Int2 previous, Int2 current)>();
      
      static void Main(string[] args) {
         Part2();
      }

      static void Part1() {
         char[][] input = File.ReadAllLines("input.txt").Select(line => line.ToCharArray()).ToArray();
         // foreach (char[] line in input) {
         //    foreach (char c in line) {
         //       System.Console.Write(c);
         //    }
         //    System.Console.WriteLine();
         // }

         StepIterative(input, new Int2(0, 0), new Int2(0, 1));

         Console.WriteLine(visited.Count);
      }

      static void Part2() {
         char[][] input = File.ReadAllLines("input.txt").Select(line => line.ToCharArray()).ToArray();
         int max = 0;

         for (int i = 0; i < input.Length; i++) {
            StepIterative(input, new Int2(i, 0), new Int2(0, 1));
            max = Math.Max(max, visited.Count);
            visited.Clear();
            cache.Clear();

            StepIterative(input, new Int2(i, input[0].Length - 1), new Int2(0, -1));
            max = Math.Max(max, visited.Count);
            visited.Clear();
            cache.Clear();

            Console.WriteLine($"Row {i}: {max}");
         }

         for (int i = 0; i < input[0].Length; i++) {
            StepIterative(input, new Int2(0, i), new Int2(1, 0));
            max = Math.Max(max, visited.Count);
            visited.Clear();
            cache.Clear();

            StepIterative(input, new Int2(input.Length - 1, i), new Int2(-1, 0));
            max = Math.Max(max, visited.Count);
            visited.Clear();
            cache.Clear();

            Console.WriteLine($"Column {i}: {max}");
         }

         Console.WriteLine(max);
      }

      // StackOverflow
      static void Step(in char[][] grid, Int2 previous, Int2 current) {
         if (current.x < 0 || current.x >= grid[0].Length || current.y < 0 || current.y >= grid.Length) {
            return;
         }

         if (cache.Contains((previous, current))) {
            return;
         }

         cache.Add((previous, current));
         
         visited.Add(current);         
         
         char currentChar = grid[current.y][current.x];
         switch (currentChar) {
            case '.': {
               Int2 next = GetNextPosition(previous, current);
               Step(grid, current, next);
               return;
            }

            case '/': {
               Int2 next = Reflect(previous, current, currentChar);
               Step(grid, current, next);
               return;
            }

            case '\\': {
               Int2 next = Reflect(previous, current, currentChar);
               Step(grid, current, next);
               return;
            }

            case '|': {
               Int2 delta = current - previous;
               if (delta.y != 0) {
                  Int2 next = GetNextPosition(previous, current);
                  Step(grid, current, next);
               } else {
                  Step(grid, current, current + new Int2(1, 0));
                  Step(grid, current, current + new Int2(-1, 0));
               }
               return;
            }

            case '-': {
               Int2 delta = current - previous;
               if (delta.x != 0) {
                  Int2 next = GetNextPosition(previous, current);
                  Step(grid, current, next);
               } else {
                  Step(grid, current, current + new Int2(0, 1));
                  Step(grid, current, current + new Int2(0, -1));
               }
               return;
            }

            default: {
               throw new Exception($"Unknown character {currentChar}");
            }
         }
      }

      static void StepIterative(in char[][] grid, Int2 start, Int2 direction) {
         Stack<(Int2 previous, Int2 current)> stack = new Stack<(Int2, Int2)>();

         stack.Push((start - direction, start));

         while (stack.Count > 0) {
            (Int2 previous, Int2 current) = stack.Pop();

            if (current.x < 0 || current.x >= grid[0].Length || current.y < 0 || current.y >= grid.Length) {
               continue;
            }

            if (cache.Contains((previous, current))) {
               continue;
            }

            cache.Add((previous, current));

            visited.Add(current);

            char currentChar = grid[current.y][current.x];
            switch (currentChar) {
               case '.': {
                  Int2 next = GetNextPosition(previous, current);
                  stack.Push((current, next));
                  continue;
               }

               case '/': {
                  Int2 next = Reflect(previous, current, currentChar);
                  stack.Push((current, next));
                  continue;
               }

               case '\\': {
                  Int2 next = Reflect(previous, current, currentChar);
                  stack.Push((current, next));
                  continue;
               }

               case '|': {
                  Int2 delta = current - previous;
                  if (delta.y != 0) {
                     Int2 next = GetNextPosition(previous, current);
                     stack.Push((current, next));
                  } else {
                     stack.Push((current, current + new Int2(1, 0)));
                     stack.Push((current, current + new Int2(-1, 0)));
                  }
                  continue;
               }

               case '-': {
                  Int2 delta = current - previous;
                  if (delta.x != 0) {
                     Int2 next = GetNextPosition(previous, current);
                     stack.Push((current, next));
                  } else {
                     stack.Push((current, current + new Int2(0, 1)));
                     stack.Push((current, current + new Int2(0, -1)));
                  }
                  continue;
               }

               default: {
                  throw new Exception($"Unknown character {currentChar}");
               }
            }
         }
      }

      static Int2 GetNextPosition(Int2 previous, Int2 current) {
         Int2 delta = current - previous;
         return current + delta;
      }

      static Int2 Reflect(Int2 previous, Int2 current, char mirror) {
         Int2 delta = current - previous;
         switch (mirror) {
            case '/': {
               if (delta.x == 1) {
                  return new Int2(previous.y - 1, previous.x + 1);             
               } else if (delta.x == -1) {                  
                  return new Int2(previous.y + 1, previous.x - 1);                  
               } else if (delta.y == 1) {                
                  return new Int2(previous.y + 1, previous.x - 1);                  
               } else if (delta.y == -1) {                  
                  return new Int2(previous.y - 1, previous.x + 1);                  
               } else {                
                  throw new Exception($"Invalid delta {delta}");                 
               }               
            }

            case '\\': {
               if (delta.x == 1) {
                  return new Int2(previous.y + 1, previous.x + 1);
               } else if (delta.x == -1) {
                  return new Int2(previous.y - 1, previous.x - 1);
               } else if (delta.y == 1) {
                  return new Int2(previous.y + 1, previous.x + 1);
               } else if (delta.y == -1) {
                  return new Int2(previous.y - 1, previous.x - 1);
               } else {
                  throw new Exception($"Invalid delta {delta}");
               }
            }

            default: {
               throw new Exception($"Unknown mirror {mirror}");
            }
         }
      }
   }

   struct Int2 {
      public int x;
      public int y;

      public Int2(int y, int x) {
         this.x = x;
         this.y = y;
      }

      public static Int2 operator +(in Int2 a, in Int2 b) {
         return new Int2(a.y + b.y, a.x + b.x);
      }

      public static Int2 operator -(in Int2 a, in Int2 b) {
         return new Int2(a.y - b.y, a.x - b.x);
      }

      public override string ToString() {
         return $"({x}, {y})";
      }
   }
}