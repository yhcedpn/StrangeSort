using System.Numerics;
using StrangeSort.EpsteinSort;

namespace StrangeSortTest.EpsteinSort;

public sealed class EpsteinSorterTests
{
    [Fact]
    public void RetainOnlyMinorsAndSort_Array_NullValues_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => EpsteinSorter.RetainOnlyMinorsAndSort((int[]?)null!));
    }

    [Fact]
    public void RetainOnlyMinorsAndSort_List_NullValues_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => EpsteinSorter.RetainOnlyMinorsAndSort((List<int>?)null!));
    }

    [Fact]
    public void RetainOnlyMinorsAndSort_Array_Empty_ReturnsEmpty()
    {
        var result = EpsteinSorter.RetainOnlyMinorsAndSort(Array.Empty<int>());

        Assert.Empty(result);
    }

    [Fact]
    public void RetainOnlyMinorsAndSort_List_Empty_RemainsEmpty()
    {
        var values = new List<int>();

        EpsteinSorter.RetainOnlyMinorsAndSort(values);

        Assert.Empty(values);
    }

    [Fact]
    public void RetainOnlyMinorsAndSort_Array_SingleElement_RespectsFilteringWithoutMutatingInput()
    {
        var retainedValues = new[] { 7 };
        var retainedSnapshot = (int[])retainedValues.Clone();
        var removedValues = new[] { 18 };
        var removedSnapshot = (int[])removedValues.Clone();

        var retainedResult = EpsteinSorter.RetainOnlyMinorsAndSort(retainedValues);
        var removedResult = EpsteinSorter.RetainOnlyMinorsAndSort(removedValues);

        Assert.Equal(new[] { 7 }, retainedResult);
        Assert.NotSame(retainedValues, retainedResult);
        Assert.Equal(retainedSnapshot, retainedValues);

        Assert.Empty(removedResult);
        Assert.Equal(removedSnapshot, removedValues);
    }

    [Fact]
    public void RetainOnlyMinorsAndSort_List_SingleElement_AppliesFilteringInPlace()
    {
        var retainedValues = new List<int> { 7 };
        var removedValues = new List<int> { -1 };

        EpsteinSorter.RetainOnlyMinorsAndSort(retainedValues);
        EpsteinSorter.RetainOnlyMinorsAndSort(removedValues);

        Assert.Equal(new[] { 7 }, retainedValues);
        Assert.Empty(removedValues);
    }

    [Fact]
    public void RetainOnlyMinorsAndSort_Array_BoundariesAndSorting_AreApplied_WithoutMutatingInput()
    {
        var values = new[] { 18, 0, -5, 17, 5, 18, 2, -1 };
        var snapshot = (int[])values.Clone();

        var result = EpsteinSorter.RetainOnlyMinorsAndSort(values);

        Assert.Equal(new[] { 0, 2, 5, 17 }, result);
        Assert.NotSame(values, result);
        Assert.Equal(snapshot, values);
        AssertOrdered(result);
    }

    [Fact]
    public void RetainOnlyMinorsAndSort_List_BoundariesAndSorting_AreAppliedInPlace()
    {
        var values = new List<int> { 18, 0, -5, 17, 5, 18, 2, -1 };

        EpsteinSorter.RetainOnlyMinorsAndSort(values);

        Assert.Equal(new[] { 0, 2, 5, 17 }, values);
        AssertOrdered(values);
    }

    [Fact]
    public void RetainOnlyMinorsAndSort_NullComparer_MatchesDefaultComparer()
    {
        var source = new[] { 20, 3, 1, 18, 2, -1 };

        var arrayWithNullComparer = EpsteinSorter.RetainOnlyMinorsAndSort((int[])source.Clone(), comparer: null);
        var arrayWithDefaultComparer = EpsteinSorter.RetainOnlyMinorsAndSort((int[])source.Clone(), Comparer<int>.Default);

        var listWithNullComparer = source.ToList();
        var listWithDefaultComparer = source.ToList();
        EpsteinSorter.RetainOnlyMinorsAndSort(listWithNullComparer, comparer: null);
        EpsteinSorter.RetainOnlyMinorsAndSort(listWithDefaultComparer, Comparer<int>.Default);

        Assert.Equal(arrayWithDefaultComparer, arrayWithNullComparer);
        Assert.Equal(listWithDefaultComparer, listWithNullComparer);
    }

    [Fact]
    public void RetainOnlyMinorsAndSort_CustomComparer_CanChangeSortOrder()
    {
        var comparer = Comparer<int>.Create((left, right) =>
        {
            var parityComparison = (left % 2).CompareTo(right % 2);
            return parityComparison != 0 ? parityComparison : left.CompareTo(right);
        });
        var source = new[] { 7, 2, 5, 4, 1, 16, 21, -3 };
        var expected = new[] { 2, 4, 16, 1, 5, 7 };

        var arrayResult = EpsteinSorter.RetainOnlyMinorsAndSort((int[])source.Clone(), comparer);
        var listResult = source.ToList();
        EpsteinSorter.RetainOnlyMinorsAndSort(listResult, comparer);

        Assert.Equal(expected, arrayResult);
        Assert.Equal(expected, listResult);
        AssertOrdered(arrayResult, comparer);
        AssertOrdered(listResult, comparer);
    }

    [Fact]
    public void RetainOnlyMinorsAndSort_ReverseComparer_DoesNotChangeFilteringRange()
    {
        var comparer = Comparer<int>.Create((left, right) => right.CompareTo(left));
        var source = new[] { -1, 2, 18, 5, 19, 0, 17 };
        var expected = new[] { 17, 5, 2, 0 };

        var arrayResult = EpsteinSorter.RetainOnlyMinorsAndSort((int[])source.Clone(), comparer);
        var listResult = source.ToList();
        EpsteinSorter.RetainOnlyMinorsAndSort(listResult, comparer);

        Assert.Equal(expected, arrayResult);
        Assert.Equal(expected, listResult);
        AssertOrdered(arrayResult, comparer);
        AssertOrdered(listResult, comparer);
    }

    [Fact]
    public void RetainOnlyMinorsAndSort_Double_SpecialValues_AreRemoved()
    {
        var source = new[] { double.NaN, 3.5, double.PositiveInfinity, -1.0, 0.0, 18.0, double.NegativeInfinity, 17.25 };
        var snapshot = (double[])source.Clone();
        var expected = new[] { 0.0, 3.5, 17.25 };

        var arrayResult = EpsteinSorter.RetainOnlyMinorsAndSort(source);
        var listResult = source.ToList();
        EpsteinSorter.RetainOnlyMinorsAndSort(listResult);

        Assert.Equal(expected, arrayResult);
        Assert.Equal(expected, listResult);
        Assert.Equal(snapshot, source);
    }

    [Fact]
    public void RetainOnlyMinorsAndSort_BigInteger_IsSupported()
    {
        var source = new[] { new BigInteger(17), new BigInteger(-1), new BigInteger(5), new BigInteger(18), BigInteger.Zero };
        var expected = new[] { BigInteger.Zero, new BigInteger(5), new BigInteger(17) };

        var arrayResult = EpsteinSorter.RetainOnlyMinorsAndSort((BigInteger[])source.Clone());
        var listResult = source.ToList();
        EpsteinSorter.RetainOnlyMinorsAndSort(listResult);

        Assert.Equal(expected, arrayResult);
        Assert.Equal(expected, listResult);
    }

    [Fact]
    public void RetainOnlyMinorsAndSort_Array_ComparerExceptions_ArePropagated_WithoutMutatingInput()
    {
        var values = new[] { 20, 2, 1, -1 };
        var snapshot = (int[])values.Clone();
        var expectedException = new InvalidOperationException("The comparer failed.");

        var actualException = Assert.Throws<InvalidOperationException>(() => EpsteinSorter.RetainOnlyMinorsAndSort(values, CreateComparerThatThrowsOnFirstComparison(expectedException)));

        Assert.Same(expectedException, actualException.InnerException);
        Assert.Equal(snapshot, values);
    }

    [Fact]
    public void RetainOnlyMinorsAndSort_List_ComparerExceptions_ArePropagated_AndFilteringMayAlreadyBeApplied()
    {
        var values = new List<int> { 20, 2, 1, -1 };
        var expectedException = new InvalidOperationException("The comparer failed.");

        var actualException = Assert.Throws<InvalidOperationException>(() => EpsteinSorter.RetainOnlyMinorsAndSort(values, CreateComparerThatThrowsOnFirstComparison(expectedException)));

        Assert.Same(expectedException, actualException.InnerException);
        Assert.Equal(2, values.Count);
        Assert.All(values, value => Assert.InRange(value, 0, 17));
    }

    private static void AssertOrdered<T>(IReadOnlyList<T> values, IComparer<T>? comparer = null)
    {
        comparer ??= Comparer<T>.Default;

        for (var index = 1; index < values.Count; index++)
        {
            Assert.True(comparer.Compare(values[index - 1], values[index]) <= 0);
        }
    }

    private static IComparer<int> CreateComparerThatThrowsOnFirstComparison(Exception exception)
    {
        var hasThrown = false;

        return Comparer<int>.Create((left, right) =>
        {
            if (!hasThrown)
            {
                hasThrown = true;
                throw exception;
            }

            return left.CompareTo(right);
        });
    }
}
