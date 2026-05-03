# PruneUntilSorted 实现计划

## 已确认设计决策

1. 算法语义：若输入序列满足 `comparer.Compare(previous, current) <= 0`，则立即结束；否则随机删除恰好一半元素，并重复该过程，直到剩余序列有序。
2. 公共 API 不引入单独的升序/降序开关；顺序完全由 `IComparer<T>` 决定。未提供比较器时使用 `Comparer<T>.Default`，这通常意味着自然升序；若要按自然降序裁剪，由调用方传入反向比较器。
3. 幸存元素必须保留原相对顺序，因此最终结果是输入序列的一个有序子序列，而不是重排后的排序结果。
4. 公共入口采用静态工具类，不提供扩展方法作为首版入口。
5. 首版只支持整个 `T[]` 和整个 `List<T>`，不提供 `Span<T>`、`Memory<T>`、`IEnumerable<T>`、`LinkedList<T>`、`Dictionary<TKey, TValue>`、区间重载或扩展方法。
6. `T[]` API 不修改输入，并返回表示最终结果的数组；不对返回数组与输入数组的引用关系作额外承诺。
7. `List<T>` API 原地修改，不返回值；调用完成后的 `values.Count` 表示最终剩余元素数。
8. 比较器通过 `IComparer<T>` 提供；未提供时使用 `Comparer<T>.Default`。
9. 对于默认比较语义不明确、或 `Comparer<T>.Default` 无法比较的类型，调用方必须显式传入 `IComparer<T>`。
10. 随机性通过 `Random` 提供；未提供 `Random` 参数的重载使用 `Random.Shared`。
11. 奇数长度时的“删除一半”通过公开枚举配置，首版支持向下取半和向上取半两种策略。
12. 首版不暴露轮数、总删除数等额外统计信息；`List<T>` 的最终剩余元素数通过 `values.Count` 获取。

## 首版非目标

1. 不保证得到最长有序子序列，也不保证删除次数最少。
2. 不提供概率分布或统计稳定性的公开承诺。
3. 不为测试而引入专用随机抽象、事件回调或调试 API。
4. 不引入反射、动态代码生成或其他可能影响 AOT 兼容性的机制。

## 首版类型取舍原则

1. `T[]`：支持。数组拥有自己的存储，但长度固定；算法结束后的有效长度通常会缩短，因此最清晰的 contract 是返回结果数组而不是原地缩短输入。
2. `List<T>`：支持。列表可变且可缩短，适合原地压缩并直接移除尾部元素。
3. `IEnumerable<T>`：首版不支持。该算法依赖重复有序性检查和随机删除位置抽样，若接受 `IEnumerable<T>` 只能先完整物化，容易隐藏分配和多次遍历成本。
4. `LinkedList<T>`：首版不支持。链表缺少高效随机访问，这与该算法的抽样和压缩模式不匹配。
5. `Dictionary<TKey, TValue>`：首版不支持。它不是单纯的顺序序列，“有序”究竟按 key、value 还是键值对比较并不明确。
6. `Span<T>` / `Memory<T>`：首版不支持。它们更适合另一组低分配缓冲区 API，而那组 API 需要单独确定“返回新长度还是返回切片视图”的 contract；若后续扩展，优先考虑 `Span<T>`。

## 建议公共类型与命名

1. 在 `StrangeSort` 项目中新增枚举 `RemovalCountStrategy`。
2. 在 `StrangeSort` 项目中新增静态类 `StrangeSorter`。
3. 公共 API 采用重载组收敛到最全重载，而不是只依赖可选参数，以贴近 BCL 风格；`Random` 参数位于最后，仅在调用方需要显式控制随机源时提供。
4. 首版为每个容器固定提供以下四个公共重载，所有简化重载都收敛到最全重载；`Random` 参数位于最后：

