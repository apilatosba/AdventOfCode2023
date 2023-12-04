using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Day4 {
   internal class Program {
      static void Main(string[] args) {
         Part2();
      }

      static void Part1() {
         string[] input = File.ReadAllLines("input.txt");
         List<Card> cards = new List<Card>();

         {
            Regex regex = new Regex(@"Card +(\d+):([ \d]*)\|([ \d]*)");
            for (int i = 0; i < input.Length; i++) {
               Match match = regex.Match(input[i]);

               uint cardNumber = uint.Parse(match.Groups[1].Value);
               List<int> winningNumbers = new List<int>();
               List<int> numbersYouHave = new List<int>();

               foreach (string x in match.Groups[2].Value.Split(' ').Where(x => x != "").Select(x => x.Trim())) {
                  winningNumbers.Add(int.Parse(x));
               }

               foreach (string x in match.Groups[3].Value.Split(' ').Where(x => x != "").Select(x => x.Trim())) {
                  numbersYouHave.Add(int.Parse(x));
               }

               cards.Add(new Card(cardNumber, winningNumbers, numbersYouHave));
            }
         }

         foreach (Card card in cards) {
            int worth = 0;
            foreach (int number in card.numbersYouHave) {
               if (card.winningNumbers.Contains(number)) {
                  worth++;
               }
            }
            card.worth = worth == 0 ? 0 : (int)Math.Pow(2, worth - 1);
         }

         ulong total = 0;
         foreach (Card card in cards) {
            total += (ulong)card.worth;
         }

         Console.WriteLine(total);
      }

      static void Part2() {
         string[] input = File.ReadAllLines("input.txt");
         List<Card> cards = new List<Card>();
         //List<Card> processedCards = new List<Card>();
         List<Card> unprocessedCards;

         {
            Regex regex = new Regex(@"Card +(\d+):([ \d]*)\|([ \d]*)");
            for (int i = 0; i < input.Length; i++) {
               Match match = regex.Match(input[i]);

               uint cardNumber = uint.Parse(match.Groups[1].Value);
               List<int> winningNumbers = new List<int>();
               List<int> numbersYouHave = new List<int>();

               foreach (string x in match.Groups[2].Value.Split(' ').Where(x => x != "").Select(x => x.Trim())) {
                  winningNumbers.Add(int.Parse(x));
               }

               foreach (string x in match.Groups[3].Value.Split(' ').Where(x => x != "").Select(x => x.Trim())) {
                  numbersYouHave.Add(int.Parse(x));
               }

               cards.Add(new Card(cardNumber, winningNumbers, numbersYouHave));
            }
         }

         foreach (Card card in cards) {
            int numberOfWinningNumbers = 0;
            foreach (int number in card.numbersYouHave) {
               if (card.winningNumbers.Contains(number)) {
                  numberOfWinningNumbers++;
               }
            }

            card.numberOfWinningNumbers = numberOfWinningNumbers;
         }

         //unprocessedCards = new List<Card>(cards);

         //ulong total = 0;
         //for (; unprocessedCards.Count > 0;) {
         //   Card card = unprocessedCards[0];

         //   for (int i = 0; i < card.numberOfWinningNumbers; i++) {
         //      unprocessedCards.Add(cards[(int)card.cardNumber + i].Copy());
         //   }

         //   unprocessedCards.Remove(card);
         //   //processedCards.Add(card);
         //   //total++;
         //}

         //Console.WriteLine(total);

         for (int i = cards.Count - 1; i >= 0; i--) {
            cards[i].totalCardScratchWorth = 1;

            for (int j = 0; j < cards[i].numberOfWinningNumbers; j++) {
               cards[i].totalCardScratchWorth += cards[(int)cards[i].cardNumber + j].totalCardScratchWorth;
            }
         }

         ulong total = 0;
         for (int i = 0; i < cards.Count; i++) {
            total += (ulong)cards[i].totalCardScratchWorth;
         }

         Console.WriteLine(total);
      }
   }

   public class Card {
      public uint cardNumber;
      public List<int> winningNumbers;
      public List<int> numbersYouHave;
      public int worth;
      public int numberOfWinningNumbers;
      public int totalCardScratchWorth;

      public Card(uint cardNumber, List<int> winningNumbers, List<int> numbersYouHave) {
         this.cardNumber = cardNumber;
         this.winningNumbers = winningNumbers;
         this.numbersYouHave = numbersYouHave;
      }

      public Card Copy() {
         return (Card)MemberwiseClone();
      }
   }
}
