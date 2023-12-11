using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Transactions;

namespace Day10 {
   internal class Program {
      static void Main(string[] args) {
         Part1();
      }

      static void Part1() {
         string[] input = File.ReadAllLines("input.txt");
         int yOfS = 0;
         int xOfS = 0;

         for (int i = 0; i < input.Length; i++) {
            if (input[i].Contains('S')) {
               yOfS = i;
               xOfS = input[i].IndexOf('S');
               break;
            }
         }

         List<(int[], Direction firstMove)> positionsOfPipesConnectedToS = new List<(int[], Direction)>();
         // North
         {
            try {
               int[] pos = new int[] { yOfS - 1, xOfS };
               if (input[pos[0]][pos[1]] != '.' &&
                     GetDirections(input[pos[0]][pos[1]]).Contains(Direction.South)) {
                  positionsOfPipesConnectedToS.Add((pos, Direction.North));
               }
            } catch (IndexOutOfRangeException) { }
         }

         // East
         {
            try {
               int[] pos = new int[] { yOfS, xOfS + 1 };
               if (input[pos[0]][pos[1]] != '.' &&
                     GetDirections(input[pos[0]][pos[1]]).Contains(Direction.West)) {
                  positionsOfPipesConnectedToS.Add((pos, Direction.East));
               }
            } catch (IndexOutOfRangeException) { }
         }

         // South
         {
            try {
               int[] pos = new int[] { yOfS + 1, xOfS };
               if (input[pos[0]][pos[1]] != '.' &&
                     GetDirections(input[pos[0]][pos[1]]).Contains(Direction.North)) {
                  positionsOfPipesConnectedToS.Add((pos, Direction.South));
               }
            } catch (IndexOutOfRangeException) { }
         }

         // West
         {
            try {
               int[] pos = new int[] { yOfS, xOfS - 1 };
               if (input[pos[0]][pos[1]] != '.' &&
                     GetDirections(input[pos[0]][pos[1]]).Contains(Direction.East)) {
                  positionsOfPipesConnectedToS.Add((pos, Direction.West));
               }
            } catch (IndexOutOfRangeException) { }
         }

         Pipe root = new Pipe();
         foreach (var pos in positionsOfPipesConnectedToS) {
            bool success = false;
            //Walk(input, pos.Item1[0], pos.Item1[1], root, pos.firstMove, ref success);
            RecursiveToIterative(input, pos.Item1[0], pos.Item1[1], root, pos.firstMove, out success);
            //success = WalkIterative(input, pos.Item1[0], pos.Item1[1], root, pos.firstMove);

            if (success) break;
            else root = new Pipe();
         }

         int numberOfPipes = 1;
         Pipe current = root;
         while (current.outgoing != null) {
            numberOfPipes++;
            current = current.outgoing;
         }

         Console.WriteLine(numberOfPipes / 2);
      }

