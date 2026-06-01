namespace StrangeSort.TrumpSort;

/// <summary>
/// 提供 TrumpSort 单次随机重排帮助方法，通过一次 Fisher-Yates shuffle 打乱输入内容。
/// </summary>
/// <remarks>
/// <para>此类型不是传统意义上的“排序”。它不会保证结果有序，只会对输入序列执行一次随机重排。</para>
/// <para>名称中的“Win”只是命名梗，不表示算法成功找到某种最优顺序。</para>
/// <para>数组重载不会修改输入数组，而是返回承载随机排列结果的新数组。列表重载会原地修改输入列表。</para>
/// <para>默认重载使用 <see cref="Random.Shared"/>。若要复现结果或编写稳定测试，请使用显式传入 <see cref="Random"/> 的重载。</para>
/// </remarks>
public static class TrumpSorter
{
    /// <summary>
    /// 对数组执行一次随机重排，并返回结果数组。
    /// </summary>
    /// <remarks>
    /// 此重载使用 <see cref="Random.Shared"/>。
    /// 该操作不是传统排序，不保证返回结果有序。
    /// 此重载不会修改 <paramref name="values"/>。
    /// </remarks>
    /// <param name="values">要随机重排的数组。</param>
    /// <returns>包含与输入相同元素多重集的随机排列结果数组。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="values"/> 为 <see langword="null"/>。</exception>
    public static T[] ShuffleAndWin<T>(T[] values)
    {
        return ShuffleAndWin(values, Random.Shared);
    }

    /// <summary>
    /// 对数组执行一次随机重排，并返回结果数组。
    /// </summary>
    /// <remarks>
    /// 该操作不是传统排序，不保证返回结果有序。
    /// 此重载不会修改 <paramref name="values"/>。如果 <paramref name="random"/> 在重排过程中抛出异常，原数组仍保持不变。
    /// </remarks>
    /// <param name="values">要随机重排的数组。</param>
    /// <param name="random">执行抽样时使用的随机源。</param>
    /// <returns>包含与输入相同元素多重集的随机排列结果数组。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="values"/> 或 <paramref name="random"/> 为 <see langword="null"/>。</exception>
    public static T[] ShuffleAndWin<T>(T[] values, Random random)
    {
        ArgumentNullException.ThrowIfNull(values);
        ArgumentNullException.ThrowIfNull(random);

        var result = new T[values.Length];
        Array.Copy(values, result, values.Length);

        if (result.Length <= 1)
        {
            return result;
        }

        ShuffleCore(result, random);
        return result;
    }

    /// <summary>
    /// 对列表执行一次原地随机重排。
    /// </summary>
    /// <remarks>
    /// 此重载使用 <see cref="Random.Shared"/>。
    /// 该操作不是传统排序，不保证处理后列表有序。
    /// 此重载会原地修改 <paramref name="values"/>。如果随机源在处理过程中抛出异常，列表可能已经被部分修改，且不会回滚。
    /// </remarks>
    /// <param name="values">要原地随机重排的列表。</param>
    /// <exception cref="ArgumentNullException"><paramref name="values"/> 为 <see langword="null"/>。</exception>
    public static void ShuffleAndWin<T>(List<T> values)
    {
        ShuffleAndWin(values, Random.Shared);
    }

    /// <summary>
    /// 对列表执行一次原地随机重排。
    /// </summary>
    /// <remarks>
    /// 该操作不是传统排序，不保证处理后列表有序。
    /// 此重载会原地修改 <paramref name="values"/>。如果 <paramref name="random"/> 在处理过程中抛出异常，列表可能已经被部分修改，且不会回滚。
    /// </remarks>
    /// <param name="values">要原地随机重排的列表。</param>
    /// <param name="random">执行抽样时使用的随机源。</param>
    /// <exception cref="ArgumentNullException"><paramref name="values"/> 或 <paramref name="random"/> 为 <see langword="null"/>。</exception>
    public static void ShuffleAndWin<T>(List<T> values, Random random)
    {
        ArgumentNullException.ThrowIfNull(values);
        ArgumentNullException.ThrowIfNull(random);

        if (values.Count <= 1)
        {
            return;
        }

        ShuffleCore(values, random);
    }

    private static void ShuffleCore<T>(IList<T> values, Random random)
    {
        for (var index = values.Count - 1; index > 0; index--)
        {
            var swapIndex = random.Next(index + 1);
            (values[index], values[swapIndex]) = (values[swapIndex], values[index]);
        }
    }
}
