using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Day5 {
   internal class Program {
      static void Main(string[] args) {
         Part2();
      }

      static void Part1() {
         string input = File.ReadAllText("sample input.txt");
         Regex regex = new Regex(
@"seeds:([\s\d]+)
se.+:
([\d\s]+)
so.+:
([\d\s]+)
fe.+:
([\d\s]+)
wa.+:
([\d\s]+)
li.+:
([\d\s]+)
te.+:
([\d\s]+)
hu.+:
([\d\s]+)");

         Match match = regex.Match(input);
         List<string> seeds = ParseSeeds(match.Groups[1].Value);
         List<string[]> seedToSoil = Parse(match.Groups[2].Value);
         List<string[]> soilToFertilizer = Parse(match.Groups[3].Value);
         List<string[]> fertilizerToWater = Parse(match.Groups[4].Value);
         List<string[]> waterToLight = Parse(match.Groups[5].Value);
         List<string[]> lightToTemperature = Parse(match.Groups[6].Value);
         List<string[]> temperatureToHumidity = Parse(match.Groups[7].Value);
         List<string[]> humidityToLocation = Parse(match.Groups[8].Value);

         ulong minimumLocation = ulong.MaxValue;

         foreach (string seed in seeds) {
            string soil = FindMappedValue(seedToSoil, seed);
            string fertilizer = FindMappedValue(soilToFertilizer, soil);
            string water = FindMappedValue(fertilizerToWater, fertilizer);
            string light = FindMappedValue(waterToLight, water);
            string temperature = FindMappedValue(lightToTemperature, light);
            string humidity = FindMappedValue(temperatureToHumidity, temperature);
            string location = FindMappedValue(humidityToLocation, humidity);

            minimumLocation = Math.Min(minimumLocation, ulong.Parse(location));
         }

         Console.WriteLine(minimumLocation);
      }

      static void Part2() {
         string input = File.ReadAllText("input.txt");
         Regex regex = new Regex(
@"seeds:([\s\d]+)
se.+:
([\d\s]+)
so.+:
([\d\s]+)
fe.+:
([\d\s]+)
wa.+:
([\d\s]+)
li.+:
([\d\s]+)
te.+:
([\d\s]+)
hu.+:
([\d\s]+)");

         Match match = regex.Match(input);
         List<ulong[]> seeds = ParseSeeds2(match.Groups[1].Value);
         List<ulong[]> seedToSoil = Parse2(match.Groups[2].Value);
         List<ulong[]> soilToFertilizer = Parse2(match.Groups[3].Value);
         List<ulong[]> fertilizerToWater = Parse2(match.Groups[4].Value);
         List<ulong[]> waterToLight = Parse2(match.Groups[5].Value);
         List<ulong[]> lightToTemperature = Parse2(match.Groups[6].Value);
         List<ulong[]> temperatureToHumidity = Parse2(match.Groups[7].Value);
         List<ulong[]> humidityToLocation = Parse2(match.Groups[8].Value);

         //humidityToLocation.Sort((a, b) => a[0].CompareTo(b[0]));

         //humidityToLocation = humidityToLocation.Select(a => new ulong[4] {
         //   a[0],
         //   a[1],
         //   a[2],
         //   a[0]
         //}).ToList();

         for (ulong i = 0; ; i++) {
            ulong humidity = FindMappedValueInverse(humidityToLocation, i);
            ulong temperature = FindMappedValueInverse(temperatureToHumidity, humidity);
            ulong light = FindMappedValueInverse(lightToTemperature, temperature);
            ulong water = FindMappedValueInverse(waterToLight, light);
            ulong fertilizer = FindMappedValueInverse(fertilizerToWater, water);
            ulong soil = FindMappedValueInverse(soilToFertilizer, fertilizer);
            ulong seed = FindMappedValueInverse(seedToSoil, soil);

            if(IsValidSeed(seeds, seed)) {
               Console.WriteLine(i);
               return;
            }
         }

         //foreach (string[] array in humidityToLocation) {
         //   for (ulong i = ulong.Parse(array[1]); ; i++) { //i: 239902715
         //      string temperature = FindMappedValueInverse(temperatureToHumidity, i.ToString());
         //      string light = FindMappedValueInverse(lightToTemperature, temperature);
         //      string water = FindMappedValueInverse(waterToLight, light);
         //      string fertilizer = FindMappedValueInverse(fertilizerToWater, water);
         //      string soil = FindMappedValueInverse(soilToFertilizer, fertilizer);
         //      string seed = FindMappedValueInverse(seedToSoil, soil);

         //      if (IsValidSeed(seeds, ulong.Parse(seed))) {
         //         Console.WriteLine(seed);
         //         return;
         //      }

         //      Console.WriteLine(ulong.Parse(array[0]) + i - ulong.Parse(array[1]));
         //   }

         //   Console.WriteLine("One more done");
         //}

         //Console.WriteLine("Donk no seed bruv");

         //ulong minimumLocation = ulong.MaxValue;

         //foreach (ulong[] seed in seeds) {
         //   for (ulong i = seed[0], n = seed[0] + seed[1]; i < n; i++) {
         //      string soil = FindMappedValue(seedToSoil, i.ToString());
         //      string fertilizer = FindMappedValue(soilToFertilizer, soil);
         //      string water = FindMappedValue(fertilizerToWater, fertilizer);
         //      string light = FindMappedValue(waterToLight, water);
         //      string temperature = FindMappedValue(lightToTemperature, light);
         //      string humidity = FindMappedValue(temperatureToHumidity, temperature);
         //      string location = FindMappedValue(humidityToLocation, humidity);

         //      minimumLocation = Math.Min(minimumLocation, ulong.Parse(location));
         //      Console.WriteLine(minimumLocation);
         //   }
         //}

         //Console.WriteLine(minimumLocation);
      }

      static bool IsValidSeed(List<ulong[]> seeds, ulong id) {
         foreach (ulong[] seed in seeds) {
            if (id >= seed[0] && id < seed[0] + seed[1]) {
               return true;
            }
         }

         return false;
      }

      static string FindMappedValue(List<string[]> input, string id) {
         foreach (string[] array in input) {
            ulong x = ulong.Parse(id);
            ulong a = ulong.Parse(array[0]);
            ulong b = ulong.Parse(array[1]);
            ulong c = ulong.Parse(array[2]);

            if (x >= b && x < b + c) {
               return ((x - b) + a).ToString();
            }
         }

         return id;
      }

      static ulong FindMappedValueInverse(List<ulong[]> input, ulong id) {
         foreach (ulong[] array in input) {
            ulong x = id;
            ulong a = array[0];
            ulong b = array[1];
            ulong c = array[2];

            if (x >= a && x < a + c) {
               return (x - a) + b;
            }
         }

         return id;
      }

      static List<string[]> Parse(string regexInput) {
         List<string[]> result = new List<string[]>();

         string[] array = regexInput.Trim().Split(new char[] { ' ', '\n' }, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

         for (int i = 0; i < array.Length; i += 3) {
            result.Add(new string[3] {
               array[i],
               array[i + 1],
               array[i + 2]
            });
         }

         return result;
      }

      static List<ulong[]> Parse2(string regexInput) {
         List<ulong[]> result = new List<ulong[]>();

         ulong[] array = regexInput.Trim().Split(new char[] { ' ', '\n' }, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).Select(ulong.Parse).ToArray();

         for (int i = 0; i < array.Length; i += 3) {
            result.Add(new ulong[3] {
               array[i],
               array[i + 1],
               array[i + 2]
            });
         }

         return result;
      }

      static List<string> ParseSeeds(string regexInput) {
         return regexInput.Trim().Split(new char[] { ' ', '\n' }, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).ToList();
      }

      static List<ulong[]> ParseSeeds2(string regexInput) {
         List<ulong[]> result = new List<ulong[]>();

         var collection = regexInput.Trim().Split(new char[] { ' ', '\n' }, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).Select(ulong.Parse).ToImmutableList();

         for (int i = 0, n = collection.Count(); i < n; i += 2) {
            result.Add(new ulong[2] {
               collection[i],
               collection[i + 1]
            });
         }

         return result;
      }
   }
}
