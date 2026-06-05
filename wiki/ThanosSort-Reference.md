# ThanosSort Reference

导航： [Home](Home) | [Getting Started](Getting-Started) | [Algorithm Overview](Algorithm-Overview)

说明：本页是参考文档，不是公开行为契约来源；如有冲突，以 [StrangeSort 项目开发规范](https://github.com/yhcedpn/StrangeSort/blob/main/specs/StrangeSort%20%E9%A1%B9%E7%9B%AE%E5%BC%80%E5%8F%91%E8%A7%84%E8%8C%83.md) 为准。

## 算法说明

`ThanosSort` 同样不是传统排序。它的行为是：

- 先判断当前序列是否已经有序
- 如果无序，就随机删除当前序列的一半元素
- 重复以上过程，直到结果有序

保留下来的元素仍然保持原有相对顺序，因此输出仍然是输入序列的一个有序子序列。

当当前长度为奇数时，每轮删除多少个元素由 `RemovalCountStrategy` 控制：

- `RemovalCountStrategy.FloorHalf = 0`：删除 `count / 2`
- `RemovalCountStrategy.CeilingHalf = 1`：删除 `(count + 1) / 2`

## 公共 API

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

## 输入与输出

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

## 异常

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

## 调用示例

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

导航： [Home](Home) | [Getting Started](Getting-Started) | [Algorithm Overview](Algorithm-Overview)
