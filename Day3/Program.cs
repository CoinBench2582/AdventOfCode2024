using static System.Console;
using static AdventOfCode.Day3.Methods;

namespace AdventOfCode.Day3;

internal class Program
{
#if DEBUG
#pragma warning disable IDE0051 // Odebrat nepoužité soukromé členy
    const string _test = @"Test.txt";
    const string _target = @"Source.txt";
#pragma warning restore IDE0051 // Odebrat nepoužité soukromé členy
    static readonly string _localPath = Path.GetFullPath(_target);
#endif

    static void Main()
    {
        string path;
#if DEBUG
        path = _localPath;
#else
        WriteLine($"Vložte úplnou cestu k cílovému souboru");
        path = ReadLine() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
        {
            WriteLine("Cesta není platná!");
            return;
        }
#endif
        Exception? ex = TryGetResult(path, out long simple, out long toggled);
        if (ex is not null)
        {
            WriteLine($"Stala se chyba: {ex.Message}");
            return;
        }
        WriteLine($"Součet všech vynásobených hodnot: {simple}");
        WriteLine($"Součet vynásobených hodnot s přepínáním: {toggled}");
        _ = ReadLine();
    }
}

public static class Methods
{
    public static Exception? TryGetResult(string path, out long simple, out long toggled)
    {
        try
        {
#if DEBUG
            WriteLine("\r\nSimple Parse:");
#endif
            simple = GetInput(path).ParseAgressive().SumPairs();
#if DEBUG
            WriteLine("\r\n\r\n\r\n\r\nToggled Parse:");
#endif
            toggled = GetInput(path).ParseToggled().SumPairs();
            return null;
        }
        catch (Exception e)
        {
            simple = default;
            toggled = default;
            return e;
        }
    }

    private static StreamReader GetInput(string path) => File.OpenText(path);

    #region Part 2
    internal static IEnumerable<MulPair> ParseToggled(this StreamReader stream)
    {
        if (stream is null || !stream.BaseStream.CanRead || stream.EndOfStream)
            throw new ArgumentException(null, nameof(stream));

        // Token: mul(xxx,yyy)
        const int maxMulLen = 3 + 1 + 3 + 1 + 3 + 1;
        // Token: don't()
        const int maxToggleLen = 2 + 3 + 2;

        char[] buffer = new char[maxMulLen];

        bool enabled = true;
        char last; int count; bool between;
#pragma warning disable IDE0018 // Vložená deklarace proměnné
        MulPair pair; Exception? ex;
#pragma warning restore IDE0018 // Vložená deklarace proměnné
        try
        {
            while (!stream.EndOfStream)
            {
                for (last = (char)stream.Read(); !(last is 'm' or 'd' || stream.EndOfStream); last = (char)stream.Read());
                if (stream.EndOfStream) yield break;
                else buffer[0] = last;

                switch (last)
                {
                    case 'd':
                        for (count = 1; !(last == ')' || count >= maxToggleLen || stream.EndOfStream); count++)
                        {
                            if (stream.Peek() is 'm' or 'd') break;
                            buffer[count] = last = (char)stream.Read();
                        }

                        ex = buffer[..count].ParseToggle(out between);
                        if (ex is null)
                            enabled = between;
#if DEBUG
                        else WriteLine($"TOGGLE - {new string(buffer[..count])}: {ex}");
#endif
                        break;

                    case 'm':
                        for (count = 1; !(last == ')' || count >= maxMulLen || stream.EndOfStream); count++)
                        {
                            if (stream.Peek() is 'm' or 'd') break;
                            buffer[count] = last = (char)stream.Read();
                        }

                        if (enabled)
                        {
                            ex = buffer[..count].ParsePair(out pair);
                            if (ex is null)
                                yield return pair;
#if DEBUG
                            else WriteLine($"MUL_INS - {new string(buffer[..count])}: {ex}");
#endif
                        }
                        break;

                    default:
                        throw new Exception("unexpected result");
                }
            }
            yield break;
        }
        finally
        {
            stream?.Close();
            Array.Clear(buffer);
        }
    }