```csharp
public static T[] PruneUntilSorted<T>(
    T[] values)

public static T[] PruneUntilSorted<T>(
    T[] values,
    IComparer<T>? comparer)

public static T[] PruneUntilSorted<T>(
    T[] values,
    IComparer<T>? comparer,
    RemovalCountStrategy removalStrategy)

public static T[] PruneUntilSorted<T>(
    T[] values,
    IComparer<T>? comparer,
    RemovalCountStrategy removalStrategy,
    Random random)

public static void PruneUntilSorted<T>(
    List<T> values)

public static void PruneUntilSorted<T>(
    List<T> values,
    IComparer<T>? comparer)

public static void PruneUntilSorted<T>(
    List<T> values,
    IComparer<T>? comparer,
    RemovalCountStrategy removalStrategy)

public static void PruneUntilSorted<T>(
    List<T> values,
    IComparer<T>? comparer,
    RemovalCountStrategy removalStrategy,
    Random random)
```

5. 参数较少的重载默认行为固定如下：
   - 仅传 `values`：使用 `Comparer<T>.Default`、`RemovalCountStrategy.FloorHalf`、`Random.Shared`。
   - 传 `values + comparer`：使用传入比较器；若 `comparer` 为 `null` 则退回 `Comparer<T>.Default`，并继续使用 `RemovalCountStrategy.FloorHalf` 与 `Random.Shared`。
   - 传 `values + comparer + removalStrategy`：使用传入比较器；若 `comparer` 为 `null` 则退回 `Comparer<T>.Default`，并使用传入删除策略与 `Random.Shared`。
6. 首版不额外提供 `values + removalStrategy`、`values + random`、`values + removalStrategy + random` 等跳过 `comparer` 位置的公共重载，避免扩大 API 面。
7. `values` 为 `null` 时抛出 `ArgumentNullException`。
8. `comparer` 为 `null` 不抛异常，退回默认比较器；带 `Random` 参数的重载若 `random` 为 `null`，则抛出 `ArgumentNullException`。
9. `removalStrategy` 传入未定义的枚举值时抛出 `ArgumentOutOfRangeException`。
10. 若调用方未提供比较器，而 `Comparer<T>.Default` 无法比较 `T`，则仅在实际发生比较时传播原始 `ArgumentException`，不额外包装异常；`0` 或 `1` 个元素的输入应直接成功。
11. 数组版本即使执行失败，也不修改传入数组；列表版本若比较器或随机源在执行过程中抛出异常，则列表可能已被部分修改，不保证回滚。

## 推荐文件布局

1. `StrangeSort/RemovalCountStrategy.cs`
2. `StrangeSort/StrangeSorter.cs`
3. `StrangeSortTest/StrangeSorterTests.cs`

保持最小文件数即可，不额外拆分随机抽样接口或结果对象。

## 实施步骤

### 步骤 1：建立公共契约

目标：先固定公共行为、命名和异常边界，避免后续实现偏离已确认设计。

实施内容：

1. 新增 `RemovalCountStrategy`，包含：
   - `FloorHalf`：删除 `count / 2` 个元素。
   - `CeilingHalf`：删除 `(count + 1) / 2` 个元素。
2. 新增静态类 `StrangeSorter`。
3. 为数组和列表分别定义上文列出的四个公共重载，并收敛到最全重载。
4. 为枚举和公共方法添加 XML 注释，明确写出以下 contract：
    - “有序”明确定义为 `Compare(prev, current) <= 0`。
    - 不提供单独的升序/降序参数；降序由调用方传入反向比较器实现。
    - 该算法不是传统意义上的排序。
    - 最终结果是有序子序列。
    - 幸存元素保持原相对顺序。
    - 数组方法不修改输入并返回结果数组，列表方法原地修改且不返回值。
    - 仅传 `values` 时使用 `Comparer<T>.Default`、`RemovalCountStrategy.FloorHalf`、`Random.Shared`。
    - 传 `values + comparer` 时仍默认使用 `RemovalCountStrategy.FloorHalf` 与 `Random.Shared`。
    - 传 `values + comparer + removalStrategy` 时默认使用 `Random.Shared`。
    - `comparer: null` 等同于省略比较器。
    - 首版不提供跳过 `comparer` 参数位置的其他公共重载。
    - 对默认顺序不明确的类型，调用方应显式传入比较器。
    - 默认比较失败时仅在实际发生比较时传播原始 `ArgumentException`；`0` 或 `1` 个元素的输入应直接成功。
    - 列表方法执行失败时可能已部分修改输入，不保证回滚。

