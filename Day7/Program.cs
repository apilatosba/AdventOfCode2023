using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Transactions;

namespace Day7 {
   internal class Program {
      static void Main(string[] args) {
         Part2();
      }

      static void Part1() {
         string[] input = File.ReadAllLines("input.txt");
         List<Hand> hands = new List<Hand>(input.Length);

         //List<Hand> fiveOfAKind = new List<Hand>();
         //List<Hand> fourOfAKind = new List<Hand>();
         //List<Hand> fullHouse = new List<Hand>();
         //List<Hand> threeOfAKind = new List<Hand>();
         //List<Hand> twoPairs = new List<Hand>();
         //List<Hand> onePair = new List<Hand>();
         //List<Hand> highCard = new List<Hand>();

         {
            Regex regex = new Regex(@"([\w\d]{5}) +(\d+)");

            foreach (string line in input) {
               Match match = regex.Match(line);

               Hand hand = new Hand(match.Groups[1].Value, ulong.Parse(match.Groups[2].Value));
               hand.type = FindHandType(hand.cards);
               hands.Add(hand);
            }
         }

         //foreach (Hand hand in hands) {
         //   HandType handType = FindHandType(hand.cards);
         //   //hand.type = handType;

         //   switch (handType) {
         //      case HandType.FiveOfAKind:
         //         //fiveOfAKind.Add(hand);
         //         break;
         //      case HandType.FourOfAKind:
         //         //fourOfAKind.Add(hand);
         //         break;
         //      case HandType.FullHouse:
         //         //fullHouse.Add(hand);
         //         break;
         //      case HandType.ThreeOfAKind:
         //         //threeOfAKind.Add(hand);
         //         break;
         //      case HandType.TwoPairs:
         //         //twoPairs.Add(hand);
         //         break;
         //      case HandType.OnePair:
         //         //onePair.Add(hand);
         //         break;
         //      case HandType.HighCard:
         //         //highCard.Add(hand);
         //         break;
         //   }
         //}

         Chunk root = new Chunk(hands, null);

         root.childrenChunks = new List<Chunk>(Enum.GetNames<HandType>().Length);

         for (int i = 0; i < Enum.GetNames<HandType>().Length; i++) {
            root.childrenChunks.Add(new Chunk(new List<Hand>(), root));
         }

         foreach (Hand hand in hands) {
            int index = GetHandTypeValue(hand.type);
            root.childrenChunks[index].hands.Add(hand);
         }

         foreach (Chunk chunk in root.childrenChunks) {
            int depth = 0;
            Chunk currentChunk = chunk;

            WalkTree(currentChunk, depth);

            //while (currentChunk.hands.Count > 1) {
            //   List<Chunk> childrenChunks = WalkTree(currentChunk, depth);
            //   for (int i = 0; i < childrenChunks.Count; i++) {
            //      Chunk child = childrenChunks[i];
            //      if (child.hands.Count == 1) {
            //         currentChunk = child;
            //         break;
            //      }
            //   }

            //   depth++;
            //}
            
            //{
            //   currentChunk.childrenChunks = new List<Chunk>(13);
            //   for (int i = 0; i < 13; i++) {
            //      currentChunk.childrenChunks.Add(new Chunk(new List<Hand>(), currentChunk));
            //   }

            //   foreach (Hand hand in currentChunk.hands) {
            //      int index = GetCardValue(hand.cards[depth].ToString());
            //      currentChunk.childrenChunks[index].hands.Add(hand);
            //   }

            //   foreach (Chunk child in currentChunk.childrenChunks) {
            //      if (child.hands.Count == 0) {
            //         currentChunk.childrenChunks.Remove(child);
            //      }
            //   }

            //   depth++;
            //}
         }

         List<Hand> output = new List<Hand>();
         InOrderTraverse(root, output);

         ulong result = 0;
         for (int i = 0; i < output.Count; i++) {
            int rank = i + 1;
            result += output[i].bid * (ulong)rank;
         }

         Console.WriteLine(result);
      }

      static void Part2() {
         string[] input = File.ReadAllLines("input.txt");
         List<Hand> hands = new List<Hand>(input.Length);

         {
            Regex regex = new Regex(@"([\w\d]{5}) +(\d+)");

            foreach (string line in input) {
               Match match = regex.Match(line);

               Hand hand = new Hand(match.Groups[1].Value, ulong.Parse(match.Groups[2].Value));
               hand.type = FindHandType(hand.cards);
               hand.type = GetHandTypeAfterJokerIsUsed(hand);
               hands.Add(hand);
            }
         }

         Chunk root = new Chunk(hands, null);

         root.childrenChunks = new List<Chunk>(Enum.GetNames<HandType>().Length);

         for (int i = 0; i < Enum.GetNames<HandType>().Length; i++) {
            root.childrenChunks.Add(new Chunk(new List<Hand>(), root));
         }

         foreach (Hand hand in hands) {
            int index = GetHandTypeValue(hand.type);
            root.childrenChunks[index].hands.Add(hand);
         }

         foreach (Chunk chunk in root.childrenChunks) {
            int depth = 0;
            Chunk currentChunk = chunk;

            WalkTree(currentChunk, depth);
         }

         List<Hand> output = new List<Hand>();
         InOrderTraverse(root, output);

         foreach (Hand hand in output) {
            Console.WriteLine(hand);
         }

         ulong result = 0;
         for (int i = 0; i < output.Count; i++) {
            int rank = i + 1;
            result += output[i].bid * (ulong)rank;
         }

         Console.WriteLine(result);
      }

