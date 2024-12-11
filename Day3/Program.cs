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

        private static StreamReader GetInput(string path) => File.OpenText(path);

        #region Part 1
        internal static IEnumerable<MulPair> ParseAgressive(this StreamReader stream)
        {
            if (stream is null || !stream.BaseStream.CanRead || stream.EndOfStream || stream.)
                throw new ArgumentException();

            // Token: mul(xxx,yyy)
            const int maxTokenLen = 3 + 1 + 3 + 1 + 3 + 1;

            return Impl();

            IEnumerable<MulPair> Impl()
            {
                char[] buffer = new char[maxTokenLen];
                try
                {
                    
                }
                finally
                {
                    stream?.Close();
                    Array.Clear(buffer);
                    buffer = null;
                }
            }
        }
        #endregion

        #region Utils
        internal static long SumPairs(this IEnumerable<MulPair> pairs)
            => pairs.Select(pair => (long)pair.Mul()).Sum();

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        internal static int Mul(this MulPair pair) => pair.Left * pair.Right;
        #endregion
    }

    internal record struct MulPair(short Left, short Right);
}
