using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day11 {
   internal class Program {
      static void Main(string[] args) {
         Part2();
      }

      static void Part1() {
         List<string> input = File.ReadAllLines("input.txt").ToList();
         List<string> expanded = new List<string>();

         {
            List<string> rowsExpanded = new List<string>();

            // Rows
            for (int i = 0; i < input.Count; i++) {
               if (input[i].All(c => c == '.')) {
                  rowsExpanded.Add(input[i]);
                  rowsExpanded.Add(input[i]);
                  continue;
               } else {
                  rowsExpanded.Add(input[i]);
               }
            }

            // Columns
            for (int i = 0; i < rowsExpanded[i].Length; i++) {
               string column = "";
               for (int j = 0; j < rowsExpanded.Count; j++) {
                  column += rowsExpanded[j][i];
               }

               if (column.All(c => c == '.')) {
                  expanded.Add(column);
                  expanded.Add(column); // I rotate space here but doesnt matter since distances are the same
               } else {
                  expanded.Add(column);
               }
            }
         }

         List<Galaxy> galaxies = new List<Galaxy>();
         for (int i = 0; i < expanded.Count; i++) {
            for (int j = 0; j < expanded[i].Length; j++) {
               if (expanded[i][j] == '#') {
                  galaxies.Add(new Galaxy(j, i, galaxies.Count));
               }
            }
         }

         List<int> distances = new List<int>();
         {
            //List<Galaxy> toCalculate = new List<Galaxy>(galaxies);

            //for (; ; ) {
            //   if (toCalculate.Count == 0) {
            //      break;
            //   }

            //   Galaxy current = toCalculate[0];

            //   for (int i = 1; i < toCalculate.Count; i++) {
            //      int distance = CalculateDistance(current, toCalculate[i]);
            //      distances.Add(distance);
            //   }

            //   toCalculate.RemoveAt(0);
            //}

            for (int i = 0; i < galaxies.Count; i++) {
               for (int j = i + 1; j < galaxies.Count; j++) {
                  int distance = CalculateDistance(galaxies[i], galaxies[j]);
                  distances.Add(distance);
               }
            }
         }

         Console.WriteLine(distances.Sum());
      }

      static void Part2() {
         List<string> input = File.ReadAllLines("input.txt").ToList();
         List<int> indicesOfDotRows = new List<int>();
         List<int> indicesOfDotColumns = new List<int>();

         // Rows
         for (int i = 0; i < input.Count; i++) {
            if (input[i].All(c => c == '.')) {
               indicesOfDotRows.Add(i);
            }
         }

         // Columns
         for (int i = 0; i < input[0].Length; i++) {
            string column = "";
            for (int j = 0; j < input.Count; j++) {
               column += input[j][i];
            }

            if (column.All(c => c == '.')) {
               indicesOfDotColumns.Add(i);
            }
         }

         List<Galaxy> galaxies = new List<Galaxy>();
         for (int i = 0; i < input.Count; i++) {
            for (int j = 0; j < input[i].Length; j++) {
               if (input[i][j] == '#') {
                  galaxies.Add(new Galaxy(j, i, galaxies.Count));
               }
            }
         }

         List<ulong> distances = new List<ulong>();
         {
            for (int i = 0; i < galaxies.Count; i++) {
               for (int j = i + 1; j < galaxies.Count; j++) {
                  ulong distance = CalculateDistance(galaxies[i], galaxies[j], indicesOfDotRows, indicesOfDotColumns, 1000000);
                  distances.Add(distance);
               }
            }
         }

         Console.WriteLine(distances.Aggregate((acc, x) => acc + x));
      }

      static int CalculateDistance(in Galaxy a, in Galaxy b) {
         return Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y);
      }

      static ulong CalculateDistance(in Galaxy a, in Galaxy b, in List<int> dotRows, in List<int> dotColumns, ulong expansionAmount) {
         ulong xDistance = 0;
         ulong yDistance = 0;

         // X
         Galaxy left = a.x < b.x ? a : b;
         Galaxy right = a.x < b.x ? b : a;
         for (int i = left.x; i < right.x; i++) {
            if (dotColumns.Contains(i)) {
               xDistance += expansionAmount;
            } else {
               xDistance++;
            }
         }

         // Y
         Galaxy top = a.y < b.y ? a : b;
         Galaxy bottom = a.y < b.y ? b : a;

         for (int i = top.y; i < bottom.y; i++) {
            if (dotRows.Contains(i)) {
               yDistance += expansionAmount;
            } else {
               yDistance++;
            }
         }

         return xDistance + yDistance;
      }
   }

   class Galaxy {
      public int x;
      public int y;
      public int id;

      public Galaxy(int x, int y, int id) {
         this.x = x;
         this.y = y;
         this.id = id;
      }
   }
}
