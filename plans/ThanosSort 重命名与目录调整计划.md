# ThanosSort 重命名与目录调整计划

## 背景

当前库里已实现的特殊算法以 `StrangeSorter` 作为公开入口，核心代码位于项目根层，测试也位于测试项目根层。为了让该算法的命名与行为更一致，并为后续增加新的特殊算法预留清晰的目录形态，需要将该算法整体重命名为 `ThanosSort`，并把其生产代码与测试代码分别收拢到单独文件夹中。

## 已确认决策

1. 本次调整按完全破坏性变更处理，不保留任何旧 API、旧命名空间或兼容转发层。
2. 保留项目名与程序集名 `StrangeSort`，不把整个库改名为 `ThanosSort`。
3. 生产代码目录采用单层结构：`StrangeSort/ThanosSort/`。
4. 测试代码目录采用单层结构：`StrangeSortTest/ThanosSort/`。
5. 生产代码命名空间改为 `StrangeSort.ThanosSort`。
6. 测试代码命名空间改为 `StrangeSortTest.ThanosSort`。
7. 公开类名由 `StrangeSorter` 改为 `ThanosSorter`。
8. 公开方法名保持为 `PruneUntilSorted`，不改为 `ThanosSort(...)` 或 `Sort(...)`。
9. `RemovalCountStrategy` 保留类型名，只迁移到 `StrangeSort.ThanosSort` 命名空间下。
10. 旧的计划文件视为历史计划，本次不修改 `plans/` 中已有文件。

## 目标

1. 将现有算法的公开身份统一为 `ThanosSort`。
2. 同步调整类名、命名空间名、注释、测试引用与测试命名空间。
3. 将该算法的实现与测试收拢到独立文件夹，提升后续新增特殊算法时的局部性。
4. 保持算法运行逻辑、随机性语义、异常语义与现有测试契约不变。

## 非目标

1. 不保留旧类型、旧命名空间或 `[Obsolete]` 兼容入口。
2. 不修改 `PruneUntilSorted` 的核心算法。
3. 不引入公共接口、基类、注册器、工厂或其他额外架构抽象。
4. 不修改历史计划文件中的旧命名。
5. 不新增与本次重命名无关的重构。

## 实施后文件布局

1. `StrangeSort/ThanosSort/ThanosSorter.cs`
2. `StrangeSort/ThanosSort/RemovalCountStrategy.cs`
3. `StrangeSortTest/ThanosSort/ThanosSorterTests.cs`

## 实施步骤

### 步骤 1：迁移并重命名生产代码

目标：先完成生产代码侧的目录调整、命名空间调整和公开类型重命名，同时保持实现行为不变。

实施内容：

1. 将 `StrangeSort/StrangeSorter.cs` 移动并重命名为 `StrangeSort/ThanosSort/ThanosSorter.cs`。
2. 将 `StrangeSort/RemovalCountStrategy.cs` 移动到 `StrangeSort/ThanosSort/RemovalCountStrategy.cs`。
3. 将两个文件中的命名空间从 `StrangeSort` 改为 `StrangeSort.ThanosSort`。
4. 将公开类名从 `StrangeSorter` 改为 `ThanosSorter`。
5. 保持所有 `PruneUntilSorted` 重载、私有辅助方法、参数顺序与异常边界不变。
6. 精确修改 XML 注释、`cref` 与算法描述，使其统一表达为 `ThanosSort`，同时继续明确该算法不是传统排序，而是随机删除直到得到有序子序列。

完成标准：

1. 生产代码项目内不再存在 `StrangeSorter` 公开类型。
2. 生产代码项目内不再存在 `namespace StrangeSort;` 形式的旧算法命名空间声明。
3. 生产代码可编译，且公开方法组仍完整可用。

### 步骤 2：迁移并重命名测试代码

目标：让测试代码与新的生产代码目录和命名同步，继续覆盖现有公开契约。

实施内容：

1. 将 `StrangeSortTest/StrangeSorterTests.cs` 移动并重命名为 `StrangeSortTest/ThanosSort/ThanosSorterTests.cs`。
2. 将测试文件中的 `using StrangeSort;` 改为 `using StrangeSort.ThanosSort;`。
3. 将测试命名空间从 `StrangeSortTest` 改为 `StrangeSortTest.ThanosSort`。
4. 将测试类名从 `StrangeSorterTests` 改为 `ThanosSorterTests`。
5. 将测试中的类型引用从 `StrangeSorter` 改为 `ThanosSorter`。
6. 保持现有测试场景、断言重点和辅助测试替身不变；测试方法名若仅用于表达 `PruneUntilSorted` 契约，可保持原意，不为追求全量重命名而制造无价值噪音。

完成标准：

1. 测试项目内所有对旧公开类型的代码引用都已迁移到新命名。
2. 测试仍覆盖数组与列表主路径、异常路径和随机性相关契约。

### 步骤 3：全局清理并验证

目标：确认代码范围内的重命名已完整生效，并验证本次破坏性调整未引入行为回归。

实施内容：

1. 在代码文件范围内搜索 `StrangeSorter`、`using StrangeSort;`、`namespace StrangeSort;` 等旧标识，确认旧引用已清理完成。
2. 不修改 `plans/` 下的历史计划文件，即使其中仍保留旧命名。
3. 运行 `dotnet build "StrangeSort.slnx"`。
4. 运行 `dotnet test "StrangeSort.slnx"`。

完成标准：

1. 代码项目中的旧命名已完成迁移。
2. 解决方案构建通过。
3. 测试全部通过。
4. 本次调整仅改变公开命名和代码布局，不改变既有算法语义。

## 风险与注意事项

1. 这是明确的 breaking change，所有旧调用方都需要从 `StrangeSort.StrangeSorter` 迁移到 `StrangeSort.ThanosSort.ThanosSorter`。
2. 任何写死旧全名的反射代码、文档示例、`using static` 语句或 XML `cref` 都会失效，必须同步迁移到新全名。
3. SDK 风格项目默认会递归包含子目录中的 `*.cs` 文件，正常情况下不需要为本次文件移动修改 `.csproj` 或 `.slnx`。
