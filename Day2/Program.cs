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
    }
}

public static class Methods
{

}