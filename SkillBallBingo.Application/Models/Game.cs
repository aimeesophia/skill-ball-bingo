using SkillBallBingo.Application.Enums;
using SkillBallBingo.Console;

namespace SkillBallBingo.Application.Models;

public class Game
{
    public Cell?[][] Ticket { get; }
    
    public int CurrentNumber { get; private set; }
    
    public bool IsOver { get; private set; }
    
    public CountdownTimer Timer { get; }
    
    public Result? Result { get; private set; }

    public List<int> CalledNumbers { get; }
    
    private Random Random { get; }

    private List<int> AvailableNumbers { get; }
    
    public Game()
    {
        Random = new Random();
        AvailableNumbers = Enumerable.Range(1, 90).ToList();
        CalledNumbers = [];
        Ticket = GenerateTicket();
        CurrentNumber = GetRandomNumber();
        Timer = new CountdownTimer();
        Timer.Start();
        Timer.OnTimeUp += HandleTimeUp;
    }

    private void HandleTimeUp()
    {
        IsOver = true;
        Result = Enums.Result.Lose;
    }

    public void AcceptCurrentNumber()
    {
        if (NumberExistsInTicket(CurrentNumber))
        {
            var foundCell = Ticket
                .SelectMany(cell => cell)
                .Single(cell => cell?.Number == CurrentNumber);
            
            foundCell!.IsMarked = true;
            
            Timer.AddTime();
            CheckForBingo();
        }
        else
        {
            Timer.RemoveTime();
        }
        
        CalledNumbers.Add(CurrentNumber);
        CurrentNumber = GetRandomNumber();
    }

    public void RejectCurrentNumber()
    {
        if (NumberExistsInTicket(CurrentNumber))
        {
            Timer.RemoveTime();
        }
        
        CalledNumbers.Add(CurrentNumber);
        CurrentNumber = GetRandomNumber();
    }

    private Cell[][] GenerateTicket()
    {
        Cell[][] ticket = new Cell[3][];
            for (int i = 0; i < 3; i++)
            {
                ticket[i] = new Cell[9];
            }
        
            // Fill each row with 5 random numbers
            Random rand = new Random();
            for (int row = 0; row < 3; row++)
            {
                int count = 0;
                while (count < 5)
                {
                    int col = rand.Next(0, 9);
                    if (ticket[row][col] == null)
                    {
                        ticket[row][col] = new Cell(0);  // placeholder for a number
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
                    if (ticket[row][col] != null)
                    {
                        int randomIndex = rand.Next(0, availableNumbers.Count);
                        ticket[row][col] = new Cell(availableNumbers[randomIndex]);
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
                    if (ticket[row][col] != null)
                    {
                        columnNumbers.Add(ticket[row][col].Number);
                    }
                }
                columnNumbers.Sort();
        
                for (int row = 0, i = 0; row < 3; row++)
                {
                    if (ticket[row][col] != null)
                    {
                        ticket[row][col] = new Cell(columnNumbers[i]);
                        i++;
                    }
                }
            }
        
            return ticket;
    }
    
    private int GetRandomNumber()
    {
        if (AvailableNumbers.Count == 0)
        {
            IsOver = true;
            Result = Enums.Result.Lose;
            return 0;
        }
        
        var randomIndex = Random.Next(0, AvailableNumbers.Count);
        var selectedNumber = AvailableNumbers[randomIndex];
        AvailableNumbers.Remove(selectedNumber);
        
        return selectedNumber;
    }
    
    private bool NumberExistsInTicket(int number)
    {
        return Ticket.SelectMany(row => row)
            .Where(cell => cell != null)
            .Any(cell => cell!.Number == number);
    }

    private void CheckForBingo()
    {
        foreach (var row in Ticket)
        {
            var numberedCells = row.Where(cell => cell != null);
            if (numberedCells.All(cell => cell!.IsMarked))
            {
                IsOver = true;
                Result = Enums.Result.Win;
            }
        }
    }
}