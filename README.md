# StrangeSort

`StrangeSort` 是一个基于 .NET 10 的 C# 类库，只收录和实现几种冠以**经典人物的名字**的奇异算法。

这个仓库当前包含 4 组公开算法：

- `StalinSort`：从左到右删除违序元素，保留一个有序子序列。大力清洗所有不符合要求的元素！
- `ThanosSort`：随机删除当前序列的一半元素，重复执行直到序列有序。一键清除多余元素，剩下多少你别问，嘿嘿。
- `EpsteinSort`：先保留数值区间 `[0, 18)` 内的元素，再对保留元素排序。只保留未成年数字，太老的全部斩杀！
- `BrezhnevSort`：无视输入值本身，只根据输入长度把结果改写为从 `1` 到 `n` 的序数序列。内容不重要，排场最重要！

## 作者的话

如果你想添加更多有趣的算法，可以提出 issue 告诉我，或提出 Pull request 以直接添加它！

## 项目概览

- 目标运行时：`.NET 10`
- 核心项目：`StrangeSort`
- 测试项目：`StrangeSortTest`
- 核心库开启了 AOT 兼容性检查，测试项目开启了 AOT 发布

## 如何引用

如果你在同一个解决方案中使用它，直接给你的项目添加对 `StrangeSort/StrangeSort.csproj` 的项目引用即可。

调用前通常需要引入对应命名空间：

```csharp
using StrangeSort.StalinSort;
using StrangeSort.ThanosSort;
using StrangeSort.EpsteinSort;
using StrangeSort.BrezhnevSort;
```

## 算法总览

| 算法 | 命名空间 | 核心行为 | 是否重排元素 | 是否依赖随机性 |
| --- | --- | --- | --- | --- |
| StalinSort | `StrangeSort.StalinSort` | 从左到右删除相对最近保留元素违序的元素 | 否，只删除 | 否 |
| ThanosSort | `StrangeSort.ThanosSort` | 每轮随机删除一半元素，直到结果有序 | 否，只删除 | 是 |
| EpsteinSort | `StrangeSort.EpsteinSort` | 先过滤到 `[0, 18)`，再排序 | 是，会排序保留元素 | 否 |
| BrezhnevSort | `StrangeSort.BrezhnevSort` | 无视原值，只按输入长度生成 `1..n` | 否，不重排，但会重写所有值 | 否 |

## StalinSort

### 算法说明

`StalinSort` 不是传统排序算法。它不会交换元素位置，而是从左到右扫描：

- 第一个元素总是保留。
- 后续元素如果满足 `comparer.Compare(lastKept, current) <= 0`，就保留。
- 否则直接删除。

因此输出结果一定是输入序列的一个子序列，并且保留元素的相对顺序不变。

例如：

- 输入：`[2, 1, 3, 2, 4]`
- 输出：`[2, 3, 4]`

### 公共 API

```csharp
public static T[] RemoveOutOfOrder<T>(T[] values)
public static T[] RemoveOutOfOrder<T>(T[] values, IComparer<T>? comparer)
public static void RemoveOutOfOrder<T>(List<T> values)
public static void RemoveOutOfOrder<T>(List<T> values, IComparer<T>? comparer)
```

### 输入与输出

数组重载：

- 输入：`T[] values`
- 可选输入：`IComparer<T>? comparer`
- 输出：`T[]`
- 行为：不修改输入数组，返回一个有序子序列
- 特别说明：
  - 当 `values.Length <= 1` 时，直接返回输入数组引用
  - 当输入已经有序时，返回一个内容等价的新数组，不会改动原数组
  - 当 `comparer == null` 时，使用 `Comparer<T>.Default`

`List<T>` 重载：

- 输入：`List<T> values`
- 可选输入：`IComparer<T>? comparer`
- 输出：无返回值，结果直接写回原列表
- 行为：原地删除违序元素
- 当 `comparer == null` 时，使用 `Comparer<T>.Default`