完成标准：

1. 公共 API 编译通过。
2. 从方法签名和注释即可看出算法的破坏性与随机性边界。

### 步骤 2：实现共享核心逻辑

目标：在不扩大公共 API 的前提下，实现可复现、可测试、保持相对顺序的随机裁剪核心。

实施内容：

1. 在 `StrangeSorter` 内部实现私有辅助逻辑，至少覆盖：
   - 判断当前片段是否已按比较器非降序排列。
   - 根据 `RemovalCountStrategy` 计算本轮删除数。
   - 从当前索引范围中无放回地等概率抽取恰好 `k` 个删除位置。
   - 校验 `RemovalCountStrategy` 是否为已定义值。
2. 抽样实现建议采用“部分 Fisher-Yates 洗牌 + 标记删除位”的最小方案：
   - 为当前有效范围准备候选索引数组。
   - 执行前 `k` 次交换，得到 `k` 个互不重复的删除位置。
   - 将这些位置标记到布尔数组中。
   - 单次线性压缩幸存元素，天然保留原相对顺序。
3. 每轮结束后更新当前有效长度，并进入下一轮有序性检查。
4. 保证当序列仍无序时，每轮至少删除 1 个元素，因此算法必定终止。
5. 不引入额外随机抽象；测试复现完全依赖调用方传入的 `Random`。
6. 对外不捕获并包装来自比较器或随机源的异常，保持原始异常传播。

完成标准：

1. 对同一输入、同一比较器、同一种删除策略、同一个随机种子，结果可复现。
2. 所有轮次都不会改变幸存元素之间的相对顺序。
3. 内部实现不依赖反射或动态代码，保持 AOT 友好。

### 步骤 3：完成容器层封装

目标：让数组和列表 API 在共享算法语义之上，各自满足不同的可变性 contract。

实施内容：

1. 数组入口：
   - 在不修改输入数组的前提下执行裁剪；需要工作缓冲区时使用输入副本。
   - 若输入已排序，可直接返回原数组或返回内容相同的结果数组；公共 contract 不约束引用身份。
   - 裁剪结束后生成长度精确匹配的结果数组并返回。
2. 列表入口：
   - 直接在原 `List<T>` 上压缩幸存元素。
   - 压缩结束后移除尾部多余区间。
   - 若输入已排序，则保持实例不变并直接结束。
   - 调用完成后的 `values.Count` 表示最终剩余元素数。
   - 若比较器或随机源在执行过程中抛出异常，不保证列表回滚到调用前状态。
3. 确保数组版本与列表版本在同一组随机输入下遵守同一算法语义，只是可变性不同。

完成标准：

1. 数组方法不会修改传入数组。
2. 列表方法会修改传入列表本身，而不是替换为新实例。
3. 两种容器对“是否有序”的判定与删除策略保持一致。
4. 列表方法成功完成后，`values.Count` 等于最终剩余元素数。

### 步骤 4：编写测试

目标：围绕公开 contract 建立稳定测试，覆盖随机性控制、容器差异和边界行为。

实施内容：

1. 为数组和列表分别覆盖以下测试场景：
   - 空输入。
   - 单元素输入。
   - 已排序输入。
   - 明显无序输入。
   - 含重复值输入。
   - 使用自定义 `IComparer<T>` 的输入。
   - 使用反向比较器得到自然降序结果的输入。
