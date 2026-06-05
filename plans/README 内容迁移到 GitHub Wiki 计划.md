# README 内容迁移到 GitHub Wiki 计划

## 背景

当前 `README.md` 同时承担仓库首页说明与算法详细参考文档两类职责，篇幅较长，不利于用户快速理解项目定位。

本次计划的目标是把适合长期维护的详细参考内容迁移到 GitHub Wiki，并让 `README.md` 回归“项目简介 + 快速上手 + 文档导航”的首页职责。

## 重要前提

`specs/StrangeSort 项目开发规范.md` 才是项目公开算法行为的唯一契约来源。

因此，本次迁移的目标是调整文档职责分工，而不是迁移或改写契约本身。`README.md` 与 GitHub Wiki 的内容必须与规范文件保持一致，但二者都不是契约来源。

## 迁移范围

保留在 `README.md` 的内容：

1. 项目标题与简介。
2. 五个算法的一句话介绍。
3. `项目概览`。
4. `如何引用`。
5. `算法总览`。
6. 指向 Wiki 的文档入口。
7. `作者的话` 与 `鸣谢`。

迁移到 GitHub Wiki 的内容：

1. `StalinSort` 详细参考。
2. `ThanosSort` 详细参考。
3. `EpsteinSort` 详细参考。
4. `BrezhnevSort` 详细参考。
5. `TrumpSort` 详细参考。

每个算法页面都应包含：

1. 算法说明。
2. 公共 API。
3. 输入与输出行为。
4. 异常说明。
5. 调用示例。

## Wiki 目标结构

本次迁移统一采用以下页面命名：

1. `Home`
2. `Getting Started`
3. `Algorithm Overview`
4. `StalinSort Reference`
5. `ThanosSort Reference`
6. `EpsteinSort Reference`
7. `BrezhnevSort Reference`
8. `TrumpSort Reference`
9. `_Sidebar`

对应的 Wiki 文件名应为：

1. `Home.md`
2. `Getting-Started.md`
3. `Algorithm-Overview.md`
4. `StalinSort-Reference.md`
5. `ThanosSort-Reference.md`
6. `EpsteinSort-Reference.md`
7. `BrezhnevSort-Reference.md`
8. `TrumpSort-Reference.md`
9. `_Sidebar.md`

## 实施步骤

### 步骤 1：准备 Wiki 仓库工作区

目标：让 Wiki 内容能够像普通 Markdown 文档一样被版本化维护。

变更范围：

1. 确认仓库 Wiki 已启用并已创建初始页面。
2. 克隆 `https://github.com/yhcedpn/StrangeSort.wiki.git` 到本地工作目录。
3. 在 Wiki 仓库中建立上述目标页面文件骨架。

完成标准：

1. 本地存在可编辑的 Wiki 仓库副本。
2. 所有目标页面文件均已创建并可继续填充内容。

### 步骤 2：建立 Wiki 导航页与侧边栏

目标：先搭好导航骨架，再填充详细内容，避免后续交叉链接混乱。

变更范围：

1. 编写 `Home.md`，包含项目简介、文档导航与页面入口。
2. 编写 `Getting-Started.md`，承接当前 `README.md` 中适合作为入门说明的内容。
3. 编写 `Algorithm-Overview.md`，放置算法总览表，并为每个算法添加指向对应参考页的链接。
4. 编写 `_Sidebar.md`，把所有页面纳入统一侧边导航。

完成标准：

1. Wiki 首页可作为完整导航入口使用。
2. 所有核心页面可通过 `_Sidebar.md` 和正文链接互相到达。

### 步骤 3：拆分五个算法参考页

目标：将 `README.md` 中的长篇算法参考内容拆分成单页、可独立阅读的 Wiki 文档。

变更范围：

1. 把 `README.md` 中 `StalinSort` 章节迁移到 `StalinSort-Reference.md`。
2. 把 `ThanosSort` 章节迁移到 `ThanosSort-Reference.md`。
3. 把 `EpsteinSort` 章节迁移到 `EpsteinSort-Reference.md`。
4. 把 `BrezhnevSort` 章节迁移到 `BrezhnevSort-Reference.md`。
5. 把 `TrumpSort` 章节迁移到 `TrumpSort-Reference.md`。
6. 为每个页面补齐返回 `Home`、`Getting Started`、`Algorithm Overview` 的导航链接。

完成标准：

1. 五个算法页面均能脱离 `README.md` 独立阅读。
2. 页面内的代码示例、API 标题、异常说明与原始契约描述保持一致，不产生语义漂移。

### 步骤 4：精简 README 并补充 Wiki 入口

目标：让 `README.md` 回归首页职责，同时保留必要摘要与稳定入口。

变更范围：

1. 保留项目简介、项目概览、引用方式、算法总览和仓库附加说明。
2. 删除已迁移到 Wiki 的五个算法详细参考章节。
3. 在 `README.md` 中新增文档入口区块，链接到：
   - Wiki 首页
   - `Getting Started`
   - `Algorithm Overview`
   - 五个算法参考页
4. 确保 `README.md` 中仍保留足够的项目摘要信息，使首次访问仓库的用户无需进入 Wiki 也能理解项目定位。

完成标准：

1. `README.md` 明显短于迁移前版本。
2. `README.md` 中所有 Wiki 链接都指向正确页面。
3. `README.md` 不再承载重复的长篇算法参考内容。

### 步骤 5：同步修正文档引用与规范描述

目标：避免仓库内仍残留旧的文档定位说明。

变更范围：

1. 搜索仓库内是否存在对旧 README 锚点或“README/Wiki 是契约来源”的引用。
2. 若存在相关说明，则改为指向新的 Wiki 页面或新的职责划分描述。
3. 若测试、脚本或辅助文档中存在依赖旧文档结构的内容，则一并修正。

完成标准：

1. 仓库中不存在与新文档结构冲突的说明。
2. 不存在明显失效的 README 内部锚点引用。

### 步骤 6：统一校验链接与渲染结果

目标：在提交前完成一次文档级验收，防止页面创建了但互链失效。

变更范围：

1. 校验 `README.md` 中所有 Wiki 外链。
2. 校验 Wiki 首页、总览页、侧边栏与算法参考页之间的互链。
3. 校验页面标题、文件名和链接 slug 是否一致。
4. 目视检查 Markdown 渲染效果，确认代码块、表格和列表显示正常。

完成标准：

1. `README.md -> Wiki` 的入口链路完整可用。
2. Wiki 内部页面可互相正确跳转。
3. 没有因页面命名不一致导致的 404 或错误跳转。

## 预期产物

完成本计划后，应得到以下产物：

1. 一个精简后的 `README.md`。
2. 与当前契约来源规则保持一致的文档说明。
3. 一套完整的 Wiki 页面：`Home`、`Getting Started`、`Algorithm Overview`、五个算法参考页以及 `_Sidebar`。
4. 一组经过校验的稳定文档链接。

## 非目标

本次计划不包含以下内容：

1. 不修改任何算法实现。
2. 不改变任何公开算法行为。
3. 不以“文档迁移”为名重写 API、示例语义或异常定义。
4. 不对未被本次迁移影响的历史计划文件做修改。
