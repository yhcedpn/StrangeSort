# ThanosSort Guide

`ThanosSort` 会反复随机删除一半元素，直到剩余结果有序。

## 什么时候用

当你想保留原顺序，但接受用随机裁剪换取“最终有序”时可以用它。灭霸在此！

## 示例

```csharp
var values = new[] { 5, 1, 4, 4, 2, 3 };
var result = ThanosSorter.PruneUntilSorted(
    values,
    comparer: null,
    removalStrategy: RemovalCountStrategy.CeilingHalf,
    random: new Random(2026));
```

## 要点

1. 它不是随机排序，而是随机删到有序。
2. 幸存元素仍保持原有相对顺序。
3. 固定随机种子时结果可复现。

## 相关规范

[StrangeSort 开发规范](https://github.com/yhcedpn/StrangeSort/blob/main/specs/StrangeSort%20%E5%BC%80%E5%8F%91%E8%A7%84%E8%8C%83.md)