      //static bool IsFourOfAKind(string cards) {
      //   int count = 0;
      //   char c = cards[0];

      //   foreach (char card in cards) {
      //      if (card == c) {
      //         count++;
      //      }
      //   }

      //   if (count == 4) {
      //      return true;
      //   } else if (count == 1) {
      //      return Regex.IsMatch(cards, $@"{cards[1]}{{4}}");
      //   } else {
      //      return false;
      //   }
      //}

      static List<CardInfo> GetCardInfos(string cards) {
         List<CardInfo> cardInfos = new List<CardInfo>(5);

         for (int i = 0; i < cards.Length; i++) {
            string card = cards[i].ToString();

            if (cardInfos.Exists(c => c.card == card)) {
               CardInfo cardInfo = cardInfos.Find(c => c.card == card);
               cardInfo.count++;
               cardInfo.indices.Add(i);
            } else {
               cardInfos.Add(new CardInfo(card, 1, new List<int>() { i }));
            }
         }

         return cardInfos;
      }

      //static bool IsFullHouse(string cards) {
      //   List<CardInfo> cardInfos = GetCardInfos(cards);
      //   if (cardInfos.Count == 2) {
      //      return cardInfos.Exists(c => c.count == 3) && cardInfos.Exists(c => c.count == 2);
      //   } else {
      //      return false;
      //   }
      //}

      static HandType FindHandType(string cards) {
         List<CardInfo> cardInfos = GetCardInfos(cards);

         // Five of a kind
         if (Regex.IsMatch(cards, $@"{cards[0]}{{5}}")) {
            return HandType.FiveOfAKind;
         }

         // Four of a kind
         {
            int count = 0;
            char c = cards[0];

            foreach (char card in cards) {
               if (card == c) {
                  count++;
               }
            }

            if (count == 4) {
               return HandType.FourOfAKind;
            } else if (count == 1) {
               bool match = Regex.IsMatch(cards, $@"{cards[1]}{{4}}");
               if (match) return HandType.FourOfAKind;
            }
         }

         // Full house
         if (cardInfos.Count == 2) {
            if (cardInfos.Exists(c => c.count == 3) && cardInfos.Exists(c => c.count == 2)) {
               return HandType.FullHouse;
            }
         }

         // Three of a kind
         if (cardInfos.Exists(c => c.count == 3) && cardInfos.Exists(c => c.count == 1)) {
            return HandType.ThreeOfAKind;
         }

         // Two pairs
         if (cardInfos.Count == 3 &&
               cardInfos.Exists(c => c.count == 2) &&
               cardInfos.Exists(c => c.count == 1)) {
            return HandType.TwoPairs;
         }

         // One pair
         if (cardInfos.Count == 4)
            return HandType.OnePair;

         // High card
         if (cardInfos.Count == 5)
            return HandType.HighCard;

         throw new Exception("No hand type found");
      }

      static int GetHandTypeValue(HandType handType) {
         switch (handType) {
            case HandType.FiveOfAKind:
               return 6;
            case HandType.FourOfAKind:
               return 5;
            case HandType.FullHouse:
               return 4;
            case HandType.ThreeOfAKind:
               return 3;
            case HandType.TwoPairs:
               return 2;
            case HandType.OnePair:
               return 1;
            case HandType.HighCard:
               return 0;
            default:
               throw new Exception("No hand type found");
         }
      }

      static int GetCardValue(string card) {
         switch (card) {
            case "A":
               return 12;
            case "K":
               return 11;
            case "Q":
               return 10;
            case "J":
               return 9;
            case "T":
               return 8;
            default:
               return int.Parse(card) - 2;
         }
      }

      static int GetCardValue2(string card) {
         switch (card) {
            case "A":
               return 12;
            case "K":
               return 11;
            case "Q":
               return 10;
            case "J":
               return 0;
            case "T":
               return 9;
            default:
               return int.Parse(card) - 1;
         }
      }

      //static string CompareCards(string c1, string c2) {
      //   int v1 = GetCardValue(c1);
      //   int v2 = GetCardValue(c2);

      //   return v1 > v2 ? c1 : c2;
      //}

