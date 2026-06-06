# GitHub Packages 与 Release 自动发布计划

## 目标

为现有 `.NET 10` 类库建立稳定版自动发布流程：

1. 仅在推送稳定 tag `vX.Y.Z` 时触发正式发布。
2. 自动完成正式打包并发布到 GitHub Packages；日常构建与测试继续由现有 CI 负责。
3. 自动创建 GitHub Release，并在 Release 正文中写入当前 tag 对应的 changelog。
4. Release 附带 `nupkg` 与 `snupkg` 资产。
5. 复用并修改现有 `README.md`，让其同时适用于仓库首页与包说明页。
6. 保持 `CHANGELOG.md` 作为仓库内的全量变更记录文件，但该文件改为由正式发布工作流基于已推送稳定 tag 增量更新，并通过自动 PR 回写到 `main`。

## 已确认约束

1. 不发布 preview 版本；正式发布只接受稳定语义版本 tag。
2. 版本号以 Git tag 为唯一真源，`csproj` 不手工维护固定 `Version`。
3. tag 必须指向 `main` 分支历史中的提交。
4. 当前正式分发渠道为 GitHub Packages 与 GitHub Release，同时为未来扩展到 `NuGet.org` 预留结构空间。
5. `README.md` 采用“两者平衡”的写法：既保留项目介绍，也补齐包安装与使用说明。
6. `README.md` 中当前包安装说明先按现有 owner 编写，后续仓库迁移时再同步调整文案。
7. `main` 分支已启用提交保护，发布相关的 `CHANGELOG.md` 回写必须通过 PR 完成，不为 workflow 增加直推例外。
8. `CHANGELOG.md` 不采用每次全量重建策略，而是基于当前稳定 tag 进行增量 prepend。
9. 当前 tag 的 GitHub Release 正文与 `CHANGELOG.md` 新增段落必须复用同一份 `git-cliff` 生成结果，避免重复生成导致内容漂移。

## 实施步骤

### 步骤 1：补齐打包所需元数据与 README 基础信息

目标：让类库具备可被 `dotnet pack` 正确产出的最小发布条件，并让 `README.md` 与包发布场景一致。

实施内容：

1. 在 `StrangeSort/StrangeSort.csproj` 中补齐 NuGet 打包元数据，例如 `PackageId`、`Description`、`Authors`、`PackageTags`、`RepositoryUrl`、`RepositoryType`、`PackageProjectUrl`、`PackageReadmeFile`、`PackageLicenseFile`、`IncludeSymbols`、`SymbolPackageFormat`、`PublishRepositoryUrl`。
2. 确认 `LICENSE.txt` 被打入包内容，并优先使用 `PackageLicenseFile`，避免在许可证表达式尚未最终确认前写死 SPDX 表达式。
3. 修改现有 `README.md`，把“如何引用”改为以包安装和快速使用为主，不再以同解项目引用为默认路径。
4. 在 `README.md` 中补入 GitHub Packages 的核心安装说明、包名 `StrangeSort`、当前 owner 对应的源地址示例，并保留未来扩展到标准 NuGet 源的提示。
5. 保留 README 中现有项目概览和 Wiki 导航，但重新组织顺序，使仓库访客和包使用者都能快速找到入口。

完成标准：

1. 本地执行 `dotnet pack` 时能够生成包含 README 与许可证信息的 `nupkg`/`snupkg`。
2. `README.md` 不再误导用户优先采用源码级项目引用。

### 步骤 2：明确 CHANGELOG 回写策略与首轮基线

目标：让 `CHANGELOG.md` 与受保护的 `main` 分支、稳定 tag 发布流程兼容，并避免每次发布都全量重写 changelog。

实施内容：

1. 明确 `CHANGELOG.md` 不再作为打 tag 前置条件；稳定 tag 推送后，由 Release workflow 基于当前 tag 生成当前版本的 changelog 片段，并通过 PR 回写到 `main`。
2. 由于 `main` 已启用提交保护，`CHANGELOG.md` 的同步方式固定为“自动创建分支 + 自动创建 PR + 开启 auto-merge”，不为 changelog 回写配置直推 `main` 的例外项。
3. `CHANGELOG.md` 采用增量 prepend 策略：
   - 若文件已存在，则只把当前 tag 对应的 changelog 段落插入到文件顶部。
   - 若文件不存在，则首次直接创建该文件。
4. 首次落地前，手动基于现有历史 tag 生成一版基线 `CHANGELOG.md`，为后续自动 prepend 提供稳定起点。
5. 当前 tag 的 GitHub Release 正文与 `CHANGELOG.md` 新增段落必须复用同一份 `git-cliff` 输出，不在工作流中分别独立生成两次。
6. 明确 Release notes 与 `CHANGELOG.md` 的质量仍依赖 conventional commits；若提交不规范，生成内容可能缺项或可读性不足。

完成标准：

1. 计划内不要求在打 tag 前预生成并提交 `CHANGELOG.md`。
2. `main` 的提交保护规则保持不变，changelog 同步通过 PR 完成。
3. 首次基线初始化与后续增量 prepend 的边界清晰。
4. 当前 tag 的 Release 正文与 `CHANGELOG.md` 新增段落具备同源约束。

### 步骤 3：新增稳定 tag 触发的正式发布工作流

目标：建立只面向正式版本的 GitHub Actions 发布流水线。

实施内容：

