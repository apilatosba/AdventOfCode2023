using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Windows;

namespace Day24 {
   class Program {
      static void Main() {
         checked {
            Part2();
         }
      }

      static void Part1() {
         string[] input = File.ReadAllLines("input.txt");
         List<Hailstone> hailstones = new List<Hailstone>(input.Length);

         foreach (string line in input) {
            string[] positionAndVelocity = line.Split(" @ ");

            double[] position = positionAndVelocity[0].Split(", ").Select(double.Parse).ToArray();
            double[] velocity = positionAndVelocity[1].Split(", ").Select(double.Parse).ToArray();

            hailstones.Add(new Hailstone {
               position = new Double3(position[0], position[1], position[2]),
               velocity = new Double3(velocity[0], velocity[1], velocity[2])
            });
         }

         int sum = 0;
         for (int i = 0; i < hailstones.Count; i++) {
            for (int j = i + 1; j < hailstones.Count; j++) {
               if (Intersects(hailstones[i], hailstones[j], out Double3 intersection)) {
                  if (IsInTestArea(intersection)) {
                     if (IsInPast(hailstones[i], intersection) ||
                         IsInPast(hailstones[j], intersection)) {
                        continue;
                     }

                     Console.WriteLine($"Intersection: {intersection} | {i} {j}");
                     sum++;
                  }
               }
            }
         }

         Console.WriteLine(sum);
      }

      static void Part2() {
         string[] input = File.ReadAllLines("sample input.txt");
         List<Hailstone> hailstones = new List<Hailstone>(input.Length);

         foreach (string row in input) {
            string[] positionAndVelocity = row.Split(" @ ");

            double[] position = positionAndVelocity[0].Split(", ").Select(double.Parse).ToArray();
            double[] velocity = positionAndVelocity[1].Split(", ").Select(double.Parse).ToArray();

            hailstones.Add(new Hailstone {
               position = new Double3(position[0], position[1], position[2]),
               velocity = new Double3(velocity[0], velocity[1], velocity[2])
            });
         }

         // Find the line
         Hailstone line;

         {
            Double3 p1;
            Double3 p2;
            int q = 0;
            tryAgain:
            try {
               p1 = PlaneLine(hailstones[q], hailstones[q + 1], hailstones[q + 2]);
               p2 = PlaneLine(hailstones[q], hailstones[q + 2], hailstones[q + 1]);
            } catch (Exception) {
               q++;
               goto tryAgain;
            }

            line = new Hailstone {
               position = p1,
               velocity = p2 - p1
            };
         }

         // Find intersection points with every other line
         List<(Double3, Hailstone)> intersections = new List<(Double3, Hailstone)>(input.Length);

         for (int i = 0; i < hailstones.Count; i++) {
            if (Intersects3D(hailstones[i], line, out Double3 intersection)) {
               intersections.Add((intersection, hailstones[i]));
            } else {
               throw new Exception("line does not intersect with the one of the hailstones. problem");
            }
         }

         // Sort the intersection points by their positions on the line
         SortedList<double, (Double3 intersectionPoint, Hailstone hailstone)> sortedIntersections = new SortedList<double, (Double3, Hailstone)>(intersections.Count);
         foreach ((Double3 intersection, Hailstone h) in intersections) {
            double distance = intersection.Distance(line.position);
            sortedIntersections.Add(distance, (intersection, h));
         }

         // Sanity check
         for (int i = 0; i < sortedIntersections.Count - 1; i++) {
            double distance = sortedIntersections.Values[i].intersectionPoint.Distance(sortedIntersections.Values[i + 1].intersectionPoint);
            Console.WriteLine($"Step {i},{i + 1}: {distance}");
         }

         // Find which end to start from
         Hailstone start = sortedIntersections.Values[0].hailstone;

         Double3 result;
         if ((start.position + start.velocity) == sortedIntersections.Values[0].intersectionPoint) {
            Double3 velocity = sortedIntersections.Values[1].intersectionPoint - start.position;
            result = start.position - velocity;
         } else {
            Debug.Assert(sortedIntersections.Values[sortedIntersections.Count - 1].intersectionPoint == (sortedIntersections.Values[sortedIntersections.Count - 1].hailstone.position + sortedIntersections.Values[sortedIntersections.Count - 1].hailstone.velocity));
            Double3 velocity = sortedIntersections.Values[sortedIntersections.Count - 2].intersectionPoint - sortedIntersections.Values[sortedIntersections.Count - 1].intersectionPoint;
            result = sortedIntersections.Values[sortedIntersections.Count - 1].intersectionPoint - velocity;
         }

         Console.WriteLine(result);
      }

      static Double3 PlaneLine(Hailstone plane1, Hailstone plane2, Hailstone line) {
         if (Intersects3D(plane1, plane2, out Double3 intersection)) {
            Plane plane = new Plane() {
               position = intersection,
               normal = plane1.velocity.Cross(plane2.velocity)
            };

            if (Intersects(line, plane, out intersection)) {
               return intersection;
            } else {
               throw new Exception("plane does not intersect with the line. problem");
            }

         } else {
            throw new Exception("try another pair");
         }
      }

      static bool IsInTestArea(Double3 intersection) {
         double lowerLimit = 200000000000000d;
         double upperLimit = 400000000000000d;

         return intersection.X >= lowerLimit && intersection.X <= upperLimit &&
                intersection.Y >= lowerLimit && intersection.Y <= upperLimit;
      }

