# Jx.Cms 项目说明

## 项目概述
Jx.Cms 是一个基于 ASP.NET 9.0 的内容管理系统(CMS)，采用前后端分离架构：
- 前端使用传统 ASP.NET Core MVC，服务端渲染，对 SEO 更友好
- 后台管理使用 Blazor Server，配合 BootstrapBlazor 组件库快速开发
- 支持插件热插拔和动态主题更换功能

## 技术栈
- 底层框架：Furion
- ORM：FreeSql
- 后台UI：BootstrapBlazor
- 数据库支持：SQLite, MySQL, SQL Server, Oracle, PostgreSQL

## 项目结构

### 核心模块 (src/core/)
- **Jx.Cms.Common**: 公共组件和工具类
- **Jx.Cms.Web**: 主Web应用程序，包含前端MVC和后台Blazor
- **Jx.Cms.DbContext**: 数据库上下文和配置
- **Jx.Cms.Themes**: 主题系统支持
- **Jx.Cms.Plugin**: 插件系统支持
- **Jx.Cms.Install**: 安装向导模块

### 插件系统 (src/plugin/)
- **DemoPlugin**: 示例插件
- **HighlightingPlugin**: 代码高亮插件
- **CnBlogAsync**: 博客园同步插件

### 主题系统 (src/theme/)
- **Blogs**: 默认博客主题

## 构建和运行

### 环境要求
- .NET 9.0 SDK
- Visual Studio 2022 或 Visual Studio Code

### 运行命令
```bash
# 还原依赖
dotnet restore

# 构建项目
dotnet build

# 运行项目
cd src/core/Jx.Cms.Web
dotnet run
```

### 数据库配置
数据库配置通过 `DbConfig` 类管理，支持多种数据库类型。首次运行会自动进入安装向导完成数据库初始化。

## 开发约定

### 插件开发
- 插件项目需使用 `Microsoft.NET.Sdk.Razor` SDK
- 需要创建 `plugin.json` 文件描述插件信息
- 构建后会自动复制到 `Jx.Cms.Web/Theme` 目录

### 主题开发
- 主题项目需使用 `Microsoft.NET.Sdk.Razor` SDK
- 需要创建 `theme.json` 文件描述主题信息
- 静态资源通过 `EmbeddedResource` 包含

### 代码规范
- 使用 C# 最新语言特性
- 启用 `ImplicitUsings`
- 禁用 `Nullable` 检查
- 遵循 Furion 框架的开发模式

## 扩展开发

### 添加新插件
1. 在 `src/plugin` 目录下创建新的插件项目
2. 实现 `RazorPlugin` 或相关接口
3. 添加 `plugin.json` 配置文件
4. 在解决方案文件中添加项目引用

### 添加新主题
1. 在 `src/theme` 目录下创建新的主题项目
2. 创建必要的 Razor 页面和布局
3. 添加 `theme.json` 配置文件
4. 在解决方案文件中添加项目引用

## 部署说明
项目支持 Docker 部署，使用 Linux 容器。Dockerfile 位于项目根目录。