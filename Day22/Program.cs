using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day22 {
   class Program {
      static void Main() {
         Part2();
      }

      static void Part1() {
         string[] input = File.ReadAllLines("input.txt");
         // Sorted on lowest z. if zs are equal then no particular order. The sorted structure is not needed anymore, i changed to voxel grid.
         List<Brick>[] bricks = new List<Brick>[512];

         for (int i = 0; i < bricks.Length; i++) {
            bricks[i] = new List<Brick>();
         }

         int maxX = 0;
         int maxY = 0;
         int maxZ = 0;

         // Parse input
         {
            foreach (string line in input) {
               string[] xyz = line.Split('~');
               int[] xyzStart = xyz[0].Split(',').Select(int.Parse).ToArray();
               int[] xyzEnd = xyz[1].Split(',').Select(int.Parse).ToArray();

               Int3 start = new Int3() {
                  x = xyzStart[0],
                  y = xyzStart[1],
                  z = xyzStart[2]
               };

               Int3 end = new Int3() {
                  x = xyzEnd[0],
                  y = xyzEnd[1],
                  z = xyzEnd[2]
               };

               Brick brick = new Brick() {
                  start = start,
                  end = end
               };

               int lowestZIndex = Math.Min(start.z, end.z);
               bricks[lowestZIndex].Add(brick);

               maxX = Math.Max(maxX, Math.Max(start.x, end.x));
               maxY = Math.Max(maxY, Math.Max(start.y, end.y));
               maxZ = Math.Max(maxZ, Math.Max(start.z, end.z));
            }
         }

         Brick[,,] voxels = new Brick[maxZ + 1, maxY + 1, maxX + 1];

         for (int z = 0; z < voxels.GetLength(0); z++) {
            for (int y = 0; y < voxels.GetLength(1); y++) {
               for (int x = 0; x < voxels.GetLength(2); x++) {
                  voxels[z, y, x] = null;
               }
            }
         }

         for (int i = 0; i < bricks.Length; i++) {
            foreach (Brick brick in bricks[i]) {
               for (int z = brick.start.z; z <= brick.end.z; z++) {
                  for (int y = brick.start.y; y <= brick.end.y; y++) {
                     for (int x = brick.start.x; x <= brick.end.x; x++) {
                        voxels[z, y, x] = brick;
                     }
                  }
               }
            }
         }

         // I noticed start.z is always lower than end.z
         HashSet<Brick> falledBricks = new HashSet<Brick>();
         for (int voxelHeight = 0; voxelHeight < voxels.GetLength(0); voxelHeight++) {
            HashSet<Brick> bricksAtLevel = GetBricksAtLevel(voxels, voxelHeight);

            if (bricksAtLevel.Count == 0) {
               continue;
            }

            foreach (Brick brick in bricksAtLevel) {
               if (falledBricks.Contains(brick)) {
                  continue;
               }

               while (CanFallOneDown(voxels, brick)) {
                  // Clear current voxels
                  for (int z = brick.start.z; z <= brick.end.z; z++) {
                     for (int y = brick.start.y; y <= brick.end.y; y++) {
                        for (int x = brick.start.x; x <= brick.end.x; x++) {
                           voxels[z, y, x] = null;
                        }
                     }
                  }

                  // Update brick
                  brick.start.z--;
                  brick.end.z--;

                  // Update voxels
                  for (int z = brick.start.z; z <= brick.end.z; z++) {
                     for (int y = brick.start.y; y <= brick.end.y; y++) {
                        for (int x = brick.start.x; x <= brick.end.x; x++) {
                           voxels[z, y, x] = brick;
                        }
                     }
                  }
               }

               falledBricks.Add(brick);
            }
         }

         // Looks correct to me
         foreach (Brick brick in falledBricks) {
            Console.WriteLine($"Brick: {brick.start.x},{brick.start.y},{brick.start.z}~{brick.end.x},{brick.end.y},{brick.end.z}");
         }

         HashSet<Brick> bricksThatCantBeDisintegrated = new HashSet<Brick>();

         foreach (Brick brick in falledBricks) {
            HashSet<Brick> supporterBricks = GetSupporterBricks(voxels, brick);

            if (supporterBricks.Count == 1) {
               _ = bricksThatCantBeDisintegrated.Add(supporterBricks.First());
            }
         }

         Console.WriteLine(falledBricks.Count - bricksThatCantBeDisintegrated.Count);
      }

      static void Part2() {
         string[] input = File.ReadAllLines("input.txt");
         // Sorted on lowest z. if zs are equal then no particular order. The sorted structure is not needed anymore, i changed to voxel grid.
         List<Brick>[] bricks = new List<Brick>[512];

         for (int i = 0; i < bricks.Length; i++) {
            bricks[i] = new List<Brick>();
         }

         int maxX = 0;
         int maxY = 0;
         int maxZ = 0;

         // Parse input
         {
            foreach (string line in input) {
               string[] xyz = line.Split('~');
               int[] xyzStart = xyz[0].Split(',').Select(int.Parse).ToArray();
               int[] xyzEnd = xyz[1].Split(',').Select(int.Parse).ToArray();

               Int3 start = new Int3() {
                  x = xyzStart[0],
                  y = xyzStart[1],
                  z = xyzStart[2]
               };

               Int3 end = new Int3() {
                  x = xyzEnd[0],
                  y = xyzEnd[1],
                  z = xyzEnd[2]
               };

               Brick brick = new Brick() {
                  start = start,
                  end = end
               };

               int lowestZIndex = Math.Min(start.z, end.z);
               bricks[lowestZIndex].Add(brick);

               maxX = Math.Max(maxX, Math.Max(start.x, end.x));
               maxY = Math.Max(maxY, Math.Max(start.y, end.y));
               maxZ = Math.Max(maxZ, Math.Max(start.z, end.z));
            }
         }

         Brick[,,] voxels = new Brick[maxZ + 1, maxY + 1, maxX + 1];

         for (int z = 0; z < voxels.GetLength(0); z++) {
            for (int y = 0; y < voxels.GetLength(1); y++) {
               for (int x = 0; x < voxels.GetLength(2); x++) {
                  voxels[z, y, x] = null;
               }
            }
         }

         for (int i = 0; i < bricks.Length; i++) {
            foreach (Brick brick in bricks[i]) {
               for (int z = brick.start.z; z <= brick.end.z; z++) {
                  for (int y = brick.start.y; y <= brick.end.y; y++) {
                     for (int x = brick.start.x; x <= brick.end.x; x++) {
                        voxels[z, y, x] = brick;
                     }
                  }
               }
            }
         }

         // I noticed start.z is always lower than end.z
         HashSet<Brick> falledBricks = new HashSet<Brick>();
         for (int voxelHeight = 0; voxelHeight < voxels.GetLength(0); voxelHeight++) {
            HashSet<Brick> bricksAtLevel = GetBricksAtLevel(voxels, voxelHeight);

            if (bricksAtLevel.Count == 0) {
               continue;
            }

            foreach (Brick brick in bricksAtLevel) {
               if (falledBricks.Contains(brick)) {
                  continue;
               }

               while (CanFallOneDown(voxels, brick)) {
                  // Clear current voxels
                  for (int z = brick.start.z; z <= brick.end.z; z++) {
                     for (int y = brick.start.y; y <= brick.end.y; y++) {
                        for (int x = brick.start.x; x <= brick.end.x; x++) {
                           voxels[z, y, x] = null;
                        }
                     }
                  }

                  // Update brick
                  brick.start.z--;
                  brick.end.z--;

                  // Update voxels
                  for (int z = brick.start.z; z <= brick.end.z; z++) {
                     for (int y = brick.start.y; y <= brick.end.y; y++) {
                        for (int x = brick.start.x; x <= brick.end.x; x++) {
                           voxels[z, y, x] = brick;
                        }
                     }
                  }
               }

               falledBricks.Add(brick);
            }
         }

         // Looks correct to me
         foreach (Brick brick in falledBricks) {
            Console.WriteLine($"Brick: {brick.start.x},{brick.start.y},{brick.start.z}~{brick.end.x},{brick.end.y},{brick.end.z}");
         }

         HashSet<Brick> bricksThatCantBeDisintegrated = new HashSet<Brick>();

         foreach (Brick brick in falledBricks) {
            HashSet<Brick> supporterBricks = GetSupporterBricks(voxels, brick);

            if (supporterBricks.Count == 1) {
               _ = bricksThatCantBeDisintegrated.Add(supporterBricks.First());
            }
         }

         ulong count = 0;
         foreach (Brick brick in bricksThatCantBeDisintegrated) {
            Queue<Brick> queue = new Queue<Brick>();
            HashSet<Brick> bricksFalled = new HashSet<Brick>();
            
            queue.Enqueue(brick);
            
            while (queue.Count > 0) {
               Brick currentBrick = queue.Dequeue();
               HashSet<Brick> supportedBricks = GetSupportedBricks(voxels, currentBrick);
               bricksFalled.Add(currentBrick);

               foreach (Brick supportedBrick in supportedBricks) {
                  if (DoesBrickFallWithoutGivenSupporterBricks(voxels, supportedBrick, bricksFalled)) {
                     count++;
                     queue.Enqueue(supportedBrick);
                     // bricksFalled.Add(supportedBrick);
                  }
               }
            }
         }

         Console.WriteLine(count);
      }

      static bool DoesBrickFallWithoutGivenSupporterBrick(Brick[,,] voxels, Brick brick, Brick supporterBrick) {
         HashSet<Brick> supporterBricks = GetSupporterBricks(voxels, brick);
         if (!supporterBricks.Remove(supporterBrick)) {
            throw new Exception("supporterBrick does not support the brick at all");
         }

         return supporterBricks.Count == 0;
      }

      static bool DoesBrickFallWithoutGivenSupporterBricks(Brick[,,] voxels, Brick brick, HashSet<Brick> supporterBricks) {
         HashSet<Brick> allTheSupporterBricks = GetSupporterBricks(voxels, brick);
         allTheSupporterBricks.ExceptWith(supporterBricks);

         return allTheSupporterBricks.Count == 0;
      }

      static HashSet<Brick> GetSupportedBricks(Brick[,,] voxels, Brick brick) {
         HashSet<Brick> supportedBricks = new HashSet<Brick>();

         int zOneUp = brick.end.z + 1;

         for (int y = brick.start.y; y <= brick.end.y; y++) {
            for (int x = brick.start.x; x <= brick.end.x; x++) {
               if (voxels[zOneUp, y, x] != null) {
                  _ = supportedBricks.Add(voxels[zOneUp, y, x]);
               }
            }
         }

         return supportedBricks;
      }

      static HashSet<Brick> GetSupporterBricks(Brick[,,] voxels, Brick brick) {
         HashSet<Brick> supporterBricks = new HashSet<Brick>();

         int zOneDown = brick.start.z - 1;

         for (int y = brick.start.y; y <= brick.end.y; y++) {
            for (int x = brick.start.x; x <= brick.end.x; x++) {
               if (voxels[zOneDown, y, x] != null) {
                  _ = supporterBricks.Add(voxels[zOneDown, y, x]);
               }
            }
         }

         return supporterBricks;
      }

      static HashSet<Brick> GetBricksAtLevel(Brick[,,] voxels, int z) {
         HashSet<Brick> bricks = new HashSet<Brick>();

         for (int y = 0; y < voxels.GetLength(1); y++) {
            for (int x = 0; x < voxels.GetLength(2); x++) {
               if (voxels[z, y, x] != null) {
                  _ = bricks.Add(voxels[z, y, x]);
               }
            }
         }

         return bricks;
      }

      static bool CanFallOneDown(Brick[,,] voxels, Brick brick) {
         if (brick.start.z <= 1) {
            return false;
         }

         int zOneDown = brick.start.z - 1;

         for (int y = brick.start.y; y <= brick.end.y; y++) {
            for (int x = brick.start.x; x <= brick.end.x; x++) {
               if (voxels[zOneDown, y, x] != null) {
                  return false;
               }
            }
         }

         return true;
      }
   }

   class Brick {
      public Int3 start;
      public Int3 end;
   }

   struct Int3 {
      public int z;
      public int y;
      public int x;
   }
}