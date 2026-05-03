namespace StrangeSort;

/// <summary>
/// Provides random pruning helpers that remove elements until a sequence is ordered according to a comparer.
/// </summary>
/// <remarks>
/// <para>"Ordered" means <c>comparer.Compare(previous, current) &lt;= 0</c> for every adjacent pair.</para>
/// <para>This type does not expose a separate ascending or descending flag. To prune toward descending natural order, pass a reverse comparer.</para>
/// <para>This algorithm is not a traditional sort. It repeatedly removes a random half of the current sequence, so the result is an ordered subsequence whose surviving elements keep their original relative order.</para>
/// <para>Array overloads never modify the input array and return the final subsequence as an array. List overloads modify the input list in place.</para>
/// <para>This first version intentionally exposes only overloads that keep the comparer parameter in a fixed position; there are no public overloads that skip directly to a removal strategy or random source.</para>
/// <para>For types without a meaningful default order, pass an explicit comparer. If the default comparer for the element type cannot compare the values, the original <see cref="ArgumentException"/> is only observed when a comparison is actually required. Empty and single-element inputs succeed without comparing.</para>
/// </remarks>
public static class StrangeSorter
{
    /// <summary>
    /// Returns an ordered subsequence by repeatedly removing a random half of the array until it is ordered.
    /// </summary>
    /// <remarks>
    /// This overload uses <see cref="Comparer{T}.Default"/>, <see cref="RemovalCountStrategy.FloorHalf"/>, and <see cref="Random.Shared"/>.
    /// </remarks>
    /// <param name="values">The array to prune.</param>
    /// <returns>The final ordered subsequence. The returned array may or may not reuse the input array reference.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="values"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><see cref="Comparer{T}.Default"/> cannot compare <typeparamref name="T"/> and a comparison is required.</exception>
    public static T[] PruneUntilSorted<T>(T[] values)
    {
        return PruneUntilSorted(values, comparer: null, RemovalCountStrategy.FloorHalf, Random.Shared);
    }

    /// <summary>
    /// Returns an ordered subsequence by repeatedly removing a random half of the array until it is ordered.
    /// </summary>
    /// <remarks>
    /// This overload uses the supplied comparer, or <see cref="Comparer{T}.Default"/> when <paramref name="comparer"/> is <see langword="null"/>, together with <see cref="RemovalCountStrategy.FloorHalf"/> and <see cref="Random.Shared"/>.
    /// </remarks>
    /// <param name="values">The array to prune.</param>
    /// <param name="comparer">The comparer that defines the target order. <see langword="null"/> uses <see cref="Comparer{T}.Default"/>.</param>
    /// <returns>The final ordered subsequence. The returned array may or may not reuse the input array reference.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="values"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><see cref="Comparer{T}.Default"/> cannot compare <typeparamref name="T"/> and a comparison is required.</exception>
    public static T[] PruneUntilSorted<T>(T[] values, IComparer<T>? comparer)
    {
        return PruneUntilSorted(values, comparer, RemovalCountStrategy.FloorHalf, Random.Shared);
    }

    /// <summary>
    /// Returns an ordered subsequence by repeatedly removing a random half of the array until it is ordered.
    /// </summary>
    /// <remarks>
    /// This overload uses the supplied comparer, or <see cref="Comparer{T}.Default"/> when <paramref name="comparer"/> is <see langword="null"/>, together with the supplied removal strategy and <see cref="Random.Shared"/>.
    /// </remarks>
    /// <param name="values">The array to prune.</param>
    /// <param name="comparer">The comparer that defines the target order. <see langword="null"/> uses <see cref="Comparer{T}.Default"/>.</param>
    /// <param name="removalStrategy">Controls whether odd-length rounds remove the floor or ceiling half of the current sequence.</param>
    /// <returns>The final ordered subsequence. The returned array may or may not reuse the input array reference.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="values"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="removalStrategy"/> is not a defined value.</exception>
    /// <exception cref="ArgumentException"><see cref="Comparer{T}.Default"/> cannot compare <typeparamref name="T"/> and a comparison is required.</exception>
    public static T[] PruneUntilSorted<T>(T[] values, IComparer<T>? comparer, RemovalCountStrategy removalStrategy)
    {
        return PruneUntilSorted(values, comparer, removalStrategy, Random.Shared);
    }

