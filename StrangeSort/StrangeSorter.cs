namespace StrangeSort;

/// <summary>
/// 提供随机剪枝帮助方法，通过删除元素直到序列按比较器定义变为有序。
/// </summary>
/// <remarks>
/// <para>“有序”表示每一对相邻元素都满足 <c>comparer.Compare(previous, current) &lt;= 0</c>。</para>
/// <para>此类型不单独提供升序或降序标志。若要朝自然降序方向剪枝，请传入反向比较器。</para>
/// <para>此算法不是传统排序。它会反复随机删除当前序列的一半元素，因此结果是一个有序子序列，且存活元素保持原有相对顺序。</para>
/// <para>数组重载绝不会修改输入数组，并以数组形式返回最终子序列。列表重载会原地修改输入列表。</para>
/// <para>此首个版本有意只公开将比较器参数放在固定位置的重载；不提供可直接跳过比较器并指定删除策略或随机源的公共重载。</para>
/// <para>对于没有有意义默认顺序的类型，请传入显式比较器。如果元素类型的默认比较器无法比较这些值，则只有在实际需要比较时才会观察到原始 <see cref="ArgumentException"/>。空输入和单元素输入无需比较即可成功。</para>
/// </remarks>
public static class StrangeSorter
{
    /// <summary>
    /// 通过反复随机删除数组中的一半元素直到其变为有序，返回一个有序子序列。
    /// </summary>
    /// <remarks>
    /// 此重载使用 <see cref="Comparer{T}.Default"/>、<see cref="RemovalCountStrategy.FloorHalf"/> 和 <see cref="Random.Shared"/>。
    /// </remarks>
    /// <param name="values">要剪枝的数组。</param>
    /// <returns>最终的有序子序列。返回的数组可能会复用输入数组引用，也可能不会。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="values"/> 为 <see langword="null"/>。</exception>
    /// <exception cref="ArgumentException">当需要进行比较时，<see cref="Comparer{T}.Default"/> 无法比较 <typeparamref name="T"/>。</exception>
    public static T[] PruneUntilSorted<T>(T[] values)
    {
        return PruneUntilSorted(values, comparer: null, RemovalCountStrategy.FloorHalf, Random.Shared);
    }

    /// <summary>
    /// 通过反复随机删除数组中的一半元素直到其变为有序，返回一个有序子序列。
    /// </summary>
    /// <remarks>
    /// 此重载使用提供的比较器；当 <paramref name="comparer"/> 为 <see langword="null"/> 时使用 <see cref="Comparer{T}.Default"/>，并搭配 <see cref="RemovalCountStrategy.FloorHalf"/> 和 <see cref="Random.Shared"/>。
    /// </remarks>
    /// <param name="values">要剪枝的数组。</param>
    /// <param name="comparer">定义目标顺序的比较器。为 <see langword="null"/> 时使用 <see cref="Comparer{T}.Default"/>。</param>
    /// <returns>最终的有序子序列。返回的数组可能会复用输入数组引用，也可能不会。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="values"/> 为 <see langword="null"/>。</exception>
    /// <exception cref="ArgumentException">当需要进行比较时，<see cref="Comparer{T}.Default"/> 无法比较 <typeparamref name="T"/>。</exception>
    public static T[] PruneUntilSorted<T>(T[] values, IComparer<T>? comparer)
    {
        return PruneUntilSorted(values, comparer, RemovalCountStrategy.FloorHalf, Random.Shared);
    }

    /// <summary>
    /// 通过反复随机删除数组中的一半元素直到其变为有序，返回一个有序子序列。
    /// </summary>
    /// <remarks>
    /// 此重载使用提供的比较器；当 <paramref name="comparer"/> 为 <see langword="null"/> 时使用 <see cref="Comparer{T}.Default"/>，并搭配提供的删除策略和 <see cref="Random.Shared"/>。
    /// </remarks>
    /// <param name="values">要剪枝的数组。</param>
    /// <param name="comparer">定义目标顺序的比较器。为 <see langword="null"/> 时使用 <see cref="Comparer{T}.Default"/>。</param>
    /// <param name="removalStrategy">控制在当前序列长度为奇数时，每轮删除当前序列的下取整一半还是上取整一半元素。</param>
    /// <returns>最终的有序子序列。返回的数组可能会复用输入数组引用，也可能不会。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="values"/> 为 <see langword="null"/>。</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="removalStrategy"/> 不是已定义的值。</exception>
    /// <exception cref="ArgumentException">当需要进行比较时，<see cref="Comparer{T}.Default"/> 无法比较 <typeparamref name="T"/>。</exception>
    public static T[] PruneUntilSorted<T>(T[] values, IComparer<T>? comparer, RemovalCountStrategy removalStrategy)
    {
        return PruneUntilSorted(values, comparer, removalStrategy, Random.Shared);
    }

