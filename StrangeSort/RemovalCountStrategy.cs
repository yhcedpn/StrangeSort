namespace StrangeSort;

/// <summary>
/// 指定当前序列长度为奇数时，每轮剪枝删除多少个元素。
/// </summary>
public enum RemovalCountStrategy
{
    /// <summary>
    /// 删除 <c>count / 2</c> 个元素。
    /// </summary>
    FloorHalf = 0,

    /// <summary>
    /// 删除 <c>(count + 1) / 2</c> 个元素。
    /// </summary>
    CeilingHalf = 1,
}
