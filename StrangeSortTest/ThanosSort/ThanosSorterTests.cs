using StrangeSort.ThanosSort;

namespace StrangeSortTest.ThanosSort;

public sealed class ThanosSorterTests
{
    [Fact]
    public void PruneUntilSorted_Array_NullValues_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => ThanosSorter.PruneUntilSorted((int[]?)null!));
    }

    [Fact]
    public void PruneUntilSorted_List_NullValues_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => ThanosSorter.PruneUntilSorted((List<int>?)null!));
    }

    [Fact]
    public void PruneUntilSorted_Array_NullRandom_Throws()
    {
        var values = new[] { 2, 1 };

        Assert.Throws<ArgumentNullException>(() => ThanosSorter.PruneUntilSorted(values, comparer: null, RemovalCountStrategy.FloorHalf, random: null!));
    }

    [Fact]
    public void PruneUntilSorted_List_NullRandom_Throws()
    {
        var values = new List<int> { 2, 1 };

        Assert.Throws<ArgumentNullException>(() => ThanosSorter.PruneUntilSorted(values, comparer: null, RemovalCountStrategy.FloorHalf, random: null!));
    }

    [Theory]
    [InlineData((RemovalCountStrategy)(-1))]
    [InlineData((RemovalCountStrategy)99)]
    public void PruneUntilSorted_Array_InvalidRemovalStrategy_Throws(RemovalCountStrategy removalStrategy)
    {
        var values = new[] { 2, 1 };

        Assert.Throws<ArgumentOutOfRangeException>(() => ThanosSorter.PruneUntilSorted(values, comparer: null, removalStrategy, new Random(7)));
    }

    [Theory]
    [InlineData((RemovalCountStrategy)(-1))]
    [InlineData((RemovalCountStrategy)99)]
    public void PruneUntilSorted_List_InvalidRemovalStrategy_Throws(RemovalCountStrategy removalStrategy)
    {
        var values = new List<int> { 2, 1 };

        Assert.Throws<ArgumentOutOfRangeException>(() => ThanosSorter.PruneUntilSorted(values, comparer: null, removalStrategy, new Random(7)));
    }

    [Fact]
    public void PruneUntilSorted_Array_EmptyAndSingleElement_DoNotRequireDefaultComparer()
    {
        var empty = Array.Empty<NonComparable>();
        var single = new[] { new NonComparable() };

        var emptyResult = ThanosSorter.PruneUntilSorted(empty);
        var singleResult = ThanosSorter.PruneUntilSorted(single);

        Assert.Empty(emptyResult);
        Assert.Equal(single, singleResult);
    }

    [Fact]
    public void PruneUntilSorted_List_EmptyAndSingleElement_DoNotRequireDefaultComparer()
    {
        var empty = new List<NonComparable>();
        var single = new List<NonComparable> { new() };

        ThanosSorter.PruneUntilSorted(empty);
        ThanosSorter.PruneUntilSorted(single);

        Assert.Empty(empty);
        Assert.Single(single);
    }

    [Fact]
    public void PruneUntilSorted_Array_AlreadySorted_LeavesContentsIntact()
    {
        var values = new[] { 1, 1, 2, 3 };
        var snapshot = (int[])values.Clone();

        var result = ThanosSorter.PruneUntilSorted(values);

        Assert.Equal(snapshot, result);
        Assert.Equal(snapshot, values);
        AssertOrdered(result);
        AssertSubsequence(result, snapshot);
    }

    [Fact]
    public void PruneUntilSorted_List_AlreadySorted_LeavesContentsIntact()
    {
        var values = new List<int> { 1, 1, 2, 3 };
        var snapshot = values.ToArray();

        ThanosSorter.PruneUntilSorted(values);

        Assert.Equal(snapshot, values);
        AssertOrdered(values);
        AssertSubsequence(values, snapshot);
    }

    [Fact]
    public void PruneUntilSorted_Array_Unsorted_ResultIsAnOrderedSubsequence_AndInputIsUnchanged()
    {
        var values = new[] { 5, 1, 4, 4, 2, 3 };
        var snapshot = (int[])values.Clone();

        var result = ThanosSorter.PruneUntilSorted(values, comparer: null, RemovalCountStrategy.CeilingHalf, new Random(12345));

        AssertOrdered(result);
        AssertSubsequence(result, snapshot);
        Assert.Equal(snapshot, values);
        Assert.True(result.Length < values.Length);
    }

    [Fact]
    public void PruneUntilSorted_List_Unsorted_ResultIsAnOrderedSubsequence_InPlace()
    {
        var values = new List<int> { 5, 1, 4, 4, 2, 3 };
        var snapshot = values.ToArray();

        ThanosSorter.PruneUntilSorted(values, comparer: null, RemovalCountStrategy.CeilingHalf, new Random(12345));

        AssertOrdered(values);
        AssertSubsequence(values, snapshot);
        Assert.True(values.Count < snapshot.Length);
    }

    [Fact]
    public void PruneUntilSorted_NullComparer_MatchesDefaultComparer()
    {
        var source = new[] { 8, 3, 6, 1, 5, 2, 7, 4 };

        var arrayWithNullComparer = ThanosSorter.PruneUntilSorted((int[])source.Clone(), comparer: null, RemovalCountStrategy.FloorHalf, new Random(42));
        var arrayWithDefaultComparer = ThanosSorter.PruneUntilSorted((int[])source.Clone(), Comparer<int>.Default, RemovalCountStrategy.FloorHalf, new Random(42));

        var listWithNullComparer = source.ToList();
        var listWithDefaultComparer = source.ToList();
        ThanosSorter.PruneUntilSorted(listWithNullComparer, comparer: null, RemovalCountStrategy.FloorHalf, new Random(42));
        ThanosSorter.PruneUntilSorted(listWithDefaultComparer, Comparer<int>.Default, RemovalCountStrategy.FloorHalf, new Random(42));

        Assert.Equal(arrayWithDefaultComparer, arrayWithNullComparer);
        Assert.Equal(listWithDefaultComparer, listWithNullComparer);
    }

    [Fact]
    public void PruneUntilSorted_FloorAndCeilingHalf_CanProduceDifferentResultsOnOddLengthInput()
    {
        var values = new[] { 3, 1, 2 };
        int[] expectedFloorResult = [1, 2];
        int[] expectedCeilingResult = [2];

        var floorResult = ThanosSorter.PruneUntilSorted((int[])values.Clone(), comparer: null, RemovalCountStrategy.FloorHalf, new ZeroSampleRandom());
        var ceilingResult = ThanosSorter.PruneUntilSorted((int[])values.Clone(), comparer: null, RemovalCountStrategy.CeilingHalf, new ZeroSampleRandom());

        Assert.Equal(expectedFloorResult, floorResult);
        Assert.Equal(expectedCeilingResult, ceilingResult);
        AssertOrdered(floorResult);
        AssertOrdered(ceilingResult);
        AssertSubsequence(floorResult, values);
        AssertSubsequence(ceilingResult, values);
    }

    [Fact]
    public void PruneUntilSorted_Array_AndList_WithSameSeed_ProduceSameContent()
    {
        var source = new[] { 9, 1, 8, 2, 7, 3, 6, 4, 5 };
        var arrayResult = ThanosSorter.PruneUntilSorted((int[])source.Clone(), comparer: null, RemovalCountStrategy.CeilingHalf, new Random(777));
        var listResult = source.ToList();

        ThanosSorter.PruneUntilSorted(listResult, comparer: null, RemovalCountStrategy.CeilingHalf, new Random(777));

        Assert.Equal(arrayResult, listResult);
    }

    [Fact]
    public void PruneUntilSorted_Array_SameSeed_IsReproducible()
    {
        var source = new[] { 7, 2, 6, 1, 5, 4, 3 };

        var first = ThanosSorter.PruneUntilSorted((int[])source.Clone(), comparer: null, RemovalCountStrategy.FloorHalf, new Random(2026));
        var second = ThanosSorter.PruneUntilSorted((int[])source.Clone(), comparer: null, RemovalCountStrategy.FloorHalf, new Random(2026));

        Assert.Equal(first, second);
    }

    [Fact]
    public void PruneUntilSorted_List_SameSeed_IsReproducible()
    {
        var first = new List<int> { 7, 2, 6, 1, 5, 4, 3 };
        var second = new List<int> { 7, 2, 6, 1, 5, 4, 3 };

        ThanosSorter.PruneUntilSorted(first, comparer: null, RemovalCountStrategy.FloorHalf, new Random(2026));
        ThanosSorter.PruneUntilSorted(second, comparer: null, RemovalCountStrategy.FloorHalf, new Random(2026));

        Assert.Equal(first, second);
    }

    [Fact]
    public void PruneUntilSorted_CustomComparer_CanDefineAlternativeOrder()
    {
        var comparer = Comparer<string>.Create((left, right) => left.Length.CompareTo(right.Length));
        var source = new[] { "pear", "a", "banana", "kiwi", "bb" };

        var arrayResult = ThanosSorter.PruneUntilSorted((string[])source.Clone(), comparer, RemovalCountStrategy.FloorHalf, new Random(314));
        var listResult = source.ToList();
        ThanosSorter.PruneUntilSorted(listResult, comparer, RemovalCountStrategy.FloorHalf, new Random(314));

        AssertOrdered(arrayResult, comparer);
        AssertOrdered(listResult, comparer);
        AssertSubsequence(arrayResult, source);
        AssertSubsequence(listResult, source);
    }

    [Fact]
    public void PruneUntilSorted_ReverseComparer_CanProduceDescendingSubsequence()
    {
        var comparer = Comparer<int>.Create((left, right) => right.CompareTo(left));
        var source = new[] { 2, 5, 1, 4, 3 };

        var arrayResult = ThanosSorter.PruneUntilSorted((int[])source.Clone(), comparer, RemovalCountStrategy.FloorHalf, new Random(11));
        var listResult = source.ToList();
        ThanosSorter.PruneUntilSorted(listResult, comparer, RemovalCountStrategy.FloorHalf, new Random(11));

        AssertOrdered(arrayResult, comparer);
        AssertOrdered(listResult, comparer);
        AssertSubsequence(arrayResult, source);
        AssertSubsequence(listResult, source);
    }

    [Fact]
    public void PruneUntilSorted_DefaultComparer_ThrowsOnlyWhenComparisonIsRequired()
    {
        var pairArray = new[] { new NonComparable(), new NonComparable() };
        var pairList = new List<NonComparable> { new(), new() };

        Assert.Throws<ArgumentException>(() => ThanosSorter.PruneUntilSorted(pairArray));
        Assert.Throws<ArgumentException>(() => ThanosSorter.PruneUntilSorted(pairList));
    }

    [Fact]
    public void PruneUntilSorted_Array_PropagatesRandomExceptions_WithoutMutatingInput()
    {
        var values = new[] { 3, 1, 2 };
        var snapshot = (int[])values.Clone();

        Assert.Throws<InvalidOperationException>(() => ThanosSorter.PruneUntilSorted(values, comparer: null, RemovalCountStrategy.FloorHalf, new ThrowingRandom()));
        Assert.Equal(snapshot, values);
    }

    [Fact]
    public void PruneUntilSorted_Array_PropagatesComparerExceptions_WithoutMutatingInput()
    {
        var values = new[] { 3, 1, 2 };
        var snapshot = (int[])values.Clone();
        var expectedException = new InvalidOperationException("The comparer failed.");

        var actualException = Assert.Throws<InvalidOperationException>(() => ThanosSorter.PruneUntilSorted(values, CreateComparerThatThrowsOnThirdComparison(expectedException), RemovalCountStrategy.FloorHalf, new ZeroSampleRandom()));

        Assert.Same(expectedException, actualException);
        Assert.Equal(snapshot, values);
    }

    [Fact]
    public void PruneUntilSorted_List_PropagatesRandomExceptions()
    {
        var values = new List<int> { 3, 1, 2 };

        Assert.Throws<InvalidOperationException>(() => ThanosSorter.PruneUntilSorted(values, comparer: null, RemovalCountStrategy.FloorHalf, new ThrowingRandom()));
    }

    [Fact]
    public void PruneUntilSorted_List_PropagatesComparerExceptions()
    {
        var values = new List<int> { 3, 1, 2 };
        var expectedException = new InvalidOperationException("The comparer failed.");

        var actualException = Assert.Throws<InvalidOperationException>(() => ThanosSorter.PruneUntilSorted(values, CreateComparerThatThrowsOnThirdComparison(expectedException), RemovalCountStrategy.FloorHalf, new ZeroSampleRandom()));

        Assert.Same(expectedException, actualException);
    }

    [Fact]
    public void PruneUntilSorted_DefaultRandomOverloads_SatisfyThePublicInvariants()
    {
        var arraySource = new[] { 4, 1, 3, 2, 5 };
        var arraySnapshot = (int[])arraySource.Clone();
        var listSource = arraySnapshot.ToList();

        var arrayResult = ThanosSorter.PruneUntilSorted(arraySource);
        ThanosSorter.PruneUntilSorted(listSource);

        AssertOrdered(arrayResult);
        AssertOrdered(listSource);
        AssertSubsequence(arrayResult, arraySnapshot);
        AssertSubsequence(listSource, arraySnapshot);
        Assert.Equal(arraySnapshot, arraySource);
    }

    private static void AssertOrdered<T>(IReadOnlyList<T> values, IComparer<T>? comparer = null)
    {
        comparer ??= Comparer<T>.Default;

        for (var index = 1; index < values.Count; index++)
        {
            Assert.True(comparer.Compare(values[index - 1], values[index]) <= 0);
        }
    }

    private static void AssertSubsequence<T>(IReadOnlyList<T> subsequence, IReadOnlyList<T> source)
    {
        var sourceIndex = 0;
        var equalityComparer = EqualityComparer<T>.Default;

        foreach (var item in subsequence)
        {
            while (sourceIndex < source.Count && !equalityComparer.Equals(source[sourceIndex], item))
            {
                sourceIndex++;
            }

            Assert.True(sourceIndex < source.Count);
            sourceIndex++;
        }
    }

    private static Comparer<int> CreateComparerThatThrowsOnThirdComparison(Exception exception)
    {
        var compareCount = 0;

        return Comparer<int>.Create((left, right) =>
        {
            compareCount++;
            if (compareCount == 3)
            {
                throw exception;
            }

            return left.CompareTo(right);
        });
    }

    private sealed class NonComparable
    {
    }

    private sealed class ZeroSampleRandom : Random
    {
        protected override double Sample()
        {
            return 0.0;
        }
    }

    private sealed class ThrowingRandom : Random
    {
        protected override double Sample()
        {
            throw new InvalidOperationException("The random source failed.");
        }
    }
}
