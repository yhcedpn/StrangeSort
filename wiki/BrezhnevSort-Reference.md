# BrezhnevSort Reference

导航： [Home](Home) | [Getting Started](Getting-Started) | [Algorithm Overview](Algorithm-Overview)

说明：本页是参考文档，不是公开行为契约来源；如有冲突，以 [StrangeSort 项目开发规范](https://github.com/yhcedpn/StrangeSort/blob/main/specs/StrangeSort%20%E9%A1%B9%E7%9B%AE%E5%BC%80%E5%8F%91%E8%A7%84%E8%8C%83.md) 为准。

## 算法说明

`BrezhnevSort` 不是传统排序。它不会比较、筛选或重排输入元素，而是只根据输入长度生成一个固定结果：

- 输入长度为 `n`
- 输出内容固定为从 `1` 到 `n` 的序数序列
- 输入元素原有的值不会被读取，也不会影响输出结果

因此，两个长度相同但内容完全不同的输入，会得到相同的输出。

例如：

- 输入：`[999, -3, 42]`
- 输出：`[1, 2, 3]`

## 公共 API

```csharp
public static T[] RewriteAsOrdinalSequence<T>(T[] values)
    where T : INumber<T>

public static void RewriteAsOrdinalSequence<T>(List<T> values)
    where T : INumber<T>
```

## 输入与输出

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

## 异常

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

## 调用示例

```csharp
var values = new[] { 999, -3, 42, 0 };
var result = BrezhnevSorter.RewriteAsOrdinalSequence(values);
// result = [1, 2, 3, 4]
// values 保持不变

var list = new List<int> { 100, -5, 8 };
BrezhnevSorter.RewriteAsOrdinalSequence(list);
// list = [1, 2, 3]
```

导航： [Home](Home) | [Getting Started](Getting-Started) | [Algorithm Overview](Algorithm-Overview)
