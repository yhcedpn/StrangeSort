using StrangeSort.StalinSort;

namespace StrangeSortTest.StalinSort;

public sealed class StalinSorterTests
{
    [Fact]
    public void RemoveOutOfOrder_Array_NullValues_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => StalinSorter.RemoveOutOfOrder((int[]?)null!));
    }

    [Fact]
    public void RemoveOutOfOrder_List_NullValues_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => StalinSorter.RemoveOutOfOrder((List<int>?)null!));
    }

    [Fact]
    public void RemoveOutOfOrder_Array_EmptyAndSingleElement_DoNotRequireDefaultComparer()
    {
        var empty = Array.Empty<NonComparable>();
        var single = new[] { new NonComparable() };

        var emptyResult = StalinSorter.RemoveOutOfOrder(empty);
        var singleResult = StalinSorter.RemoveOutOfOrder(single);

        Assert.Empty(emptyResult);
        Assert.Equal(single, singleResult);
    }

    [Fact]
    public void RemoveOutOfOrder_List_EmptyAndSingleElement_DoNotRequireDefaultComparer()
    {
        var empty = new List<NonComparable>();
        var single = new List<NonComparable> { new() };

        StalinSorter.RemoveOutOfOrder(empty);
        StalinSorter.RemoveOutOfOrder(single);

        Assert.Empty(empty);
        Assert.Single(single);
    }

    [Fact]
    public void RemoveOutOfOrder_Array_AlreadyOrdered_ReturnsEquivalentContent_WithoutMutatingInput()
    {
        var values = new[] { 1, 1, 2, 3 };
        var snapshot = (int[])values.Clone();

        var result = StalinSorter.RemoveOutOfOrder(values);

        Assert.Equal(snapshot, result);
        Assert.Equal(snapshot, values);
        Assert.NotSame(values, result);
    }

    [Fact]
    public void RemoveOutOfOrder_List_AlreadyOrdered_LeavesContentsIntact()
    {
        var values = new List<int> { 1, 1, 2, 3 };
        var snapshot = values.ToArray();

        StalinSorter.RemoveOutOfOrder(values);

        Assert.Equal(snapshot, values);
    }

    [Fact]
    public void RemoveOutOfOrder_Array_Unordered_ResultIsOrderedSubsequence_AndInputIsUnchanged()
    {
        var values = new[] { 2, 1, 3, 2, 4 };
        var snapshot = (int[])values.Clone();

        var result = StalinSorter.RemoveOutOfOrder(values);

        Assert.Equal(new[] { 2, 3, 4 }, result);
        Assert.Equal(snapshot, values);
        AssertOrdered(result);
        AssertSubsequence(result, snapshot);
    }

    [Fact]
    public void RemoveOutOfOrder_List_Unordered_ResultIsOrderedSubsequence_InPlace()
    {
        var values = new List<int> { 2, 1, 3, 2, 4 };
        var snapshot = values.ToArray();

        StalinSorter.RemoveOutOfOrder(values);

        Assert.Equal(new[] { 2, 3, 4 }, values);
        AssertOrdered(values);
        AssertSubsequence(values, snapshot);
    }

    [Fact]
    public void RemoveOutOfOrder_NullComparer_MatchesDefaultComparer()
    {
        var source = new[] { 3, 1, 2, 2, 4 };

        var arrayWithNullComparer = StalinSorter.RemoveOutOfOrder((int[])source.Clone(), comparer: null);
        var arrayWithDefaultComparer = StalinSorter.RemoveOutOfOrder((int[])source.Clone(), Comparer<int>.Default);

        var listWithNullComparer = source.ToList();
        var listWithDefaultComparer = source.ToList();
        StalinSorter.RemoveOutOfOrder(listWithNullComparer, comparer: null);
        StalinSorter.RemoveOutOfOrder(listWithDefaultComparer, Comparer<int>.Default);

        Assert.Equal(arrayWithDefaultComparer, arrayWithNullComparer);
        Assert.Equal(listWithDefaultComparer, listWithNullComparer);
    }

    [Fact]
    public void RemoveOutOfOrder_CustomComparer_CanDefineAlternativeOrder()
    {
        var comparer = Comparer<string>.Create((left, right) => left.Length.CompareTo(right.Length));
        var source = new[] { "bb", "a", "cc", "dddd", "eee" };
        var expected = new[] { "bb", "cc", "dddd" };

        var arrayResult = StalinSorter.RemoveOutOfOrder((string[])source.Clone(), comparer);
        var listResult = source.ToList();
        StalinSorter.RemoveOutOfOrder(listResult, comparer);

        Assert.Equal(expected, arrayResult);
        Assert.Equal(expected, listResult);
        AssertOrdered(arrayResult, comparer);
        AssertOrdered(listResult, comparer);
    }

    [Fact]
    public void RemoveOutOfOrder_ReverseComparer_CanProduceDescendingSubsequence()
    {
        var comparer = Comparer<int>.Create((left, right) => right.CompareTo(left));
        var source = new[] { 5, 4, 3, 6, 2, 1 };
        var expected = new[] { 5, 4, 3, 2, 1 };

        var arrayResult = StalinSorter.RemoveOutOfOrder((int[])source.Clone(), comparer);
        var listResult = source.ToList();
        StalinSorter.RemoveOutOfOrder(listResult, comparer);

        Assert.Equal(expected, arrayResult);
        Assert.Equal(expected, listResult);
        AssertOrdered(arrayResult, comparer);
        AssertOrdered(listResult, comparer);
    }

    [Fact]
    public void RemoveOutOfOrder_DefaultComparer_ThrowsOnlyWhenComparisonIsRequired()
    {
        var pairArray = new[] { new NonComparable(), new NonComparable() };
        var pairList = new List<NonComparable> { new(), new() };

        Assert.Throws<ArgumentException>(() => StalinSorter.RemoveOutOfOrder(pairArray));
        Assert.Throws<ArgumentException>(() => StalinSorter.RemoveOutOfOrder(pairList));
    }

    [Fact]
    public void RemoveOutOfOrder_Array_ComparerExceptions_ArePropagated_WithoutMutatingInput()
    {
        var values = new[] { 2, 1, 3, 0 };
        var snapshot = (int[])values.Clone();
        var expectedException = new InvalidOperationException("The comparer failed.");

        var actualException = Assert.Throws<InvalidOperationException>(() => StalinSorter.RemoveOutOfOrder(values, CreateComparerThatThrowsOnThirdComparison(expectedException)));

        Assert.Same(expectedException, actualException);
        Assert.Equal(snapshot, values);
    }

    [Fact]
    public void RemoveOutOfOrder_List_ComparerExceptions_ArePropagated()
    {
        var values = new List<int> { 2, 1, 3, 0 };
        var expectedException = new InvalidOperationException("The comparer failed.");

        var actualException = Assert.Throws<InvalidOperationException>(() => StalinSorter.RemoveOutOfOrder(values, CreateComparerThatThrowsOnThirdComparison(expectedException)));

        Assert.Same(expectedException, actualException);
    }

    [Fact]
    public void RemoveOutOfOrder_Semantics_ThreeOneTwo_MustReturnThree()
    {
        var source = new[] { 3, 1, 2 };
        var arrayResult = StalinSorter.RemoveOutOfOrder((int[])source.Clone());
        var listResult = source.ToList();

        StalinSorter.RemoveOutOfOrder(listResult);

        Assert.Equal(new[] { 3 }, arrayResult);
        Assert.Equal(new[] { 3 }, listResult);
    }

    [Fact]
    public void RemoveOutOfOrder_Semantics_OneThreeTwoFour_MustReturnOneThreeFour()
    {
        var source = new[] { 1, 3, 2, 4 };
        var arrayResult = StalinSorter.RemoveOutOfOrder((int[])source.Clone());
        var listResult = source.ToList();

        StalinSorter.RemoveOutOfOrder(listResult);

        Assert.Equal(new[] { 1, 3, 4 }, arrayResult);
        Assert.Equal(new[] { 1, 3, 4 }, listResult);
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

    private static IComparer<int> CreateComparerThatThrowsOnThirdComparison(Exception exception)
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
}
