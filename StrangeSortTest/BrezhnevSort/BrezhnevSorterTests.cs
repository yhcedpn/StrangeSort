using System.Numerics;
using StrangeSort.BrezhnevSort;

namespace StrangeSortTest.BrezhnevSort;

public sealed class BrezhnevSorterTests
{
    [Fact]
    public void RewriteAsOrdinalSequence_Array_NullValues_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => BrezhnevSorter.RewriteAsOrdinalSequence((int[]?)null!));
    }

    [Fact]
    public void RewriteAsOrdinalSequence_List_NullValues_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => BrezhnevSorter.RewriteAsOrdinalSequence((List<int>?)null!));
    }

    [Fact]
    public void RewriteAsOrdinalSequence_Array_Empty_ReturnsEmpty()
    {
        var result = BrezhnevSorter.RewriteAsOrdinalSequence(Array.Empty<int>());

        Assert.Empty(result);
    }

    [Fact]
    public void RewriteAsOrdinalSequence_List_Empty_RemainsEmpty()
    {
        var values = new List<int>();

        BrezhnevSorter.RewriteAsOrdinalSequence(values);

        Assert.Empty(values);
    }

    [Fact]
    public void RewriteAsOrdinalSequence_Array_SingleElement_ReturnsOne_WithoutMutatingInput()
    {
        var values = new[] { 999 };
        var snapshot = (int[])values.Clone();
        int[] expected = [1];

        var result = BrezhnevSorter.RewriteAsOrdinalSequence(values);

        Assert.Equal(expected, result);
        Assert.NotSame(values, result);
        Assert.Equal(snapshot, values);
    }

    [Fact]
    public void RewriteAsOrdinalSequence_List_SingleElement_RewritesToOneInPlace()
    {
        var values = new List<int> { 999 };
        int[] expected = [1];

        BrezhnevSorter.RewriteAsOrdinalSequence(values);

        Assert.Equal(expected, values);
    }

    [Fact]
    public void RewriteAsOrdinalSequence_Array_SameLengthDifferentValues_ProduceSameResult_WithoutMutatingInputs()
    {
        var first = new[] { -10, 20, 30, 40 };
        var second = new[] { 999, -3, 42, 0 };
        var firstSnapshot = (int[])first.Clone();
        var secondSnapshot = (int[])second.Clone();
        int[] expected = [1, 2, 3, 4];

        var firstResult = BrezhnevSorter.RewriteAsOrdinalSequence(first);
        var secondResult = BrezhnevSorter.RewriteAsOrdinalSequence(second);

        Assert.Equal(expected, firstResult);
        Assert.Equal(expected, secondResult);
        Assert.Equal(firstSnapshot, first);
        Assert.Equal(secondSnapshot, second);
    }

    [Fact]
    public void RewriteAsOrdinalSequence_List_SameLengthDifferentValues_ProduceSameResultInPlace()
    {
        var first = new List<int> { -10, 20, 30, 40 };
        var second = new List<int> { 999, -3, 42, 0 };
        int[] expected = [1, 2, 3, 4];

        BrezhnevSorter.RewriteAsOrdinalSequence(first);
        BrezhnevSorter.RewriteAsOrdinalSequence(second);

        Assert.Equal(expected, first);
        Assert.Equal(expected, second);
    }

    [Fact]
    public void RewriteAsOrdinalSequence_Array_IgnoresOriginalValues()
    {
        var values = new[] { 999, -3, 42 };
        var snapshot = (int[])values.Clone();
        int[] expected = [1, 2, 3];

        var result = BrezhnevSorter.RewriteAsOrdinalSequence(values);

        Assert.Equal(expected, result);
        Assert.Equal(snapshot, values);
    }

    [Fact]
    public void RewriteAsOrdinalSequence_List_IgnoresOriginalValues_InPlace()
    {
        var values = new List<int> { 999, -3, 42 };
        int[] expected = [1, 2, 3];

        BrezhnevSorter.RewriteAsOrdinalSequence(values);

        Assert.Equal(expected, values);
    }

    [Fact]
    public void RewriteAsOrdinalSequence_BigInteger_IsSupported()
    {
        var source = new[] { new BigInteger(100), new BigInteger(-3), BigInteger.Zero };
        BigInteger[] expected = [BigInteger.One, new BigInteger(2), new BigInteger(3)];

        var arrayResult = BrezhnevSorter.RewriteAsOrdinalSequence((BigInteger[])source.Clone());
        var listResult = source.ToList();

        BrezhnevSorter.RewriteAsOrdinalSequence(listResult);

        Assert.Equal(expected, arrayResult);
        Assert.Equal(expected, listResult);
    }

    [Fact]
    public void RewriteAsOrdinalSequence_DoubleAndDecimal_AreSupported()
    {
        var doubleSource = new[] { double.NaN, -3.5, double.PositiveInfinity, 1.25 };
        var decimalSource = new[] { -100m, 0m, 42m };
        double[] expectedDouble = [1d, 2d, 3d, 4d];
        decimal[] expectedDecimal = [1m, 2m, 3m];

        var doubleArrayResult = BrezhnevSorter.RewriteAsOrdinalSequence((double[])doubleSource.Clone());
        var doubleListResult = doubleSource.ToList();
        BrezhnevSorter.RewriteAsOrdinalSequence(doubleListResult);

        var decimalArrayResult = BrezhnevSorter.RewriteAsOrdinalSequence((decimal[])decimalSource.Clone());
        var decimalListResult = decimalSource.ToList();
        BrezhnevSorter.RewriteAsOrdinalSequence(decimalListResult);

        Assert.Equal(expectedDouble, doubleArrayResult);
        Assert.Equal(expectedDouble, doubleListResult);
        Assert.Equal(expectedDecimal, decimalArrayResult);
        Assert.Equal(expectedDecimal, decimalListResult);
    }

    [Fact]
    public void RewriteAsOrdinalSequence_ByteArray_Overflow_ThrowsWithoutMutatingInput()
    {
        var values = new byte[256];
        var snapshot = (byte[])values.Clone();

        Assert.Throws<OverflowException>(() => BrezhnevSorter.RewriteAsOrdinalSequence(values));
        Assert.Equal(snapshot, values);
    }

    [Fact]
    public void RewriteAsOrdinalSequence_ByteList_Overflow_Throws()
    {
        var values = new List<byte>(new byte[256]);

        Assert.Throws<OverflowException>(() => BrezhnevSorter.RewriteAsOrdinalSequence(values));
    }
}
