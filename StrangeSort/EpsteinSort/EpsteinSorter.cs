using System.Numerics;

namespace StrangeSort.EpsteinSort;

/// <summary>
/// 提供 EpsteinSort 固定过滤后排序帮助方法，先删除不在 <c>[0, 18)</c> 内的值，再对保留元素排序。
/// </summary>
/// <remarks>
/// <para>保留条件固定为 <c>value &gt;= 0 &amp;&amp; value &lt; 18</c>。</para>
/// <para>浮点类型中的 <c>NaN</c>、<c>-Infinity</c> 和 <c>+Infinity</c> 视为不在该区间内，会被删除。</para>
/// <para>过滤阶段始终使用固定数值区间，不受比较器影响；比较器只参与排序阶段。</para>
/// <para>数组重载绝不会修改输入数组，并返回承载结果的数组。列表重载会原地修改输入列表。</para>
/// </remarks>
public static class EpsteinSorter
{
    /// <summary>
    /// 执行 EpsteinSort，先删除数组中不在 <c>[0, 18)</c> 内的值，再对保留元素排序并返回结果数组。
    /// </summary>
    /// <remarks>
    /// 此重载使用 <see cref="Comparer{T}.Default"/>。
    /// </remarks>
    /// <typeparam name="T">元素类型。</typeparam>
    /// <param name="values">要过滤并排序的数组。</param>
    /// <returns>仅包含 <c>[0, 18)</c> 内元素且已完成排序的结果数组。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="values"/> 为 <see langword="null"/>。</exception>
    public static T[] RetainOnlyMinorsAndSort<T>(T[] values)
        where T : INumber<T>
    {
        return RetainOnlyMinorsAndSort(values, comparer: null);
    }

    /// <summary>
    /// 执行 EpsteinSort，先删除数组中不在 <c>[0, 18)</c> 内的值，再对保留元素排序并返回结果数组。
    /// </summary>
    /// <remarks>
    /// 当 <paramref name="comparer"/> 为 <see langword="null"/> 时使用 <see cref="Comparer{T}.Default"/>。
    /// 此重载绝不会修改 <paramref name="values"/>。
    /// </remarks>
    /// <typeparam name="T">元素类型。</typeparam>
    /// <param name="values">要过滤并排序的数组。</param>
    /// <param name="comparer">定义最终排序顺序的比较器。为 <see langword="null"/> 时使用 <see cref="Comparer{T}.Default"/>。</param>
    /// <returns>仅包含 <c>[0, 18)</c> 内元素且已完成排序的结果数组。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="values"/> 为 <see langword="null"/>。</exception>
    public static T[] RetainOnlyMinorsAndSort<T>(T[] values, IComparer<T>? comparer)
        where T : INumber<T>
    {
        ArgumentNullException.ThrowIfNull(values);

        comparer ??= Comparer<T>.Default;
        if (values.Length == 0)
        {
            return Array.Empty<T>();
        }

        var upperBound = T.CreateChecked(18);
        var buffer = new T[values.Length];
        var retainedCount = CopyRetainedValues(values, buffer, upperBound);
        Array.Sort(buffer, 0, retainedCount, comparer);

        if (retainedCount == 0)
        {
            return Array.Empty<T>();
        }

        if (retainedCount == buffer.Length)
        {
            return buffer;
        }

        var result = new T[retainedCount];
        Array.Copy(buffer, result, retainedCount);
        return result;
    }

    /// <summary>
    /// 执行 EpsteinSort，先删除列表中不在 <c>[0, 18)</c> 内的值，再原地对保留元素排序。
    /// </summary>
    /// <remarks>
    /// 此重载使用 <see cref="Comparer{T}.Default"/>。
    /// 如果在排序阶段比较器抛出异常，列表可能已经因过滤阶段而被修改，且不会回滚。
    /// </remarks>
    /// <typeparam name="T">元素类型。</typeparam>
    /// <param name="values">要原地过滤并排序的列表。</param>
    /// <exception cref="ArgumentNullException"><paramref name="values"/> 为 <see langword="null"/>。</exception>
    public static void RetainOnlyMinorsAndSort<T>(List<T> values)
        where T : INumber<T>
    {
        RetainOnlyMinorsAndSort(values, comparer: null);
    }

    /// <summary>
    /// 执行 EpsteinSort，先删除列表中不在 <c>[0, 18)</c> 内的值，再原地对保留元素排序。
    /// </summary>
    /// <remarks>
    /// 当 <paramref name="comparer"/> 为 <see langword="null"/> 时使用 <see cref="Comparer{T}.Default"/>。
    /// 此重载会原地修改 <paramref name="values"/>。如果在排序阶段比较器抛出异常，列表可能已经因过滤阶段而被修改，且不会回滚。
    /// </remarks>
    /// <typeparam name="T">元素类型。</typeparam>
    /// <param name="values">要原地过滤并排序的列表。</param>
    /// <param name="comparer">定义最终排序顺序的比较器。为 <see langword="null"/> 时使用 <see cref="Comparer{T}.Default"/>。</param>
    /// <exception cref="ArgumentNullException"><paramref name="values"/> 为 <see langword="null"/>。</exception>
    public static void RetainOnlyMinorsAndSort<T>(List<T> values, IComparer<T>? comparer)
        where T : INumber<T>
    {
        ArgumentNullException.ThrowIfNull(values);

        comparer ??= Comparer<T>.Default;
        if (values.Count == 0)
        {
            return;
        }

        var upperBound = T.CreateChecked(18);
        var retainedCount = CompactRetainedValues(values, upperBound);
        if (retainedCount < values.Count)
        {
            values.RemoveRange(retainedCount, values.Count - retainedCount);
        }

        values.Sort(comparer);
    }

    private static int CopyRetainedValues<T>(T[] source, T[] destination, T upperBound)
        where T : INumber<T>
    {
        var writeIndex = 0;

        for (var readIndex = 0; readIndex < source.Length; readIndex++)
        {
            var current = source[readIndex];
            if (!ShouldRetain(current, upperBound))
            {
                continue;
            }

            destination[writeIndex] = current;
            writeIndex++;
        }

        return writeIndex;
    }

    private static int CompactRetainedValues<T>(IList<T> values, T upperBound)
        where T : INumber<T>
    {
        var writeIndex = 0;

        for (var readIndex = 0; readIndex < values.Count; readIndex++)
        {
            var current = values[readIndex];
            if (!ShouldRetain(current, upperBound))
            {
                continue;
            }

            if (writeIndex != readIndex)
            {
                values[writeIndex] = current;
            }

            writeIndex++;
        }

        return writeIndex;
    }

    private static bool ShouldRetain<T>(T value, T upperBound)
        where T : INumber<T>
    {
        return !T.IsNaN(value) && !T.IsInfinity(value) && value >= T.Zero && value < upperBound;
    }
}