### 异常

所有重载都可能出现：

- `ArgumentNullException`
  - `values == null`

默认比较器相关异常：

- `ArgumentException`
  - 当 `comparer == null`，且 `Comparer<T>.Default` 无法比较 `T`，并且运行时确实需要比较时抛出
  - 典型情况：`T` 没有默认可比性，且输入至少需要做一次比较
  - 不会触发该异常的情况：空数组、单元素数组、空列表、单元素列表

自定义比较器相关异常：

- 比较器自己抛出的异常会继续向外传播
- 对数组重载，输入数组不会被修改
- 对 `List<T>` 重载，如果比较器在处理中途抛出异常，列表可能已经被部分修改，且不会回滚

### 调用示例

```csharp
var values = new[] { 2, 1, 3, 2, 4 };
var result = StalinSorter.RemoveOutOfOrder(values);
// result = [2, 3, 4]
// values 保持不变

var list = new List<int> { 5, 4, 3, 6, 2, 1 };
var descendingComparer = Comparer<int>.Create((left, right) => right.CompareTo(left));
StalinSorter.RemoveOutOfOrder(list, descendingComparer);
// list 可能变成 [5, 4, 3, 2, 1]
```

## ThanosSort

### 算法说明

`ThanosSort` 同样不是传统排序。它的行为是：

- 先判断当前序列是否已经有序
- 如果无序，就随机删除当前序列的一半元素
- 重复以上过程，直到结果有序

保留下来的元素仍然保持原有相对顺序，因此输出仍然是输入序列的一个有序子序列。

当当前长度为奇数时，每轮删除多少个元素由 `RemovalCountStrategy` 控制：

- `RemovalCountStrategy.FloorHalf = 0`：删除 `count / 2`
- `RemovalCountStrategy.CeilingHalf = 1`：删除 `(count + 1) / 2`

### 公共 API

```csharp
public static T[] PruneUntilSorted<T>(T[] values)
public static T[] PruneUntilSorted<T>(T[] values, IComparer<T>? comparer)
public static T[] PruneUntilSorted<T>(T[] values, IComparer<T>? comparer, RemovalCountStrategy removalStrategy)
public static T[] PruneUntilSorted<T>(T[] values, IComparer<T>? comparer, RemovalCountStrategy removalStrategy, Random random)

public static void PruneUntilSorted<T>(List<T> values)
public static void PruneUntilSorted<T>(List<T> values, IComparer<T>? comparer)
public static void PruneUntilSorted<T>(List<T> values, IComparer<T>? comparer, RemovalCountStrategy removalStrategy)
public static void PruneUntilSorted<T>(List<T> values, IComparer<T>? comparer, RemovalCountStrategy removalStrategy, Random random)
```

### 输入与输出

数组重载：

- 输入：`T[] values`
- 可选输入：`IComparer<T>? comparer`、`RemovalCountStrategy removalStrategy`、`Random random`
- 输出：`T[]`
- 行为：不修改输入数组，返回一个有序子序列
- 默认值：
  - `comparer == null` 时使用 `Comparer<T>.Default`
  - 默认删除策略是 `RemovalCountStrategy.FloorHalf`
  - 默认随机源是 `Random.Shared`
- 特别说明：
  - 当输入长度 `<= 1` 时，直接返回输入数组引用
  - 当输入本身已经有序时，直接返回输入数组引用
  - 使用同一种随机源种子时，数组重载和列表重载会得到可复现的一致内容

`List<T>` 重载：

- 输入：`List<T> values`
- 可选输入：`IComparer<T>? comparer`、`RemovalCountStrategy removalStrategy`、`Random random`
- 输出：无返回值，结果直接写回原列表
- 行为：原地随机删除元素，直到列表有序
- 默认值：
  - `comparer == null` 时使用 `Comparer<T>.Default`
  - 默认删除策略是 `RemovalCountStrategy.FloorHalf`
  - 默认随机源是 `Random.Shared`

