# EpsteinSort Guide

`EpsteinSort` 会先过滤，再排序。只有 `[0, 18)` 内的值能留下来。

## 什么时候用

当你需要固定区间过滤加排序，而不是通用排序时可以用它。像爱泼斯坦先生一样，保留并排序未成年人！

## 示例

```csharp
var values = new[] { 18, 0, -5, 17, 5, 18, 2, -1 };
var result = EpsteinSorter.RetainOnlyMinorsAndSort(values);
// result = [0, 2, 5, 17]
```

## 要点

1. 过滤规则固定是 `[0, 18)`。
2. 比较器只影响排序，不影响过滤。
3. 对浮点类型，`NaN` 和正负无穷都会被删除。

## 相关规范

[StrangeSort 开发规范](https://github.com/yhcedpn/StrangeSort/blob/main/specs/StrangeSort%20%E5%BC%80%E5%8F%91%E8%A7%84%E8%8C%83.md)
