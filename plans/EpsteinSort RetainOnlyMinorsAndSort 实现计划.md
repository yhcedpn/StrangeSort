# EpsteinSort RetainOnlyMinorsAndSort 实现计划

## 目标

为项目新增一个带固定过滤规则的“排序”能力。该能力面向由数值类型构成的数组或列表，先删除所有小于 `0` 和大于等于 `18` 的元素，再对剩余元素进行排序，最终返回结果或原地改写输入集合。

本次实现需要与当前代码库风格和 .NET 内置排序体验保持一致：

1. 公开类型采用独立命名空间和独立静态类型，不修改现有 `StalinSort` 与 `ThanosSort` 的公开契约。
2. 数组重载不修改输入数组，而是返回新的结果数组。
3. 列表重载原地修改输入列表，保持与 `List<T>.Sort` 以及仓库现有列表重载相近的使用感受。
4. 排序阶段直接复用 .NET 内置排序，而不是自研排序实现。
5. 行为必须可测试，且测试用例能够稳定锁定过滤、排序、异常和可变性语义。

## 设计定稿

### 类型与命名

1. 在 `StrangeSort/EpsteinSort/` 目录下新增 `EpsteinSorter.cs`。
2. 命名空间使用 `StrangeSort.EpsteinSort`。
3. 公开类型名为 `EpsteinSorter`。
4. 公开方法名固定为 `RetainOnlyMinorsAndSort`。

### 类型范围与约束

1. 公开泛型方法统一使用 `where T : INumber<T>`。
2. 第一版目标支持具有自然数值语义且可排序的常见数值类型，例如整数类型、浮点类型、`decimal` 与 `BigInteger`。
3. 第一版不支持 `Complex`，因为其没有自然全序，不能直接匹配本次 API 的排序语义。
4. 不通过 `dynamic`、反射或运行时判型兜底，以避免破坏 AOT 兼容性和类型安全。

### 公开 API

计划提供以下公开重载：

```csharp
public static T[] RetainOnlyMinorsAndSort<T>(T[] values)
    where T : INumber<T>;

public static T[] RetainOnlyMinorsAndSort<T>(T[] values, IComparer<T>? comparer)
    where T : INumber<T>;

public static void RetainOnlyMinorsAndSort<T>(List<T> values)
    where T : INumber<T>;

public static void RetainOnlyMinorsAndSort<T>(List<T> values, IComparer<T>? comparer)
    where T : INumber<T>;
```

本次不提供以下公开能力：

1. 不提供 `Comparison<T>` 重载。
2. 不提供 `IList<T>`、`IEnumerable<T>`、`Span<T>` 或 `Memory<T>` 公共重载。
3. 不提供可配置的上下界参数，本次过滤区间固定为 `[0, 18)`。
4. 不返回删除数量、删除索引或其他包装结果。
5. 不引入额外策略枚举，因为本次行为只有一个固定过滤规则。

### 过滤与排序语义

1. 保留条件固定为：`value >= 0 && value < 18`。
2. 所有小于 `0` 的值必须删除。
3. 所有大于等于 `18` 的值必须删除。
4. 浮点类型中的 `NaN`、`-Infinity` 和 `+Infinity` 视为不在 `[0, 18)` 内，必须删除。
5. 过滤阶段永远按固定数值区间 `[0, 18)` 判断，不受 `comparer` 影响。
6. 排序阶段使用 `comparer ?? Comparer<T>.Default`。
7. 第一版不承诺稳定排序，行为与 `Array.Sort` / `List<T>.Sort` 的既有语义保持一致。

### 异常与输入契约

1. 数组或列表输入为 `null` 时，抛出 `ArgumentNullException`。
2. 当 `comparer` 为 `null` 时，使用 `Comparer<T>.Default`。
3. 数组重载在处理过程中如果比较器抛异常，不得修改输入数组。
4. 列表重载允许在过滤完成、排序开始后因比较器异常而出现部分修改，不要求回滚；需要在文档注释中明确说明这一点。
5. 空输入和单元素输入应直接成功。

## 实现步骤

### 步骤 1：新增核心公开类型与 API 骨架

目标：建立 `EpsteinSorter` 的文件、命名空间、XML 注释和公开重载骨架，使其目录结构、命名和说明文档与现有项目风格一致。

实现要求：

1. 新增 `StrangeSort/EpsteinSort/EpsteinSorter.cs`。
2. 为数组和列表分别提供默认比较器重载与 `IComparer<T>?` 重载。
3. 为全部泛型方法添加 `where T : INumber<T>` 约束。
4. 在类型和方法注释中明确说明：
5. 该能力会先过滤 `[0, 18)` 外的值，再进行排序。
6. 数组重载不会修改输入数组。
7. 列表重载会原地修改输入列表。
8. 列表重载若在排序阶段比较器抛异常，列表可能已经部分修改且不会回滚。

完成标准：

