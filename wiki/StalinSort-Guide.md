# StalinSort Guide

`StalinSort` 会从左到右删除违序元素，最后留下一个保持原顺序的有序子序列。

## 什么时候用

当你想保留原始相对顺序，只移除“不合队列”的元素时可以用它。它专为沿袭斯大林同志的方针设计，一键清洗不正确的元素！

## 示例

```csharp
var values = new[] { 2, 1, 3, 2, 4 };
var result = StalinSorter.RemoveOutOfOrder(values);
// result = [2, 3, 4]
```

## 要点

1. 它只删除，不排序，不重排。
2. 结果一定是输入的子序列。
3. 自定义比较器可以改变“有序”的判断方向。

## 相关规范

[StrangeSort 开发规范](https://github.com/yhcedpn/StrangeSort/blob/main/specs/StrangeSort%20%E5%BC%80%E5%8F%91%E8%A7%84%E8%8C%83.md)
