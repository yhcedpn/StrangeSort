# StrangeSort 开发规范

## 架构设计要求

1. 保持高内聚、低耦合。
2. 一个类型只负责一类明确职责，避免功能堆叠。
3. 优先组合而不是继承。
4. 公共 API 变更必须谨慎，未经明确要求不要新增、删除、重命名或重排公开重载。
5. 默认参数语义、泛型约束、枚举合法值和异常行为未经明确要求不得改变。
6. 实现可以重构，公开行为不能漂移。

## 变更防漂移规定

1. 修改算法前，先明确这次改动属于“修复实现偏离现有协议”还是“有意修改协议”。
2. 如果只是修复实现，必须保持本文件中的公开行为协议不变，并补测试证明协议仍成立。
3. 如果有意修改协议，必须在同一次改动中同步更新本文件、测试和受影响的 `README.md` 或 `wiki/`。
4. 禁止只改实现不改规范，也禁止只改测试去迎合实现。
5. 禁止把这些奇异算法静默改造成正常排序算法。
6. 禁止在没有公开说明的情况下改变空输入、单元素、重复值、随机源为空、比较器为空、非法枚举值、不可比较类型和溢出等边界行为。

## 算法行为协议

### StalinSort

公开 API 族：提供 `T[]` 和 `List<T>` 两组 `RemoveOutOfOrder` 重载，支持默认比较器和显式 `IComparer<T>`。

1. 从左到右扫描输入，第一个元素总是保留。
2. 仅当 `comparer.Compare(lastKept, current) <= 0` 时保留当前元素，否则删除。
3. 输出始终是输入的子序列，保留元素相对顺序不变；禁止交换、重排或额外排序。
4. 数组重载不修改输入数组。`values.Length <= 1` 时返回原数组引用；长度大于 `1` 时总返回新数组，即使输入本身已满足顺序。
5. `List<T>` 重载原地删除违序元素。
6. `comparer == null` 时使用 `Comparer<T>.Default`；只有真的发生比较时才要求 `T` 可比较。
7. `values == null` 时抛 `ArgumentNullException`；默认比较器不可用且运行时需要比较时抛 `ArgumentException`；自定义比较器异常原样传播。
8. 数组重载异常后输入保持不变；`List<T>` 重载异常后可能已部分修改，且不回滚。

### ThanosSort

公开 API 族：提供 `T[]` 和 `List<T>` 两组 `PruneUntilSorted` 重载，支持默认比较器、显式 `IComparer<T>`、`RemovalCountStrategy` 和 `Random`。

1. 反复检查当前序列是否有序；如果无序，就随机删除当前序列的一半元素；重复直到结果有序。
2. 输出始终是输入的有序子序列，且幸存元素保持原有相对顺序。
3. `RemovalCountStrategy.FloorHalf` 在奇数长度时删除 `count / 2`；`RemovalCountStrategy.CeilingHalf` 在奇数长度时删除 `(count + 1) / 2`。
4. 数组重载不修改输入数组。`values.Length <= 1` 或输入本身已排序时返回原数组引用；其余情况下返回新数组。
5. `List<T>` 重载原地随机删除元素，直到列表有序。
6. `comparer == null` 时使用 `Comparer<T>.Default`；`random == null` 时使用 `Random.Shared`；默认删除策略为 `FloorHalf`。
7. 使用相同输入、比较器、删除策略和显式随机种子时，数组重载可复现，列表重载可复现，且两者结果内容一致。
8. `values == null` 或显式传入的 `random == null` 时抛 `ArgumentNullException`；非法 `removalStrategy` 抛 `ArgumentOutOfRangeException`；默认比较器不可用且运行时需要比较时抛 `ArgumentException`；比较器和随机源异常原样传播。
9. 数组重载异常后输入保持不变；`List<T>` 重载异常后可能已部分修改，且不回滚。

### EpsteinSort

公开 API 族：提供 `T[]` 和 `List<T>` 两组 `RetainOnlyMinorsAndSort` 重载，支持默认比较器和显式 `IComparer<T>`，其中 `T : INumber<T>`。

1. 算法分两步：先过滤，再排序。
2. 过滤规则固定为只保留 `[0, 18)` 内的值。
3. 对浮点类型，`NaN`、`-Infinity` 和 `+Infinity` 都会被删除。
4. 过滤阶段不受比较器影响；比较器只参与最终排序。
5. 数组重载不修改输入数组，并始终返回结果数组，不返回原数组引用。
6. `List<T>` 重载先原地删除所有不在 `[0, 18)` 内的元素，再对保留元素排序。
7. `comparer == null` 时使用 `Comparer<T>.Default`。
8. `values == null` 时抛 `ArgumentNullException`。如果比较器在排序阶段抛出异常，底层排序 API 会抛 `InvalidOperationException`，原始比较器异常通常作为 `InnerException` 暴露。
9. 数组重载异常后输入保持不变；`List<T>` 重载排序失败时，过滤阶段可能已经完成，且不回滚。

### BrezhnevSort

公开 API 族：提供 `T[]` 和 `List<T>` 两组 `RewriteAsOrdinalSequence` 重载，其中 `T : INumber<T>`。

1. 不比较、筛选或重排输入元素。
2. 输出只由输入长度决定。长度为 `n` 时，结果固定为 `1..n`。
3. 输入元素原值不会被读取，也不会影响结果。
4. 数组重载不修改输入数组，并始终返回新的结果数组。
5. `List<T>` 重载原地把整个列表改写为 `1..n`。
6. `values == null` 时抛 `ArgumentNullException`。
7. 当输入长度超出 `T` 的可表示范围时抛 `OverflowException`。
8. 数组重载异常后输入保持不变；`List<T>` 重载溢出时可能已部分修改，且不回滚。

### TrumpSort

公开 API 族：提供 `T[]` 和 `List<T>` 两组 `ShuffleAndWin` 重载，支持默认随机源和显式 `Random`。

1. 对输入执行一次 Fisher-Yates 随机重排。
2. 不保证结果有序，不删除元素，也不改写元素值。
3. 输出与输入长度一致，元素多重集一致。
4. 数组重载不修改输入数组，并始终返回结果数组，不返回原数组引用。
5. `List<T>` 重载原地随机重排。
6. `random == null` 时使用 `Random.Shared`。
7. 使用相同输入和显式随机种子时，数组重载可复现，列表重载可复现，且两者结果内容一致。
8. `values == null` 或显式传入的 `random == null` 时抛 `ArgumentNullException`；随机源异常原样传播。
9. 数组重载异常后输入保持不变；`List<T>` 重载异常后可能已部分修改，且不回滚。
