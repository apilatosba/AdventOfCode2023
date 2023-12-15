using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;

namespace Day14 {
   internal class Program {
      static Dictionary<string[], string[]> cache = new Dictionary<string[], string[]>(new PlatformComparer());
      static int fiot = int.MaxValue;

      static void Main(string[] args) {
         Part1();
      }

      static void Part1() {
         string[] input = File.ReadAllLines("input.txt");
         input = Day13.Program.Transpose(input);

         ulong totalCost = 0;
         foreach (string line in input) {
            totalCost += CalculateCostOfALine(line);
         }

         //for (int i = 0; i < input.Length; i++) {
         //   if (i == 1)
         //      totalCost += CalculateCostOfALine(input[i]);
         //}

         Console.WriteLine(totalCost);
      }

      static void Part2() {
         string[] input = File.ReadAllLines("input.txt");

         //string[] original = input.Clone() as string[];

         //input = Day13.Program.Transpose(input);

         //foreach (var item in input) {
         //   Console.WriteLine(item);
         //}
         //Console.WriteLine();

         //foreach (var item in original) {
         //   Console.WriteLine(item);
         //}

         //return;

         //input = Roll(input, Direction.Right);
         //foreach (var item in input) {
         //   Console.WriteLine(item);
         //}
         //return;

         for (int i = 0; i < 1000000000; i++) {
            input = Cycle(input);
            //input = Cycle(input/*, ref counter*/);
            //foreach (var item in input) {
            //   Console.WriteLine(item);
            //}
            //Console.WriteLine();
            //input = Cycle(input/*, ref counter*/);
            //foreach (var item in input) {
            //   Console.WriteLine(item);
            //}
            //break;
            ulong totalCost = 0;
            foreach (string line in input) {
               totalCost += CalculateCostOfALine(line);
            }

            Console.WriteLine(totalCost);
         }

         //ulong totalCost = 0;
         //foreach (string line in input) {
         //   totalCost += CalculateCostOfALine(line);
         //}

         //Console.WriteLine(totalCost);
      }

      static string[] Cycle(string[] platform) {
         if (cache.ContainsKey(platform)) {
            Console.WriteLine($"i have seen this one before {cache.Count} {fiot}");
            return cache[platform];
         }

         

         string[] next = (string[])platform.Clone();

         // North
         next = Day13.Program.Transpose(next);
         next = Roll(next, Direction.Left);

         // West
         next = Day13.Program.Transpose(next);
         next = Roll(next, Direction.Left);

         // South
         next = Day13.Program.Transpose(next);
         next = Roll(next, Direction.Right);

         // East
         next = Day13.Program.Transpose(next);
         next = Roll(next, Direction.Right);

         cache.Add(platform, next);
         return next;
      }

      static ulong CalculateCostOfALine(string line) {
         ulong cost = 0;
         CalculateAmountOfRolledRocks(0, ref cost);

         return cost;

         void CalculateAmountOfRolledRocks(int startIndex, ref ulong sum) {
            int rolledRocks = 0;

            for (int i = startIndex; i < line.Length; i++) {
               if (line[i] == 'O') {
                  rolledRocks++;
               } else if (line[i] == '#') {
                  sum += CalculateRolledCost(line.Length, rolledRocks, startIndex);

                  CalculateAmountOfRolledRocks(i + 1, ref sum);
                  return;
               }
            }

            sum += CalculateRolledCost(line.Length, rolledRocks, startIndex);
         }
      }

