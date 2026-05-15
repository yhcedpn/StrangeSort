# dotnet format info 级别问题修复计划

## 已确认范围与约束

1. 本计划只针对以下两条命令在当前代码库上命中的全部 `info` 级别诊断：
   - `dotnet format "StrangeSort.slnx" analyzers --severity info --verify-no-changes --no-restore`
   - `dotnet format "StrangeSort.slnx" style --severity info --verify-no-changes --no-restore`
2. 本次必须清理所有已命中的问题，不保留测试辅助方法上的 `CA1859`。
3. 以最小改动完成整改，不顺带修改公共 API 行为、排序语义、测试意图或目录结构。
4. 若在整改过程中因为代码联动引出新的同级别诊断，应一并处理，直到上述两条命令通过 `--verify-no-changes`。
5. 本计划不假设会新增或调整 `.editorconfig`、分析器规则集或项目级规则配置；默认接受当前仓库已有规则输出作为整改目标。

## 已知问题清单

1. `StrangeSort/EpsteinSort/EpsteinSorter.cs`
   - `CA1859`：`CompactRetainedValues<T>` 的 `values` 参数可从 `IList<T>` 收窄为 `List<T>`。
   - `IDE0301`：两个 `Array.Empty<T>()` 可简化为 collection expression。
2. `StrangeSort/StalinSort/StalinSorter.cs`
   - `CA1859`：`CompactOrderedSubsequence<T>` 的 `values` 参数可从 `IList<T>` 收窄为 `List<T>`。
3. `StrangeSortTest/EpsteinSort/EpsteinSorterTests.cs`
   - `CA1859`：`AssertOrdered<T>` 的 `comparer` 参数、`CreateComparerThatThrowsOnFirstComparison` 的返回类型可进一步收窄。
   - `CA1861` / `IDE0300`：多个断言中的常量数组参数和数组初始化可简化。
4. `StrangeSortTest/StalinSort/StalinSorterTests.cs`
   - `CA1859`：`AssertSubsequence<T>` 的 `source` 参数、`CreateComparerThatThrowsOnThirdComparison` 的返回类型可进一步收窄。
   - `CA1861` / `IDE0300`：多个断言中的常量数组参数和数组初始化可简化。
5. `StrangeSortTest/ThanosSort/ThanosSorterTests.cs`
   - `CA1859`：`CreateComparerThatThrowsOnThirdComparison` 的返回类型可进一步收窄。
   - `CA1861` / `IDE0300`：多个断言中的常量数组参数和数组初始化可简化。

## 目标

1. 让上述两条 `dotnet format` 检查在当前解决方案上通过 `--verify-no-changes`。
2. 保持库代码行为、测试覆盖意图和公开 contract 不变。
3. 将所有整改控制在“为满足当前规则所需的最小代码调整”范围内。

## 非目标

1. 不重构排序算法实现。
2. 不扩大测试覆盖范围，除非某个重构后的辅助方法必须做最小联动。
3. 不新增新的公共类型、公共重载或兼容层。
4. 不通过降低分析器级别、禁用规则或添加抑制来规避问题。

## 实施步骤

### 步骤 1：修正库代码中的低风险类型收窄与风格问题

目标：先处理核心库中无争议、低联动的 `CA1859` 和 `IDE0301`。

实施内容：

1. 在 `StrangeSort/EpsteinSort/EpsteinSorter.cs` 中将 `CompactRetainedValues<T>` 的参数类型从 `IList<T>` 调整为 `List<T>`，并检查唯一调用点是否仍保持原地压缩语义不变。
2. 在 `StrangeSort/StalinSort/StalinSorter.cs` 中将 `CompactOrderedSubsequence<T>` 的参数类型从 `IList<T>` 调整为 `List<T>`，保持现有调用路径和算法行为不变。
3. 在 `StrangeSort/EpsteinSort/EpsteinSorter.cs` 中把两个 `Array.Empty<T>()` 按当前 style 规则改写为等价的 collection expression 写法。
4. 不主动扩大到 `ThanosSorter.cs` 中未被当前规则命中的私有帮助方法，避免产生与本次目标无关的额外改动。

完成标准：

1. 库代码中当前已知的 `CA1859` 和 `IDE0301` 命中项全部清零。
2. 相关方法签名调整后，公开 API 不发生变化。

### 步骤 2：清理测试中的常量数组参数与集合初始化提示

目标：以最小方式消除测试中的 `CA1861` 和 `IDE0300`。

实施内容：

1. 在三个测试文件中，将重复出现或直接内联到断言参数中的常量数组提取为局部变量或按规则改写为 collection expression。
2. 优先保持测试断言的可读性，不为了压缩写法而把输入、期望值和断言混成难读的一行。
3. 对只使用一次但被 style 规则命中的数组初始化，同样采用当前仓库接受的简化写法，保证 `dotnet format style` 通过。
4. 避免把期望数组提升为跨测试共享字段，除非同一个值在同一文件中被多次复用且这样做更清晰。

完成标准：

1. 三个测试文件中的 `CA1861` 和 `IDE0300` 命中项全部清零。
2. 每个测试仍能直接看出输入、期望结果和断言意图。

### 步骤 3：按要求清零测试辅助方法上的 `CA1859`

目标：接受分析器建议，连同测试辅助方法上的类型收窄问题一起修掉。

实施内容：

1. 在 `StrangeSortTest/EpsteinSort/EpsteinSorterTests.cs` 中，按当前实际调用方式收窄 `AssertOrdered<T>` 的 `comparer` 参数类型，以及 `CreateComparerThatThrowsOnFirstComparison` 的返回类型。
2. 在 `StrangeSortTest/StalinSort/StalinSorterTests.cs` 中，按当前实际调用方式收窄 `AssertSubsequence<T>` 的 `source` 参数类型，以及 `CreateComparerThatThrowsOnThirdComparison` 的返回类型。
3. 在 `StrangeSortTest/ThanosSort/ThanosSorterTests.cs` 中，收窄 `CreateComparerThatThrowsOnThirdComparison` 的返回类型。
4. 若某个辅助方法因类型收窄导致泛型复用价值明显下降，应优先采用最小范围的重载拆分或局部具体化，而不是牵连无关测试改写。
5. 所有调整都必须保持原有测试语义不变，不允许为了满足规则而改变测试断言内容。

完成标准：

1. 所有测试辅助方法上的 `CA1859` 命中项全部清零。
2. 收窄后的辅助方法仍与当前测试调用方式一致，且不会引入新的编译错误或可读性明显退化的问题。

### 步骤 4：执行验证并闭合整改范围

目标：确认全部问题确实已被清理，没有遗漏或回归。

实施内容：

1. 运行 `dotnet format "StrangeSort.slnx" analyzers --severity info --verify-no-changes --no-restore`。
2. 运行 `dotnet format "StrangeSort.slnx" style --severity info --verify-no-changes --no-restore`。
3. 运行 `dotnet build "StrangeSort.slnx"`，确认类型收窄与风格改写未引入编译问题。
4. 运行 `dotnet test "StrangeSort.slnx"`，确认测试逻辑未被改变。
5. 若验证阶段发现新增 `info` 级诊断，回到对应步骤补齐修复，直到两条 `dotnet format` 检查和构建、测试全部通过。

完成标准：

1. 两条 `dotnet format` 命令均通过 `--verify-no-changes`。
2. 解决方案构建通过。
3. 所有测试通过。

## 交付结果

1. 一组最小且完整的代码整改，覆盖当前两条 `dotnet format` 检查命中的全部 `info` 级问题。
2. 一个在当前规则集下可稳定通过 analyzers、style、build 和 test 的解决方案状态。