1. 在 `.github/workflows/` 下新增独立发布工作流，不复用现有 `dotnet.yml`。
2. 触发条件使用稳定 tag push，例如 `v*.*.*`，并在工作流内再次校验 tag 是否符合稳定语义版本格式，拒绝带预发布后缀的 tag。
3. 在工作流中显式校验当前 tag 指向的提交属于 `main` 分支历史，校验失败则终止发布。
4. 使用 `actions/checkout` 与 `actions/setup-dotnet`，保持 `.NET 10` SDK 环境一致。
5. 正式发布工作流以 `dotnet pack` 为核心，不重复单独执行 `restore`、`build`、`test`；日常构建与测试继续由现有 `dotnet.yml` 在 `main`/PR 流程中负责。
6. 从 tag 提取纯版本号传入打包命令，生成 `nupkg` 与 `snupkg` 后发布到 GitHub Packages。
7. 工作流权限仅开放所需最小集合，至少覆盖 `contents: write`、`packages: write` 与 `pull-requests: write`。
8. 若要让 changelog PR 在分支保护下可靠触发检查并开启 auto-merge，用Squash and Merge，优先使用 GitHub App token 或专用 PAT，而不是只依赖默认 `GITHUB_TOKEN`。

完成标准：

1. 推送合法稳定 tag 时，工作流能够从零完成 tag 校验、打包与包发布。
2. 推送非法 tag、预发布 tag 或非 `main` 历史上的 tag 时，工作流会明确失败而不是静默发布。

### 步骤 4：在发布工作流中同步 CHANGELOG PR 与 GitHub Release

目标：让每个稳定 tag 在同一个发布工作流中，先生成 changelog 更新 PR 并开启 auto-merge，再复用同源内容创建 GitHub Release。

实施内容：

1. 在发布工作流中安装或调用 `git-cliff`，基于现有 `cliff.toml` 和当前稳定 tag 只生成一次“当前 tag 对应的 changelog 片段”，并保存为中间文件。
2. 使用这份中间文件更新 `CHANGELOG.md`：
   - 若 `CHANGELOG.md` 已存在，则把当前 tag 片段 prepend 到文件顶部。
   - 若 `CHANGELOG.md` 不存在，则直接用该内容创建文件。
3. 为 changelog 更新创建专用分支，分支命名固定为 `automation/changelog-vX.Y.Z`。
4. changelog 提交信息固定为 `docs: 更新 CHANGELOG 文件 vX.Y.Z`。
5. 自动创建指向 `main` 的 PR，并采用固定格式：
   - 标题：`docs: 更新 CHANGELOG 文件 vX.Y.Z`
   - 正文：说明来源 tag、生成工具 `git-cliff`、更新文件 `CHANGELOG.md`，并注明该 PR 由 Release workflow 自动创建且已开启 auto-merge。可用现有模板。
6. 工作流需要先完成 changelog 分支提交、PR 创建和 auto-merge 开启；工作流不等待 PR 实际合并完成，但若 PR 创建失败或无法开启 auto-merge，则中止后续 Release 创建。
7. 创建 GitHub Release 时，直接复用步骤 1 生成的同一份 changelog 片段作为 Release 正文，不再单独调用第二次 `git-cliff`。
8. 使用 GitHub 官方 action 或 `gh` CLI 创建 GitHub Release，并把 `nupkg` 与 `snupkg` 作为资产附加到对应 tag 的 Release。
9. Release 标题、版本号和正文内容统一以当前稳定 tag 为准。
10. 确保重复运行时具备合理幂等性：
   - 已存在同 tag changelog 分支或 PR 时，优先更新现有分支和 PR，而不是重复创建。
   - 已存在同 tag Release 时，要么更新同一 Release，要么在设计上明确拒绝重复创建。

完成标准：

1. 每个稳定 tag 都会触发一个 changelog PR 和一个 GitHub Release。
2. Release 页面显示的正文与 `CHANGELOG.md` 新增段落来自同一份源内容。
3. `CHANGELOG.md` 的更新策略是增量 prepend，而不是每次全量重建。
4. Release 页面可以直接看到当前 tag 的 changelog，并下载 `nupkg` 与 `snupkg`。

### 步骤 5：补齐验证与维护说明

目标：让发布流程在首次合入前具备可验证性，并降低后续维护成本。

实施内容：

1. 本地验证 `dotnet pack` 的发布参数组合，确保 tag 注入版本号后可正常产包，并确认 README 与许可证会正确进入包内容。
2. 检查工作流 YAML 是否与当前仓库结构一致，尤其是 `.slnx`、项目路径、输出目录与 README/许可证文件路径。
3. 在仓库文档中补一段简短维护说明，说明正式发布依赖哪些前置条件：
   - 日常代码改动先通过普通 PR 合入 `main`
   - 维护者在 `main` 上的目标提交创建并推送稳定 tag `vX.Y.Z`
   - Release workflow 自动打包、发包、创建 changelog PR 并开启 auto-merge、再创建 GitHub Release
   - 首次自动化落地前，先手动生成并提交一版基线 `CHANGELOG.md`
4. 首次落地后使用测试 tag 或受控分支演练一次完整流程，确认 GitHub Packages、changelog PR、auto-merge 配置、Release 正文和 Release 资产均符合预期。

完成标准：

1. 维护者知道完整发布路径，不需要反向阅读 workflow 才能理解流程。
2. 首次正式发布前已有至少一次受控演练或等价验证记录。
3. 文档已明确说明 `main` 不开直推例外，changelog 同步通过自动 PR 完成。