      static string[] Roll(string[] platform, Direction direction) {
         string[] rolledPlatform = CopyPlatformExceptRollingRocks(platform);
         
         List<RockInfo> rockInfos = new List<RockInfo>();

         for (int i = 0; i < platform.Length; i++) {
            switch (direction) {
               case Direction.Left: {
                  ReportRolledRocksInfo(platform[i], 0);

                  break;
               }

               case Direction.Right: {
                  ReportRolledRocksInfo(platform[i], platform[i].Length - 1);

                  break;
               }
            }

            for (int j = 0; j < rockInfos.Count; j++) {
               StringBuilder sb = new StringBuilder(rolledPlatform[i]);
               switch (direction) {
                  case Direction.Left: {
                     for (int k = rockInfos[j].index; k < rockInfos[j].index + rockInfos[j].amount; k++) {
                        sb[k] = 'O';
                     }

                     break;
                  }

                  case Direction.Right: {
                     for (int k = rockInfos[j].index; k > rockInfos[j].index - rockInfos[j].amount; k--) {
                        sb[k] = 'O';
                     }

                     break;
                  }
               }

               rolledPlatform[i] = sb.ToString();
            }

            rockInfos.Clear();
         }

         return rolledPlatform;

         void ReportRolledRocksInfo(string line, int startIndex) {
            int rolledRocks = 0;

            switch (direction) {
               case Direction.Left: {
                  for (int i = startIndex; i < line.Length; i++) {
                     if (line[i] == 'O') {
                        rolledRocks++;
                     } else if (line[i] == '#') {
                        if (rolledRocks > 0) {
                           rockInfos.Add(new RockInfo(startIndex, rolledRocks));
                        }

                        ReportRolledRocksInfo(line, i + 1);
                        return;
                     }
                  }

                  if (rolledRocks > 0) {
                     rockInfos.Add(new RockInfo(startIndex, rolledRocks));
                  }

                  break;
               }

               case Direction.Right: {
                  for (int i = startIndex; i >= 0; i--) {
                     if (line[i] == 'O') {
                        rolledRocks++;
                     } else if (line[i] == '#') {
                        if (rolledRocks > 0) {
                           rockInfos.Add(new RockInfo(startIndex, rolledRocks));
                        }

                        ReportRolledRocksInfo(line, i - 1);
                        return;
                     }
                  }

                  if (rolledRocks > 0) {
                     rockInfos.Add(new RockInfo(startIndex, rolledRocks));
                  }

                  break;
               }
            }
         }
      }

      static string CopyLineExceptRollingRocks(string line) {
         StringBuilder sb = new StringBuilder();

         foreach (char c in line) {
            if (c == 'O') {
               sb.Append('.');
            } else {
               sb.Append(c);
            }
         }

         return sb.ToString();
      }

      static string[] CopyPlatformExceptRollingRocks(string[] platform) {
         string[] copiedPlatform = new string[platform.Length];

         for (int i = 0; i < platform.Length; i++) {
            copiedPlatform[i] = CopyLineExceptRollingRocks(platform[i]);
         }

         return copiedPlatform;
      }

      static ulong CalculateRolledCost(int totalLength, int rolledRocks) {
         ulong cost = 0;

         for (int i = totalLength; i > totalLength - rolledRocks; i--) {
            cost += (ulong)i;
         }

         return cost;
      }

      static ulong CalculateRolledCost(int totalLength, int rolledRocks, int startIndex) {
         ulong cost = 0;

         for (int i = totalLength - startIndex; i > totalLength - rolledRocks - startIndex; i--) {
            cost += (ulong)i;
         }

         return cost;
      }

      static ulong CalculateCost(List<int> indices, int totalLength) {
         ulong cost = 0;

         foreach (int index in indices) {
            cost += (ulong)(totalLength - index);
         }

         return cost;
      }
   }

   enum Direction {
      Right,
      Left,
   }

   class RockInfo {
      public int index;
      public int amount;

      public RockInfo(int index, int amount) {
         this.index = index;
         this.amount = amount;
      }
   }

   class PlatformComparer : IEqualityComparer<string[]> {
      public bool Equals(string[] x, string[] y) {
         for (int i = 0; i < x.Length; i++) {
            if (x[i] != y[i]) {
               return false;
            }
         }

         return true;
      }

      public int GetHashCode([DisallowNull] string[] obj) {
         int hash = 17; // Choose a prime number as the initial value

         // Check if the array is null or empty
         if (obj == null || obj.Length == 0 || obj[0] == null || obj[0].Length == 0) {
            return hash;
         }

         // Loop through the 2D char array and calculate the hash
         foreach (string row in obj) {
            foreach (char c in row) {
               hash = hash * 31 + c.GetHashCode(); // Use a prime number multiplier
            }
         }

         return hash;
      }
   }
}
