using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Day6 {
   internal class Program {
      static void Main(string[] args) {
         Part2();
      }

      static void Part1() {
         List<Race> races = new List<Race>();
         List<int> numberOfWaysToBeatRecords = new List<int>();
         string input = File.ReadAllText("input.txt");

         Regex regex = new Regex(@".+?: *([\d\s]+).*?: *([\d\s]+)", RegexOptions.Singleline);
         Match match = regex.Match(input);

         var times = match.Groups[1].Value.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
         var distances = match.Groups[2].Value.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);

         for (int i = 0; i < times.Length; i++) {
            races.Add(new Race(float.Parse(times[i]), float.Parse(distances[i])));
         }

         foreach (Race race in races) {
            if (race.time % 2 == 0) {
               for (int i = 0; i <= race.time / 2; i++) {
                  int distance = (int)CalculateDistance(race.time, i);

                  if (distance > race.record) {
                     numberOfWaysToBeatRecords.Add(((int)race.time / 2 - i) * 2 + 1);
                     break;
                  }
               }
            } else {
               for (int i = 0; i <= race.time / 2; i++) {
                  int distance = (int)CalculateDistance(race.time, i);

                  if (distance > race.record) {
                     numberOfWaysToBeatRecords.Add(((int)race.time / 2 - i + 1) * 2);
                     break;
                  }
               }
            }
         }

         ulong result = 1;

         foreach (int number in numberOfWaysToBeatRecords) {
            result *= (ulong)number;
         }

         Console.WriteLine(result);
      }

      static void Part2() {
         Race race;
         string input = File.ReadAllText("input.txt");

         Regex regex = new Regex(@".+?: *([\d\s]+).*?: *([\d\s]+)", RegexOptions.Singleline);
         Match match = regex.Match(input);

         {
            double time = double.Parse(match.Groups[1].Value.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries).Aggregate((f, s) => f + s));
            double distance = double.Parse(match.Groups[2].Value.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries).Aggregate((f, s) => f + s));

            //for (int i = 0; i < times.Length; i++) {
            //   races.Add(new Race(float.Parse(times[i]), float.Parse(distances[i])));
            //}

            race = new Race(time, distance);
         }

         if (race.time % 2 == 0) {
            for (int i = 0; i <= race.time / 2; i++) {
               ulong distance = (ulong)CalculateDistance(race.time, i);

               if (distance > race.record) {
                  //numberOfWaysToBeatRecords.Add(((int)race.time / 2 - i) * 2 + 1);
                  Console.WriteLine((race.time / 2 - i) * 2 + 1);
                  break;
               }
            }
         } else {
            for (int i = 0; i <= race.time / 2; i++) {
               ulong distance = (ulong)CalculateDistance(race.time, i);

               if (distance > race.record) {
                  //numberOfWaysToBeatRecords.Add(((int)race.time / 2 - i + 1) * 2);
                  Console.WriteLine((race.time / 2 - i + 1) * 2);
                  break;
               }
            }
         }
      }

      static double CalculateDistance(double raceTime, double holdTime) {
         return holdTime * (raceTime - holdTime);
      }
   }

   public struct Race {
      public double time;
      public double record;

      public Race(double time, double record) {
         this.time = time;
         this.record = record;
      }

      public override string ToString() {
         return $"Time: {time}, Record: {record}";
      }
   }
}