      static bool IsInPast(Hailstone hailstone, Double3 intersection) {
         for (int i = 0; i < 3; i++) {
            double xyz = hailstone.position[i];

            if (xyz == 0) {
               continue;
            }

            double velocity = hailstone.velocity[i];
            double intersectionXYZ = intersection[i];

            return velocity > 0 ? intersectionXYZ < xyz : intersectionXYZ > xyz;
         }

         throw new Exception("Hailstone has no velocity");
      }

      // TODO needs to be in 3d
      static bool Intersects(Hailstone a, Hailstone b, out Double3 intersection) {
         Double3 ap1 = a.position;
         Double3 ap2 = a.position + a.velocity;
         Double3 bp1 = b.position;
         Double3 bp2 = b.position + b.velocity;

         double denominator;
         checked {
            denominator = (ap1.X - ap2.X) * (bp1.Y - bp2.Y) - (ap1.Y - ap2.Y) * (bp1.X - bp2.X);
         }

         // if (denominator == 0) {
         if (Math.Abs(denominator) < 0.0000000001d) {
            intersection = Double3.Zero;
            Console.WriteLine("Parallel");
            return false;
         }

         double xNumerator = (ap1.X * ap2.Y - ap1.Y * ap2.X) * (bp1.X - bp2.X) - (ap1.X - ap2.X) * (bp1.X * bp2.Y - bp1.Y * bp2.X);
         double yNumerator = (ap1.X * ap2.Y - ap1.Y * ap2.X) * (bp1.Y - bp2.Y) - (ap1.Y - ap2.Y) * (bp1.X * bp2.Y - bp1.Y * bp2.X);
         intersection = new Double3(xNumerator / denominator, yNumerator / denominator, 0);
         return true;
      }

      static bool Intersects3D(Hailstone a, Hailstone b, out Double3 intersection) {
         Double3 result;
         intersection = Double3.Zero;
         
         // XY
         if (Intersects(a, b, out result)) {
            intersection.x = result.x;
            intersection.y = result.y;
         } else {
            intersection = Double3.Zero;
            return false;
         }

         // YZ
         if (Intersects(
            new Hailstone() {
               position = new Double3(a.position.y, a.position.Z, 0),
               velocity = new Double3(a.velocity.y, a.velocity.Z, 0)
            },
            new Hailstone() {
               position = new Double3(b.position.y, b.position.Z, 0),
               velocity = new Double3(b.velocity.y, b.velocity.Z, 0)
            },
            out result)) {
            intersection.z = result.y;
         } else {
            intersection = Double3.Zero;
            return false;
         }

         return true;
      }

      static bool Intersects(Hailstone h, Plane p, out Double3 intersection) {
         if (Math.Abs(h.velocity.Dot(p.normal)) < 0.0000000001d) {
            intersection = Double3.Zero;
            Console.WriteLine("Parallel");
            return false;
         }

         double d = (p.position - h.position).Dot(p.normal) / h.velocity.Dot(p.normal);

         intersection = h.position + h.velocity * d;
         return true;
      }
   }

   class Plane {
      public Double3 position;
      public Double3 normal;
   }

   class Hailstone {
      public Double3 position;
      public Double3 velocity;
   }

   struct Double3 {
      public double x;
      public double y;
      public double z;

      public double X => x;
      public double Y => y;
      public double Z => z;

      public static Double3 Zero => new Double3(0, 0, 0);

      public Double3(double x, double y, double z) {
         this.x = x;
         this.y = y;
         this.z = z;
      }

      public Double3 Cross(Double3 other) {
         return new Double3(
            y * other.z - z * other.y,
            z * other.x - x * other.z,
            x * other.y - y * other.x
         );
      }

      public double Dot(Double3 other) {
         return x * other.x + y * other.y + z * other.z;
      }

      public Double3 Project(Hailstone line) {
         Double3 QP = this - line.position;
         Double3 lineDir = line.velocity.Normalize();

         return line.position + lineDir * QP.Dot(lineDir);
      }

      public Double3 Normalize() {
         double length = Math.Sqrt(x * x + y * y + z * z);
         return new Double3(x / length, y / length, z / length);
      }

      public double Length() {
         return Math.Sqrt(x * x + y * y + z * z);
      }

      public double LengthSquared() {
         return x * x + y * y + z * z;
      }

      public double Distance(Double3 other) {
         return (this - other).Length();
      }

      public readonly double this[int i] => i switch {
         0 => x,
         1 => y,
         2 => z,
         _ => throw new IndexOutOfRangeException(),
      };

      public override string ToString() {
         return $"({x}, {y}, {z})";
      }

      public static Double3 operator +(Double3 a, Double3 b) {
         return new Double3(a.x + b.x, a.y + b.y, a.z + b.z);
      }

      public static Double3 operator -(Double3 a, Double3 b) {
         return new Double3(a.x - b.x, a.y - b.y, a.z - b.z);
      }

      public static Double3 operator *(Double3 a, double b) {
         return new Double3(a.x * b, a.y * b, a.z * b);
      }

      public static bool operator ==(Double3 a, Double3 b) {
         return a.Distance(b) < 0.0000000001d;
      }

      public static bool operator !=(Double3 a, Double3 b) {
         return !(a == b);
      }
   }
}