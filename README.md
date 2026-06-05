# StrangeSort

`StrangeSort` 是一个基于 .NET 10 的 C# 类库，只收录和实现几种冠以**经典人物的名字**的奇异算法。

这个仓库当前包含 5 组公开算法：

- `StalinSort`：从左到右删除违序元素，保留一个有序子序列。大力清洗所有不符合要求的元素！
- `ThanosSort`：随机删除当前序列的一半元素，重复执行直到序列有序。一键清除多余元素，剩下多少你别问，嘿嘿。
- `EpsteinSort`：先保留数值区间 `[0, 18)` 内的元素，再对保留元素排序。只保留未成年数字，太老的全部斩杀！
- `BrezhnevSort`：无视输入值本身，只根据输入长度把结果改写为从 `1` 到 `n` 的序数序列。内容不重要，排场最重要！
- `TrumpSort`：对输入内容执行一次随机重排。具体发生了什么不重要，先赢再说！

## 作者的话

如果你想添加更多有趣的算法，可以提出 issue 告诉我，或提出 Pull request 以直接添加它！

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

## 算法总览

| 算法 | 命名空间 | 核心行为 | 是否重排元素 | 是否依赖随机性 |
| --- | --- | --- | --- | --- |
| StalinSort | `StrangeSort.StalinSort` | 从左到右删除相对最近保留元素违序的元素 | 否，只删除 | 否 |
| ThanosSort | `StrangeSort.ThanosSort` | 每轮随机删除一半元素，直到结果有序 | 否，只删除 | 是 |
| EpsteinSort | `StrangeSort.EpsteinSort` | 先过滤到 `[0, 18)`，再排序 | 是，会排序保留元素 | 否 |
| BrezhnevSort | `StrangeSort.BrezhnevSort` | 无视原值，只按输入长度生成 `1..n` | 否，不重排，但会重写所有值 | 否 |
| TrumpSort | `StrangeSort.TrumpSort` | 对输入执行一次 Fisher-Yates 随机重排 | 是，重排全部元素 | 是 |

## 文档入口

- Wiki 首页：<https://github.com/yhcedpn/StrangeSort/wiki>
- Getting Started：<https://github.com/yhcedpn/StrangeSort/wiki/Getting-Started>
- Algorithm Overview：<https://github.com/yhcedpn/StrangeSort/wiki/Algorithm-Overview>
- StalinSort Reference：<https://github.com/yhcedpn/StrangeSort/wiki/StalinSort-Reference>
- ThanosSort Reference：<https://github.com/yhcedpn/StrangeSort/wiki/ThanosSort-Reference>
- EpsteinSort Reference：<https://github.com/yhcedpn/StrangeSort/wiki/EpsteinSort-Reference>
- BrezhnevSort Reference：<https://github.com/yhcedpn/StrangeSort/wiki/BrezhnevSort-Reference>
- TrumpSort Reference：<https://github.com/yhcedpn/StrangeSort/wiki/TrumpSort-Reference>

公开行为契约以规范文件为准，`README.md` 与 GitHub Wiki 只承担项目介绍、导航和补充说明职责：[StrangeSort 项目开发规范](https://github.com/yhcedpn/StrangeSort/blob/main/specs/StrangeSort%20%E9%A1%B9%E7%9B%AE%E5%BC%80%E5%8F%91%E8%A7%84%E8%8C%83.md)

## 鸣谢

感谢 GPT-5.4 模型搭建并全权维护本项目！可以说，它的能力让人叹为观止，我指哪打哪，毫无疑问是强大的生产力工具。
