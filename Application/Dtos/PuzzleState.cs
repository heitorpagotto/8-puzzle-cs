namespace Application.Dtos;

public class PuzzleState : IComparable<PuzzleState>
{
    public int[,] Puzzle { get; set; }
    public int EmptyRow { get; set; }
    public int EmptyCol { get; set; }
    public int Cost { get; set; }
    public int Depth { get; set; }
    public PuzzleState Parent { get; set; }

    public PuzzleState(int[,] puzzle, int emptyRow, int emptyCol, int depth, PuzzleState parent)
    {
        Puzzle = (int[,])puzzle.Clone();
        EmptyRow = emptyRow;
        EmptyCol = emptyCol;
        Depth = depth;
        Parent = parent;
        Cost = depth + GetManhattanDistance();
    }

    private int GetManhattanDistance()
    {
        int distance = 0;
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                int value = Puzzle[i, j];
                if (value != 0)
                {
                    int targetRow = (value - 1) / 3;
                    int targetCol = (value - 1) % 3;
                    distance += Math.Abs(i - targetRow) + Math.Abs(j - targetCol);
                }
            }
        }
        return distance;
    }

    public int CompareTo(PuzzleState other)
    {
        return Cost.CompareTo(other.Cost);
    }

    public List<PuzzleState> GenerateSuccessors()
    {
        List<PuzzleState> successors = new List<PuzzleState>();
        int[,] directions = { { 1, 0 }, { -1, 0 }, { 0, 1 }, { 0, -1 } };

        for (int i = 0; i < directions.GetLength(0); i++)
        {
            int newRow = EmptyRow + directions[i, 0];
            int newCol = EmptyCol + directions[i, 1];

            if (newRow >= 0 && newRow < 3 && newCol >= 0 && newCol < 3)
            {
                int[,] newPuzzle = (int[,])Puzzle.Clone();
                newPuzzle[EmptyRow, EmptyCol] = newPuzzle[newRow, newCol];
                newPuzzle[newRow, newCol] = 0;
                successors.Add(new PuzzleState(newPuzzle, newRow, newCol, Depth + 1, this));
            }
        }
        return successors;
    }

    public bool IsGoal()
    {
        int[,] goal = { { 0, 1, 2 }, { 3, 4, 5 }, { 6, 7, 8 } };
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (Puzzle[i, j] != goal[i, j])
                    return false;
            }
        }
        return true;
    }
    
    public void PrintPuzzle()
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                Console.Write(Puzzle[i, j] + " ");
            }
            Console.WriteLine();
        }
        Console.WriteLine();
    }
}