2. 针对数组方法增加以下断言：
   - 返回结果有序。
   - 返回结果是原数组的子序列。
   - 原数组内容未被修改。
   - 输入已排序时，返回值与输入内容相同，且原数组内容不变。
3. 针对列表方法增加以下断言：
   - 调用后列表内容有序。
   - 调用后列表内容是调用前内容的子序列。
   - 调用后 `Count` 等于最终剩余元素数。
   - 输入已排序时列表实例不变且内容不变。
4. 随机性测试采用三类方式：
    - 固定 seed，验证相同输入可重复得到相同结果。
    - 必要时在测试项目中提供一个最小的 `Random` 派生测试替身，以精确驱动奇数长度策略分支。
    - 在同一输入、同一比较器、同一种删除策略、同一个测试随机源下，数组版本和列表版本得到相同的最终内容。
5. 参数较少的重载测试：
    - 省略比较器的重载对可默认比较类型可正常完成；对不可默认比较类型仅在实际发生比较时传播 `ArgumentException`；`0` 或 `1` 个元素输入应直接成功。
    - 省略 `Random` 的重载只验证结果满足公开不变量，不对 `Random.Shared` 的具体抽样轨迹作断言。
    - `FloorHalf` 与 `CeilingHalf` 的确定性差异验证统一通过显式传入 `Random` 的重载完成。
6. 参数验证测试：
    - `null` 数组抛出 `ArgumentNullException`。
    - `null` 列表抛出 `ArgumentNullException`。
    - 带 `Random` 参数的重载传入 `null` 随机源时抛出 `ArgumentNullException`。
    - 未定义的 `RemovalCountStrategy` 抛出 `ArgumentOutOfRangeException`。
7. 比较器行为测试：
    - 对没有默认比较能力的类型，未提供比较器时仅在实际发生比较时传播 `ArgumentException`；`0` 或 `1` 个元素输入应直接成功。
    - 对同一类型提供显式比较器后可正常完成裁剪。
8. 失败状态测试：
    - 数组版本在比较器或随机源抛出异常后，原数组内容保持不变。
    - 列表版本在比较器或随机源抛出异常后，仅验证异常原样传播；不对失败后的列表内容或是否已回滚作断言。
9. 测试中不要把“某个随机序列下的具体最终值”当作公共长期契约，除非该用例的目标就是验证复现性。

完成标准：

1. 测试覆盖数组和列表的主路径、边界条件和异常路径。
2. 测试重点验证不变量，而不是脆弱的随机细节。

### 步骤 5：验证与收尾

目标：确保库在解决方案内可构建、可测试，并在实现上保持符合仓库的 AOT 约束。

实施内容：

1. 运行 `dotnet build "StrangeSort.slnx"`。
2. 运行 `dotnet test "StrangeSort.slnx"`。
3. 默认不运行 `dotnet publish "StrangeSortTest/StrangeSortTest.csproj" -c Release` 这类会产生发布构建产物的命令；只有在用户明确要求发布一个版本时，才执行该命令。
4. 若构建、测试或用户明确要求时执行的发布过程暴露 API 语义不清的问题，优先修正文档和命名，再考虑调整实现。

完成标准：

1. 解决方案构建通过。
2. 测试全部通过。
3. 实现不引入明显违反仓库 AOT 约束的机制；若用户明确要求发布一个版本，则额外要求 AOT 发布检查未引入新的兼容性问题。

## 后续可扩展项（不纳入首版）

1. 增加区间重载，使其更接近 `Array.Sort(array, index, length, comparer)` 的形式。
2. 增加统计结果类型，暴露轮数、总删除数等诊断信息。
3. 若后续要扩展低分配缓冲区入口，优先增加 `Span<T>` API，再视需要评估 `Memory<T>`。
4. 视需要增加扩展方法，仅作为便捷入口，不替代静态主入口。
