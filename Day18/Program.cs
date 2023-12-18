using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

// xor fill, shoelace, pick's theorem. xor fill has the same problem as this one (raycasting), passing through edges (tangent) is no good
namespace Day18 {
   class Program {
      static void Main() {
         Part1();
      }

      static void Part1() {
         string[] input = File.ReadAllLines("input.txt");
         Instruction[] instructions = new Instruction[input.Length];

         // Parse input
         {
            Regex regex = new Regex(@"(?<direction>[RUDL]) (?<steps>\d+) \(#(?<color>\w+)\)");

            for (int i = 0; i < input.Length; i++) {
               Match match = regex.Match(input[i]);
               instructions[i] = new Instruction {
                  direction = match.Groups["direction"].Value switch {
                     "R" => Direction.Right,
                     "U" => Direction.Up,
                     "D" => Direction.Down,
                     "L" => Direction.Left,
                     _ => throw new Exception("Invalid direction")
                  },
                  steps = int.Parse(match.Groups["steps"].Value)
               };
            }
         }

         int maxX = 0;
         int maxY = 0;
         int minX = 0;
         int minY = 0;

         // Preprocess to find boundaries
         {
            int x = 0;
            int y = 0;

            foreach (Instruction instruction in instructions) {
               switch (instruction.direction) {
                  case Direction.Up: {
                     y -= instruction.steps;
                     break;
                  }

                  case Direction.Down: {
                     y += instruction.steps;
                     break;
                  }

                  case Direction.Left: {
                     x -= instruction.steps;
                     break;
                  }

                  case Direction.Right: {
                     x += instruction.steps;
                     break;
                  }
               }

               maxX = Math.Max(maxX, x);
               maxY = Math.Max(maxY, y);
               minX = Math.Min(minX, x);
               minY = Math.Min(minY, y);
            }
         }

         List<(int x, Direction direction, int steps)>[] edgesY = new List<(int x, Direction direction, int steps)>[maxY - minY + 1];
         List<(int y, Direction direction, int steps)>[] edgesX = new List<(int y, Direction direction, int steps)>[maxX - minX + 1];

         // Initialize edges
         {
            for (int i = 0; i < edgesY.Length; i++) {
               edgesY[i] = new List<(int x, Direction direction, int steps)>();
            }

            for (int i = 0; i < edgesX.Length; i++) {
               edgesX[i] = new List<(int y, Direction direction, int steps)>();
            }
         }

         // Find edges
         {
            int x = -minX;
            int y = -minY;

            foreach (Instruction instruction in instructions) {
               edgesY[y].Add((x, instruction.direction, instruction.steps));
               edgesX[x].Add((y, instruction.direction, instruction.steps));

               switch (instruction.direction) {
                  case Direction.Up: {
                     y -= instruction.steps;
                     break;
                  }

                  case Direction.Down: {
                     y += instruction.steps;
                     break;
                  }

                  case Direction.Left: {
                     x -= instruction.steps;
                     break;
                  }

                  case Direction.Right: {
                     x += instruction.steps;
                     break;
                  }
               }
            }
         }

         int count = 0;
         bool[,] isInside = new bool[maxY - minY + 1, maxX - minX + 1];
         int edgeCount = 0;

         // Iterate through every point and check if it is inside and increase the counter
         {
            for (int y = 0; y <= maxY - minY; y++) {
               for (int x = 0; x <= maxX - minX; x++) {
                  if (IsOnEdge(y, x, edgesY, edgesX)) {
                     count++;
                     edgeCount++;
                     isInside[y, x] = true;
                     continue;
                  }
                  
                  int intersections;
                  Direction wayToGo = FindClosestExitWay(y, x, maxY - minY, maxX - minX);
                  bool notPerpendicularExists;
                  List<Direction> triedWays = new List<Direction>(4);

                  bool failedToDetermine = false;
                  for (; ; ) {
                     notPerpendicularExists = false;
                     intersections = 0;
                     
                     switch (wayToGo) {
                        case Direction.Up: {
                           for (int yy = y - 1; yy >= 0; yy--) {
                              if (IsOnEdge(yy, x, edgesY, edgesX)) {
                                 if (!IsPerpendicular(wayToGo, yy, x, edgesY, edgesX)) {
                                    notPerpendicularExists = true;
                                 }

                                 intersections++;
                              }
                           }

                           triedWays.Add(wayToGo);
                           break;
                        }

                        case Direction.Down: {
                           for (int yy = y + 1; yy <= maxY - minY; yy++) {
                              if (IsOnEdge(yy, x, edgesY, edgesX)) {
                                 if (!IsPerpendicular(wayToGo, yy, x, edgesY, edgesX)) {
                                    notPerpendicularExists = true;
                                 }
                                 
                                 intersections++;
                              }
                           }

                           triedWays.Add(wayToGo);
                           break;
                        }

                        case Direction.Left: {
                           for (int xx = x - 1; xx >= 0; xx--) {
                              if (IsOnEdge(y, xx, edgesY, edgesX)) {
                                 if (!IsPerpendicular(wayToGo, y, xx, edgesY, edgesX)) {
                                    notPerpendicularExists = true;
                                 }
                                 
                                 intersections++;
                              }
                           }

                           triedWays.Add(wayToGo);
                           break;
                        }

                        case Direction.Right: {
                           for (int xx = x + 1; xx <= maxX - minX; xx++) {
                              if (IsOnEdge(y, xx, edgesY, edgesX)) {
                                 if (!IsPerpendicular(wayToGo, y, xx, edgesY, edgesX)) {
                                    notPerpendicularExists = true;
                                 }
                                 
                                 intersections++;
                              }
                           }

                           triedWays.Add(wayToGo);
                           break;
                        }
                     }

                     if (notPerpendicularExists) {
                        if (triedWays.Count == 4) {
                           // throw new Exception("tried all ways but still cant determine if it is inside or outside.");
                           // Console.WriteLine($"tried all ways but still cant determine if it is inside or outside. ({y}, {x})");
                           failedToDetermine = true;
                           break;
                        } else {
                           Direction[] ways = new Direction[] { Direction.Up, Direction.Down, Direction.Left, Direction.Right };
                           wayToGo = ways.Except(triedWays).First();
                        }
                     } else {
                        if (intersections % 2 == 1) {
                           count++;
                           isInside[y, x] = true;
                           break;
                        } else {
                           // outside
                           break;
                        }
                     }

                     // if (intersections % 2 == 1) {
                     //    count++;
                     //    break;
                     // } else {
                     //    if (notPerpendicularExists) {
                     //       // cannot determine going to this way. should try another way
                     //       if (triedWays.Count == 4) {
                     //          // throw new Exception("tried all ways but still cant determine if it is inside or outside.");
                     //          Console.WriteLine($"tried all ways but still cant determine if it is inside or outside. ({y}, {x})");
                     //          failedToDetermine = true;
                     //          break;
                     //       } else {
                     //          Direction[] ways = new Direction[] { Direction.Up, Direction.Down, Direction.Left, Direction.Right };
                     //          wayToGo = ways.Except(triedWays).First();
                     //       }
                     //    } else {
                     //       break; // outside
                     //    }
                     // }
                  }

                  if (failedToDetermine) {
                     // Now i am gonna try diagonal ways

                     intersections = 0;
                     bool vertexExists = false;

                     // Up left
                     {
                        int xx = x - 1;
                        int yy = y - 1;

                        while (xx >= 0 && yy >= 0) {
                           if (IsOnEdge(yy, xx, edgesY, edgesX)) {
                              if (IsVertex(yy, xx, edgesY, edgesX)) {
                                 vertexExists = true;
                              }

                              intersections++;
                           }
                           
                           xx--;
                           yy--;
                        }

                        if (intersections % 2 == 1) {
                           count++;
                           isInside[y, x] = true;
                           continue;
                        } else {
                           if (vertexExists) {
                              // try next diagonal
                           } else {
                              // outside
                              continue;
                           }
                        }
                     }

                     intersections = 0;
                     vertexExists = false;

                     // Up right
                     {
                        int xx = x + 1;
                        int yy = y - 1;

                        while (xx <= maxX - minX && yy >= 0) {
                           if (IsOnEdge(yy, xx, edgesY, edgesX)) {
                              if (IsVertex(yy, xx, edgesY, edgesX)) {
                                 vertexExists = true;
                              }

                              intersections++;
                           }

                           xx++;
                           yy--;
                        }

                        if (intersections % 2 == 1) {
                           count++;
                           isInside[y, x] = true;
                           continue;
                        } else {
                           if (vertexExists) {
                              // try next diagonal
                           } else {
                              // outside
                              continue;
                           }
                        }
                     }

                     intersections = 0;
                     vertexExists = false;

                     // Down left
                     {
                        int xx = x - 1;
                        int yy = y + 1;

                        while (xx >= 0 && yy <= maxY - minY) {
                           if (IsOnEdge(yy, xx, edgesY, edgesX)) {
                              if (IsVertex(yy, xx, edgesY, edgesX)) {
                                 vertexExists = true;
                              }

                              intersections++;
                           }

                           xx--;
                           yy++;
                        }

                        if (intersections % 2 == 1) {
                           count++;
                           isInside[y, x] = true;
                           continue;
                        } else {
                           if (vertexExists) {
                              // try next diagonal
                           } else {
                              // outside
                              continue;
                           }
                        }
                     }

                     intersections = 0;
                     vertexExists = false;

                     // Down right
                     {
                        int xx = x + 1;
                        int yy = y + 1;

                        while (xx <= maxX - minX && yy <= maxY - minY) {
                           if (IsOnEdge(yy, xx, edgesY, edgesX)) {
                              if (IsVertex(yy, xx, edgesY, edgesX)) {
                                 vertexExists = true;
                              }

                              intersections++;
                           }

                           xx++;
                           yy++;
                        }

                        if (intersections % 2 == 1) {
                           count++;
                           isInside[y, x] = true;
                           continue;
                        } else {
                           if (vertexExists) {
                              // throw new Exception("tried all ways including diagonals (8 ways in total) but still cant determine if it is inside or outside.");
                              if (!IsOnEdge(y - 1, x, edgesY, edgesX)) {
                                 if (isInside[y - 1, x]) {
                                    count++;
                                    isInside[y, x] = true;
                                 }
                                 continue;
                              } else {
                                 if (!IsOnEdge(y, x - 1, edgesY, edgesX)) {
                                    if (isInside[y, x - 1]) {
                                       count++;
                                       isInside[y, x] = true;
                                    }

                                    continue;
                                 }
                              }

                              throw new Exception("tried all ways including diagonals (8 ways in total) and two others but still cant determine if it is inside or outside.");
                           } else {
                              // outside
                              continue;
                           }
                        }
                     }
                  }
               }
            }
         }

         Console.WriteLine(count);
      }