1. 代码文件可编译。
2. 公开 API 名称与计划一致。
3. 注释完整且使用简体中文。

### 步骤 2：实现数组重载的过滤与排序逻辑

目标：完成数组版本 `RetainOnlyMinorsAndSort` 的实际行为，确保返回值仅包含 `[0, 18)` 内元素，且输入数组保持不变。

实现要求：

1. 对 `values` 执行空值校验。
2. 将 `comparer ??= Comparer<T>.Default` 作为统一比较器入口。
3. 使用 `T.Zero` 和 `T.CreateChecked(18)` 构造过滤边界。
4. 使用线性扫描把合法元素复制到缓冲区前缀。
5. 过滤完成后，对保留前缀调用 `Array.Sort`。
6. 返回仅包含保留元素的新数组。
7. 不在原数组上做任何原地修改。

完成标准：

1. 数组输入不会被修改。
2. 返回结果仅包含 `[0, 18)` 内元素。
3. 返回结果满足比较器定义的排序条件。

### 步骤 3：实现列表重载的原地压缩与排序逻辑

目标：完成列表版本 `RetainOnlyMinorsAndSort` 的原地处理，尽量复用与数组版本一致的过滤规则，同时保持列表 API 的使用体验。

实现要求：

1. 对 `values` 执行空值校验。
2. 使用与数组版本相同的过滤边界和保留判定语义。
3. 通过写指针覆盖方式保留合法元素，避免逐项 `RemoveAt` 带来的重复移动成本。
4. 扫描完成后，调用一次 `RemoveRange` 删除尾部多余元素。
5. 对剩余元素调用 `List<T>.Sort(comparer)` 完成最终排序。
6. 如果列表原本全都不在范围内，结果应为空列表。

完成标准：

1. 原地处理后的列表仅包含 `[0, 18)` 内元素。
2. 列表内容满足比较器定义的排序条件。
3. 不引入额外公开辅助类型。

### 步骤 4：补齐测试用例并锁定公共语义

目标：在 `StrangeSortTest/EpsteinSort/` 下新增测试文件，覆盖输入校验、边界、特殊数值、自定义比较器和可变性语义，确保后续重构不会破坏这套约定。

建议测试清单：

1. `null` 数组输入抛出 `ArgumentNullException`。
2. `null` 列表输入抛出 `ArgumentNullException`。
3. 空数组返回空数组。
4. 空列表处理后保持为空。
5. 单元素数组在范围内时返回等价内容，且输入数组不变。
6. 单元素数组不在范围内时返回空数组，且输入数组不变。
7. 单元素列表在范围内时处理后保持原值。
8. 单元素列表不在范围内时处理后变为空列表。
9. 边界语义测试：`0` 保留，`18` 删除，负数删除。
10. 混合输入先过滤再排序，数组输入保持不变。
11. 混合输入先过滤再排序，列表输入被原地修改。
12. `null comparer` 与 `Comparer<T>.Default` 行为一致。
13. 自定义比较器可以改变最终排序顺序。
14. 逆序比较器只影响排序阶段，不影响 `[0, 18)` 过滤规则。
15. `float` 或 `double` 中的 `NaN`、`-Infinity`、`+Infinity` 被删除。
16. `BigInteger` 可以正常参与过滤与排序。
17. 数组重载在比较器抛异常时不修改输入数组。
18. 列表重载在比较器抛异常时传播异常。

完成标准：

1. 测试命名与现有测试风格一致。
2. 测试不依赖随机性。
3. 测试覆盖数组与列表两个公开入口。

### 步骤 5：执行验证并检查对外一致性

目标：通过构建和测试确认新增功能可用，并检查命名、注释和行为描述是否与既有代码风格一致。

验证要求：

1. 执行 `dotnet test .\StrangeSortTest\StrangeSortTest.csproj`。
2. 如有必要，执行 `dotnet build .\StrangeSort\StrangeSort.csproj` 以检查类库单独构建结果。
3. 确认 XML 注释风格与现有 `StalinSorter`、`ThanosSorter` 保持接近。
4. 确认没有意外扩大公开 API 面。

完成标准：

1. 构建通过。
2. 全量测试通过。
3. `EpsteinSorter` 的数组与列表行为符合本计划约定。

## 验收口径

功能完成后，应满足以下验收条件：

1. 仓库中存在新的 `StrangeSort.EpsteinSort` 命名空间与 `EpsteinSorter` 类型。
2. 用户可以对 `T[]` 和 `List<T>` 调用 `RetainOnlyMinorsAndSort`。
3. 数组调用后输入保持不变，返回值仅包含 `[0, 18)` 内元素，且已完成排序。
4. 列表调用后原列表被原地过滤并排序。
5. `comparer` 只影响最终排序，不影响固定过滤区间 `[0, 18)`。
6. 行为由测试稳定覆盖，包括边界值、浮点特殊值、自定义比较器和异常传播场景。
