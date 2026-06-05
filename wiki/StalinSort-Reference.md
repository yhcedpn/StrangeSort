# StalinSort Reference

导航： [Home](Home) | [Getting Started](Getting-Started) | [Algorithm Overview](Algorithm-Overview)

说明：本页是参考文档，不是公开行为契约来源；如有冲突，以 [StrangeSort 项目开发规范](https://github.com/yhcedpn/StrangeSort/blob/main/specs/StrangeSort%20%E9%A1%B9%E7%9B%AE%E5%BC%80%E5%8F%91%E8%A7%84%E8%8C%83.md) 为准。

## 算法说明

`StalinSort` 不是传统排序算法。它不会交换元素位置，而是从左到右扫描：

- 第一个元素总是保留。
- 后续元素如果满足 `comparer.Compare(lastKept, current) <= 0`，就保留。
- 否则直接删除。

因此输出结果一定是输入序列的一个子序列，并且保留元素的相对顺序不变。

例如：

- 输入：`[2, 1, 3, 2, 4]`
- 输出：`[2, 3, 4]`

## 公共 API

```csharp
public static T[] RemoveOutOfOrder<T>(T[] values)
public static T[] RemoveOutOfOrder<T>(T[] values, IComparer<T>? comparer)
public static void RemoveOutOfOrder<T>(List<T> values)
public static void RemoveOutOfOrder<T>(List<T> values, IComparer<T>? comparer)
```

## 输入与输出

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

## 异常

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

## 调用示例

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

导航： [Home](Home) | [Getting Started](Getting-Started) | [Algorithm Overview](Algorithm-Overview)
