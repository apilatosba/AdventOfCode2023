using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text.RegularExpressions;

namespace Day8 {
   internal class Program {
      static void Main(string[] args) {
         Part2();
      }

      static void Part1() {
         string[] input = File.ReadAllLines("input.txt");

         string instructions = input[0];

         Regex regex = new Regex(@"(\w{3}) += +\((\w{3}), +(\w{3})\)");
         Dictionary<string, string[]> nodes = new Dictionary<string, string[]>();

         foreach (string line in input) {
            Match match = regex.Match(line);
            if (!match.Success) continue;

            string name = match.Groups[1].Value;
            string left = match.Groups[2].Value;
            string right = match.Groups[3].Value;

            nodes.Add(name, new string[] { left, right });
         }

         string current = "AAA";

         ulong steps = 0;
         for(bool found = false; !found ;) {
            foreach(char instruction in instructions) {
               if (current == "ZZZ") {
                  found = true;
                  break;
               }
               
               if (instruction == 'L') {
                  current = nodes[current][0];
               } else if (instruction == 'R') {
                  current = nodes[current][1];
               } else {
                  throw new Exception("Instruction unclear");
               }

               steps++;
            }
         }

         Console.WriteLine(steps);
      }

      static void Part2() {
         string[] input = File.ReadAllLines("input.txt");

         string instructions = input[0];

         Regex regex = new Regex(@"(\w{3}) += +\((\w{3}), +(\w{3})\)");
         Dictionary<string, string[]> nodes = new Dictionary<string, string[]>();
         List<string> currentNodes = new List<string>();

         foreach (string line in input) {
            Match match = regex.Match(line);
            if (!match.Success) continue;

            string name = match.Groups[1].Value;
            string left = match.Groups[2].Value;
            string right = match.Groups[3].Value;

            nodes.Add(name, new string[] { left, right });

            if (name.EndsWith('A')) {
               currentNodes.Add(name);
            }
         }

         List<ulong> steps = new List<ulong>();

         for (int i = 0; i < currentNodes.Count; i++) {
            for (ulong j = 0; ; j++) {
               char instruction = instructions[(int)(j % (ulong)instructions.Length)];
               currentNodes[i] = Step(instruction, currentNodes[i], nodes);

               if (currentNodes[i].EndsWith('Z')) {
                  steps.Add(j + 1);
                  break;
               }
            }
         }

         Console.WriteLine(steps.Aggregate(LCM));

         //ulong steps = 0;
         //for (bool found = false/*, isThereASingleNodeThatDoesntEndWithZ = false*/; !found;) {
         //   foreach (char instruction in instructions) {
         //      //isThereASingleNodeThatDoesntEndWithZ = false;
         //      string endings = "";

         //      //if (!currentNodes.Exists(x => !x.EndsWith('Z'))) {
         //      //   found = true;
         //      //   break;
         //      //}

         //      for (int i = 0; i < currentNodes.Count; i++) {
         //         if (instruction == 'L') {
         //            currentNodes[i] = nodes[currentNodes[i]][0];
         //         } else if (instruction == 'R') {
         //            currentNodes[i] = nodes[currentNodes[i]][1];
         //         } else {
         //            throw new Exception("Instruction unclear");
         //         }

         //         endings += currentNodes[i][2];

         //         if (!currentNodes[i].EndsWith('Z')) {
         //            //isThereASingleNodeThatDoesntEndWithZ = true;
         //         }
         //      }

         //      steps++;

         //      Console.WriteLine($"{endings}  {steps}");

         //      if (!isThereASingleNodeThatDoesntEndWithZ) {
         //         found = true;
         //         break;
         //      }
         //   }
         //}

         //List<StepInfo> stepInfos = new List<StepInfo>();

         //for (int i = 0; i < currentNodes.Count; i++) {
         //   stepInfos.Add(new StepInfo() {
         //      instructionIndex = 0,
         //      name = currentNodes[i],
         //      steps = 0
         //   });
         //}

         //for(int nodeIndex = 0; nodeIndex < stepInfos.Count; ) {
         //   Step(stepInfos[nodeIndex], instructions, nodes);

         //   if (stepInfos[nodeIndex].name.EndsWith('Z')) {
         //      nodeIndex++;
         //   } else {
         //      nodeIndex = 0;
         //   }

         //   Console.WriteLine($"{nodeIndex}  {stepInfos[nodeIndex].steps}");
         //}

         //stepInfos.ForEach(s => Console.WriteLine($"{s.name} {s.steps}"));

         //int nodeIndex = 0;
         //ulong steps = 0;
         //for (bool endsWithZ = false; ;) {
         //   foreach(char instruction in instructions) {
         //      if (currentNodes[0].EndsWith('Z')) {
         //         for (int nodeIdx = 1; nodeIdx < currentNodes.Count; nodeIdx++) {
         //            for(ulong i = 0; i < steps; i++) {
         //               currentNodes[nodeIdx] = Step(instructions[(int)(i % (ulong)instructions.Length)], currentNodes[nodeIdx], nodes);
         //            }

         //            if (currentNodes[nodeIdx].EndsWith('Z')) {
         //               continue;
         //            } else {
         //               break;
         //            }
         //         }
         //      }

         //      if (instruction == 'L') {
         //         currentNodes[0] = nodes[currentNodes[0]][0];
         //      } else if (instruction == 'R') {
         //         currentNodes[0] = nodes[currentNodes[0]][1];
         //      } else {
         //         throw new Exception("Instruction unclear");
         //      }

         //      steps++;
         //   }
         //}

         //Console.WriteLine(steps);
      }

      static string Step(char instruction, string node, Dictionary<string, string[]> nodes) {
         if (instruction == 'L') {
            return nodes[node][0];
         } else if (instruction == 'R') {
            return nodes[node][1];
         } else {
            throw new Exception("Instruction unclear");
         }
      }

      //static void Step(StepInfo stepInfo, string instructions, Dictionary<string, string[]> nodes) {
      //   string newName = Step(instructions[stepInfo.instructionIndex], stepInfo.name, nodes);
      //   stepInfo.instructionIndex = (stepInfo.instructionIndex + 1) % instructions.Length;
      //   stepInfo.name = newName;
      //   stepInfo.steps++;
      //}

      static ulong LCM(ulong a, ulong b) {
         return (a * b) / GCD(a, b);
      }

      static ulong GCD(ulong a, ulong b) {
         while (b != 0) {
            ulong temp = b;
            b = a % b;
            a = temp;
         }

         return a;
      }
   }

   //class StepInfo {
   //   public int instructionIndex;
   //   public string name;
   //   public ulong steps;
   //}

   //class Node {
   //   public string name;
   //   public Node left;
   //   public Node right;

   //   public Node(string name, Node left, Node right) {
   //      this.name = name;
   //      this.left = left;
   //      this.right = right;
   //   }

   //   public Node(string name) {
   //      this.name = name;
   //   }
   //}

   //struct NodeInfo {
   //   public string name;
   //   public string leftName;
   //   public string rightName;

   //   public NodeInfo(string name, string leftName, string rightName) {
   //      this.name = name;
   //      this.leftName = leftName;
   //      this.rightName = rightName;
   //   }
   //}
}
