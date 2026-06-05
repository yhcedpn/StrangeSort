# Algorithm Overview

导航： [Home](Home) | [Getting Started](Getting-Started)

| 算法 | 命名空间 | 核心行为 | 是否重排元素 | 是否依赖随机性 |
| --- | --- | --- | --- | --- |
| [StalinSort Reference](StalinSort-Reference) | `StrangeSort.StalinSort` | 从左到右删除相对最近保留元素违序的元素 | 否，只删除 | 否 |
| [ThanosSort Reference](ThanosSort-Reference) | `StrangeSort.ThanosSort` | 每轮随机删除一半元素，直到结果有序 | 否，只删除 | 是 |
| [EpsteinSort Reference](EpsteinSort-Reference) | `StrangeSort.EpsteinSort` | 先过滤到 `[0, 18)`，再排序 | 是，会排序保留元素 | 否 |
| [BrezhnevSort Reference](BrezhnevSort-Reference) | `StrangeSort.BrezhnevSort` | 无视原值，只按输入长度生成 `1..n` | 否，不重排，但会重写所有值 | 否 |
| [TrumpSort Reference](TrumpSort-Reference) | `StrangeSort.TrumpSort` | 对输入执行一次 Fisher-Yates 随机重排 | 是，重排全部元素 | 是 |

## 一句话介绍

- `StalinSort`：从左到右删除违序元素，保留一个有序子序列。
- `ThanosSort`：随机删除当前序列的一半元素，重复执行直到序列有序。
- `EpsteinSort`：先保留数值区间 `[0, 18)` 内的元素，再对保留元素排序。
- `BrezhnevSort`：无视输入值本身，只根据输入长度把结果改写为从 `1` 到 `n` 的序数序列。
- `TrumpSort`：对输入内容执行一次随机重排。

公开行为契约仍以 [StrangeSort 项目开发规范](https://github.com/yhcedpn/StrangeSort/blob/main/specs/StrangeSort%20%E9%A1%B9%E7%9B%AE%E5%BC%80%E5%8F%91%E8%A7%84%E8%8C%83.md) 为准。