    /// <summary>
    /// 通过反复随机删除数组中的一半元素直到其变为有序，返回一个有序子序列。
    /// </summary>
    /// <param name="values">要剪枝的数组。</param>
    /// <param name="comparer">定义目标顺序的比较器。为 <see langword="null"/> 时使用 <see cref="Comparer{T}.Default"/>。</param>
    /// <param name="removalStrategy">控制在当前序列长度为奇数时，每轮删除当前序列的下取整一半还是上取整一半元素。</param>
    /// <param name="random">每轮选择要删除哪些元素时使用的随机源。</param>
    /// <returns>最终的有序子序列。返回的数组可能会复用输入数组引用，也可能不会。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="values"/> 或 <paramref name="random"/> 为 <see langword="null"/>。</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="removalStrategy"/> 不是已定义的值。</exception>
    /// <exception cref="ArgumentException">当需要进行比较时，<see cref="Comparer{T}.Default"/> 无法比较 <typeparamref name="T"/>。</exception>
    public static T[] PruneUntilSorted<T>(T[] values, IComparer<T>? comparer, RemovalCountStrategy removalStrategy, Random random)
    {
        ArgumentNullException.ThrowIfNull(values);
        ArgumentNullException.ThrowIfNull(random);

        ValidateRemovalStrategy(removalStrategy);
        comparer ??= Comparer<T>.Default;

        if (values.Length <= 1 || IsSorted(values, values.Length, comparer))
        {
            return values;
        }

        var buffer = new T[values.Length];
        Array.Copy(values, buffer, values.Length);

        var finalLength = PruneUntilSortedCore(buffer, buffer.Length, comparer, removalStrategy, random);
        var result = new T[finalLength];
        Array.Copy(buffer, result, finalLength);
        return result;
    }

    /// <summary>
    /// 通过反复随机删除列表中的一半元素直到其变为有序。
    /// </summary>
    /// <remarks>
    /// 此重载使用 <see cref="Comparer{T}.Default"/>、<see cref="RemovalCountStrategy.FloorHalf"/> 和 <see cref="Random.Shared"/>。
    /// </remarks>
    /// <param name="values">要原地剪枝的列表。</param>
    /// <exception cref="ArgumentNullException"><paramref name="values"/> 为 <see langword="null"/>。</exception>
    /// <exception cref="ArgumentException">当需要进行比较时，<see cref="Comparer{T}.Default"/> 无法比较 <typeparamref name="T"/>。</exception>
    public static void PruneUntilSorted<T>(List<T> values)
    {
        PruneUntilSorted(values, comparer: null, RemovalCountStrategy.FloorHalf, Random.Shared);
    }

    /// <summary>
    /// 通过反复随机删除列表中的一半元素直到其变为有序。
    /// </summary>
    /// <remarks>
    /// 此重载使用提供的比较器；当 <paramref name="comparer"/> 为 <see langword="null"/> 时使用 <see cref="Comparer{T}.Default"/>，并搭配 <see cref="RemovalCountStrategy.FloorHalf"/> 和 <see cref="Random.Shared"/>。
    /// </remarks>
    /// <param name="values">要原地剪枝的列表。</param>
    /// <param name="comparer">定义目标顺序的比较器。为 <see langword="null"/> 时使用 <see cref="Comparer{T}.Default"/>。</param>
    /// <exception cref="ArgumentNullException"><paramref name="values"/> 为 <see langword="null"/>。</exception>
    /// <exception cref="ArgumentException">当需要进行比较时，<see cref="Comparer{T}.Default"/> 无法比较 <typeparamref name="T"/>。</exception>
    public static void PruneUntilSorted<T>(List<T> values, IComparer<T>? comparer)
    {
        PruneUntilSorted(values, comparer, RemovalCountStrategy.FloorHalf, Random.Shared);
    }

    /// <summary>
    /// 通过反复随机删除列表中的一半元素直到其变为有序。
    /// </summary>
    /// <remarks>
    /// 此重载使用提供的比较器；当 <paramref name="comparer"/> 为 <see langword="null"/> 时使用 <see cref="Comparer{T}.Default"/>，并搭配提供的删除策略和 <see cref="Random.Shared"/>。
    /// </remarks>
    /// <param name="values">要原地剪枝的列表。</param>
    /// <param name="comparer">定义目标顺序的比较器。为 <see langword="null"/> 时使用 <see cref="Comparer{T}.Default"/>。</param>
    /// <param name="removalStrategy">控制在当前序列长度为奇数时，每轮删除当前序列的下取整一半还是上取整一半元素。</param>
    /// <exception cref="ArgumentNullException"><paramref name="values"/> 为 <see langword="null"/>。</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="removalStrategy"/> 不是已定义的值。</exception>
    /// <exception cref="ArgumentException">当需要进行比较时，<see cref="Comparer{T}.Default"/> 无法比较 <typeparamref name="T"/>。</exception>
    public static void PruneUntilSorted<T>(List<T> values, IComparer<T>? comparer, RemovalCountStrategy removalStrategy)
    {
        PruneUntilSorted(values, comparer, removalStrategy, Random.Shared);
    }