      static void WalkTree(Chunk currentChunk, int depth) {
         if (currentChunk.hands.Count == 1) {
            return;
         }

         currentChunk.childrenChunks = new List<Chunk>(13);
         for (int i = 0; i < 13; i++) {
            currentChunk.childrenChunks.Add(new Chunk(new List<Hand>(), currentChunk));
         }

         foreach (Hand hand in currentChunk.hands) {
            int index = GetCardValue2(hand.cards[depth].ToString());
            currentChunk.childrenChunks[index].hands.Add(hand);
         }

         //foreach (Chunk child in currentChunk.childrenChunks) {
         //   if (child.hands.Count == 0) {
         //      currentChunk.childrenChunks.Remove(child);
         //   }
         //}

         for (int i = 0; i < currentChunk.childrenChunks.Count; i++) {
            Chunk child = currentChunk.childrenChunks[i];
            if(child.hands.Count == 0) {
               currentChunk.childrenChunks.Remove(child);
               i--;
            }
         }

         for (int i = 0; i < currentChunk.childrenChunks.Count; i++) {
            Chunk child = currentChunk.childrenChunks[i];
            //int d = 0;
            //{
            //   Chunk cc = currentChunk;
            //   while (cc.parentChunk != null) {
            //      d++;
            //      cc = cc.parentChunk;
            //   }
            //}
            //WalkTree(child, d);
            //WalkTree(child, ++depth); // ++depth is not working.
            WalkTree(child, depth + 1); // depth + 1 is working. but ++depth is not. UPDATE: oh. you fucking dummy. it is in for loop. stupid dumb fuck. looking for hours just to notice this. omaygat i cant even.
         }
      }

      static void InOrderTraverse(Chunk chunk, List<Hand> output) {
         //if (chunk == null) {
         //   //Console.WriteLine(chunk.hands[0]);
         //   return;
         //}

         //if (chunk.childrenChunks.Count == 0) {
         //   //Console.WriteLine(chunk.hands[0]);
         //   return;
         //}

         if (chunk.childrenChunks == null) {
            //Console.WriteLine(chunk.hands[0]);
            output.Add(chunk.hands[0]);
            return;
         }

         for (int i = 0; i < chunk.childrenChunks.Count; i++) {
            InOrderTraverse(chunk.childrenChunks[i], output);
         }
      }

      //static string GetCardsAfterJokerIsUsed(string cards) {
      //   List<CardInfo> cardsInfos = GetCardInfos(cards);

      //   if (!cardsInfos.Exists(c => c.card == "J")) {
      //      return cards;
      //   } else {
      //      // Five of a kind
      //      if (cardsInfos.Count == 2)
      //   }
      //}

      static HandType GetHandTypeAfterJokerIsUsed(Hand hand) {
         List<CardInfo> cardsInfos = GetCardInfos(hand.cards);

         if (!cardsInfos.Exists(c => c.card == "J")) {
            return hand.type;
         } else {
            int jokerCount = cardsInfos.Find(c => c.card == "J").count;

            // Five of a kind
            if (cardsInfos.Count == 2 || cardsInfos.Count == 1)
               return HandType.FiveOfAKind;

            // Four of a kind
            if (cardsInfos.Count == 3) {
               if (cardsInfos.FindAll(c => c.card != "J").Exists(c => c.count == 1))
                  return HandType.FourOfAKind;
               else
                  // Full house
                  return HandType.FullHouse;
            }

            // Three of a kind
            if (cardsInfos.Count == 4) {
               return HandType.ThreeOfAKind;
            }

            // Two pairs
            // You don't do it. You would do three of a kind instead.

            // One pair
            if (cardsInfos.Count == 5) {
               return HandType.OnePair;
            }

            // High card
            // You don't do it. You would do one pair instead.

            throw new Exception("Couldn't find value after joker");
         }
      }
   }

   class Hand {
      public string cards;
      public ulong bid;
      public HandType type;
      //public string cardsAfterJokerIsUsed;

      public Hand(string cards, ulong bid) {
         this.cards = cards;
         this.bid = bid;
      }

      public override string ToString() {
         return $"Cards: {cards}, Bid: {bid}, Type: {type}";
      }
   }

   class CardInfo {
      public string card;
      public int count;
      public List<int> indices;

      public CardInfo(string card, int count, List<int> indices) {
         this.card = card;
         this.count = count;
         this.indices = indices;
      }
   }

   enum HandType {
      FiveOfAKind,
      FourOfAKind,
      FullHouse,
      ThreeOfAKind,
      TwoPairs,
      OnePair,
      HighCard
   }

   /// <summary>
   /// Right ones are higher rank, left ones are lower rank
   /// </summary>
   class Chunk {
      public List<Hand> hands;
      public List<Chunk> childrenChunks;
      public Chunk parentChunk;

      public Chunk(List<Hand> hands, Chunk parentChunk) {
         this.hands = hands;
         this.parentChunk = parentChunk;
      }
   }
}
