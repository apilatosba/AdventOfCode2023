using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Day3 {
   internal class Program {
      static void Main(string[] args) {
         Part2();
      }

      static void Part1() {
         string[] input = File.ReadAllLines("input.txt");

         long sum = 0;

         for (int i = 0; i < input.Length; i++) {
            //Regex regex = new Regex(@"-?\d+");
            Regex regex = new Regex(@"\d+");
            MatchCollection matches = regex.Matches(input[i]);

            foreach (Match match in matches) {
               int x = match.Index;
               int y = i;
               int length = match.Length;

               if (IsPartNumber(GetNeighbourCharacters(input, x, y, length))) {
                  sum += long.Parse(match.Value);
                  //Console.WriteLine($"{match.Value}, {match.Length}");

                  //foreach (char c in match.Value) {
                  //   if (!(c >= '0' && c <= '9')) {
                  //      throw new Exception("Not a number");
                  //   }
                  //}
               }

               //foreach (Character character in GetNeighbourCharacters(input, x, y, length)) {
               //   Console.Write(character.value);
               //}
               //Console.WriteLine();
            }

            //break;
         }

         Console.WriteLine(sum);
      }

      static void Part2() {
         string[] input = File.ReadAllLines("input.txt");
         List<Gear> gears = new List<Gear>();
         ulong sum = 0;

         for (int i = 0; i < input.Length; i++) {
            Regex regex = new Regex(@"\d+");
            MatchCollection matches = regex.Matches(input[i]);

            foreach (Match match in matches) {
               int x = match.Index;
               int y = i;
               int length = match.Length;

               foreach (Character character in GetNeighbourCharacters(input, x, y, length)) {
                  if(character.value == '*') {
                     try {
                        gears.Where(g => g.character.x == character.x && g.character.y == character.y).First().ints.Add(int.Parse(match.Value));
                     } catch (InvalidOperationException) { // .First() throws this exception if no element is found
                        Gear gear = new Gear(character);
                        gear.ints.Add(int.Parse(match.Value));
                        gears.Add(gear);
                     }
                  }
               }
            }
         }

         for (int i = 0; i < gears.Count; i++) {
            if (IsValidGear(gears[i])) {
               sum += CalculateGearRatio(gears[i]);
            }
         }

         Console.WriteLine(sum);
      }

      static List<Character> GetNeighbourCharacters(string[] input, int x, int y, int length) {
         List<Character> characters = new List<Character>();

         // Top and bottom
         for (int i = x - 1; i <= x + length; i++) {
            try {
               Character top = new Character(i, y - 1, input[y - 1][i]);
               characters.Add(top);
            } catch (IndexOutOfRangeException) {
            }

            try {
               Character bottom = new Character(i, y + 1, input[y + 1][i]);
               characters.Add(bottom);
            } catch (IndexOutOfRangeException) {
            }
         }

         try {
            // Left
            characters.Add(new Character(x - 1, y, input[y][x - 1]));
         } catch (IndexOutOfRangeException) {
            // Do nothing
         }

         try {
            // Right
            characters.Add(new Character(x + length, y, input[y][x + length]));
         } catch (IndexOutOfRangeException) {
            // Do nothing
         }

         return characters;
      }

      static bool IsPartNumber(List<Character> neighbourChars) {
         char[] symbols = new char[] {
            '*',
            '#',
            '+',
            '$',
            '%',
            '&',
            '/',
            '=',
            '?',
            '!',
            ':',
            ';',
            ',',
            '-',
            '_',
            '@',
            '^',
            '~',
            '`',
            '|',
            '\\',
            '"',
            '\'',
            '<',
            '>',
            '(',
            ')',
            '[',
            ']',
            '{',
            '}',
            //'0',
            //'1',
            //'2',
            //'3',
            //'4',
            //'5',
            //'6',
            //'7',
            //'8',
            //'9',
         };

         foreach (Character character in neighbourChars) {
            if (symbols.Contains(character.value)) {
               return true;
            }
            //if (char.IsSymbol(character.value) /* Sayse that '*' is not a symbol */) {
            //   return true;
            //} 
         }

         return false;
      }

      static bool IsValidGear(Gear gear) {
         return gear.ints.Count == 2;
      }

      static ulong CalculateGearRatio(Gear gear) {
         return (ulong)gear.ints[0] * (ulong)gear.ints[1];
      }
   }

   public struct Character {
      public int x;
      public int y;
      public char value;

      public Character(int x, int y, char value) {
         this.x = x;
         this.y = y;
         this.value = value;
      }
   }

   public class Gear {
      public Character character;
      public List<int> ints;

      public Gear(Character character) {
         this.character = character;
         ints = new List<int>();
      }
   }
}
