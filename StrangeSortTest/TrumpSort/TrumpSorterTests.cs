using StrangeSort.TrumpSort;

namespace StrangeSortTest.TrumpSort;

public sealed class TrumpSorterTests
{
    [Fact]
    public void ShuffleAndWin_Array_NullValues_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => TrumpSorter.ShuffleAndWin((int[]?)null!));
    }

    [Fact]
    public void ShuffleAndWin_List_NullValues_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => TrumpSorter.ShuffleAndWin((List<int>?)null!));
    }

    [Fact]
    public void ShuffleAndWin_Array_NullRandom_Throws()
    {
        var values = new[] { 1, 2, 3 };

        Assert.Throws<ArgumentNullException>(() => TrumpSorter.ShuffleAndWin(values, random: null!));
    }

    [Fact]
    public void ShuffleAndWin_List_NullRandom_Throws()
    {
        var values = new List<int> { 1, 2, 3 };

        Assert.Throws<ArgumentNullException>(() => TrumpSorter.ShuffleAndWin(values, random: null!));
    }

    [Fact]
    public void ShuffleAndWin_Array_Empty_ReturnsEmptyResult()
    {
        var values = Array.Empty<int>();

        var result = TrumpSorter.ShuffleAndWin(values);

        Assert.Empty(result);
        Assert.Equal(values, result);
    }

    [Fact]
    public void ShuffleAndWin_List_Empty_StaysEmpty()
    {
        var values = new List<int>();

        TrumpSorter.ShuffleAndWin(values);

        Assert.Empty(values);
    }

    [Fact]
    public void ShuffleAndWin_Array_SingleElement_ReturnsEquivalentContent_WithoutMutatingInput()
    {
        var values = new[] { 42 };
        var snapshot = (int[])values.Clone();

        var result = TrumpSorter.ShuffleAndWin(values);

        Assert.Equal(snapshot, result);
        Assert.Equal(snapshot, values);
    }

    [Fact]
    public void ShuffleAndWin_List_SingleElement_LeavesContentIntact()
    {
        var values = new List<int> { 42 };

        TrumpSorter.ShuffleAndWin(values);

        Assert.Equal([42], values);
    }

    [Fact]
    public void ShuffleAndWin_Array_SameSeed_IsReproducible()
    {
        var source = new[] { 1, 2, 3, 4, 5, 6 };

        var first = TrumpSorter.ShuffleAndWin((int[])source.Clone(), new Random(2026));
        var second = TrumpSorter.ShuffleAndWin((int[])source.Clone(), new Random(2026));

        Assert.Equal(first, second);
    }

    [Fact]
    public void ShuffleAndWin_List_SameSeed_IsReproducible()
    {
        var first = new List<int> { 1, 2, 3, 4, 5, 6 };
        var second = new List<int> { 1, 2, 3, 4, 5, 6 };

        TrumpSorter.ShuffleAndWin(first, new Random(2026));
        TrumpSorter.ShuffleAndWin(second, new Random(2026));

        Assert.Equal(first, second);
    }

    [Fact]
    public void ShuffleAndWin_Array_AndList_WithSameSeed_ProduceSameContent()
    {
        var source = new[] { 9, 1, 8, 2, 7, 3, 6, 4, 5 };
        var arrayResult = TrumpSorter.ShuffleAndWin((int[])source.Clone(), new Random(777));
        var listResult = source.ToList();

        TrumpSorter.ShuffleAndWin(listResult, new Random(777));

        Assert.Equal(arrayResult, listResult);
    }

    [Fact]
    public void ShuffleAndWin_Array_ReturnsPermutation_WithoutMutatingInput()
    {
        var values = new[] { 5, 1, 4, 4, 2, 3 };
        var snapshot = (int[])values.Clone();

        var result = TrumpSorter.ShuffleAndWin(values, new Random(12345));

        Assert.Equal(snapshot.Length, result.Length);
        Assert.Equal(snapshot, values);
        Assert.NotSame(values, result);
        AssertSameMultiset(snapshot, result);
    }

    [Fact]
    public void ShuffleAndWin_List_ShufflesInPlace_AndPreservesLength()
    {
        var values = new List<int> { 5, 1, 4, 4, 2, 3 };
        var snapshot = values.ToArray();
        var sameInstance = values;

        TrumpSorter.ShuffleAndWin(values, new Random(12345));

        Assert.Same(sameInstance, values);
        Assert.Equal(snapshot.Length, values.Count);
        AssertSameMultiset(snapshot, values);
    }

    [Fact]
    public void ShuffleAndWin_DuplicateValues_PreserveTheElementMultiset()
    {
        var source = new[] { 1, 2, 2, 3, 3, 3, 4 };

        var arrayResult = TrumpSorter.ShuffleAndWin((int[])source.Clone(), new Random(31415));
        var listResult = source.ToList();
        TrumpSorter.ShuffleAndWin(listResult, new Random(31415));

        AssertSameMultiset(source, arrayResult);
        AssertSameMultiset(source, listResult);
    }

    [Fact]
    public void ShuffleAndWin_DefaultRandomOverloads_SatisfyThePublicInvariants()
    {
        var arraySource = new[] { 4, 1, 3, 2, 5 };
        var arraySnapshot = (int[])arraySource.Clone();
        var listSource = arraySnapshot.ToList();

        var arrayResult = TrumpSorter.ShuffleAndWin(arraySource);
        TrumpSorter.ShuffleAndWin(listSource);

        Assert.Equal(arraySnapshot.Length, arrayResult.Length);
        Assert.Equal(arraySnapshot.Length, listSource.Count);
        Assert.Equal(arraySnapshot, arraySource);
        AssertSameMultiset(arraySnapshot, arrayResult);
        AssertSameMultiset(arraySnapshot, listSource);
    }

    [Fact]
    public void ShuffleAndWin_Array_PropagatesRandomExceptions_WithoutMutatingInput()
    {
        var values = new[] { 3, 1, 2, 4 };
        var snapshot = (int[])values.Clone();

        Assert.Throws<InvalidOperationException>(() => TrumpSorter.ShuffleAndWin(values, new ThrowingRandom()));
        Assert.Equal(snapshot, values);
    }

    [Fact]
    public void ShuffleAndWin_List_PropagatesRandomExceptions()
    {
        var values = new List<int> { 3, 1, 2, 4 };

        Assert.Throws<InvalidOperationException>(() => TrumpSorter.ShuffleAndWin(values, new ThrowingRandom()));
    }

    private static void AssertSameMultiset(IReadOnlyList<int> expected, IReadOnlyList<int> actual)
    {
        var expectedCounts = CountValues(expected);
        var actualCounts = CountValues(actual);

        Assert.Equal(expected.Count, actual.Count);
        Assert.Equal(expectedCounts.Count, actualCounts.Count);

        foreach (var pair in expectedCounts)
        {
            Assert.True(actualCounts.TryGetValue(pair.Key, out var actualCount));
            Assert.Equal(pair.Value, actualCount);
        }
    }

    private static Dictionary<int, int> CountValues(IReadOnlyList<int> values)
    {
        var counts = new Dictionary<int, int>();

        foreach (var value in values)
        {
            counts.TryGetValue(value, out var count);
            counts[value] = count + 1;
        }

        return counts;
    }

    private sealed class ThrowingRandom : Random
    {
        protected override double Sample()
        {
            throw new InvalidOperationException("The random source failed.");
        }
    }
}
