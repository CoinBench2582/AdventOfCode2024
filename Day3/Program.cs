using static System.Console;
using static Day3.Methods;

namespace Day3
{
    internal class Program
    {
#if DEBUG
        const string test = @"Test.text";
        const string target = @"Source.txt";
        readonly static string localPath = Path.GetFullPath(test);
#endif

        static void Main()
        {
            string path;
#if DEBUG
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
            Exception? ex = TryGetResult(path);
            if (ex is not null)
            {
                WriteLine($"Stala se chyba: {ex.Message}");
                return;
            }
            WriteLine($"");
            _ = ReadLine();
        }
    }

    public static class Methods
    {
        public static Exception? TryGetResult(string path)
        {
            return new NotImplementedException();
        }


        #region Part 1

        #endregion

        #region Utils

        #endregion
    }
}
