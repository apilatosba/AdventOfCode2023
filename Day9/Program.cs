using System;
using System.Collections.Generic;
using System.IO;

namespace Day9 {
   internal class Program {
      static void Main(string[] args) {
         Part2();
      }

      static void Part1() {
         string[] input = File.ReadAllLines("input.txt");
         List<History> histories = new List<History>(input.Length);

         foreach(string line in input) {
            List<long> numbers = new List<long>();

            foreach(string number in line.Split(' ')) {
               numbers.Add(long.Parse(number));
            }

            histories.Add(new History(numbers));
         }

         long sum = 0;
         foreach(History history in histories) {
            Extrapolate(history.numbers, ref sum);
         }

         Console.WriteLine(sum);
      }

      static void Part2() {
         string[] input = File.ReadAllLines("input.txt");
         List<History> histories = new List<History>(input.Length);

         foreach (string line in input) {
            List<long> numbers = new List<long>();

            foreach (string number in line.Split(' ')) {
               numbers.Add(long.Parse(number));
            }

            histories.Add(new History(numbers));
         }

         long sum = 0;
         foreach (History history in histories) {
            Extrapolate2(history.numbers, ref sum, true);
         }

         Console.WriteLine(sum);
      }

      static void Extrapolate(List<long> numbers, ref long result) {
         // Termination
         {
            if (numbers.Count == 1 && numbers[0] != 0) {
               throw new Exception("waduhek");
            }

            if (!numbers.Exists(n => n != 0)) {
               return;
            }
         }

         // Step
         {
            result += numbers[numbers.Count - 1];

            List<long> next = new List<long>();

            for (int i = 0; i < numbers.Count - 1; i++) {
               next.Add(numbers[i + 1] - numbers[i]);
            }

            Extrapolate(next, ref result);
         }
      }

      static void Extrapolate2(List<long> numbers, ref long result, bool isEven) {
         // Termination
         {
            if (numbers.Count == 1 && numbers[0] != 0) {
               throw new Exception("waduhek");
            }

            if (!numbers.Exists(n => n != 0)) {
               return;
            }
         }

         // Step
         {
            result += isEven ? numbers[0] : -numbers[0];

            List<long> next = new List<long>();

            for (int i = 0; i < numbers.Count - 1; i++) {
               next.Add(numbers[i + 1] - numbers[i]);
            }

            Extrapolate2(next, ref result, !isEven);
         }
      }
   }

   class History {
      public List<long> numbers;
      //public long extrapolatedNumber;

      public History(List<long> numbers) {
         this.numbers = numbers;
      }
   }
}
