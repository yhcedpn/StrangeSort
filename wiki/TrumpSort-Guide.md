# TrumpSort Guide

`TrumpSort` 会对输入执行一次随机重排，不保证结果有序。

## 什么时候用

当你只想打乱顺序，而不是排序或过滤时可以用它。无论如何，先赢再说！用它的人每一天都会赢！

## 示例

```csharp
var values = new[] { 1, 2, 3, 4, 5 };
var result = TrumpSorter.ShuffleAndWin(values, new Random(2026));
```

## 要点

1. 它只做一次 Fisher-Yates shuffle。
2. 不删除元素，不改写元素值。
3. 固定随机种子时结果可复现。

## 相关规范

[StrangeSort 开发规范](https://github.com/yhcedpn/StrangeSort/blob/main/specs/StrangeSort%20%E5%BC%80%E5%8F%91%E8%A7%84%E8%8C%83.md)