      static void Part2() {
         string[] input = File.ReadAllLines("input.txt");
         int yOfS = 0;
         int xOfS = 0;

         for (int i = 0; i < input.Length; i++) {
            if (input[i].Contains('S')) {
               yOfS = i;
               xOfS = input[i].IndexOf('S');
               break;
            }
         }

         List<(int[], Direction firstMove)> positionsOfPipesConnectedToS = new List<(int[], Direction)>();
         // North
         {
            try {
               int[] pos = new int[] { yOfS - 1, xOfS };
               if (input[pos[0]][pos[1]] != '.' &&
                     GetDirections(input[pos[0]][pos[1]]).Contains(Direction.South)) {
                  positionsOfPipesConnectedToS.Add((pos, Direction.North));
               }
            } catch (IndexOutOfRangeException) { }
         }

         // East
         {
            try {
               int[] pos = new int[] { yOfS, xOfS + 1 };
               if (input[pos[0]][pos[1]] != '.' &&
                     GetDirections(input[pos[0]][pos[1]]).Contains(Direction.West)) {
                  positionsOfPipesConnectedToS.Add((pos, Direction.East));
               }
            } catch (IndexOutOfRangeException) { }
         }

         // South
         {
            try {
               int[] pos = new int[] { yOfS + 1, xOfS };
               if (input[pos[0]][pos[1]] != '.' &&
                     GetDirections(input[pos[0]][pos[1]]).Contains(Direction.North)) {
                  positionsOfPipesConnectedToS.Add((pos, Direction.South));
               }
            } catch (IndexOutOfRangeException) { }
         }

         // West
         {
            try {
               int[] pos = new int[] { yOfS, xOfS - 1 };
               if (input[pos[0]][pos[1]] != '.' &&
                     GetDirections(input[pos[0]][pos[1]]).Contains(Direction.East)) {
                  positionsOfPipesConnectedToS.Add((pos, Direction.West));
               }
            } catch (IndexOutOfRangeException) { }
         }

         Pipe root = new Pipe();
         foreach (var pos in positionsOfPipesConnectedToS) {
            bool success = false;
            //Walk(input, pos.Item1[0], pos.Item1[1], root, pos.firstMove, ref success);
            success = WalkIterative(input, pos.Item1[0], pos.Item1[1], root, pos.firstMove);

            if (success) break;
            else root = new Pipe();
         }

         root.directions = new Direction[] {
            Direction.West,
            Direction.South
         };

         root.outgoingMove = Direction.South;
         root.y = yOfS;
         root.x = xOfS;

         List<AreaInfo> rows = new List<AreaInfo>();
         List<AreaInfo> columns = new List<AreaInfo>();
         {
            Pipe current = root;
            int y = yOfS;
            int x = xOfS;
            Direction nextMove = Direction.South;

            do {
               if (current.directions.Contains(Direction.East) || current.directions.Contains(Direction.West)) {
                  if (x == 8)
                     ;
                  if (columns.Exists(ai => ai.index == x)) {
                     columns.Where(ai => ai.index == x).First().restrictions.Add(y);
                  } else {
                     columns.Add(new AreaInfo() {
                        index = x,
                        restrictions = new List<int>() { y }
                     });
                  }
               }

               if (current.directions.Contains(Direction.North) || current.directions.Contains(Direction.South)) {
                  if (rows.Exists(ai => ai.index == y)) {
                     rows.Where(ai => ai.index == y).First().restrictions.Add(x);
                  } else {
                     rows.Add(new AreaInfo() {
                        index = y,
                        restrictions = new List<int>() { x }
                     });
                  }
               }

               CalculateNextPosition(ref y, ref x, nextMove);
               nextMove = current.outgoing.directions.First(d => d != GetOppositeDirection(nextMove)); //pipe.directions.First(d => d != GetOppositeDirection(lastMove))
               y = current.outgoing.y;
               x = current.outgoing.x;
               nextMove = current.outgoing.outgoingMove;
               current = current.outgoing;
            } while (current != root);

            foreach (AreaInfo ai in rows) ai.restrictions.Sort();
            foreach (AreaInfo ai in columns) ai.restrictions.Sort();
         }

         int result = 0;
         for (int y = 0; y < input.Length; y++) {
            for (int x = 0; x < input[y].Length; x++) {
               char c = input[y][x];

               if (y == 5 && x == 8)
                  ;

               if (c == '.') {
                  bool inVerticalRange = false;
                  bool inHorizontalRange = false;
                  // Vertical
                  if (columns.Exists(ai => ai.index == x)) {
                     List<int> restrictions = columns.Where(ai => ai.index == x).First().restrictions;
                     for (int i = 0; i < restrictions.Count; i += 2) {
                        if (i + 1 >= restrictions.Count) break;
                        if (y > restrictions[i] && y < restrictions[i + 1]) {
                           inVerticalRange = true;
                           break;
                        }
                     }
                  }

                  if (!inVerticalRange) continue;

                  if (rows.Exists(ai => ai.index == y)) {
                     List<int> restrictions = rows.Where(ai => ai.index == y).First().restrictions;
                     for (int i = 0; i < restrictions.Count; i += 2) {
                        if (i + 1 >= restrictions.Count) break;
                        if (x > restrictions[i] && x < restrictions[i + 1]) {
                           inHorizontalRange = true;
                           break;
                        }
                     }
                  }

                  if (inHorizontalRange) {
                     result++;
                  }
               }
            }
         }

         Console.WriteLine(result);
      }

      // StackOverflow
      // Very unlucky. C# compiler doesn't support tail call optimization. I found out this today and I am sad now, I am crying.
      static void Walk(string[] map, int y, int x, Pipe whereIComeFrom, Direction lastMove, ref bool success) {
         // Termination
         {
            if (y < 0 || y >= map.Length || x < 0 || x >= map[y].Length) {
               success = false;
               return;
            }

            if (map[y][x] == '.') {
               //throw new Exception("pipe is not connected to another pipe");
               success = false;
               return;
            }

            if (map[y][x] == 'S') {
               //Pipe pipe = new Pipe();
               //pipe.incoming = whereIComeFrom;
               //// Set outgoing after the recursion. It is gonna be the pipe you started walking.
               //whereIComeFrom.outgoing = pipe;
               success = true;
               return;
            }
         }

         // Step
         {
            Pipe pipe = new Pipe();
            pipe.directions = GetDirections(map[y][x]);
            whereIComeFrom.outgoing = pipe;
            pipe.incoming = whereIComeFrom;

            Direction nextMove = pipe.directions.First(d => d != GetOppositeDirection(lastMove));
            CalculateNextPosition(ref y, ref x, nextMove);

            Walk(map, y, x, pipe, nextMove, ref success);
         }
      }

