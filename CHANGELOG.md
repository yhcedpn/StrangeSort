## [1.0.0] - 2026-06-14

### 🚀 Features

- 实现 PruneUntilSorted 随机裁剪算法
- 实现 StalinSort RemoveOutOfOrder 算法
- 实现 EpsteinSort RetainOnlyMinorsAndSort
- 添加 BrezhnevSort RewriteAsOrdinalSequence (#5)
- 添加 TrumpSorter ShuffleAndWin 算法实现 (#11)

### 🐛 Bug Fixes

- 修复 sync-wiki GitHub Action 配置文件的格式问题 (#16)

### 🚜 Refactor

- [**breaking**] 将 StrangeSorter 重命名为 ThanosSorter 并调整 ThanosSort 目录结构
- 清理 dotnet format info 级诊断

### 📚 Documentation

- 添加 PruneUntilSorted 实现计划
- 补充技能文件约定与技能说明
- 添加英文注释中文化整改计划
- 中文化 StrangeSort 公共注释
- 补充开发计划文件的写入约定
- 添加审阅意见问题修复计划
- 明确列表重载异常时的失败语义
- 补充计划文件历史计划处理约定
- 添加 ThanosSort 重命名与目录调整计划
- 添加 StalinSort RemoveOutOfOrder 实现计划
- 添加 README.md 和 LICENSE.txt
- 添加 EpsteinSort RetainOnlyMinorsAndSort 实现计划
- 完善 README 项目说明
- 补充 Git 与 GitHub 交互技能说明
- 添加 dotnet format info 级别问题修复计划
- 补充 Git 标签附注要求
- 添加 CHANGELOG
- 补充 Git 与 GitHub 交互说明
- 补充 GitHub Pull Request 规范
- 补充 Pull Request 模板中的关联 Issue 项
- 完善 PR 关联 Issue 说明与 changelog 维护流程 (#7)
- 补充 README 中的 BrezhnevSort 算法说明 (#9)
- 明确 PR 无关联 Issue 时的模板填写方式 (#10)
- 在 AGENTS.md 补充程序的运行时的说明 (#12)
- 补充 README 中的 TrumpSort 算法说明 (#13)
- 补充算法契约规范并清理旧仓库治理文件 (#14)
- 建立 GitHub Wiki 文档结构并精简 README (#15)
- 补充行为契约同步要求并调整 README 文案 (#18)
- 重组文档导航并明确规范职责 (#20)
- 更新 README 中的 Wiki 文档入口 (#21)
- 清理掉过时的 CHANGELOG 文件 (#22)

### 🧪 Testing

- 添加 PruneUntilSorted 随机裁剪算法的契约测试
- 补充 comparer 异常传播测试
- 添加 StalinSort RemoveOutOfOrder 算法的测试
- 添加 EpsteinSort RetainOnlyMinorsAndSort 的测试

### ⚙️ Miscellaneous Tasks

- 添加 .gitattributes 和 .gitignore
- 添加项目文件
- 为项目 StrangeSortTest 添加 StrangeSort 项目的引用
- 添加 git-cliff 配置
- 添加 GitHub Pull Request 模板
- 更新 sync-wiki 使用的 actions/checkout 依赖项 (#17)
- 添加 CI 与 GitHub Packages/Release 自动发布支持 (#19)
