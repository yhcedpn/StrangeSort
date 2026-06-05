# EpsteinSort Reference

导航： [Home](Home) | [Getting Started](Getting-Started) | [Algorithm Overview](Algorithm-Overview)

说明：本页是参考文档，不是公开行为契约来源；如有冲突，以 [StrangeSort 项目开发规范](https://github.com/yhcedpn/StrangeSort/blob/main/specs/StrangeSort%20%E9%A1%B9%E7%9B%AE%E5%BC%80%E5%8F%91%E8%A7%84%E8%8C%83.md) 为准。

## 算法说明

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

## 公共 API

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

## 输入与输出

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

## 异常

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

## 调用示例

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

导航： [Home](Home) | [Getting Started](Getting-Started) | [Algorithm Overview](Algorithm-Overview)
