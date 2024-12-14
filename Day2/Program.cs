﻿using static System.Console;
using static Day2.Methods;

namespace Day2;

internal class Program
{
#if DEBUG
    const string testPath = @"TestData.txt";
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
        Exception? ex = TryGetResult(path, out int safe, out int pity);
        if (ex is not null)
        {
            WriteLine($"Stala se chyba: {ex.Message}");
            return;
        }
        WriteLine($"Bezpečných výkazů: {safe}");
        WriteLine($"Včetně omilostněných: {pity}");
    }
}

public static class Methods
{
    public static Exception? TryGetResult(string pathOfSource, out int countOfSafe, out int withPity)
    {
        try
        {
            var output = ParseData(pathOfSource);
            countOfSafe = output.Count(x => IsValidReport(x) == -1);
            withPity = output.Count(r =>
            {
                int offender = IsValidReport(r);
                return offender == -1
                    || IsValidReport(r.Exclude(offender)) == -1
                    || (offender != 0 && IsValidReport(r.Exclude(offender - 1)) == -1);
            });
            return null;
        }
        catch (Exception ex)
        {
            withPity = countOfSafe = default;
            return ex;
        }
    }

    private static IEnumerable<IEnumerable<int>> ParseData(string aboslutePath)
        => File.ReadAllLines(aboslutePath)
            .Select(s => s.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
            .Select(x => x.Select(n => int.Parse(n)));

    #region Part 2
    internal static bool IsValidPity(this IEnumerable<int> report)
    {
        int lastValidNum = default;
        int lastUsedNum = report.First();
        int countOfBad = 0;
        bool firstNotFound = true;
        ChangeStatus lastFine, curr;
        lastFine = ChangeStatus.None;

        foreach (int num in report.Skip(1))
        {
            curr = AssertDiff(num, lastUsedNum);

            (lastFine, lastUsedNum) = Assert( curr, num);
            if (countOfBad > 1)
                return false;
        }
        return true;

        (ChangeStatus, int) Assert(ChangeStatus current, int currNum)
        {
            if (curr is ChangeStatus.None or ChangeStatus.OutOfRange)
            {
                countOfBad++;
                return (lastFine, firstNotFound ? currNum : lastUsedNum);
            }

            ChangeStatus againstLastValid = AssertDiff(currNum, lastValidNum);
            if (firstNotFound)
            {
                firstNotFound = false;
            }
            else if (againstLastValid != current && current != lastFine)
            {
                countOfBad++;
                return (lastFine, lastUsedNum);
            }

            lastValidNum = currNum;
            return (current, currNum);
        }
    }
    #endregion

    #region Part 1
    /// <summary>
    /// Finds out if the report is valid
    /// </summary>
    /// <returns>
    /// Returns the index of the first invalid item.
    /// If <c>-1</c> is returned, the report is valid.
    /// </returns>
    internal static int IsValidReport(this IEnumerable<int> report)
    {
        int last = report.First();
        ChangeStatus status, curr;
        status = ChangeStatus.None;
        int index = 1;
        foreach (int num in report.Skip(1))
        {
            curr = AssertDiff(last, num);
            if (curr is ChangeStatus.None or ChangeStatus.OutOfRange || (status != ChangeStatus.None && curr != status))
                return index;

            status = curr;
            index++;
            last = num;
        }
        return -1;
    }
    #endregion

    #region Utils
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    private static ChangeStatus AssertDiff(int curr, int next)
        => (curr - next) switch
        {
            0 => ChangeStatus.None,
            > 3 => ChangeStatus.OutOfRange,
            < -3 => ChangeStatus.OutOfRange,
            > 0 => ChangeStatus.Increasing,
            < 0 => ChangeStatus.Decreasing,
        };

    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<T> Exclude<T>(this IEnumerable<T> iter, int index)
        => iter.Take(index).Concat(iter.Skip(index+1));
    #endregion
}

internal enum ChangeStatus
{
    None,
    Increasing,
    Decreasing,
    OutOfRange
}