namespace SkillBallBingo.Application.Models;

public class Cell
{
    public int Number { get; init; }

    public bool IsMarked { get; set; }

    public Cell(int number)
    {
        Number = number;
        IsMarked = false;
    }
}