    /// <summary>
    /// Returns an ordered subsequence by repeatedly removing a random half of the array until it is ordered.
    /// </summary>
    /// <param name="values">The array to prune.</param>
    /// <param name="comparer">The comparer that defines the target order. <see langword="null"/> uses <see cref="Comparer{T}.Default"/>.</param>
    /// <param name="removalStrategy">Controls whether odd-length rounds remove the floor or ceiling half of the current sequence.</param>
    /// <param name="random">The random source used to choose which elements are removed each round.</param>
    /// <returns>The final ordered subsequence. The returned array may or may not reuse the input array reference.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="values"/> or <paramref name="random"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="removalStrategy"/> is not a defined value.</exception>
    /// <exception cref="ArgumentException"><see cref="Comparer{T}.Default"/> cannot compare <typeparamref name="T"/> and a comparison is required.</exception>
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
    /// Repeatedly removes a random half of the list until it is ordered.
    /// </summary>
    /// <remarks>
    /// This overload uses <see cref="Comparer{T}.Default"/>, <see cref="RemovalCountStrategy.FloorHalf"/>, and <see cref="Random.Shared"/>.
    /// </remarks>
    /// <param name="values">The list to prune in place.</param>
    /// <exception cref="ArgumentNullException"><paramref name="values"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><see cref="Comparer{T}.Default"/> cannot compare <typeparamref name="T"/> and a comparison is required.</exception>
    public static void PruneUntilSorted<T>(List<T> values)
    {
        PruneUntilSorted(values, comparer: null, RemovalCountStrategy.FloorHalf, Random.Shared);
    }

    /// <summary>
    /// Repeatedly removes a random half of the list until it is ordered.
    /// </summary>
    /// <remarks>
    /// This overload uses the supplied comparer, or <see cref="Comparer{T}.Default"/> when <paramref name="comparer"/> is <see langword="null"/>, together with <see cref="RemovalCountStrategy.FloorHalf"/> and <see cref="Random.Shared"/>.
    /// </remarks>
    /// <param name="values">The list to prune in place.</param>
    /// <param name="comparer">The comparer that defines the target order. <see langword="null"/> uses <see cref="Comparer{T}.Default"/>.</param>
    /// <exception cref="ArgumentNullException"><paramref name="values"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><see cref="Comparer{T}.Default"/> cannot compare <typeparamref name="T"/> and a comparison is required.</exception>
    public static void PruneUntilSorted<T>(List<T> values, IComparer<T>? comparer)
    {
        PruneUntilSorted(values, comparer, RemovalCountStrategy.FloorHalf, Random.Shared);
    }

    /// <summary>
    /// Repeatedly removes a random half of the list until it is ordered.
    /// </summary>
    /// <remarks>
    /// This overload uses the supplied comparer, or <see cref="Comparer{T}.Default"/> when <paramref name="comparer"/> is <see langword="null"/>, together with the supplied removal strategy and <see cref="Random.Shared"/>.
    /// </remarks>
    /// <param name="values">The list to prune in place.</param>
    /// <param name="comparer">The comparer that defines the target order. <see langword="null"/> uses <see cref="Comparer{T}.Default"/>.</param>
    /// <param name="removalStrategy">Controls whether odd-length rounds remove the floor or ceiling half of the current sequence.</param>
    /// <exception cref="ArgumentNullException"><paramref name="values"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="removalStrategy"/> is not a defined value.</exception>
    /// <exception cref="ArgumentException"><see cref="Comparer{T}.Default"/> cannot compare <typeparamref name="T"/> and a comparison is required.</exception>
    public static void PruneUntilSorted<T>(List<T> values, IComparer<T>? comparer, RemovalCountStrategy removalStrategy)
    {
        PruneUntilSorted(values, comparer, removalStrategy, Random.Shared);
    }

    /// <summary>
    /// Repeatedly removes a random half of the list until it is ordered.
    /// </summary>
    /// <remarks>
    /// If the comparer or random source throws after pruning has started, the list may already have been partially modified and is not rolled back.
    /// </remarks>
    /// <param name="values">The list to prune in place.</param>
    /// <param name="comparer">The comparer that defines the target order. <see langword="null"/> uses <see cref="Comparer{T}.Default"/>.</param>
    /// <param name="removalStrategy">Controls whether odd-length rounds remove the floor or ceiling half of the current sequence.</param>
    /// <param name="random">The random source used to choose which elements are removed each round.</param>
    /// <exception cref="ArgumentNullException"><paramref name="values"/> or <paramref name="random"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="removalStrategy"/> is not a defined value.</exception>
    /// <exception cref="ArgumentException"><see cref="Comparer{T}.Default"/> cannot compare <typeparamref name="T"/> and a comparison is required.</exception>
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
        // The first removalCount Fisher-Yates swaps choose removalCount unique indexes without replacement.
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
