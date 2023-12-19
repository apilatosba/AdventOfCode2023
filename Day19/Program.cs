using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Day19 {
   class Program {
      static void Main() {
         Part2();
      }

      static void Part1() {
         string input = File.ReadAllText("input.txt");
         Dictionary<string, Workflow> workFlows = new Dictionary<string, Workflow>();
         List<Part> parts = new List<Part>();

         // Parse input
         {
            Regex workflowRegex = new Regex(@"(?<name>\w+)\{(?<rules>.+)\}");
            Regex partRegex = new Regex(@"\{x=(?<x>\d+),m=(?<m>\d+),a=(?<a>\d+),s=(?<s>\d+)\}");
            Regex ruleRegex = new Regex(@"((?<condition>.+):)?(?<placeToGo>.+)"); // split on , first
            Regex conditionRegex = new Regex(@"(?<partName>\w)(?<operation>[><])(?<value>\d+)");

            MatchCollection workflowMatches = workflowRegex.Matches(input);

            foreach (Match match in workflowMatches) {
               Workflow workflow = new Workflow(match.Groups["name"].Value);

               string rules = match.Groups["rules"].Value;
               string[] ruleStrings = rules.Split(',');

               foreach (string ruleString in ruleStrings) {
                  Match ruleMatch = ruleRegex.Match(ruleString);
                  Rule rule;
                  Match conditionMatch = conditionRegex.Match(ruleMatch.Groups["condition"].Value);

                  if (ruleMatch.Groups["condition"].Value == "") {
                     rule = new Rule {
                        placeToGo = ruleMatch.Groups["placeToGo"].Value
                     };
                  } else {
                     rule = new Rule {
                        leftHandSide = conditionMatch.Groups["partName"].Value[0],
                        operation = conditionMatch.Groups["operation"].Value == ">" ? Operation.GreaterThan : Operation.LessThan,
                        rightHandSide = int.Parse(conditionMatch.Groups["value"].Value),
                        placeToGo = ruleMatch.Groups["placeToGo"].Value
                     };
                  }

                  workflow.rules.Add(rule);
               }

               workFlows.Add(workflow.name, workflow);
            }

            MatchCollection partMatches = partRegex.Matches(input);

            foreach (Match match in partMatches) {
               Part part = new Part {
                  x = int.Parse(match.Groups["x"].Value),
                  m = int.Parse(match.Groups["m"].Value),
                  a = int.Parse(match.Groups["a"].Value),
                  s = int.Parse(match.Groups["s"].Value)
               };

               parts.Add(part);
            }
         }

         List<Part> acceptedParts = new List<Part>();

         foreach (Part part in parts) {
            string workflowName = "in";

            for (; ; ) {
               string placeToGo = ProcessPart(workFlows[workflowName], part);

               if (placeToGo == "A") {
                  acceptedParts.Add(part);
                  break;
               } else if (placeToGo == "R") {
                  break;
               } else {
                  workflowName = placeToGo;
               }
            }
         }

         Console.WriteLine(acceptedParts.Aggregate<Part, ulong>(0, (acc, part) => acc + (ulong)(part.x + part.m + part.a + part.s)));
      }

      static void Part2() {
         string input = File.ReadAllText("input.txt");
         Dictionary<string, Workflow> workFlows = new Dictionary<string, Workflow>();

         // Parse input
         {
            Regex workflowRegex = new Regex(@"(?<name>\w+)\{(?<rules>.+)\}");
            Regex ruleRegex = new Regex(@"((?<condition>.+):)?(?<placeToGo>.+)"); // split on , first
            Regex conditionRegex = new Regex(@"(?<partName>\w)(?<operation>[><])(?<value>\d+)");

            MatchCollection workflowMatches = workflowRegex.Matches(input);

            foreach (Match match in workflowMatches) {
               Workflow workflow = new Workflow(match.Groups["name"].Value);

               string rules = match.Groups["rules"].Value;
               string[] ruleStrings = rules.Split(',');

               foreach (string ruleString in ruleStrings) {
                  Match ruleMatch = ruleRegex.Match(ruleString);
                  Rule rule;
                  Match conditionMatch = conditionRegex.Match(ruleMatch.Groups["condition"].Value);

                  if (ruleMatch.Groups["condition"].Value == "") {
                     rule = new Rule {
                        placeToGo = ruleMatch.Groups["placeToGo"].Value
                     };
                  } else {
                     rule = new Rule {
                        leftHandSide = conditionMatch.Groups["partName"].Value[0],
                        operation = conditionMatch.Groups["operation"].Value == ">" ? Operation.GreaterThan : Operation.LessThan,
                        rightHandSide = int.Parse(conditionMatch.Groups["value"].Value),
                        placeToGo = ruleMatch.Groups["placeToGo"].Value
                     };
                  }

                  workflow.rules.Add(rule);
               }

               workFlows.Add(workflow.name, workflow);
            }
         }

         List<(Range x, Range m, Range a, Range s)> acceptedRanges = new List<(Range x, Range m, Range a, Range s)>();

         Queue<StepInfo> stepInfos = new Queue<StepInfo>();
         stepInfos.Enqueue(new StepInfo {
            workflow = workFlows["in"],
            ruleIndex = 0,
            x = new Range(1, 4000),
            m = new Range(1, 4000),
            a = new Range(1, 4000),
            s = new Range(1, 4000)
         });

         while (stepInfos.Count > 0) {
            StepInfo stepInfo = stepInfos.Dequeue();
            Rule rule = stepInfo.workflow.rules[stepInfo.ruleIndex];

            if (!rule.HasCondition) {
               if (rule.placeToGo == "A") {
                  acceptedRanges.Add((stepInfo.x, stepInfo.m, stepInfo.a, stepInfo.s));
               } else if (rule.placeToGo == "R") {
                  // Do nothing
               } else {
                  stepInfos.Enqueue(new StepInfo {
                     workflow = workFlows[stepInfo.workflow.rules[stepInfo.ruleIndex].placeToGo],
                     ruleIndex = 0,
                     x = stepInfo.x,
                     m = stepInfo.m,
                     a = stepInfo.a,
                     s = stepInfo.s
                  });
               }
            } else { // If i have a condition
               switch (rule.operation) {
                  case Operation.GreaterThan: { // x > right
                     if (rule.rightHandSide < stepInfo[rule.leftHandSide].Start.Value) {
                        // No branching Success
                        if (rule.placeToGo == "A") {
                           acceptedRanges.Add((stepInfo.x, stepInfo.m, stepInfo.a, stepInfo.s));
                        } else if (rule.placeToGo == "R") {
                           // Do nothing
                        } else {
                           stepInfos.Enqueue(new StepInfo {
                              workflow = workFlows[rule.placeToGo],
                              ruleIndex = 0,
                              x = stepInfo.x,
                              m = stepInfo.m,
                              a = stepInfo.a,
                              s = stepInfo.s
                           });
                        }
                     } else if (rule.rightHandSide >= stepInfo[rule.leftHandSide].End.Value) {
                        // No branching Failure
                        stepInfos.Enqueue(new StepInfo {
                           workflow = stepInfo.workflow,
                           ruleIndex = stepInfo.ruleIndex + 1,
                           x = stepInfo.x,
                           m = stepInfo.m,
                           a = stepInfo.a,
                           s = stepInfo.s
                        });
                     } else {
                        // Branching

                        StepInfo toBeQueuedFailure = rule.leftHandSide switch {
                           'x' => new StepInfo {
                              workflow = stepInfo.workflow,
                              ruleIndex = stepInfo.ruleIndex + 1,
                              x = new Range(stepInfo.x.Start.Value, rule.rightHandSide),
                              m = stepInfo.m,
                              a = stepInfo.a,
                              s = stepInfo.s
                           },

                           'm' => new StepInfo {
                              workflow = stepInfo.workflow,
                              ruleIndex = stepInfo.ruleIndex + 1,
                              x = stepInfo.x,
                              m = new Range(stepInfo.m.Start.Value, rule.rightHandSide),
                              a = stepInfo.a,
                              s = stepInfo.s
                           },

                           'a' => new StepInfo {
                              workflow = stepInfo.workflow,
                              ruleIndex = stepInfo.ruleIndex + 1,
                              x = stepInfo.x,
                              m = stepInfo.m,
                              a = new Range(stepInfo.a.Start.Value, rule.rightHandSide),
                              s = stepInfo.s
                           },

                           's' => new StepInfo {
                              workflow = stepInfo.workflow,
                              ruleIndex = stepInfo.ruleIndex + 1,
                              x = stepInfo.x,
                              m = stepInfo.m,
                              a = stepInfo.a,
                              s = new Range(stepInfo.s.Start.Value, rule.rightHandSide)
                           },

                           _ => throw new Exception("Unknown part")
                        };

                        stepInfos.Enqueue(toBeQueuedFailure);

                        StepInfo toBeQueuedSuccess = rule.leftHandSide switch {
                           'x' => new StepInfo {
                              // workflow = workFlows[rule.placeToGo],
                              ruleIndex = 0,
                              x = new Range(rule.rightHandSide + 1, stepInfo.x.End.Value),
                              m = stepInfo.m,
                              a = stepInfo.a,
                              s = stepInfo.s
                           },

                           'm' => new StepInfo {
                              // workflow = workFlows[rule.placeToGo],
                              ruleIndex = 0,
                              x = stepInfo.x,
                              m = new Range(rule.rightHandSide + 1, stepInfo.m.End.Value),
                              a = stepInfo.a,
                              s = stepInfo.s
                           },

                           'a' => new StepInfo {
                              // workflow = workFlows[rule.placeToGo],
                              ruleIndex = 0,
                              x = stepInfo.x,
                              m = stepInfo.m,
                              a = new Range(rule.rightHandSide + 1, stepInfo.a.End.Value),
                              s = stepInfo.s
                           },

                           's' => new StepInfo {
                              // workflow = workFlows[rule.placeToGo],
                              ruleIndex = 0,
                              x = stepInfo.x,
                              m = stepInfo.m,
                              a = stepInfo.a,
                              s = new Range(rule.rightHandSide + 1, stepInfo.s.End.Value)
                           },

                           _ => throw new Exception("Unknown part")
                        };

                        if (rule.placeToGo == "A") {
                           acceptedRanges.Add((toBeQueuedSuccess.x, toBeQueuedSuccess.m, toBeQueuedSuccess.a, toBeQueuedSuccess.s));
                        } else if (rule.placeToGo == "R") {
                           // Do nothing
                        } else {
                           toBeQueuedSuccess.workflow = workFlows[rule.placeToGo];

                           stepInfos.Enqueue(toBeQueuedSuccess);
                        }
                     }

                     break;
                  }

                  case Operation.LessThan: { // x < right
                     if (rule.rightHandSide > stepInfo[rule.leftHandSide].End.Value) {
                        // No branching Success
                        if (rule.placeToGo == "A") {
                           acceptedRanges.Add((stepInfo.x, stepInfo.m, stepInfo.a, stepInfo.s));
                        } else if (rule.placeToGo == "R") {
                           // Do nothing
                        } else {
                           stepInfos.Enqueue(new StepInfo {
                              workflow = workFlows[rule.placeToGo],
                              ruleIndex = 0,
                              x = stepInfo.x,
                              m = stepInfo.m,
                              a = stepInfo.a,
                              s = stepInfo.s
                           });
                        }
                     } else if (rule.rightHandSide <= stepInfo[rule.leftHandSide].Start.Value) {
                        // All failed
                        stepInfos.Enqueue(new StepInfo {
                           workflow = stepInfo.workflow,
                           ruleIndex = stepInfo.ruleIndex + 1,
                           x = stepInfo.x,
                           m = stepInfo.m,
                           a = stepInfo.a,
                           s = stepInfo.s
                        });
                     } else {
                        // Half succes half failure
                        StepInfo toBeQueuedFailure = rule.leftHandSide switch {
                           'x' => new StepInfo {
                              workflow = stepInfo.workflow,
                              ruleIndex = stepInfo.ruleIndex + 1,
                              x = new Range(rule.rightHandSide, stepInfo.x.End.Value),
                              m = stepInfo.m,
                              a = stepInfo.a,
                              s = stepInfo.s
                           },

                           'm' => new StepInfo {
                              workflow = stepInfo.workflow,
                              ruleIndex = stepInfo.ruleIndex + 1,
                              x = stepInfo.x,
                              m = new Range(rule.rightHandSide, stepInfo.m.End.Value),
                              a = stepInfo.a,
                              s = stepInfo.s
                           },

                           'a' => new StepInfo {
                              workflow = stepInfo.workflow,
                              ruleIndex = stepInfo.ruleIndex + 1,
                              x = stepInfo.x,
                              m = stepInfo.m,
                              a = new Range(rule.rightHandSide, stepInfo.a.End.Value),
                              s = stepInfo.s
                           },

                           's' => new StepInfo {
                              workflow = stepInfo.workflow,
                              ruleIndex = stepInfo.ruleIndex + 1,
                              x = stepInfo.x,
                              m = stepInfo.m,
                              a = stepInfo.a,
                              s = new Range(rule.rightHandSide, stepInfo.s.End.Value)
                           },

                           _ => throw new Exception("Unknown part")
                        };

                        stepInfos.Enqueue(toBeQueuedFailure);

                        if (rule.placeToGo == "A") {
                           acceptedRanges.Add((stepInfo.x, stepInfo.m, stepInfo.a, stepInfo.s));
                        } else if (rule.placeToGo == "R") {
                           // Do nothing
                        } else {
                           StepInfo toBeQueuedSuccess = rule.leftHandSide switch {
                              'x' => new StepInfo {
                                 workflow = workFlows[rule.placeToGo],
                                 ruleIndex = 0,
                                 x = new Range(stepInfo.x.Start.Value, rule.rightHandSide - 1),
                                 m = stepInfo.m,
                                 a = stepInfo.a,
                                 s = stepInfo.s
                              },

                              'm' => new StepInfo {
                                 workflow = workFlows[rule.placeToGo],
                                 ruleIndex = 0,
                                 x = stepInfo.x,
                                 m = new Range(stepInfo.m.Start.Value, rule.rightHandSide - 1),
                                 a = stepInfo.a,
                                 s = stepInfo.s
                              },

                              'a' => new StepInfo {
                                 workflow = workFlows[rule.placeToGo],
                                 ruleIndex = 0,
                                 x = stepInfo.x,
                                 m = stepInfo.m,
                                 a = new Range(stepInfo.a.Start.Value, rule.rightHandSide - 1),
                                 s = stepInfo.s
                              },

                              's' => new StepInfo {
                                 workflow = workFlows[rule.placeToGo],
                                 ruleIndex = 0,
                                 x = stepInfo.x,
                                 m = stepInfo.m,
                                 a = stepInfo.a,
                                 s = new Range(stepInfo.s.Start.Value, rule.rightHandSide - 1)
                              },

                              _ => throw new Exception("Unknown part")
                           };

                           stepInfos.Enqueue(toBeQueuedSuccess);
                        }
                     }

                     break;
                  }

                  default: {
                     throw new Exception("Unknown operation");
                  } // TODO ranges are not inclusive fix them
               }
            }
         }

         List<(Range x, Range m, Range a, Range s)> countedRanges = new List<(Range x, Range m, Range a, Range s)>();
         List< (Range x, Range m, Range a, Range s)> overlappingRanges = new List<(Range x, Range m, Range a, Range s)>();
         
         checked {
            ulong sum = 0;
            foreach ((Range x, Range m, Range a, Range s) in acceptedRanges) {
               foreach ((Range x2, Range m2, Range a2, Range s2) in countedRanges) {
                  (Range x3, Range m3, Range a3, Range s3)? overlappingRange = GetOverlappingRange((x, m, a, s), (x2, m2, a2, s2));
                  
                  if (overlappingRange != null) {
                     overlappingRanges.Add(overlappingRange.Value);
                     sum -= (ulong)(overlappingRange.Value.x3.End.Value - overlappingRange.Value.x3.Start.Value + 1) * (ulong)(overlappingRange.Value.m3.End.Value - overlappingRange.Value.m3.Start.Value + 1) * (ulong)(overlappingRange.Value.a3.End.Value - overlappingRange.Value.a3.Start.Value + 1) * (ulong)(overlappingRange.Value.s3.End.Value - overlappingRange.Value.s3.Start.Value + 1);
                  }
               }
               
               sum += (ulong)(x.End.Value - x.Start.Value + 1) * (ulong)(m.End.Value - m.Start.Value + 1) * (ulong)(a.End.Value - a.Start.Value + 1) * (ulong)(s.End.Value - s.Start.Value + 1);
               countedRanges.Add((x, m, a, s));
            }

            Console.WriteLine(sum);
         }

         System.Console.WriteLine("**********************Accepted ranges***********************************");
         foreach ((Range x, Range m, Range a, Range s) in acceptedRanges) {
            Console.WriteLine($"x: {x,-12}, m: {m,-12}, a: {a,-12}, s: {s,-12}");
         }

         System.Console.WriteLine("**********************Overlapping ranges***********************************");
         foreach ((Range x, Range m, Range a, Range s) in overlappingRanges) {
            Console.WriteLine($"x: {x,-12}, m: {m,-12}, a: {a,-12}, s: {s,-12}");
         }
      }

      static Range? GetOverlappingRange(Range a, Range b) {
         if (a.Start.Value > b.End.Value || b.Start.Value > a.End.Value) {
            return null;
         }

         return new Range(Math.Max(a.Start.Value, b.Start.Value), Math.Min(a.End.Value, b.End.Value));
      }

      static (Range x, Range m, Range a, Range s)? GetOverlappingRange((Range x, Range m, Range a, Range s) a, (Range x, Range m, Range a, Range s) b) {
         Range? x = GetOverlappingRange(a.x, b.x);
         Range? m = GetOverlappingRange(a.m, b.m);
         Range? aa = GetOverlappingRange(a.a, b.a);
         Range? s = GetOverlappingRange(a.s, b.s);

         if (x == null || m == null || aa == null || s == null) {
            return null;
         }

         return (x.Value, m.Value, aa.Value, s.Value);
      }

      static string ProcessPart(Workflow workflow, Part part) {
         string placeToGo = null;

         foreach (Rule rule in workflow.rules) {
            if (!rule.HasCondition) {
               placeToGo = rule.placeToGo;
               break;
            }

            if (rule.operation == Operation.GreaterThan) {
               if (rule.leftHandSide == 'x') {
                  if (part.x > rule.rightHandSide) {
                     placeToGo = rule.placeToGo;
                     break;
                  }
               } else if (rule.leftHandSide == 'm') {
                  if (part.m > rule.rightHandSide) {
                     placeToGo = rule.placeToGo;
                     break;
                  }
               } else if (rule.leftHandSide == 'a') {
                  if (part.a > rule.rightHandSide) {
                     placeToGo = rule.placeToGo;
                     break;
                  }
               } else if (rule.leftHandSide == 's') {
                  if (part.s > rule.rightHandSide) {
                     placeToGo = rule.placeToGo;
                     break;
                  }
               } else {
                  throw new Exception("Unknown left hand side");
               }
            } else if (rule.operation == Operation.LessThan) {
               if (rule.leftHandSide == 'x') {
                  if (part.x < rule.rightHandSide) {
                     placeToGo = rule.placeToGo;
                     break;
                  }
               } else if (rule.leftHandSide == 'm') {
                  if (part.m < rule.rightHandSide) {
                     placeToGo = rule.placeToGo;
                     break;
                  }
               } else if (rule.leftHandSide == 'a') {
                  if (part.a < rule.rightHandSide) {
                     placeToGo = rule.placeToGo;
                     break;
                  }
               } else if (rule.leftHandSide == 's') {
                  if (part.s < rule.rightHandSide) {
                     placeToGo = rule.placeToGo;
                     break;
                  }
               } else {
                  throw new Exception("Unknown left hand side");
               }
            } else {
               throw new Exception("Unknown operation");
            }
         }

         return placeToGo;
      }
   }

   class StepInfo {
      public Workflow workflow;
      public int ruleIndex;
      public Range x;
      public Range m;
      public Range a;
      public Range s;

      public Range this[char c] {
         get {
            switch (c) {
               case 'x': {
                  return x;
               }

               case 'm': {
                  return m;
               }

               case 'a': {
                  return a;
               }

               case 's': {
                  return s;
               }

               default: {
                  throw new Exception("Unknown part");
               }
            }
         }

         set {
            switch (c) {
               case 'x': {
                  x = value;
                  break;
               }

               case 'm': {
                  m = value;
                  break;
               }

               case 'a': {
                  a = value;
                  break;
               }

               case 's': {
                  s = value;
                  break;
               }

               default: {
                  throw new Exception("Unknown part");
               }
            }
         }
      }
   }

   class Part {
      public int x;
      public int m;
      public int a;
      public int s;
   }

   class Workflow {
      public string name;
      public List<Rule> rules;

      public Workflow(string name) {
         this.name = name;
         rules = new List<Rule>();
      }
   }

   class Rule {
      public char leftHandSide;
      public Operation operation;
      public int rightHandSide;
      public string placeToGo;
      public bool HasCondition => leftHandSide != '\0';
   }

   enum Operation {
      GreaterThan,
      LessThan,
   }
}
