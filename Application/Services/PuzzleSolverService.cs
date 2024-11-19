using System.Text;
using Application.Dtos;

namespace Application.Services;

public class PuzzleSolverService : IPuzzleSolverService
{
    public List<List<int>> Solve()
    {
        // Gera um novo array aleatório que pode ser resolvido e instancia a lista de retorno
        var initialPuzzle = GenerateSolvableArray();
        var returnList = new List<List<int>>();
        
        // Cria uma nova fila de prioridade para puzzles não resolvidos
        PriorityQueue<PuzzleState, int> openList = new PriorityQueue<PuzzleState, int>();
        // Cria um Hashset de string para guardar os estados dos puzzles já resolvidos anteriormente
        HashSet<string> closedList = new HashSet<string>();

        // Pega a posição x e y do campo vazio
        var emptyPosition = GetEmptyPositions(initialPuzzle);
        
        // Cria uma instancia de puzzle state para guardar o estado inicial do puzzle
        var startState = new PuzzleState(initialPuzzle, emptyPosition.row, emptyPosition.col, 0, null);
        
        // Adiciona o puzzle state inicial para a fila
        openList.Enqueue(startState, startState.Cost);

        // Define puzzle state desejado como null por enquanto, para deixar que o loop rode até que o estado
        PuzzleState? goalState = null;
        
        // Enquanto ainda existir itens na fila aberta
        while (openList.Count > 0)
        {
            // Pega o puzzle state mais recente da fila e transforma a matriz em string
            // Ex: {{1,2,3}} -> "1,2,3"
            var current = openList.Dequeue();
            var puzzleString = PuzzleToString(current.Puzzle);

            // Valida se o puzzle atual é igual a solução desejada (ordenado)
            if (current.IsGoal())
            {
                // Atribui o puzzle atual para
                goalState = current;
                // Printa a solução no console
                PrintSolution(current);
            }

            // Adiciona estado atual em string para lista fechada
            closedList.Add(puzzleString);

            // Gera sucessores válidos se baseando no puzzle atual
            foreach (var successor in current.GenerateSuccessors())
            {
                // Valida se o sucessor gerado não está na lista fechada; Se não estiver adiciona na fila aberta para passar pelo while 
                if (!closedList.Contains(PuzzleToString(successor.Puzzle)))
                {
                    openList.Enqueue(successor, successor.Cost);
                }
            }
        }

        // Enquanto o goalState é diferente de null
        while (goalState != null)
        {
            // é iterado os pais partindo da solução desejada para adicioná-los como lista de números na lista de retorno, montando uma árvore de solução
            returnList.Add(ConvertStringToList(PuzzleToString(goalState.Puzzle)));
            goalState = goalState.Parent;
        }

        // Reverte a lista de retorno para que a solução fique no final
        returnList.Reverse();

        return returnList;
    }

    private static List<int> ConvertStringToList(string puzzleString)
    {
        // Converte uma string de números para uma lista de inteiros
        return puzzleString
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(int.Parse)
            .ToList();
    }
    
    private static void PrintSolution(PuzzleState? state)
    {
        // Printa no console o puzzle
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
        // Gera um array de número de 0 a 8
        List<int> numbers = new List<int>();
        for (int i = 0; i < 9; i++) numbers.Add(i);
        
        Random rng = new Random();

        // Gera um array de números até ser solucionável
        do
        {
            for (int i = numbers.Count - 1; i > 0; i--)
            {
                int j = rng.Next(i + 1);
                (numbers[i], numbers[j]) = (numbers[j], numbers[i]);
            }
        } while (!IsSolvable(numbers.ToArray()));

        // Ajusta o array para um matriz
        int[,] puzzle = new int[3, 3];
        for (int i = 0; i < 9; i++)
        {
            puzzle[i / 3, i % 3] = numbers[i];
        }

        return puzzle;
    }

    private static bool IsSolvable(int[] puzzle)
    {
        // Valida se o array gerado é solucionável baseado no número de inversões, sendo par.
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

    private static string PuzzleToString(int[,] puzzle)
    {
        // Converte uma matriz em string com delimitador
        var sb = new StringBuilder();
        foreach (var value in puzzle)
        {
            sb.Append(value).Append(',');
        }
        return sb.ToString();
    }

    private static (int row, int col) GetEmptyPositions(int[,] puzzle)
    {
        // Pega as posições de linha e coluna do número 0 (vazio)
        var emptyRow = 0;
        var emptyCol = 0;
        
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (puzzle[i, j] == 0)
                {
                    emptyRow = i;
                    emptyCol = j;
                }
            }
        }

        return (row: emptyRow, col: emptyCol);
    }
}