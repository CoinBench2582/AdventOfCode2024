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
        var result = TryGetResult(path);
        if (result.Item2 is not null)
        {
            WriteLine($"Stala se chyba: {result.Item2.Message}");
            return;
        }
        WriteLine($"Výsledek: {result.Item1}");
        ReadLine();
    }
}

public static class Methods
{
    public static (int?, Exception?) TryGetResult(string pathOfSource)
    {
        try
        {
            var (left, right) = ParseData(pathOfSource);
        }
        catch (Exception ex)
        {
            return (null, ex);
        }
    }

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
                        ? new { left = int.Parse(x[0]), right = int.Parse(x[1]) }
                        : throw new ArgumentException($"Neočekávaná délka řádku: {x.Length}"));
            int i = 0;
            foreach (var item in reduced)
                (left[i], right[i]) = (item.left, item.right);
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
}