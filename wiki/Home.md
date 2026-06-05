# StrangeSort Wiki

`StrangeSort` 是一个基于 .NET 10 的 C# 类库，只收录和实现几种冠以**经典人物的名字**的奇异算法。

这个仓库当前包含 5 组公开算法：

- `StalinSort`：从左到右删除违序元素，保留一个有序子序列。大力清洗所有不符合要求的元素！
- `ThanosSort`：随机删除当前序列的一半元素，重复执行直到序列有序。一键清除多余元素，剩下多少你别问，嘿嘿。
- `EpsteinSort`：先保留数值区间 `[0, 18)` 内的元素，再对保留元素排序。只保留未成年数字，太老的全部斩杀！
- `BrezhnevSort`：无视输入值本身，只根据输入长度把结果改写为从 `1` 到 `n` 的序数序列。内容不重要，排场最重要！
- `TrumpSort`：对输入内容执行一次随机重排。具体发生了什么不重要，先赢再说！

## 文档导航

- [Getting Started](Getting-Started)
- [Algorithm Overview](Algorithm-Overview)
- [StalinSort Reference](StalinSort-Reference)
- [ThanosSort Reference](ThanosSort-Reference)
- [EpsteinSort Reference](EpsteinSort-Reference)
- [BrezhnevSort Reference](BrezhnevSort-Reference)
- [TrumpSort Reference](TrumpSort-Reference)

## 文档职责说明

GitHub Wiki 用于承载详细参考文档；仓库首页 `README.md` 用于项目简介、快速上手和导航。

项目公开行为契约只以规范文件为准；如果 Wiki、`README.md`、实现或测试之间出现不一致，应优先检查规范文件：

- [StrangeSort 项目开发规范](https://github.com/yhcedpn/StrangeSort/blob/main/specs/StrangeSort%20%E9%A1%B9%E7%9B%AE%E5%BC%80%E5%8F%91%E8%A7%84%E8%8C%83.md)
- [仓库 README](https://github.com/yhcedpn/StrangeSort#readme)