      static void Part2() {

      }

      static bool IsVertex(int y, int x, List<(int x, Direction direction, int steps)>[] edgesY, List<(int y, Direction direction, int steps)>[] edgesX) {
         foreach ((int xx, Direction direction, int steps) in edgesY[y]) {
            if (x == xx) {
               return true;
            }

            if (direction == Direction.Left) {
               if (x == xx - steps) {
                  return true;
               }
            } else if (direction == Direction.Right) {
               if (x == xx + steps) {
                  return true;
               }
            }
         }

         foreach ((int yy, Direction direction, int steps) in edgesX[x]) {
            if (y == yy) {
               return true;
            }

            if (direction == Direction.Up) {
               if (y == yy - steps) {
                  return true;
               }
            } else if (direction == Direction.Down) {
               if (y == yy + steps) {
                  return true;
               }
            }
         }

         return false;
      }

      static bool IsPerpendicular(Direction travellingDirection, int y, int x, List<(int x, Direction direction, int steps)>[] edgesY, List<(int y, Direction direction, int steps)>[] edgesX) {
         if (!IsOnEdge(y, x, edgesY, edgesX)) {
            throw new Exception("Not on edge");
         }

         if (travellingDirection == Direction.Up || travellingDirection == Direction.Down) {
            foreach ((int xx, Direction direction, int steps) in edgesY[y]) {
               if (direction == Direction.Left) {
                  if (x <= xx && x >= xx - steps) {
                     return true;
                  }
               } else if (direction == Direction.Right) {
                  if (x >= xx && x <= xx + steps) {
                     return true;
                  }
               }
            }
         } else { // If travelling right or left
            foreach ((int yy, Direction direction, int steps) in edgesX[x]) {
               if (direction == Direction.Up) {
                  if (y <= yy && y >= yy - steps) {
                     return true;
                  }
               } else if (direction == Direction.Down) {
                  if (y >= yy && y <= yy + steps) {
                     return true;
                  }
               }
            }
         }

         return false;
      }

