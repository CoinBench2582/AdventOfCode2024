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
        Exception? ex = TryGetResult(path, out int distance);
        if (ex is not null)
        {
            WriteLine($"Stala se chyba: {ex.Message}");
            return;
        }
        WriteLine($"Výsledná vzdálenost: {distance}");
        ReadLine();
    }
}

public static class Methods
{
    public static Exception? TryGetResult(string pathOfSource, out int distance)
    {
        try
        {
            var (left, right) = ParseData(pathOfSource).SplitData();
            distance = left.GetDistance(right);
            return null;
        }
        catch (Exception ex)
        {
            distance = default;
            return ex;
        }
    }

    public static int GetDistance(this IEnumerable<int> left, IEnumerable<int> right)
        => left.Order().Zip(right.Order())
                .Select(t => int.Abs(t.First - t.Second))
                .Sum();

    public static int GetDistance(this IEnumerable<Pair> data)
        => data.Select(t => int.Abs(t.Left - t.Right)).Sum();

    internal static (IEnumerable<int> left, IEnumerable<int> right) SplitData(this IEnumerable<Pair> data)
    {
        int count = data.Count();
        (List<int> left, List<int> right) = (new(++count), new(count));
        left.AddRange(data.Select(p => p.Left));
        right.AddRange(data.Select(p => p.Right));
        return (left, right);
    }

    private static IEnumerable<Pair> ParseData(string aboslutePath)
        => File.ReadAllLines(aboslutePath)
            .Select(s => s.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
            .Select(x => x.Length == 2
                ? new Pair(int.Parse(x[0]), int.Parse(x[1]))
                : throw new ArgumentException($"Neočekávaná délka řádku: {x.Length}"));
}

public record struct Pair(int Left, int Right);