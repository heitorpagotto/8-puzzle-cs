using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

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
        Cost = depth + GetManhattanDistance(); // A* usa profundidade + heurística
    }

    // Calcula a distância de Manhattan
    private int GetManhattanDistance()
    {
        int distance = 0;
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                int value = Puzzle[i, j];
                if (value != 0) // Ignora o espaço vazio
                {
                    int targetRow = (value - 1) / 3;
                    int targetCol = (value - 1) % 3;
                    distance += Math.Abs(i - targetRow) + Math.Abs(j - targetCol);
                }
            }
        }

        return distance;
    }

    // Implementação da interface IComparable para ordenar na fila de prioridade
    public int CompareTo(PuzzleState other)
    {
        return Cost.CompareTo(other.Cost);
    }

    // Gera estados de movimento (subproblemas) ao mover a peça vazia
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
                // Cria uma nova configuração do quebra-cabeça
                int[,] newPuzzle = (int[,])Puzzle.Clone();
                newPuzzle[EmptyRow, EmptyCol] = newPuzzle[newRow, newCol];
                newPuzzle[newRow, newCol] = 0;
                successors.Add(new PuzzleState(newPuzzle, newRow, newCol, Depth + 1, this));
            }
        }

        return successors;
    }

    // Verifica se o estado atual é a solução
    public bool IsGoal()
    {
        int[,] goal = { { 1, 2, 3 }, { 4, 5, 6 }, { 7, 8, 0 } };
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

    // Exibe o estado atual do puzzle
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

    // Converte o puzzle para string para verificar estados visitados
    private static string PuzzleToString(int[,] puzzle)
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

    // Imprime a sequência de movimentos até a solução
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
        int[,] initialPuzzle = GenerateRandomArray();
        PuzzleSolver.Solve(initialPuzzle);
    }

    private static int[,] GenerateRandomArray()
    {
        int[,] initialPuzzle =
        {
            { -1, -1, -1 },
            { -1, -1, -1 },
            { -1, -1, -1 }
        };
        var random = new Random();

        for (int row = 0; row < 3; row++)
        {
            for (int col = 0; col < 3; col++)
            {
                var nextInt =  random.Next(0, 9);
                while (FindNumber(initialPuzzle, nextInt))
                {
                    nextInt = random.Next(0, 9);
                }
                initialPuzzle[row, col] = nextInt;
            }
        }

        return initialPuzzle;
    }
    
    public static bool FindNumber(int[,] array, int target)
    {
        for (int i = 0; i < array.GetLength(0); i++) // Loop through rows
        {
            for (int j = 0; j < array.GetLength(1); j++) // Loop through columns
            {
                if (array[i, j] == target)
                {
                    return true; // Return true if the number is found
                }
            }
        }
        return false; // Return false if the number is not found
    }
}