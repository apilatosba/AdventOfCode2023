using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day2 {
   internal class Program {
      static void Main() {
         List<Game> games = new List<Game>();

         string[] input = File.ReadAllLines("input.txt");

         for (int i = 0; i < input.Length; i++) {
            games.Add(ParseGame(input[i]));
         }

         for (int i = 0; i < games.Count; i++) {
            int minimumRed = 0;
            int minimumGreen = 0;
            int minimumBlue = 0;

            for (int j = 0; j < games[i].data.Length; j++) {
               if (int.TryParse(games[i].data[j], out int result)) {
                  string color = games[i].data[j + 1].TrimEnd(',', ';');
                  if (color == "red") {
                     minimumRed = Math.Max(minimumRed, result);
                  } else if (color == "green") {
                     minimumGreen = Math.Max(minimumGreen, result);
                  } else if (color == "blue") {
                     minimumBlue = Math.Max(minimumBlue, result);
                  } else {
                     throw new Exception("Invalid color");
                  }
               }
            }

            games[i].minimumPower = minimumRed * minimumGreen * minimumBlue;
         }

         Console.WriteLine(games.Select(game => game.minimumPower).Sum());
      }

      static Game ParseGame(string s) {
         string[] ss = s.Split(' ');
         int id = int.Parse(ss[1].TrimEnd(':'));
         string[] data = ss.TakeLast(ss.Length - 2).ToArray();

         return new Game(id, data);
      }
   }

   public class Game {
      public int id;
      public string[] data;
      public int minimumPower;

      public Game(int id, string[] data) {
         this.id = id;
         this.data = data;
      }
   }
}
