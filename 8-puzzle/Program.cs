using System;
using System.Collections.Generic;

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

public class PuzzleSolver
{
    public static void Solve(int[,] initialPuzzle)
    {
        PriorityQueue<PuzzleState, int> openList = new PriorityQueue<PuzzleState, int>();
        HashSet<string> closedList = new HashSet<string>();

        int emptyRow = 0, emptyCol = 0;
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (initialPuzzle[i, j] == 0)
                {
                    emptyRow = i;
                    emptyCol = j;
                }
            }
        }

        PuzzleState startState = new PuzzleState(initialPuzzle, emptyRow, emptyCol, 0, null);
        openList.Enqueue(startState, startState.Cost);

        while (openList.Count > 0)
        {
            PuzzleState current = openList.Dequeue();
            if (current.IsGoal())
            {
                PrintSolution(current);
                return;
            }

            string puzzleString = PuzzleToString(current.Puzzle);
            closedList.Add(puzzleString);

            foreach (PuzzleState successor in current.GenerateSuccessors())
            {
                if (!closedList.Contains(PuzzleToString(successor.Puzzle)))
                {
                    openList.Enqueue(successor, successor.Cost);
                }
            }
        }

        Console.WriteLine("Solução não encontrada.");
    }

    public static string PuzzleToString(int[,] puzzle)
    {
        string result = "";
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                result += puzzle[i, j] + ",";
            }
        }
        return result;
    }

    private static void PrintSolution(PuzzleState state)
    {
        Stack<PuzzleState> path = new Stack<PuzzleState>();
        while (state != null)
        {
            path.Push(state);
            state = state.Parent;
        }

        while (path.Count > 0)
        {
            PuzzleState step = path.Pop();
            step.PrintPuzzle();
        }
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        int[,] initialPuzzle = GenerateSolvableArray();
        PuzzleSolver.Solve(initialPuzzle);
    }

    private static int[,] GenerateSolvableArray()
    {
        List<int> numbers = new List<int>();
        for (int i = 0; i < 9; i++) numbers.Add(i);
        Random rng = new Random();
        
        do
        {
            for (int i = numbers.Count - 1; i > 0; i--)
            {
                int j = rng.Next(i + 1);
                int temp = numbers[i];
                numbers[i] = numbers[j];
                numbers[j] = temp;
            }
        } while (!IsSolvable(numbers.ToArray()));

        int[,] puzzle = new int[3, 3];
        for (int i = 0; i < 9; i++)
        {
            puzzle[i / 3, i % 3] = numbers[i];
        }
        return puzzle;
    }

    private static bool IsSolvable(int[] puzzle)
    {
        int inversions = 0;
        for (int i = 0; i < puzzle.Length; i++)
        {
            for (int j = i + 1; j < puzzle.Length; j++)
            {
                if (puzzle[i] > puzzle[j] && puzzle[i] != 0 && puzzle[j] != 0)
                    inversions++;
            }
        }
        return inversions % 2 == 0;
    }
}
