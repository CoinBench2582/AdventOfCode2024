using static System.Console;
using static Day2.Methods;

namespace Day2;

internal class Program
{
#if DEBUG
    const string localPath = @"Source.txt";
    static readonly string debugPath = Path.GetFullPath(localPath);
#endif

    static void Main()
    {
        string path;
#if DEBUG
        path = debugPath;
#else
        WriteLine($"Vložte úplnou cestu k cílovému souboru");
        path = ReadLine() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
        {
            WriteLine("Cesta není platná!");
            return;
        }
#endif
        Exception? ex = TryGetResult(path, out int safe);
        if (ex is not null)
        {
            WriteLine($"Stala se chyba: {ex.Message}");
            return;
        }
        WriteLine($"Bezpečných výkazů: {safe}");
        _ = ReadLine();
    }
}

public static class Methods
{
    public static Exception? TryGetResult(string pathOfSource, out int countOfSafe)
    {
        try
        {
            var output = ParseData(pathOfSource);
            countOfSafe = output.Count(IsValidReport);
            return null;
        }
        catch (Exception ex)
        {
            countOfSafe = default;
            return ex;
        }
    }

    private static IEnumerable<IEnumerable<int>> ParseData(string aboslutePath)
        => File.ReadAllLines(aboslutePath)
            .Select(s => s.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
            .Select(x => x.Select(n => int.Parse(n)));

    #region Part1
    internal static bool IsValidReport(this IEnumerable<int> report)
    {
        int last = report.First();
        ChangeStatus status, curr;
        status = ChangeStatus.None;
        foreach (int num in report.Skip(1))
        {
            curr = (num - last) switch
            {
                0 => ChangeStatus.None,
                > 3 => ChangeStatus.None,
                < -3 => ChangeStatus.None,
                > 0 => ChangeStatus.Increasing,
                < 0 => ChangeStatus.Decreasing,
            };
            status = curr != ChangeStatus.None && (status == ChangeStatus.None || status == curr) ? curr : ChangeStatus.None;
            if (status == ChangeStatus.None)
                return false;
            last = num;
        }
        return true;
    }
    #endregion

    #region Utils

    #endregion
}

internal enum ChangeStatus
{
    None,
    Increasing,
    Decreasing
}