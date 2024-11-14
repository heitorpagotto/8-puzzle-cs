using Application.Dtos;

namespace Application.Services;

public class PuzzleSolverService : IPuzzleSolverService
{
    public List<List<int>> Solve()
    {
        var initialPuzzle = GenerateSolvableArray();

        var returnList = new List<List<int>>();
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

        PuzzleState goalState = null;
        while (openList.Count > 0)
        {
            PuzzleState current = openList.Dequeue();
            string puzzleString = PuzzleToString(current.Puzzle);

            if (current.IsGoal())
            {
                goalState = current;
                PrintSolution(current);
                //returnList.Add(ConvertStringToList(puzzleString));
                //break;
            }

            closedList.Add(puzzleString);

            foreach (PuzzleState successor in current.GenerateSuccessors())
            {
                if (!closedList.Contains(PuzzleToString(successor.Puzzle)))
                {
                    openList.Enqueue(successor, successor.Cost);
                }
            }
        }

        while (goalState != null)
        {
            returnList.Add(ConvertStringToList(PuzzleToString(goalState.Puzzle)));
            goalState = goalState.Parent;
        }

        returnList.Reverse();

        return returnList;
    }

    private List<int> ConvertStringToList(string puzzleString)
    {
        var numberArray = puzzleString.Split(",").Where(x => !string.IsNullOrEmpty(x));

        return numberArray.Select(x => Convert.ToInt32(x)).ToList();
    }
    
    private void PrintSolution(PuzzleState state)
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

    private int[,] GenerateSolvableArray()
    {
        List<int> numbers = new List<int>();
        for (int i = 0; i < 9; i++) numbers.Add(i);
        Random rng = new Random();

        do
        {
            for (int i = numbers.Count - 1; i > 0; i--)
            {
                int j = rng.Next(i + 1);
                (numbers[i], numbers[j]) = (numbers[j], numbers[i]);
            }
        } while (!IsSolvable(numbers.ToArray()));

        int[,] puzzle = new int[3, 3];
        for (int i = 0; i < 9; i++)
        {
            puzzle[i / 3, i % 3] = numbers[i];
        }

        return puzzle;
    }

    private bool IsSolvable(int[] puzzle)
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

    private string PuzzleToString(int[,] puzzle)
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
}