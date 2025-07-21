namespace SkillBallBingo.Application;

public class TicketGenerator
{
    private const int Rows = 3;
    private const int Cols = 9;
    private const int NumbersPerRow = 5;
    private const int TotalNumbers = Rows * NumbersPerRow; // 15

    // Represents the generated ticket grid. Null means blank.
    private int?[,] _ticket = new int?[Rows, Cols];
    // Tracks which cells are designated to hold a number before the number is generated
    private bool[,] _cellIsAllocated = new bool[Rows, Cols];
    // Tracks numbers already used on this ticket to ensure uniqueness
    private readonly HashSet<int> _usedNumbers = new HashSet<int>();

    /// <summary>
    /// Generates a single UK 90-ball bingo ticket.
    /// </summary>
    /// <returns>A 3x9 grid representing the bingo ticket. Null values indicate blank cells.</returns>
    public int?[,] GenerateTicket()
    {
        // Reset state for a new ticket
        _ticket = new int?[Rows, Cols];
        _cellIsAllocated = new bool[Rows, Cols];
        _usedNumbers.Clear();

        AllocateNumberPositions();
        FillAllocatedPositionsWithNumbers();

        return _ticket;
    }

    /// <summary>
    /// Step 1: Determine which cells will contain numbers, ensuring row/column constraints.
    /// This implementation uses the common structure: 3 columns with 1 number, 6 columns with 2 numbers.
    /// </summary>
    private void AllocateNumberPositions()
    {
        // 1. Determine which columns get 1 number and which get 2
        var columnIndices = Enumerable.Range(0, Cols).ToList();
        Shuffle(columnIndices); // Randomize column order

        var singleNumberCols = columnIndices.Take(3).ToList();
        var doubleNumberCols = columnIndices.Skip(3).Take(6).ToList();

        // 2. Allocate positions for single-number columns
        // To ensure each row gets 5 numbers eventually, the 3 single numbers
        // must be placed one in each row.
        var rowsForSingleCols = Enumerable.Range(0, Rows).ToList();
        Shuffle(rowsForSingleCols);
        for (int i = 0; i < singleNumberCols.Count; i++)
        {
            int col = singleNumberCols[i];
            int row = rowsForSingleCols[i];
            _cellIsAllocated[row, col] = true;
        }

        // 3. Allocate positions for double-number columns
        // We need to place 6 pairs of numbers. To ensure each row ends up with 5 numbers total
        // (it currently has 1 from the single-number columns), each row needs 4 more numbers.
        // This requires exactly two pairs in rows (0,1), two in (0,2), and two in (1,2).
        var rowPairs = new List<(int, int)>
        {
            (0, 1), (0, 1),
            (0, 2), (0, 2),
            (1, 2), (1, 2)
        };
        Shuffle(rowPairs);

        for (int i = 0; i < doubleNumberCols.Count; i++)
        {
            int col = doubleNumberCols[i];
            (int row1, int row2) = rowPairs[i];
            _cellIsAllocated[row1, col] = true;
            _cellIsAllocated[row2, col] = true;
        }
    }

    /// <summary>
    /// Step 2: Fill the allocated cells with valid, unique random numbers,
    /// respecting column ranges and sorting within columns.
    /// </summary>
    private void FillAllocatedPositionsWithNumbers()
    {
        for (int c = 0; c < Cols; c++)
        {
            List<int> numbersInColumn = new List<int>();
            List<int> rowsToFillInColumn = new List<int>();

            // Find which rows need a number in this column
            for (int r = 0; r < Rows; r++)
            {
                if (_cellIsAllocated[r, c])
                {
                    rowsToFillInColumn.Add(r);
                }
            }

            if (rowsToFillInColumn.Count == 0) continue; // Should not happen with the allocation logic

            // Determine the valid number range for this column
            (int min, int max) = GetColumnRange(c);

            // Generate the required number of unique random numbers within the range
            while (numbersInColumn.Count < rowsToFillInColumn.Count)
            {
                int potentialNumber = Random.Shared.Next(min, max + 1);
                // Ensure uniqueness across the entire ticket
                if (!_usedNumbers.Contains(potentialNumber))
                {
                    numbersInColumn.Add(potentialNumber);
                    _usedNumbers.Add(potentialNumber);
                }
                // Very unlikely, but prevents infinite loops if range is small and numbers taken
                if (_usedNumbers.Count >= 90) break;
            }

            // Sort numbers ascendingly for placement within the column
            numbersInColumn.Sort();

            // Place the sorted numbers into the allocated cells for this column
            for (int i = 0; i < rowsToFillInColumn.Count; i++)
            {
                int row = rowsToFillInColumn[i]; // Rows are already implicitly sorted 0, 1, 2
                _ticket[row, c] = numbersInColumn[i];
            }
        }
    }

    /// <summary>
    /// Gets the valid number range (inclusive) for a given column index.
    /// </summary>
    private (int Min, int Max) GetColumnRange(int colIndex)
    {
        if (colIndex < 0 || colIndex >= Cols)
            throw new ArgumentOutOfRangeException(nameof(colIndex));

        if (colIndex == 0) return (1, 9);       // Col 0: 1-9
        if (colIndex == Cols - 1) return (80, 90); // Col 8: 80-90 (Last column special)

        // Other columns: 10-19, 20-29, ..., 70-79
        int min = colIndex * 10;
        int max = min + 9;
        return (min, max);
    }

    /// <summary>
    /// Shuffles a list in place using the Fisher-Yates algorithm.
    /// </summary>
    private static void Shuffle<T>(IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Shared.Next(n + 1);
            (list[k], list[n]) = (list[n], list[k]); // Tuple swap
        }
    }
}