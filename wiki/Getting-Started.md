# Getting Started

导航： [Home](Home) | [Algorithm Overview](Algorithm-Overview)

`StrangeSort` 是一个基于 .NET 10 的 C# 类库，只收录和实现几种冠以**经典人物的名字**的奇异算法。

## 项目概览

- 目标运行时：`.NET 10`
- 核心项目：`StrangeSort`
- 测试项目：`StrangeSortTest`
- 核心库开启了 AOT 兼容性检查，测试项目开启了 AOT 发布

## 如何引用

如果你在同一个解决方案中使用它，直接给你的项目添加对 `StrangeSort/StrangeSort.csproj` 的项目引用即可。

调用前通常需要引入对应命名空间：

```csharp
using StrangeSort.StalinSort;
using StrangeSort.ThanosSort;
using StrangeSort.EpsteinSort;
using StrangeSort.BrezhnevSort;
using StrangeSort.TrumpSort;
```

## 下一步阅读

- 想先快速了解五个算法差异：查看 [Algorithm Overview](Algorithm-Overview)
- 想直接查某个算法的公共 API、输入输出与异常：进入各算法参考页
- 想确认公开行为契约：查看 [StrangeSort 项目开发规范](https://github.com/yhcedpn/StrangeSort/blob/main/specs/StrangeSort%20%E9%A1%B9%E7%9B%AE%E5%BC%80%E5%8F%91%E8%A7%84%E8%8C%83.md)
