namespace StrangeSort.StalinSort;

/// <summary>
/// 提供 StalinSort 确定性剪枝帮助方法，通过从左到右删除违序元素得到有序子序列。
/// </summary>
/// <remarks>
/// <para>“有序”表示每一对相邻保留元素都满足 <c>comparer.Compare(previous, current) &lt;= 0</c>。</para>
/// <para>此类型不提供升序或降序标志。若要得到自然降序方向的结果，请传入反向比较器。</para>
/// <para>此算法不是传统排序。它不会重排元素，只会删除当前相对于最近一个保留元素违序的元素，因此结果是保持原有相对顺序的有序子序列。</para>
/// <para>数组重载绝不会修改输入数组，并返回承载结果的数组。列表重载会原地修改输入列表。</para>
/// <para>对于没有有意义默认顺序的类型，请传入显式比较器。如果元素类型的默认比较器无法比较这些值，则只有在实际需要比较时才会观察到原始 <see cref="ArgumentException"/>。空输入和单元素输入无需比较即可成功。</para>
/// </remarks>
public static class StalinSorter
{
    /// <summary>
    /// 执行 StalinSort，通过从左到右删除数组中的违序元素，返回一个有序子序列。
    /// </summary>
    /// <remarks>
    /// 此重载使用 <see cref="Comparer{T}.Default"/>。
    /// </remarks>
    /// <param name="values">要剪枝的数组。</param>
    /// <returns>保持输入相对顺序的有序子序列。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="values"/> 为 <see langword="null"/>。</exception>
    /// <exception cref="ArgumentException">当需要进行比较时，<see cref="Comparer{T}.Default"/> 无法比较 <typeparamref name="T"/>。</exception>
    public static T[] RemoveOutOfOrder<T>(T[] values)
    {
        return RemoveOutOfOrder(values, comparer: null);
    }

    /// <summary>
    /// 执行 StalinSort，通过从左到右删除数组中的违序元素，返回一个有序子序列。
    /// </summary>
    /// <remarks>
    /// 当 <paramref name="comparer"/> 为 <see langword="null"/> 时使用 <see cref="Comparer{T}.Default"/>。
    /// 此重载绝不会修改 <paramref name="values"/>。
    /// </remarks>
    /// <param name="values">要剪枝的数组。</param>
    /// <param name="comparer">定义目标顺序的比较器。为 <see langword="null"/> 时使用 <see cref="Comparer{T}.Default"/>。</param>
    /// <returns>保持输入相对顺序的有序子序列。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="values"/> 为 <see langword="null"/>。</exception>
    /// <exception cref="ArgumentException">当需要进行比较时，<see cref="Comparer{T}.Default"/> 无法比较 <typeparamref name="T"/>。</exception>
    public static T[] RemoveOutOfOrder<T>(T[] values, IComparer<T>? comparer)
    {
        ArgumentNullException.ThrowIfNull(values);

        comparer ??= Comparer<T>.Default;
        if (values.Length <= 1)
        {
            return values;
        }

        var result = new T[values.Length];
        var keptCount = CopyOrderedSubsequence(values, result, comparer);
        if (keptCount == result.Length)
        {
            return result;
        }

        var trimmed = new T[keptCount];
        Array.Copy(result, trimmed, keptCount);
        return trimmed;
    }

    /// <summary>
    /// 执行 StalinSort，通过从左到右删除列表中的违序元素。
    /// </summary>
    /// <remarks>
    /// 此重载使用 <see cref="Comparer{T}.Default"/>。
    /// 如果在处理开始后比较器抛出异常，列表可能已经被部分修改，且不会回滚。
    /// </remarks>
    /// <param name="values">要原地剪枝的列表。</param>
    /// <exception cref="ArgumentNullException"><paramref name="values"/> 为 <see langword="null"/>。</exception>
    /// <exception cref="ArgumentException">当需要进行比较时，<see cref="Comparer{T}.Default"/> 无法比较 <typeparamref name="T"/>。</exception>
    public static void RemoveOutOfOrder<T>(List<T> values)
    {
        RemoveOutOfOrder(values, comparer: null);
    }

    /// <summary>
    /// 执行 StalinSort，通过从左到右删除列表中的违序元素。
    /// </summary>
    /// <remarks>
    /// 当 <paramref name="comparer"/> 为 <see langword="null"/> 时使用 <see cref="Comparer{T}.Default"/>。
    /// 此重载会原地修改 <paramref name="values"/>。如果在处理开始后比较器抛出异常，列表可能已经被部分修改，且不会回滚。
    /// </remarks>
    /// <param name="values">要原地剪枝的列表。</param>
    /// <param name="comparer">定义目标顺序的比较器。为 <see langword="null"/> 时使用 <see cref="Comparer{T}.Default"/>。</param>
    /// <exception cref="ArgumentNullException"><paramref name="values"/> 为 <see langword="null"/>。</exception>
    /// <exception cref="ArgumentException">当需要进行比较时，<see cref="Comparer{T}.Default"/> 无法比较 <typeparamref name="T"/>。</exception>
    public static void RemoveOutOfOrder<T>(List<T> values, IComparer<T>? comparer)
    {
        ArgumentNullException.ThrowIfNull(values);

        comparer ??= Comparer<T>.Default;
        if (values.Count <= 1)
        {
            return;
        }

        var keptCount = CompactOrderedSubsequence(values, values.Count, comparer);
        if (keptCount < values.Count)
        {
            values.RemoveRange(keptCount, values.Count - keptCount);
        }
    }

    private static int CopyOrderedSubsequence<T>(T[] source, T[] destination, IComparer<T> comparer)
    {
        destination[0] = source[0];

        var writeIndex = 1;
        var lastKept = source[0];

        for (var readIndex = 1; readIndex < source.Length; readIndex++)
        {
            var current = source[readIndex];
            if (comparer.Compare(lastKept, current) > 0)
            {
                continue;
            }

            destination[writeIndex] = current;
            lastKept = current;
            writeIndex++;
        }

        return writeIndex;
    }

    private static int CompactOrderedSubsequence<T>(List<T> values, int length, IComparer<T> comparer)
    {
        var writeIndex = 1;
        var lastKept = values[0];

        for (var readIndex = 1; readIndex < length; readIndex++)
        {
            var current = values[readIndex];
            if (comparer.Compare(lastKept, current) > 0)
            {
                continue;
            }

            if (writeIndex != readIndex)
            {
                values[writeIndex] = current;
            }

            lastKept = current;
            writeIndex++;
        }

        return writeIndex;
    }
}