      static void RecursiveToIterative(string[] map, int y, int x, Pipe whereIComeFrom, Direction lastMove, out bool success) {
         for (; ; ) {
            // Termination
            {
               if (y < 0 || y >= map.Length || x < 0 || x >= map[y].Length) {
                  success = false;
                  break;
               }

               if (map[y][x] == '.') {
                  //throw new Exception("pipe is not connected to another pipe");
                  success = false;
                  break;
               }

               if (map[y][x] == 'S') {
                  //Pipe pipe = new Pipe();
                  //pipe.incoming = whereIComeFrom;
                  //// Set outgoing after the recursion. It is gonna be the pipe you started walking.
                  //whereIComeFrom.outgoing = pipe;
                  success = true;
                  break;
               }
            }

            // Step
            {
               Pipe pipe = new Pipe();
               pipe.directions = GetDirections(map[y][x]);
               whereIComeFrom.outgoing = pipe;
               pipe.incoming = whereIComeFrom;

               Direction nextMove = pipe.directions.First(d => d != GetOppositeDirection(lastMove));
               CalculateNextPosition(ref y, ref x, nextMove);

               whereIComeFrom = pipe;
               lastMove = nextMove;
            }
         }
      }

      static void WalkFakeRecursive(string[] map, int y, int x, Pipe whereIComeFrom, Direction lastMove, ref bool success) {
         Action<string[], int, int, Pipe, Direction, bool> action = (string[] map, int y, int x, Pipe whereIComeFrom, Direction lastMove, bool success) => {
            // Termination
            {
               if (y < 0 || y >= map.Length || x < 0 || x >= map[y].Length) {
                  success = false;
                  return;
               }

               if (map[y][x] == '.') {
                  //throw new Exception("pipe is not connected to another pipe");
                  success = false;
                  return;
               }

               if (map[y][x] == 'S') {
                  //Pipe pipe = new Pipe();
                  //pipe.incoming = whereIComeFrom;
                  //// Set outgoing after the recursion. It is gonna be the pipe you started walking.
                  //whereIComeFrom.outgoing = pipe;
                  success = true;
                  return;
               }
            }

            // Step
            {
               Pipe pipe = new Pipe();
               pipe.directions = GetDirections(map[y][x]);
               whereIComeFrom.outgoing = pipe;
               pipe.incoming = whereIComeFrom;

               Direction nextMove = pipe.directions.First(d => d != GetOppositeDirection(lastMove));
               CalculateNextPosition(ref y, ref x, nextMove);
            }
         };

         Stack<WalkArguments> stack = new Stack<WalkArguments>();
         stack.Push(new WalkArguments(map, y, x, whereIComeFrom, lastMove, success));

         for (; ; ) {
            WalkArguments walkArguments = stack.Pop();
            action(walkArguments.map, walkArguments.y, walkArguments.x, walkArguments.whereIComeFrom, walkArguments.lastMove, walkArguments.success);

         }
      }

      static void LinearNonTailRecursive(int x, int y, List<string> list) {
         // Termination
         {
            if (x <= 0) return;
            if (y <= 0) return;
         }

         // Step
         {
            list.Clear();
            x++;

            LinearNonTailRecursive(x, y, list);

            list.Add("x = " + x.ToString());
         }
      }

      static void Iterative(int x, int y, List<string> list) {
         Stack<(int, int, List<string>, int returnAddress)> stack = new Stack<(int, int, List<string>, int)>();

         stack.Push((x, y, list, 1));
         (int, int, List<string>, int returnAddress) stackFrame;
         for (; ; ) {
            stackFrame = stack.Pop();
            switch (stackFrame.returnAddress) {
               case 1: {
                  goto nrb1;
               }
               case 2: {
                  goto nrb2;
               }
            }

         nrb1:
            if (NRB1(ref x, ref y, ref list)) break;
            stack.Push((x, y, list, 2));
         //LinearNonTailRecursive(x, y, list);

         nrb2:
            NRB2(ref x, ref y, ref list);

         }
      }

      static void Factorial(int x, ref int acc) {
         if (x == 0) return;

         Factorial(x - 1, ref acc);
         acc *= x;
      }

