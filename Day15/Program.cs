using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Day15 {
   class Program {
      static void Main() {
         Part2();
      }

      static void Part1() {
         string rawInput = File.ReadAllText("input.txt");
         string[] input = rawInput.Split(',').Select(s => s.TrimEnd()).ToArray();

         ulong sum = 0;
         foreach (string s in input) {
            int hash = Hash(s);
            sum += (ulong)hash;
            // Console.WriteLine($"{s} -> {hash}");
         }

         Console.WriteLine(sum);
      }

      static void Part2() {
         string rawInput = File.ReadAllText("input.txt");
         string[] input = rawInput.Split(',').Select(s => s.TrimEnd()).ToArray();

         Box[] boxes = new Box[256];
         List<Lens> lenses = new List<Lens>(input.Length);

         // Parse lenses
         {
            Regex regex = new Regex(@"(?<label>\w+)(?<operation>[-=])(?<focalLength>.*)");
            foreach (string s in input) {
               Lens lens = new Lens();
               Match match = regex.Match(s);

               lens.label = match.Groups["label"].Value;
               lens.operation = match.Groups["operation"].Value == "-" ? Operation.Dash : Operation.Equals;
               if (lens.operation == Operation.Equals) {
                  lens.focalLength = int.Parse(match.Groups["focalLength"].Value);
               }

               lenses.Add(lens);
            }
         }

         // Create boxes
         {
            for (int i = 0; i < boxes.Length; i++) {
               boxes[i] = new Box();
            }
         }

         // Do operations
         {
            foreach (Lens lens in lenses) {
               switch (lens.operation) {
                  case Operation.Dash: {
                     Box box = boxes[Hash(lens.label)];

                     Lens alreadyExistingLens = box.lenses.Find(l => l.label == lens.label);
                     if (alreadyExistingLens != null) {
                        box.lenses.Remove(alreadyExistingLens);
                     }
                     
                     break;
                  }

                  case Operation.Equals: {
                     Box box = boxes[Hash(lens.label)];

                     Lens alreadyExistingLens = box.lenses.Find(l => l.label == lens.label);
                     if (alreadyExistingLens != null) {
                        alreadyExistingLens.focalLength = lens.focalLength;
                     } else {
                        box.lenses.Add(lens);
                     }

                     break;
                  }
               }
            }
         }

         ulong total = 0;
         for (int i = 0; i < boxes.Length; i++) {
            for (int j = 0; j < boxes[i].lenses.Count; j++){
               total += FocusingPower(boxes[i].lenses[j].focalLength, i, j + 1);
               System.Console.WriteLine($"{boxes[i].lenses[j]} -> slot:{j + 1} box:{i} {FocusingPower(boxes[i].lenses[j].focalLength, i, j + 1)}");
            }
         }
         Console.WriteLine(total);
      }

      static ulong FocusingPower(int focalLength, int boxNumber, int slotNumber) {
         ulong power = 1;

         power *= 1 + (ulong)boxNumber;
         power *= (ulong)slotNumber;
         power *= (ulong)focalLength;

         return power;
      }

      static int Hash(string s) {
         int hash = 0;
         
         for (int i = 0; i < s.Length; i++) {
            hash += s[i];
            hash *= 17;
            hash %= 256;
         }

         return hash;
      }
   }

   class Box {
      public List<Lens> lenses;

      public Box() {
         lenses = new List<Lens>();
      }
   }

   class Lens {
      public string label;
      public Operation operation;
      public int focalLength;

      public override string ToString() {
         return $"{label} {focalLength}";
      }
   }

   enum Operation {
      Dash,
      Equals
   }
}