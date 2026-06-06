# StrangeSort

`StrangeSort` 是一个基于 .NET 10 的 C# 类库，只收录和实现几种冠以**经典人物的名字**的奇异算法。

这个仓库当前包含 5 组公开算法：

- `StalinSort`：从左到右删除违序元素，保留一个有序子序列。大力清洗所有不符合要求的元素！
- `ThanosSort`：随机删除当前序列的一半元素，重复执行直到序列有序。一键清除多余元素，剩下多少你别问，嘿嘿。
- `EpsteinSort`：先保留数值区间 `[0, 18)` 内的元素，再对保留元素排序。只保留未成年数字，太老的全部斩杀！
- `BrezhnevSort`：无视输入值本身，只根据输入长度把结果改写为从 `1` 到 `n` 的序数序列。内容不重要，排场最重要！
- `TrumpSort`：对输入内容执行一次随机重排。具体发生了什么不重要，先赢再说！

## 快速开始

### 从 GitHub Packages 安装

当前包名是 `StrangeSort`，GitHub Packages 源地址示例为 `https://nuget.pkg.github.com/yhcedpn/index.json`。

首次使用时，先添加包源：

```powershell
dotnet nuget add source "https://nuget.pkg.github.com/yhcedpn/index.json" --name github-yhcedpn --username YOUR_GITHUB_USERNAME --password YOUR_GITHUB_PAT --store-password-in-clear-text
```

然后安装包：

```powershell
dotnet add package StrangeSort --version <VERSION> --source github-yhcedpn
```

### 快速使用

调用前通常需要引入对应命名空间：

```csharp
using StrangeSort.StalinSort;
using StrangeSort.ThanosSort;
using StrangeSort.EpsteinSort;
using StrangeSort.BrezhnevSort;
using StrangeSort.TrumpSort;

int[] source = [3, 1, 2, 4];
int[] result = StalinSorter.RemoveOutOfOrder(source);
```

## 详细文档

详见 Wiki：<https://github.com/yhcedpn/StrangeSort/wiki>

公开行为契约以规范文件为准，`README.md` 与 GitHub Wiki 只承担项目介绍、导航和补充说明职责。