      static unsafe void IterativeForm(int x, ref int acc) {
         Stack<(int x, int acc, int returnAddress)> stack = new Stack<(int, int, int)>();
         stack.Push((x, acc, 4));

         for (; ; ) {
            if (stack.Count == 0) break;
            var stackFrame = stack.Pop();

         fnrb1:
            if (FNRB1(ref stackFrame.x, ref stackFrame.acc)) goto next;

            //Factorial(x - 1, ref acc);
            stack.Push((stackFrame.x - 1, stackFrame.acc, 2));
            continue;

         fnrb2:
            FNRB2(ref stackFrame.x, ref stackFrame.acc);

         next:
            switch (stackFrame.returnAddress) {
               case 1: {
                  goto fnrb1;
               }
               case 2: {
                  goto fnrb2;
               }
               case 3: {
                  goto next;
               }
               case 4: {
                  goto gg;
               }
            }
         }

      gg:
         ;
      }

      static bool FNRB1(ref int x, ref int acc) {
         if (x == 0) return true;
         return false;
      }

      static void FNRB2(ref int x, ref int acc) {
         acc *= x;
      }

      static bool NRB1(ref int x, ref int y, ref List<string> list) {
         // Termination
         {
            if (x <= 0) return true;
            if (y <= 0) return true;
         }

         // Step
         list.Clear();
         x++;

         return false;
      }

      static void NRB2(ref int x, ref int y, ref List<string> list) {
         list.Add("x = " + x.ToString());
      }

      static bool WalkIterative(string[] map, int y, int x, Pipe root, Direction firstDirection) {
         Pipe currentPipe;
         Pipe previousPipe = root;
         Direction lastMove = firstDirection;
         root.y = y;
         root.x = x;

         for (; ; ) {
            if (y < 0 || y >= map.Length || x < 0 || x >= map[y].Length) {
               return false;
            }

            if (map[y][x] == '.') {
               return false;
            }

            if (map[y][x] == 'S') {
               previousPipe.outgoing = root;
               root.incoming = previousPipe;
               return true;
            }

            currentPipe = new Pipe();
            currentPipe.directions = GetDirections(map[y][x]);
            previousPipe.outgoing = currentPipe;
            currentPipe.incoming = previousPipe;
            currentPipe.y = y;
            currentPipe.x = x;

            Direction nextMove = currentPipe.directions.First(d => d != GetOppositeDirection(lastMove));
            CalculateNextPosition(ref y, ref x, nextMove);

            currentPipe.outgoingMove = nextMove;

            previousPipe = currentPipe;
            lastMove = nextMove;
         }
      }

      static Direction FindFirstDirection(Direction[] directions) {
         throw new NotImplementedException(); // Arguments are not enough. Probably need to traverse the loop once to determine.
      }

      static Direction[] GetDirections(char c) {
         Direction[] directions = new Direction[2];
         switch (c) {
            case '|': {
               directions[0] = Direction.North;
               directions[1] = Direction.South;
               break;
            }
            case '-': {
               directions[0] = Direction.East;
               directions[1] = Direction.West;
               break;
            }
            case 'L': {
               directions[0] = Direction.North;
               directions[1] = Direction.East;
               break;
            }
            case 'J': {
               directions[0] = Direction.North;
               directions[1] = Direction.West;
               break;
            }
            case '7': {
               directions[0] = Direction.South;
               directions[1] = Direction.West;
               break;
            }
            case 'F': {
               directions[0] = Direction.South;
               directions[1] = Direction.East;
               break;
            }
         }

         return directions;
      }

      static Direction GetOppositeDirection(Direction direction) {
         switch (direction) {
            case Direction.North: return Direction.South;
            case Direction.East: return Direction.West;
            case Direction.South: return Direction.North;
            case Direction.West: return Direction.East;
            default: throw new Exception("Unknown direction");
         }
      }

      static void CalculateNextPosition(ref int y, ref int x, Direction direction) {
         switch (direction) {
            case Direction.North: y--; break;
            case Direction.East: x++; break;
            case Direction.South: y++; break;
            case Direction.West: x--; break;
            default: throw new Exception("Unknown direction");
         }
      }
   }

   enum Direction {
      North,
      East,
      South,
      West
   }

   class Pipe {
      public Pipe incoming;
      public Pipe outgoing;
      public Direction[] directions;
      public int y;
      public int x;
      public Direction outgoingMove;
   }

   class AreaInfo {
      public int index;
      public List<int> restrictions;
   }

   class WalkArguments {
      public string[] map;
      public int y;
      public int x;
      public Pipe whereIComeFrom;
      public Direction lastMove;
      public bool success;

      public WalkArguments(string[] map, int y, int x, Pipe whereIComeFrom, Direction lastMove, bool success) {
         this.map = map;
         this.y = y;
         this.x = x;
         this.whereIComeFrom = whereIComeFrom;
         this.lastMove = lastMove;
         this.success = success;
      }
   }
}
