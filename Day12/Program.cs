using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Day12 {
   internal class Program {
      static void Main(string[] args) {
         Part2();
      }

      static void Part1() {
         string[] input = File.ReadAllLines("input.txt");
         List<Row> rows = new List<Row>();

         Regex regex = new Regex(@"([?.#]+) (.+)");

         foreach (string line in input) {
            Row row = new Row();
            Match match = regex.Match(line);

            row.springs = match.Groups[1].Value;
            foreach (int number in match.Groups[2].Value.Split(',').Select(int.Parse)) {
               row.conditionRecords.Add(number);
            }

            rows.Add(row);
         }

         int sum = 0;
         foreach (Row row in rows) {
            int subSum = 0;

            int totalBrokenSprings = row.conditionRecords.Sum();
            int numberOfBrokenSpringsThatINeedToPlace = totalBrokenSprings - row.springs.Count(c => c == '#');
            int numberOfEmptySpaces = row.springs.Count(c => c == '?');

            //int numberOfArragements = Choose(numberOfEmptySpaces, numberOfBrokenSpringsThatINeedToPlace);
            List<List<int>> combinations = GenerateCombinations(numberOfEmptySpaces, numberOfBrokenSpringsThatINeedToPlace);

            for (int i = 0; i < combinations.Count; i++) {
               List<int> indicesOfQuestionMarks = new List<int>();
               StringBuilder sb = new StringBuilder(row.springs);

               for (int j = 0; j < combinations[i].Count; j++) {
                  indicesOfQuestionMarks.Add(FindIndexOfQuestionMark(row.springs, combinations[i][j]));
               }

               for (int j = 0; j < indicesOfQuestionMarks.Count; j++) {
                  sb[indicesOfQuestionMarks[j]] = '#';
               }

               if (DoesSatisfyCondition(sb.ToString(), row.conditionRecords)) {
                  subSum++;
               }
            }

            Console.WriteLine(subSum);

            sum += subSum;
         }

         Console.WriteLine(sum);
      }

      static void Part2() {
         string[] input = File.ReadAllLines("input.txt");
         List<Row> rows = new List<Row>();

         Regex regex = new Regex(@"([?.#]+) (.+)");

         foreach (string line in input) {
            string theline = line;
            Row row = new Row();

            string first = line.Split(' ')[0];
            string second = line.Split(' ')[1];
            string firstExpanded = "";
            string secondExpanded = "";

            for (int i = 0; i < 4; i++) {
               firstExpanded += $"{first}?";
            }
            firstExpanded += first;

            for (int i = 0; i < 4; i++) {
               secondExpanded += $"{second},";
            }
            secondExpanded += second;

            theline = firstExpanded + " " + secondExpanded;

            Match match = regex.Match(theline);

            row.springs = match.Groups[1].Value;
            foreach (int number in match.Groups[2].Value.Split(',').Select(int.Parse)) {
               row.conditionRecords.Add(number);
            }

            rows.Add(row);
         }

         int result = 0;
         foreach (Row row in rows) {
            int subResult = 0;
            Walk(row.springs, 0, 0, row.conditionRecords, ref subResult);
            Console.WriteLine(subResult);
            result += subResult;
         }

         //Walk(rows[5].springs, 0, 0, rows[5].conditionRecords, ref result);

         Console.WriteLine(result);
      }

      static void Walk(string springs, int indexOfCondition, int indexOfSpring, in List<int> conditions, ref int result) {
         if (indexOfCondition == conditions.Count) {
            result++;
            return;
         }

         if (indexOfSpring >= springs.Length) {
            return;
         }

         StringBuilder sb = new StringBuilder(springs);
         switch (springs[indexOfSpring]) {
            case '#': {
               if (indexOfSpring + 1 >= springs.Length) {
                  if (indexOfCondition != conditions.Count - 1) {
                     return; // Failed
                  } else {
                     int current = CalculateHowManySpringsAreContigious(sb.ToString(), indexOfSpring);
                     if (current == conditions[indexOfCondition]) {
                        result++;
                        return;
                     } else {
                        return; // Failed
                     }
                  }
               }

               if (sb[indexOfSpring + 1] == '.') {
                  int current = CalculateHowManySpringsAreContigious(sb.ToString(), indexOfSpring);
                  if (current == conditions[indexOfCondition]) {
                     Walk(sb.ToString(), indexOfCondition + 1, indexOfSpring + 1, conditions, ref result);
                  } else {
                     return; // Failed
                  }
               } else {
                  Walk(sb.ToString(), indexOfCondition, indexOfSpring + 1, conditions, ref result);
               }
               break;
            }

            case '?': {
               if (indexOfSpring + 1 >= springs.Length) {
                  if (indexOfCondition != conditions.Count - 1) {
                     return; // Failed
                  }

                  int current = CalculateHowManySpringsAreContigious(sb.ToString(), indexOfSpring - 1);
                  if (current == conditions[indexOfCondition] || current == conditions[indexOfCondition] - 1) {
                     result++;
                  }

                  return;
               }

               int max = CalculateHowManyPossibleBrokenSpringsContigious(sb.ToString(), indexOfSpring);
               if (max < conditions[indexOfCondition]) {
                  if (indexOfSpring == 0) {
                     sb[indexOfSpring] = '.';
                     Walk(sb.ToString(), indexOfCondition, indexOfSpring + 1, conditions, ref result);
                  } else if (sb[indexOfSpring - 1] == '.') {
                     sb[indexOfSpring] = '.';
                     Walk(sb.ToString(), indexOfCondition, indexOfSpring + 1, conditions, ref result);
                  } else {
                     return; // Failed
                  }
               } else if (max == conditions[indexOfCondition]) {
                  sb[indexOfSpring] = '#';
                  int current = CalculateHowManySpringsAreContigious(sb.ToString(), indexOfSpring);

                  if (current == max) {
                     Walk(sb.ToString(), indexOfCondition + 1, indexOfSpring + 1, conditions, ref result);
                  } else {
                     Walk(sb.ToString(), indexOfCondition, indexOfSpring + 1, conditions, ref result);
                  }
               } else {
                  int current = CalculateHowManySpringsAreContigious(sb.ToString(), indexOfSpring - 1);

                  if (current == 0) {
                     sb[indexOfSpring] = '.';
                     Walk(sb.ToString(), indexOfCondition, indexOfSpring + 1, conditions, ref result);
                  }

                  if (current == conditions[indexOfCondition]) {
                     sb[indexOfSpring] = '.';
                     Walk(sb.ToString(), indexOfCondition + 1, indexOfSpring + 1, conditions, ref result);
                  }

                  if (current == max) {
                     sb[indexOfSpring] = '.';
                     Walk(sb.ToString(), indexOfCondition + 1, indexOfSpring + 1, conditions, ref result);
                  } else {
                     sb[indexOfSpring] = '#';
                     Walk(sb.ToString(), indexOfCondition, indexOfSpring + 1, conditions, ref result);

                     //try {
                     //   if (sb[indexOfSpring - 1] == '.') {
                     //      sb[indexOfSpring] = '.';
                     //      Walk(sb.ToString(), indexOfCondition, indexOfSpring + 1, conditions, ref result);
                     //   }
                     //} catch (IndexOutOfRangeException) { }
                  }
               }
               break;
            }

            case '.': {
               Walk(sb.ToString(), indexOfCondition, indexOfSpring + 1, conditions, ref result);
               break;
            }
         }
      }

      // This approach fails because switch case in this method tries to cut a scope (if) in midway. you cant do that. this is not the case if we were using labels and label pointers 
      // UPDATE: there is probably workaround. i am trying to figure out
      static void WalkIterative(string springs, int indexOfCondition, int indexOfSpring, in List<int> conditions, ref int result) {
         Stack<StackFrame> callStack = new Stack<StackFrame>();

         int returnAddress = 0;
         callStack.Push(new StackFrame(springs, indexOfCondition, indexOfSpring, conditions, null, 0, 0, returnAddress));

         for (; ; ) {
            switch (returnAddress) {
               case 0: {
                  if (indexOfCondition == conditions.Count) {
                     result++;
                     if (Return()) return;
                  }

                  if (indexOfSpring >= springs.Length) {
                     if (Return()) return;
                  }

                  callStack.Peek().sb = new StringBuilder(callStack.Peek().springs);
                  switch (springs[indexOfSpring]) {
                     case '#': {
                        if (indexOfSpring + 1 >= springs.Length) {
                           if (indexOfCondition != conditions.Count - 1) {
                              if (Return()) return;
                           } else {
                              int current = CalculateHowManySpringsAreContigious(callStack.Peek().sb.ToString(), indexOfSpring);
                              if (current == conditions[indexOfCondition]) {
                                 result++;
                                 if (Return()) return;
                              } else {
                                 if (Return()) return;
                              }
                           }
                        }

                        if (callStack.Peek().sb[indexOfSpring + 1] == '.') {
                           int current = CalculateHowManySpringsAreContigious(callStack.Peek().sb.ToString(), indexOfSpring);
                           if (current == conditions[indexOfCondition]) {
                              callStack.Push(new StackFrame(callStack.Peek().sb.ToString(), callStack.Peek().indexOfCondition + 1, callStack.Peek().indexOfSpring + 1, callStack.Peek().conditions, callStack.Peek().sb, 0, 0, 1));
                           } 
                        } 
                        break;
                     }
                  }
                     returnAddress = 0;
                  break;
               }

               case 1: {

                  returnAddress = 0;
                  break;
               }

               case 2: {

                  returnAddress = 0;
                  break;
               }

               case 3: {

                  returnAddress = 0;
                  break;
               }

               case 4: {

                  returnAddress = 0;
                  break;
               }

               case 5: {

                  returnAddress = 0;
                  break;
               }

               case 6: {

                  returnAddress = 0;
                  break;
               }

               case 7: {

                  returnAddress = 0;
                  break;
               }

               case 8: {

                  returnAddress = 0;
                  break;
               }

               case 9: {

                  returnAddress = 0;
                  break;
               }

               case 10: {

                  returnAddress = 0;
                  break;
               }

               case 11: {

                  returnAddress = 0;
                  break;
               }
            }
         }

         bool Return() {
            if (callStack.Count == 1) {
               return true;
            }

            var stackFrame = callStack.Pop();
            returnAddress = stackFrame.returnAddress;

            return false;
         }
      }

      static int CalculateHowManyPossibleBrokenSpringsContigious(string springs, int index) {
         if (index < 0) {
            return 0;
         }

         if (springs[index] == '.') {
            return 0;
         }

         int numberOfContigiousBrokenSprings = 0;

         for (int i = index; i < springs.Length; i++) {
            if (springs[i] == '#' || springs[i] == '?') {
               numberOfContigiousBrokenSprings++;
            } else {
               break;
            }
         }

         for (int i = index - 1; i >= 0; i--) {
            if (springs[i] == '#') {
               numberOfContigiousBrokenSprings++;
            } else {
               break;
            }
         }

         return numberOfContigiousBrokenSprings;
      }

      static int CalculateHowManySpringsAreContigious(string springs, int index) {
         if (index < 0) {
            return 0;
         }

         if (springs[index] == '.') {
            return 0;
         }

         int numberOfContigiousBrokenSprings = 0;

         for (int i = index; i < springs.Length; i++) {
            if (springs[i] == '#') {
               numberOfContigiousBrokenSprings++;
            } else {
               break;
            }
         }

         for (int i = index - 1; i >= 0; i--) {
            if (springs[i] == '#') {
               numberOfContigiousBrokenSprings++;
            } else {
               break;
            }
         }

         return numberOfContigiousBrokenSprings;
      }

      static int Choose(int n, int k) {
         int result = 1;

         for (int i = 1; i <= k; i++) {
            result *= n - (k - i);
            result /= i;
         }

         return result;
      }

      static List<List<int>> GenerateCombinations(int n, int k) {
         List<List<int>> combinations = new List<List<int>>();
         List<int> indices = new List<int>();

         GenerateCombinations(0, k, indices);
         return combinations;

         // Helper function to generate combinations recursively
         void GenerateCombinations(int start, int count, List<int> current) {
            if (count == 0) {
               combinations.Add(new List<int>(current));
               return;
            }

            for (int i = start; i <= n - count; i++) {
               current.Add(i);
               GenerateCombinations(i + 1, count - 1, current);
               current.RemoveAt(current.Count - 1);
            }
         }
      }

      static int FindIndexOfQuestionMark(string springs, int index) {
         int count = 0;

         for (int i = 0; i < springs.Length; i++) {
            if (springs[i] == '?') {
               count++;
            }

            if (count - 1 == index) {
               return i;
            }
         }

         throw new Exception("Not enough question marks");
      }

      static bool DoesSatisfyCondition(string springs, List<int> conditionRecords) {
         Regex regex = new Regex(@"#+");
         MatchCollection matches = regex.Matches(springs);

         if (conditionRecords.Count != matches.Count) {
            return false;
         }

         for (int i = 0; i < matches.Count; i++) {
            if (matches[i].Length != conditionRecords[i]) {
               return false;
            }
         }

         return true;
      }
   }

   class Row {
      public string springs;
      public List<int> conditionRecords;

      public Row() {
         conditionRecords = new List<int>();
      }
   }

   class StackFrame {
      public string springs;
      public int indexOfCondition;
      public int indexOfSpring;
      public List<int> conditions;
      public StringBuilder sb;
      public int max;
      public int current193;
      public int returnAddress;

      public StackFrame(string springs, int indexOfCondition, int indexOfSpring, List<int> conditions, StringBuilder sb, int max, int current193, int returnAddress) {
         this.springs = springs;
         this.indexOfCondition = indexOfCondition;
         this.indexOfSpring = indexOfSpring;
         this.conditions = conditions;
         this.sb = sb;
         this.max = max;
         this.current193 = current193;
         this.returnAddress = returnAddress;
      }
   }
}
