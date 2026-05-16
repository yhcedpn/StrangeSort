using System.Numerics;

namespace StrangeSort.BrezhnevSort;

/// <summary>
/// 提供 BrezhnevSort 序数改写帮助方法，根据输入长度将内容改写为从 <c>1</c> 到 <c>n</c> 的序数序列。
/// </summary>
/// <remarks>
/// <para>BrezhnevSort 不是传统排序。它不会比较、筛选或重排输入元素，只会根据输入长度生成固定结果。</para>
/// <para>输入元素原有的值不会被读取，也不会影响输出内容。</para>
/// <para>数组重载绝不会修改输入数组，并返回承载结果的新数组。列表重载会原地修改输入列表。</para>
/// </remarks>
public static class BrezhnevSorter
{
    /// <summary>
    /// 执行 BrezhnevSort，根据数组长度返回从 <c>1</c> 到 <c>n</c> 的序数序列。
    /// </summary>
    /// <remarks>
    /// 此重载不是传统排序。它不会读取 <paramref name="values"/> 中的元素值来决定输出内容，也绝不会修改 <paramref name="values"/>。
    /// </remarks>
    /// <typeparam name="T">元素类型。</typeparam>
    /// <param name="values">用于提供目标长度的数组。</param>
    /// <returns>长度与输入一致、内容固定为从 <c>1</c> 到 <c>n</c> 的新数组。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="values"/> 为 <see langword="null"/>。</exception>
    /// <exception cref="OverflowException">输入长度超出 <typeparamref name="T"/> 可表示范围。</exception>
    public static T[] RewriteAsOrdinalSequence<T>(T[] values)
        where T : INumber<T>
    {
        ArgumentNullException.ThrowIfNull(values);

        var result = new T[values.Length];
        for (var index = 0; index < result.Length; index++)
        {
            result[index] = T.CreateChecked(index + 1);
        }

        return result;
    }

    /// <summary>
    /// 执行 BrezhnevSort，根据列表长度原地将内容改写为从 <c>1</c> 到 <c>n</c> 的序数序列。
    /// </summary>
    /// <remarks>
    /// 此重载不是传统排序。它不会读取 <paramref name="values"/> 中的元素值来决定输出内容，而是会原地重写整个列表。
    /// 如果在改写途中因 <c>T.CreateChecked</c> 抛出 <see cref="OverflowException"/>，列表可能已经被部分修改，且不会回滚。
    /// </remarks>
    /// <typeparam name="T">元素类型。</typeparam>
    /// <param name="values">要原地改写的列表。</param>
    /// <exception cref="ArgumentNullException"><paramref name="values"/> 为 <see langword="null"/>。</exception>
    /// <exception cref="OverflowException">输入长度超出 <typeparamref name="T"/> 可表示范围。</exception>
    public static void RewriteAsOrdinalSequence<T>(List<T> values)
        where T : INumber<T>
    {
        ArgumentNullException.ThrowIfNull(values);

        if (values.Count == 0)
        {
            return;
        }

        for (var index = 0; index < values.Count; index++)
        {
            values[index] = T.CreateChecked(index + 1);
        }
    }
}