### 异常

所有重载都可能出现：

- `ArgumentNullException`
  - `values == null`

带 `random` 参数的重载还可能出现：

- `ArgumentNullException`
  - `random == null`

带 `removalStrategy` 参数的重载还可能出现：

- `ArgumentOutOfRangeException`
  - `removalStrategy` 不是已定义值
  - 当前只接受以下取值：
    - `RemovalCountStrategy.FloorHalf`，底层值为 `0`
    - `RemovalCountStrategy.CeilingHalf`，底层值为 `1`
  - 例如 `(RemovalCountStrategy)(-1)`、`(RemovalCountStrategy)99` 会触发该异常

默认比较器相关异常：

- `ArgumentException`
  - 当 `comparer == null`，且 `Comparer<T>.Default` 无法比较 `T`，并且运行时确实需要比较时抛出
  - 不会触发该异常的情况：空输入、单元素输入

自定义比较器或随机源相关异常：

- 比较器抛出的异常会直接向外传播
- 随机源抛出的异常也会直接向外传播
- 对数组重载，输入数组不会被修改
- 对 `List<T>` 重载，如果在处理中途抛出异常，列表可能已经被部分修改，且不会回滚

### 调用示例

```csharp
var values = new[] { 5, 1, 4, 4, 2, 3 };
var result = ThanosSorter.PruneUntilSorted(
    values,
    comparer: null,
    removalStrategy: RemovalCountStrategy.CeilingHalf,
    random: new Random(12345));

// result 是 values 的一个有序子序列
// values 保持不变

var list = new List<int> { 3, 1, 2 };
ThanosSorter.PruneUntilSorted(
    list,
    comparer: null,
    removalStrategy: RemovalCountStrategy.FloorHalf,
    random: new Random(2026));
// list 被原地裁剪为一个有序子序列
```

## EpsteinSort

### 算法说明

`EpsteinSort` 的行为分为两步：

1. 过滤：只保留数值位于 `[0, 18)` 的元素。
2. 排序：对保留元素做最终排序。

这不是一个通用任意范围排序器，而是一个“固定过滤规则 + 排序”的算法实现。

过滤规则固定为：

- `value >= 0`
- `value < 18`

对浮点数还有额外约束：

- `NaN` 会被删除
- `-Infinity` 会被删除
- `+Infinity` 会被删除

过滤阶段永远只看这个固定数值区间，不受比较器影响；比较器只参与最后的排序阶段。

### 公共 API

```csharp
public static T[] RetainOnlyMinorsAndSort<T>(T[] values)
    where T : INumber<T>

public static T[] RetainOnlyMinorsAndSort<T>(T[] values, IComparer<T>? comparer)
    where T : INumber<T>

public static void RetainOnlyMinorsAndSort<T>(List<T> values)
    where T : INumber<T>

public static void RetainOnlyMinorsAndSort<T>(List<T> values, IComparer<T>? comparer)
    where T : INumber<T>
```

### 输入与输出

数组重载：

- 输入：`T[] values`
- 类型约束：`T : INumber<T>`
- 可选输入：`IComparer<T>? comparer`
- 输出：`T[]`
- 行为：不修改输入数组，返回过滤并排序后的结果数组
- 默认值：`comparer == null` 时使用 `Comparer<T>.Default`
- 特别说明：
  - 空输入返回 `Array.Empty<T>()`
  - 结果只包含满足 `[0, 18)` 的值
  - 结果一定已经按比较器定义完成排序

`List<T>` 重载：

- 输入：`List<T> values`
- 类型约束：`T : INumber<T>`
- 可选输入：`IComparer<T>? comparer`
- 输出：无返回值，结果直接写回原列表
- 行为：先原地删除不满足 `[0, 18)` 的元素，再对保留元素排序
- 默认值：`comparer == null` 时使用 `Comparer<T>.Default`

