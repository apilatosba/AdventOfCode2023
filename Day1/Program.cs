using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Day1 {
   internal class Program {
      static void Main(string[] args) {
         Part2();
      }

      static void Part1() {
         List<byte> calibrationValues = new List<byte>();

         string[] input = File.ReadAllLines("input.txt");

         for (int i = 0; i < input.Length; i++) {
            FindFirstAndLastDigit(input[i], out char first, out char last);
            calibrationValues.Add(byte.Parse(new string(new char[2] { first, last })));
         }

         Console.WriteLine(calibrationValues.Select(i => (decimal)i).Sum());
      }

      static void Part2() {
         string[] input = File.ReadAllLines("input.txt");

         ulong sum = 0;

         for (int i = 0; i < input.Length; i++) {
            FindValue(input[i], out byte calibrationValue);
            sum += calibrationValue;
         }

         Console.WriteLine(sum);
      }

      static void FindValue(string line, out byte calibrationValue) {
         Dictionary<string, byte> englishDigits = new Dictionary<string, byte>() {
            //{"zero", 0 },
            {"one", 1},
            {"two", 2},
            {"three", 3 },
            {"four", 4},
            {"five", 5 },
            {"six", 6 },
            {"seven", 7 },
            {"eight", 8 },
            {"nine", 9 }
         };

         List<Digit> digits = new List<Digit>();

         for (int i = 0; i < englishDigits.Count; i++) {
            Regex regex = new Regex(englishDigits.Keys.ElementAt(i));
            MatchCollection matches = regex.Matches(line);

            foreach (Match match in matches) {
               digits.Add(new Digit(englishDigits.Values.ElementAt(i), (uint)match.Index));
            }
         }

         for (int i = 0; i < line.Length; i++) {
            if (IsDigit(line[i])) {
               digits.Add(new Digit(byte.Parse(line[i].ToString()), (uint)i));
            }
         }

         digits.Sort((a, b) => a.index.CompareTo(b.index));

         byte first = digits[0].value;
         byte last = digits[digits.Count - 1].value;

         calibrationValue = byte.Parse(first.ToString() + last.ToString());
      }

      static void FindFirstAndLastDigit(string line, out char first, out char last) {
         const char nullChar = '\0';

         first = nullChar;
         last = nullChar;

         foreach (char c in line) {
            if (IsDigit(c)) {
               if (first == nullChar) {
                  first = c;
               }

               last = c;
            }
         }
      }

      static bool IsDigit(char c) {
         return c >= '0' && c <= '9';
      }
   }

   public struct Digit {
      public byte value;
      public uint index;

      public Digit(byte value, uint index) {
         this.value = value;
         this.index = index;
      }
   }
}