      static Direction FindClosestExitWay(int y, int x, int totalY, int totalX) {
         int distanceToTop = y;
         int distanceToBottom = totalY - y;
         int distanceToLeft = x;
         int distanceToRight = totalX - x;

         int minDistance = Math.Min(distanceToTop, Math.Min(distanceToBottom, Math.Min(distanceToLeft, distanceToRight)));

         if (minDistance == distanceToTop) {
            return Direction.Up;
         } else if (minDistance == distanceToBottom) {
            return Direction.Down;
         } else if (minDistance == distanceToLeft) {
            return Direction.Left;
         } else {
            return Direction.Right;
         }
      }

      static bool IsOnEdge(int y, int x, List<(int x, Direction direction, int steps)>[] edgesY, List<(int y, Direction direction, int steps)>[] edgesX) {
         // Y edges
         foreach ((int xx, Direction direction, int steps) in edgesY[y]) {
            if (direction == Direction.Left) {
               if (x <= xx && x >= xx - steps) {
                  return true;
               }
            }

            if (direction == Direction.Right) {
               if (x >= xx && x <= xx + steps) {
                  return true;
               }
            }
         }

         // X edges
         foreach ((int yy, Direction direction, int steps) in edgesX[x]) {
            if (direction == Direction.Up) {
               if (y <= yy && y >= yy - steps) {
                  return true;
               }
            }

            if (direction == Direction.Down) {
               if (y >= yy && y <= yy + steps) {
                  return true;
               }
            }
         }

         return false;
      }
   }

   enum Direction {
      Up,
      Down,
      Left,
      Right
   }

   class Instruction {
      public Direction direction;
      public int steps;

      public override string ToString() {
         return $"{direction} {steps}";
      }
   }
}