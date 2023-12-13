using System;
using System.IO;
using System.Linq;

namespace Day13 {
   internal class Program {
      static void Main(string[] args) {
         Part2();
      }

      static void Part1() {
         string input = File.ReadAllText("input.txt");
         string[][] terrains = input.Split("\r\n\r\n").Select(terrain => terrain.Split("\r\n")).ToArray();

         ulong sum = 0;
         {
            foreach (string[] terrain in terrains) {
               // Rows
               for (int i = 0; i < terrain.Length - 1; i++) {
                  if (IsThereMirror(i, terrain)) {
                     sum += (ulong)(i + 1) * 100;
                     break;
                  }
               }

               // Columns
               string[] transposed = Transpose(terrain);
               for (int i = 0; i < transposed.Length - 1; i++) {
                  if (IsThereMirror(i, transposed)) {
                     sum += (ulong)i + 1;
                     break;
                  }
               }
            }
         }

         Console.WriteLine(sum);
      }

      static void Part2() {
         string input = File.ReadAllText("input.txt");
         string[][] terrains = input.Split("\r\n\r\n").Select(terrain => terrain.Split("\r\n")).ToArray();

         ulong sum = 0;
         {
            foreach (string[] terrain in terrains) {
               // Rows
               for (int i = 0; i < terrain.Length - 1; i++) {
                  if (IsThereSmudgedMirror(i, terrain)) {
                     sum += (ulong)(i + 1) * 100;
                     break;
                  }
               }

               // Columns
               string[] transposed = Transpose(terrain);
               for (int i = 0; i < transposed.Length - 1; i++) {
                  if (IsThereSmudgedMirror(i, transposed)) {
                     sum += (ulong)i + 1;
                     break;
                  }
               }
            }
         }

         Console.WriteLine(sum);
      }

      static bool IsThereMirror(int topIndex, string[] terrain) {
         for (int i = 0; ; i++) {
            try {
               if (terrain[topIndex - i] != terrain[topIndex + i + 1]) {
                  return false;
               }
            } catch (IndexOutOfRangeException) {
               return true;
            }
         }
      }

      static bool IsThereSmudgedMirror(int topIndex, string[] terrain) {
         int totalDifferences = 0;

         for (int i = 0; ; i++) {
            if (totalDifferences > 1) {
               return false;
            }

            try {
               totalDifferences += FindNumberOfDifferences(terrain[topIndex - i], terrain[topIndex + i + 1]);
            } catch (IndexOutOfRangeException) {
               return totalDifferences == 1;
            }
         }
      }

      static int FindNumberOfDifferences(string a, string b) {
         int differences = 0;
         for (int i = 0; i < a.Length; i++) {
            if (a[i] != b[i]) {
               differences++;
            }
         }

         return differences;
      }

      static string[] Transpose(string[] array) {
         if (array.Length == 0)
            return new string[0];

         int rowCount = array.Length;
         int colCount = array[0].Length;

         string[] result = new string[colCount];

         for (int i = 0; i < colCount; i++) {
            char[] colChars = new char[rowCount];
            for (int j = 0; j < rowCount; j++) {
               colChars[j] = array[j][i];
            }
            result[i] = new string(colChars);
         }

         return result;
      }
   }
}