    /// <summary>
    /// 通过反复随机删除列表中的一半元素直到其变为有序。
    /// </summary>
    /// <remarks>
    /// 如果在剪枝开始后比较器或随机源抛出异常，列表可能已经被部分修改，且不会回滚。
    /// </remarks>
    /// <param name="values">要原地剪枝的列表。</param>
    /// <param name="comparer">定义目标顺序的比较器。为 <see langword="null"/> 时使用 <see cref="Comparer{T}.Default"/>。</param>
    /// <param name="removalStrategy">控制在当前序列长度为奇数时，每轮删除当前序列的下取整一半还是上取整一半元素。</param>
    /// <param name="random">每轮选择要删除哪些元素时使用的随机源。</param>
    /// <exception cref="ArgumentNullException"><paramref name="values"/> 或 <paramref name="random"/> 为 <see langword="null"/>。</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="removalStrategy"/> 不是已定义的值。</exception>
    /// <exception cref="ArgumentException">当需要进行比较时，<see cref="Comparer{T}.Default"/> 无法比较 <typeparamref name="T"/>。</exception>
    public static void PruneUntilSorted<T>(List<T> values, IComparer<T>? comparer, RemovalCountStrategy removalStrategy, Random random)
    {
        ArgumentNullException.ThrowIfNull(values);
        ArgumentNullException.ThrowIfNull(random);

        ValidateRemovalStrategy(removalStrategy);
        comparer ??= Comparer<T>.Default;

        if (values.Count <= 1 || IsSorted(values, values.Count, comparer))
        {
            return;
        }

        var finalLength = PruneUntilSortedCore(values, values.Count, comparer, removalStrategy, random);
        if (finalLength < values.Count)
        {
            values.RemoveRange(finalLength, values.Count - finalLength);
        }
    }

    private static int PruneUntilSortedCore<T>(IList<T> values, int length, IComparer<T> comparer, RemovalCountStrategy removalStrategy, Random random)
    {
        var candidateIndexes = new int[length];
        var removed = new bool[length];

        while (!IsSorted(values, length, comparer))
        {
            var removalCount = GetRemovalCount(length, removalStrategy);
            InitializeCandidateIndexes(candidateIndexes, length);
            Array.Clear(removed, 0, length);
            MarkRemovedIndexes(candidateIndexes, removed, length, removalCount, random);
            length = Compact(values, removed, length);
        }

        return length;
    }

    private static bool IsSorted<T>(IList<T> values, int length, IComparer<T> comparer)
    {
        for (var index = 1; index < length; index++)
        {
            if (comparer.Compare(values[index - 1], values[index]) > 0)
            {
                return false;
            }
        }

        return true;
    }

    private static int GetRemovalCount(int length, RemovalCountStrategy removalStrategy)
    {
        return removalStrategy switch
        {
            RemovalCountStrategy.FloorHalf => length / 2,
            RemovalCountStrategy.CeilingHalf => (length + 1) / 2,
            _ => throw new ArgumentOutOfRangeException(nameof(removalStrategy)),
        };
    }

    private static void ValidateRemovalStrategy(RemovalCountStrategy removalStrategy)
    {
        if (removalStrategy is not RemovalCountStrategy.FloorHalf and not RemovalCountStrategy.CeilingHalf)
        {
            throw new ArgumentOutOfRangeException(nameof(removalStrategy));
        }
    }

    private static void InitializeCandidateIndexes(int[] candidateIndexes, int length)
    {
        for (var index = 0; index < length; index++)
        {
            candidateIndexes[index] = index;
        }
    }

    private static void MarkRemovedIndexes(int[] candidateIndexes, bool[] removed, int length, int removalCount, Random random)
    {
        // 前 removalCount 次 Fisher-Yates 交换会在不放回的情况下选出 removalCount 个唯一索引。
        for (var index = 0; index < removalCount; index++)
        {
            var swapIndex = random.Next(index, length);
            (candidateIndexes[index], candidateIndexes[swapIndex]) = (candidateIndexes[swapIndex], candidateIndexes[index]);
            removed[candidateIndexes[index]] = true;
        }
    }

    private static int Compact<T>(IList<T> values, bool[] removed, int length)
    {
        var writeIndex = 0;

        for (var readIndex = 0; readIndex < length; readIndex++)
        {
            if (removed[readIndex])
            {
                continue;
            }

            if (writeIndex != readIndex)
            {
                values[writeIndex] = values[readIndex];
            }

            writeIndex++;
        }

        return writeIndex;
    }
}
