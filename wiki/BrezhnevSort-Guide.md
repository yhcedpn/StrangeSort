# BrezhnevSort Guide

`BrezhnevSort` 不看输入值，只按输入长度生成 `1..n`。

## 什么时候用

当你要把任意数值序列直接改写成序数序列时可以用它，可以让任意数值序列和勃列日涅夫同志领导的中央的总路线一起摇摆！

## 示例

```csharp
var values = new[] { 999, -3, 42, 0 };
var result = BrezhnevSorter.RewriteAsOrdinalSequence(values);
// result = [1, 2, 3, 4]
```

## 要点

1. 输出只由输入长度决定。
2. 它不是重排，而是重写。
3. 输入值再特殊，也不会影响结果。

## 相关规范

[StrangeSort 开发规范](https://github.com/yhcedpn/StrangeSort/blob/main/specs/StrangeSort%20%E5%BC%80%E5%8F%91%E8%A7%84%E8%8C%83.md)
