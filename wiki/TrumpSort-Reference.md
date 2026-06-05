# TrumpSort Reference

导航： [Home](Home) | [Getting Started](Getting-Started) | [Algorithm Overview](Algorithm-Overview)

说明：本页是参考文档，不是公开行为契约来源；如有冲突，以 [StrangeSort 项目开发规范](https://github.com/yhcedpn/StrangeSort/blob/main/specs/StrangeSort%20%E9%A1%B9%E7%9B%AE%E5%BC%80%E5%8F%91%E8%A7%84%E8%8C%83.md) 为准。

## 算法说明

`TrumpSort` 不是传统排序。它不会保证结果有序，而是对输入序列执行一次 Fisher-Yates 随机重排。

它的行为是：

- 遍历序列尾部到头部的位置。
- 对每个位置随机选择一个不超过当前位置的索引。
- 交换这两个位置上的元素。

因此，输出结果与输入包含完全相同的元素多重集，只是顺序被随机打乱。

例如：

- 输入：`[1, 2, 3, 4]`
- 可能输出：`[3, 1, 4, 2]`

## 公共 API

```csharp
public static T[] ShuffleAndWin<T>(T[] values)
public static T[] ShuffleAndWin<T>(T[] values, Random random)
public static void ShuffleAndWin<T>(List<T> values)
public static void ShuffleAndWin<T>(List<T> values, Random random)
```

## 输入与输出

数组重载：

- 输入：`T[] values`
- 可选输入：`Random random`
- 输出：`T[]`
- 行为：不修改输入数组，返回一个新的随机排列结果数组
- 默认值：`random` 未显式传入时使用 `Random.Shared`
- 特别说明：
  - 返回数组长度与输入数组一致
  - 返回数组与输入数组包含相同的元素多重集
  - 即使输入为空数组或单元素数组，也会返回新的结果数组而不是原数组引用
  - 使用相同随机种子时，结果可复现

`List<T>` 重载：

- 输入：`List<T> values`
- 可选输入：`Random random`
- 输出：无返回值，结果直接写回原列表
- 行为：原地执行一次随机重排
- 默认值：`random` 未显式传入时使用 `Random.Shared`
- 特别说明：
  - 列表长度保持不变
  - 列表中的元素多重集保持不变
  - 使用相同随机种子时，结果可复现
  - 对相同输入使用相同随机种子时，数组重载和列表重载会得到一致内容

## 异常

所有重载都可能出现：

- `ArgumentNullException`
  - `values == null`

带 `random` 参数的重载还可能出现：

- `ArgumentNullException`
  - `random == null`

随机源相关异常：

- 随机源自己抛出的异常会直接向外传播
- 对数组重载，如果随机源在处理中抛出异常，输入数组不会被修改
- 对 `List<T>` 重载，如果随机源在处理中抛出异常，列表可能已经被部分修改，且不会回滚

## 调用示例

```csharp
var values = new[] { 1, 2, 3, 4, 5 };
var result = TrumpSorter.ShuffleAndWin(values, new Random(2026));
// result 是 values 的一个随机排列
// values 保持不变

var list = new List<int> { 1, 2, 3, 4, 5 };
TrumpSorter.ShuffleAndWin(list, new Random(2026));
// list 被原地随机重排
```

导航： [Home](Home) | [Getting Started](Getting-Started) | [Algorithm Overview](Algorithm-Overview)
