using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Day20 {
   public class Program {
      internal static Queue<(IModule receiever, Pulse, IModule sender)> queue = new Queue<(IModule, Pulse, IModule)>();
      internal static Dictionary<string, IModule> modules = new Dictionary<string, IModule>();
      internal static ulong lowPulseCount = 0;
      internal static ulong highPulseCount = 0;
      internal static bool isRXReached = false;

      public static void Main() {
         Part2();
      }

      static void Part1() {
         string[] input = File.ReadAllLines("input.txt");

         // Parse input
         {
            Regex regex = new Regex(@"(?<type>[&%])(?<name>\w+) -> (?<destinations>.+)"); // split on space on destinations
            Regex broadcasterRegex = new Regex(@"broadcaster -> (?<destinations>.+)"); // split on space on destinations

            Match broadcasterMatch = broadcasterRegex.Match(input[0]);
            modules.Add("broadcaster", new Broadcaster() {
               Name = "broadcaster",
               DestinationModules = new List<string>(broadcasterMatch.Groups["destinations"].Value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            });

            for (int i = 1; i < input.Length; i++) {
               Match match = regex.Match(input[i]);
               string name = match.Groups["name"].Value;
               string type = match.Groups["type"].Value;
               string[] destinations = match.Groups["destinations"].Value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

               switch (type) {
                  case "&":
                     modules.Add(name, new Conjuction() {
                        Name = name,
                        DestinationModules = new List<string>(destinations)
                     });
                     break;
                  case "%":
                     modules.Add(name, new FlipFlop() {
                        Name = name,
                        DestinationModules = new List<string>(destinations)
                     });
                     break;
               }
            }
         }

         // Set conjunctions memory
         {
            List<IModule> outputModules = new List<IModule>();
            foreach (IModule module in modules.Values) {
               foreach (string destination in module.DestinationModules) {
                  try {
                     if (modules[destination] is Conjuction conjuction) {
                        conjuction.memory.Add(module.Name, Pulse.Low);
                     }
                  } catch (KeyNotFoundException) {
                     // Then it is an output module
                     // modules.Add(destination, new Output() {
                     //    Name = destination,
                     // });
                     outputModules.Add(new Output() {
                        Name = destination,
                     });
                  }
               }
            }

            foreach (IModule outputModule in outputModules) {
               modules.Add(outputModule.Name, outputModule);
            }
         }

         int cycle = 0;
         for (; ; cycle++) {
            Console.WriteLine(cycle);
            
            // if (cycle == 1000) {
            //    break;
            // }

            if (isRXReached) {
               break;
            }
            
            // if (modules.Values.All(m => m.IsOriginalState())) {
            //    if (cycle != 0) {
            //       break;
            //    }
            // }

            queue.Enqueue((modules["broadcaster"], Pulse.Low, null));
            
            while (queue.Count > 0) {
               // if (modules.Values.All(m => m.IsOriginalState())) {
               //    if (cycle != 0) {
               //       break;
               //    }
               // }
               
               (IModule module, Pulse pulse, IModule sender) = queue.Dequeue();
               switch (pulse) {
                  case Pulse.High: {
                     module.ReceieveHighPulse(sender);
                     break;
                  }

                  case Pulse.Low: {
                     module.ReceieveLowPulse(sender);
                     break;
                  }
               }
            }
         }

         Console.WriteLine(cycle);
         Console.WriteLine(lowPulseCount);
         Console.WriteLine(highPulseCount);
      }

      static void Part2() {
         Part1();
      }

      internal static void SendPulse(List<string> destinationModules, Pulse pulse, IModule sender) {
         foreach (string destination in destinationModules) {
            queue.Enqueue((modules[destination], pulse, sender));

            // switch (pulse) {
            //    case Pulse.High:
            //       highPulseCount++;
            //       break;
            //    case Pulse.Low:
            //       lowPulseCount++;
            //       break;
            // }
         }
      }
   }

   interface IModule {
      void ReceieveLowPulse(IModule sender);
      void ReceieveHighPulse(IModule sender);
      string Name { get; set; }
      List<string> DestinationModules { get; set; }
      bool IsOriginalState();
   }

   class FlipFlop : IModule {
      public string Name { get; set; }
      public List<string> DestinationModules { get; set; }
      public bool isOn = false;

      public void ReceieveHighPulse(IModule _) {
         Program.highPulseCount++;
      }

      public void ReceieveLowPulse(IModule _) {
         Program.lowPulseCount++;
         
         isOn = !isOn;

         if (isOn) {
            Program.SendPulse(DestinationModules, Pulse.High, this);
         } else {
            Program.SendPulse(DestinationModules, Pulse.Low, this);
         }
      }

      public bool IsOriginalState() {
         return !isOn;
      }
   }

   class Conjuction : IModule {
      public string Name { get; set; }
      public List<string> DestinationModules { get; set; }
      public Dictionary<string, Pulse> memory = new Dictionary<string, Pulse>();


      public void ReceieveHighPulse(IModule sender) {
         Program.highPulseCount++;
         
         memory[sender.Name] = Pulse.High;

         if (memory.All(x => x.Value == Pulse.High)) {
            Program.SendPulse(DestinationModules, Pulse.Low, this);
         } else {
            Program.SendPulse(DestinationModules, Pulse.High, this);
         }
      }

      public void ReceieveLowPulse(IModule sender) {
         Program.lowPulseCount++;
         
         memory[sender.Name] = Pulse.Low;

         if (memory.All(x => x.Value == Pulse.High)) {
            Program.SendPulse(DestinationModules, Pulse.Low, this);
         } else {
            Program.SendPulse(DestinationModules, Pulse.High, this);
         }
      }

      public bool IsOriginalState() {
         return memory.All(x => x.Value == Pulse.Low);
      }
   }

   class Broadcaster : IModule {
      public string Name { get; set; }
      public List<string> DestinationModules { get; set; }

      public void ReceieveHighPulse(IModule _) {
         throw new Exception("Broadcaster cannot recieve high pulse");
      }

      public void ReceieveLowPulse(IModule _) {
         Program.lowPulseCount++;
         
         Program.SendPulse(DestinationModules, Pulse.Low, this);
      }

      public bool IsOriginalState() {
         return true;
      }
   }

   class Output : IModule {
      public string Name { get; set; }
      public List<string> DestinationModules { get; set; }

      public bool IsOriginalState() {
         return true;
      }

      public void ReceieveHighPulse(IModule _) {
         Program.highPulseCount++;
      }

      public void ReceieveLowPulse(IModule _) {
         Program.lowPulseCount++;

         if (Name == "rx") {
            Program.isRXReached = true;
         }
      }
   }

   enum Pulse {
      High,
      Low
   }
}
