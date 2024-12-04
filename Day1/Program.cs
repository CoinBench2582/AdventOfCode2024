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
            var output = ParseData(pathOfSource).OrderData();
            distance = output.DistanceSum();
            int similarityScore = output.SplitData().PairForScoring().SimilarityScoresSum();
            return null;
        }
        catch (Exception ex)
        {
            distance = default;
            return ex;
        }
    }

    private static IEnumerable<Pair> ParseData(string aboslutePath)
        => File.ReadAllLines(aboslutePath)
            .Select(s => s.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
            .Select(x => x.Length == 2
                ? new Pair(int.Parse(x[0]), int.Parse(x[1]))
                : throw new ArgumentException($"Neočekávaná délka řádku: {x.Length}"));

    #region Part 1
    public static int GetDistance(this (IEnumerable<int> left, IEnumerable<int> right) input)
        => (input.left, input.right).OrderData().ZipData().DistanceSum();

    public static int GetDistance(this IEnumerable<Pair> data)
        => data.OrderData().DistanceSum();

    private static int DistanceSum(this IEnumerable<Pair> data)
        => data.Select(t => int.Abs(t.Left - t.Right)).Sum();
    #endregion

    #region Part 2
    public static int GetSimilarityScores(this (IEnumerable<int> toCount, IEnumerable<int> pool) input)
        => input.OrderData()
            .PairForScoring()
            .SimilarityScoresSum();

    public static int GetSimilarityScores(this IEnumerable<Pair> data)
        => data.OrderData().SplitData()
            .PairForScoring()
            .SimilarityScoresSum();

    private static IEnumerable<Pair> PairForScoring(this (IEnumerable<int> toCount, IEnumerable<int> pool) input)
        => (input.toCount,
            input.pool.CountEach(input.toCount))
            .ZipData();

    private static int SimilarityScoresSum(this IEnumerable<Pair> data)
        => data.Select(p => p.Left * p.Right).Sum();

    private static IEnumerable<int> CountEach(this IEnumerable<int> pool, IEnumerable<int> toCount)
        => toCount.Distinct()
            .Select(target
                => pool.SkipWhile(n => n < target)
                    .TakeWhile(n => n == target)
                    .Count());
    #endregion

    #region Utils
    internal static IEnumerable<Pair> OrderData(this IEnumerable<Pair> data)
        => data.SplitData().OrderData().ZipData();

    private static (IEnumerable<int> left, IEnumerable<int> right) OrderData(this (IEnumerable<int> left, IEnumerable<int> right) input)
        => (input.left.Order(), input.right.Order());

    private static (IEnumerable<int> left, IEnumerable<int> right) SplitData(this IEnumerable<Pair> data)
        => (data.Select(p => p.Left),
            data.Select(x => x.Right));

    private static IEnumerable<Pair> ZipData(this (IEnumerable<int> left, IEnumerable<int> right) input)
        => input.left.Zip(input.right)
            .Select(p => new Pair(p.First, p.Second));
    #endregion
}

public record struct Pair(int Left, int Right);