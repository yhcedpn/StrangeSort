namespace StrangeSort;

/// <summary>
/// Specifies how many elements each pruning round removes when the current sequence length is odd.
/// </summary>
public enum RemovalCountStrategy
{
    /// <summary>
    /// Removes <c>count / 2</c> elements.
    /// </summary>
    FloorHalf = 0,

    /// <summary>
    /// Removes <c>(count + 1) / 2</c> elements.
    /// </summary>
    CeilingHalf = 1,
}
