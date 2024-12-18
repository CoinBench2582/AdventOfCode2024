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
            Exception? ex = TryGetResult(path, out long sum);
            if (ex is not null)
            {
                WriteLine($"Stala se chyba: {ex.Message}");
                return;
            }
            WriteLine($"Součet všech vynásobených hodnot: {sum}");
            _ = ReadLine();
        }
    }

    public static class Methods
    {
        public static Exception? TryGetResult(string path, out long sum)
        {
            try
            {
                sum = GetInput(path).ParseAgressive().SumPairs();
                return null;
            }
            catch (Exception e)
            {
                sum = default;
                return e;
            }
        }

        private static StreamReader GetInput(string path) => File.OpenText(path);

        #region Part 1
        internal static IEnumerable<MulPair> ParseAgressive(this StreamReader stream)
        {
            if (stream is null || !stream.BaseStream.CanRead || stream.EndOfStream)
                throw new ArgumentException(null, nameof(stream));

            // Token: mul(xxx,yyy)
            const int maxTokenLen = 3 + 1 + 3 + 1 + 3 + 1;

            return Impl();

            IEnumerable<MulPair> Impl()
            {
                char[] buffer = new char[maxTokenLen];
                try
                {
                    char last; int count;
                    MulPair pair; Exception? ex;
                    while (!stream!.EndOfStream)
                    {
                        for (last = (char)stream.Peek(); last != 'm' || stream.EndOfStream; last = (char)stream.Read());
                        if (last != 'm') break;
                        else buffer[0] = last;

                        for (count = 1; last != ')' || count < maxTokenLen || !stream.EndOfStream; count++)
                            buffer[count] = last = (char)stream.Read();

                        ex = buffer.ParsePair(out pair);
                        if (ex is null)
                            yield return pair;
                        Array.Clear(buffer, 0, maxTokenLen);
                    }
                }
                finally
                {
                    stream?.Close();
                    Array.Clear(buffer);
                }
            }
        }

        private static readonly char[] firstChars = ['m', 'u', 'l', '('];

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        internal static Exception? ParsePair(this char[] chars, out MulPair pair)
        {
            // mul(x,y)
            const int minLen = 3 + 1 + 1 + 1 + 1 + 1;
            // mul(xxx,yyy)
            const int maxLen = 3 + 1 + 3 + 1 + 3 + 1;
            try
            {
                if (chars.Length is < minLen or > maxLen)
                    throw new ArgumentOutOfRangeException(nameof(chars));

                Span<char> span = chars.AsSpan();
                int len = span.Length;
                char current;
                int i = 0;
                // check if beginning is "mul("
                for (; i < 4 && i < len; i++)
                {
                    if (span[i] != firstChars[i])
                        throw new ArgumentException("Beginning mismatched with \"mul(\"!");
                }

                // get first number
                int until = 7;
                char[] buffer = new char[3];
                int j;
                for (j = 0; i < until && i < len; i++, j++)
                {
                    current = span[i];
                    if (current is ',')
                    {
                        if (i == 4)
                            throw new ArgumentNullException(nameof(chars), "First number is missing!");
                        i++;
                        break;
                    }
                    if (current is not (>= '0' and <= '9'))
                        throw new ArgumentException("First number is invalid!");
                    buffer[j] = current;
                }
                short first = short.Parse(buffer.AsSpan()[..j]);
                // get second number
                until = int.Min(i + 2, len);
                for (j = 0; i < until; i++, j++)
                {
                    current = span[i];
                    if (current is ')')
                    {
                        if (i == 4)
                            throw new ArgumentNullException(nameof(chars), "Second number is missing!");
                        i++;
                        break;
                    }
                    if (current is not (>= '0' and <= '9'))
                        throw new ArgumentException("Second number is invalid!");
                    buffer[j] = current;
                }
                short second = short.Parse(buffer.AsSpan()[..j]);

                // check if ends with ')'
                if (chars[^1] != ')')
                    throw new ArgumentException("Doesn't end with \')\'!");
                pair = new(first, second);
                return null;
            }
            catch (Exception e)
            {
                pair = default;
                return e;
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