    private static readonly char[] _firstToggleChars = ['d', 'o'];
    private static readonly char[] _middleDisableChars = ['n', '\'', 't'];
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    private static Exception? ParseToggle(this char[] chars, out bool result)
    {
        // do()
        const int minLen = 2 + 2;
        // don't()
        const int maxLen = 2 + 3 + 2;
        try
        {
            if (chars.Length is not (minLen or maxLen))
                throw new ArgumentOutOfRangeException(nameof(chars));

            Span<char> span = chars.AsSpan();
            int len = span.Length;
            char current;
            int currIndex = 0;
            // check if beginning is "do"
            for (; currIndex < 2; currIndex++)
                if (span[currIndex] != _firstToggleChars[currIndex])
                    throw new ArgumentException("Beginning mismatched with \"do\"!");
            // assert which are we working with
            current = span[currIndex];
            switch (current)
            {
                // Enable
                case '(':
                    if (span[++currIndex] == ')')
                    {
                        result = true;
                        return null;
                    }
                    else
                        throw new ArgumentException("Doesn't end with \')\'!");
                // Disable
                case 'n':
                    for (; currIndex < 5; currIndex++)
                        if (span[currIndex] != _middleDisableChars[currIndex -2])
                            throw new ArgumentException("Middle mismatched with \"n't\"!");
                    if (span[currIndex] != '(')
                        throw new ArgumentException("No \'(\' after \"don't\"!");
                    else if (span[++currIndex] != ')')
                        throw new ArgumentException("Doesn't end with \')\'!");
                    else
                    {
                        result = false;
                        return null;
                    }

                default:
                    throw new ArgumentException("Unexpected character!");
            }
        }
        catch (Exception e)
        {
            result = default;
            return e;
        }
    }
    #endregion

    #region Part 1
    internal static IEnumerable<MulPair> ParseAgressive(this StreamReader stream)
    {
        if (stream is null || !stream.BaseStream.CanRead || stream.EndOfStream)
            throw new ArgumentException(null, nameof(stream));

        // Token: mul(xxx,yyy)
        const int maxTokenLen = 3 + 1 + 3 + 1 + 3 + 1;

        char[] buffer = new char[maxTokenLen];
        try
        {
            char last; int count;
#pragma warning disable IDE0018 // Vložená deklarace proměnné
            MulPair pair; Exception? ex;
#pragma warning restore IDE0018 // Vložená deklarace proměnné
            while (!stream.EndOfStream)
            {
                for (last = (char)stream.Read(); !(last == 'm' || stream.EndOfStream); last = (char)stream.Read());
                if (stream.EndOfStream) yield break;
                else buffer[0] = last;

                for (count = 1; !(last == ')' || count >= maxTokenLen || stream.EndOfStream); count++)
                {
                    if (stream.Peek() == 'm') break;
                    buffer[count] = last = (char)stream.Read();
                }

                ex = buffer[..count].ParsePair(out pair);
                if (ex is null)
                    yield return pair;
#if DEBUG
                else WriteLine($"{new string(buffer[..count])}: {ex}");
#endif
            }
            yield break;
        }
        finally
        {
            stream?.Close();
            Array.Clear(buffer);
        }
    }

    private static readonly char[] _firstMulChars = ['m', 'u', 'l', '('];

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
            int currIndex = 0;
            // check if beginning is "mul("
            for (; currIndex < 4; currIndex++)
            {
                if (span[currIndex] != _firstMulChars[currIndex])
                    throw new ArgumentException("Beginning mismatched with \"mul(\"!");
            }

            // get first number
            int until = 7;
            char[] buffer = new char[3];
            int j;
            for (j = 0; currIndex < until && currIndex < len; currIndex++, j++)
            {
                current = span[currIndex];
                if (current is ',')
                {
                    if (currIndex == 4)
                        throw new ArgumentNullException(nameof(chars), "First number is missing!");
                    break;
                }
                if (current is not (>= '0' and <= '9'))
                    throw new ArgumentException("First number is invalid!");
                buffer[j] = current;
            }
            short first = short.Parse(buffer.AsSpan()[..j]);
            // get second number
            int beginning = ++currIndex;
            until = int.Min(currIndex + 3, len);
            for (j = 0; currIndex < until; currIndex++, j++)
            {
                current = span[currIndex];
                if (current is ')')
                {
                    if (currIndex == beginning)
                        throw new ArgumentNullException(nameof(chars), "Second number is missing!");
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
    public static IEnumerator<char> GetEnumerator(this StreamReader reader)
    {
        while (!reader.EndOfStream)
            yield return (char)reader.Read();
    }

    internal static IEnumerable<int> MulEach(this IEnumerable<MulPair> pairs)
        => pairs.Select(pair => pair.Mul());

    internal static long SumPairs(this IEnumerable<MulPair> pairs)
        => pairs.Aggregate(0L, (prev, pair) => prev + pair.Mul());

    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    internal static int Mul(this MulPair pair) => pair.Left * pair.Right;
    #endregion
}

internal record struct MulPair(short Left, short Right);
