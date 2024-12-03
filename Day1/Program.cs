using static System.Console;
using static Day1.Methods;

namespace Day1;

internal class Program
{
    static void Main()
    {
        string path;
#if DEBUG
        const string localPath = @"D:\VOJTA\Moje\C_Sharp\AdventOfCode2024\Day1\Source.txt";
        path = localPath;
#else
        WriteLine($"Vložte úplnou cestu k cílovému souboru");
        path = ReadLine() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
        {
            WriteLine("Cesta není platná!");
            return;
        }
#endif
        (int result, Exception? ex) = TryGetResult(path);
        if (ex is not null)
        {
            WriteLine($"Stala se chyba: {ex.Message}");
            return;
        }
        WriteLine($"Výsledná vzdálenost: {result}");
        ReadLine();
    }
}

public static class Methods
{
    public static (int, Exception?) TryGetResult(string pathOfSource)
    {
        try
        {
            var (left, right) = ParseData(pathOfSource);
            int result = left.AggregateEasy(right);
            return (result, null);
        }
        catch (Exception ex)
        {
            return (default, ex);
        }
    }

    public static int AggregateEasy(this IEnumerable<int> left, IEnumerable<int> right)
        => left.Order().Zip(right.Order())
                .Select(t => int.Abs(t.First - t.Second))
                .Sum();

    private static (int[] left, int[] right) ParseData(string aboslutePath)
    {
        int[] left, right;

        try
        {
            string[] lines = File.ReadAllLines(aboslutePath);
            if (lines.Length == 0)
                throw new IOException("Soubor je prázdný!");
            (left, right) = (new int[lines.Length], new int[lines.Length]);
            var reduced =
                lines.Select(s => s.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
                    .Select(x => x.Length == 2
                        ? new Pair(int.Parse(x[0]), int.Parse(x[1]))
                        : throw new ArgumentException($"Neočekávaná délka řádku: {x.Length}"));
            int i = 0;
            foreach (Pair item in reduced)
            {
                (left[i], right[i]) = (item.Left, item.Right);
                i++;
            }
        } 
        catch (Exception ex)
        {
#if DEBUG
            WriteLine(ex.ToString());
#endif
            throw;
        }

        return (left, right);
    }

    internal record struct Pair(int Left, int Right);
}