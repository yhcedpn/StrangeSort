# Quick Start

## 1. 安装包

示例命令：

```powershell
dotnet nuget add source "https://nuget.pkg.github.com/yhcedpn/index.json" --name github-yhcedpn --username YOUR_GITHUB_USERNAME --password YOUR_GITHUB_PAT
dotnet add package StrangeSort --version <VERSION> --source github-yhcedpn
```

## 2. 引入命名空间

```csharp
using StrangeSort.BrezhnevSort;
using StrangeSort.EpsteinSort;
using StrangeSort.StalinSort;
using StrangeSort.ThanosSort;
using StrangeSort.TrumpSort;
```

## 3. 选择一个算法调用

```csharp
int[] source = [3, 1, 2, 4];

int[] stalin = StalinSorter.RemoveOutOfOrder(source);
int[] thanos = ThanosSorter.PruneUntilSorted(
    source,
    comparer: null,
    removalStrategy: RemovalCountStrategy.FloorHalf,
    random: new Random(2026));
int[] epstein = EpsteinSorter.RetainOnlyMinorsAndSort(source);
int[] brezhnev = BrezhnevSorter.RewriteAsOrdinalSequence(source);
int[] trump = TrumpSorter.ShuffleAndWin(source, new Random(2026));
```

## 4. 接下来读什么

1. 想看某个算法的使用说明：进入对应 `*-Guide` 页面。
2. 想确认精确行为：看 `specs/StrangeSort 开发规范.md`。
3. 想确认测试覆盖重点：看 `specs/StrangeSortTest 测试规范.md`。