### 异常

所有重载都可能出现：

- `ArgumentNullException`
  - `values == null`

比较器相关异常：

- 如果传入的比较器在排序阶段抛出异常，异常会继续向外传播
- 对 `EpsteinSort` 来说，排序阶段依赖 `Array.Sort` 或 `List<T>.Sort`
- 实测行为上，比较器抛出的原始异常通常会作为 `InnerException` 被包装在 `InvalidOperationException` 中抛出

数组重载异常特征：

- 即使排序失败，输入数组也不会被修改

`List<T>` 重载异常特征：

- 如果排序阶段失败，过滤阶段可能已经完成
- 也就是说，列表可能已经删掉了所有不在 `[0, 18)` 内的值
- 该修改不会回滚

### 调用示例

```csharp
var values = new[] { 18, 0, -5, 17, 5, 18, 2, -1 };
var result = EpsteinSorter.RetainOnlyMinorsAndSort(values);
// result = [0, 2, 5, 17]
// values 保持不变

var list = new List<double>
{
    double.NaN,
    3.5,
    double.PositiveInfinity,
    -1.0,
    0.0,
    18.0,
    double.NegativeInfinity,
    17.25,
};
EpsteinSorter.RetainOnlyMinorsAndSort(list);
// list = [0.0, 3.5, 17.25]
```

## BrezhnevSort

### 算法说明

`BrezhnevSort` 不是传统排序。它不会比较、筛选或重排输入元素，而是只根据输入长度生成一个固定结果：

- 输入长度为 `n`
- 输出内容固定为从 `1` 到 `n` 的序数序列
- 输入元素原有的值不会被读取，也不会影响输出结果

因此，两个长度相同但内容完全不同的输入，会得到相同的输出。

例如：

- 输入：`[999, -3, 42]`
- 输出：`[1, 2, 3]`

### 公共 API

```csharp
public static T[] RewriteAsOrdinalSequence<T>(T[] values)
    where T : INumber<T>

public static void RewriteAsOrdinalSequence<T>(List<T> values)
    where T : INumber<T>
```

### 输入与输出

数组重载：

- 输入：`T[] values`
- 类型约束：`T : INumber<T>`
- 输出：`T[]`
- 行为：不修改输入数组，返回一个新的结果数组
- 特别说明：
  - 返回数组长度与输入数组一致
  - 返回内容固定为 `1..n`
  - 输入数组中的原始值不会影响结果
  - 即使输入本身已经是 `1..n`，也仍然返回新数组而不是原数组引用

`List<T>` 重载：

- 输入：`List<T> values`
- 类型约束：`T : INumber<T>`
- 输出：无返回值，结果直接写回原列表
- 行为：原地把整个列表改写为 `1..n`
- 特别说明：
  - 空列表保持为空
  - 原列表中的原始值不会影响结果

### 异常

所有重载都可能出现：

- `ArgumentNullException`
  - `values == null`

所有重载都还可能出现：

- `OverflowException`
  - 当输入长度超出 `T` 可表示范围时抛出
  - 例如对 `byte` 来说，长度达到 `256` 就无法表示

数组重载异常特征：

- 如果抛出异常，输入数组不会被修改

`List<T>` 重载异常特征：

- 如果在改写途中因数值转换溢出抛出异常，列表可能已经被部分修改
- 该修改不会回滚

### 调用示例

```csharp
var values = new[] { 999, -3, 42, 0 };
var result = BrezhnevSorter.RewriteAsOrdinalSequence(values);
// result = [1, 2, 3, 4]
// values 保持不变

var list = new List<int> { 100, -5, 8 };
BrezhnevSorter.RewriteAsOrdinalSequence(list);
// list = [1, 2, 3]
```

## 鸣谢

感谢 GPT-5.4 模型完成了这个项目！哈哈，可以说，它的能力让人叹为观止，毫无疑问是强大的生产力工具。
