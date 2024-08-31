using Microsoft.AspNetCore.Components;

namespace SkillBallBingo.Web.Components
{
    public class BingoCardBase : ComponentBase
    {
        protected int?[][]? Card { get; private set; }

        protected override void OnInitialized()
        {
            Card = GenerateBingoCard();
        }

        private static int?[][] GenerateBingoCard()
        {
            int?[][] card = new int?[3][];
            for (int i = 0; i < 3; i++)
            {
                card[i] = new int?[9];
            }

            // Fill each row with 5 random numbers
            Random rand = new Random();
            for (int row = 0; row < 3; row++)
            {
                int count = 0;
                while (count < 5)
                {
                    int col = rand.Next(0, 9);
                    if (card[row][col] == null)
                    {
                        card[row][col] = 0;  // placeholder for a number
                        count++;
                    }
                }
            }

            // Populate the numbers for each column
            for (int col = 0; col < 9; col++)
            {
                int min = col * 10 + 1;
                int max = col == 8 ? 90 : min + 9;
                List<int> availableNumbers = new List<int>();

                for (int n = min; n <= max; n++)
                {
                    availableNumbers.Add(n);
                }

                for (int row = 0; row < 3; row++)
                {
                    if (card[row][col] != null)
                    {
                        int randomIndex = rand.Next(0, availableNumbers.Count);
                        card[row][col] = availableNumbers[randomIndex];
                        availableNumbers.RemoveAt(randomIndex);
                    }
                }
            }

            // Sort the numbers in each column from top to bottom
            for (int col = 0; col < 9; col++)
            {
                List<int> columnNumbers = new List<int>();
                for (int row = 0; row < 3; row++)
                {
                    if (card[row][col] != null)
                    {
                        columnNumbers.Add(card[row][col].Value);
                    }
                }
                columnNumbers.Sort();

                for (int row = 0, i = 0; row < 3; row++)
                {
                    if (card[row][col] != null)
                    {
                        card[row][col] = columnNumbers[i];
                        i++;
                    }
                }
            }

            return card;
        }
    }
}
