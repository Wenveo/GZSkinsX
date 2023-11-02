# 格子

一个适用于国服（腾讯服）英雄联盟 (League of Legends) 的第三方模组工具。

<a href="https://apps.microsoft.com/detail/%E6%A0%BC%E5%AD%90/9PHKH2G4X4WM?launch=true
	&mode=mini">
	<img src="https://get.microsoft.com/images/zh-cn%20dark.svg" width="200"/>
</a>

![Screenshot1](https://raw.githubusercontent.com/Wenveo/GZSkinsX/dev/Artifacts/Screenshot(1).png)
![Screenshot2](https://raw.githubusercontent.com/Wenveo/GZSkinsX/dev/Artifacts/Screenshot(2).png)

## 构建

### 1. 必要环境
- [Visual Studio 2022](https://visualstudio.microsoft.com/vs/) 以及以下组件：
    - Windows 11 SDK (10.0.22621.0)
    - .NET 7 SDK
    - Git for Windows
- [Windows App SDK 1.4](https://learn.microsoft.com/windows/apps/windows-app-sdk/downloads#current-releases)

### 2. 克隆此仓库

```ps
git clone https://github.com/Wenveo/GZSkinsX
```

### 3. 编译此项目

- 在 Visual Studio 中打开 `GZSkinsX.sln` 解决方案，找到 `GZSkinsX.App` 右键并点击 `设为启动项目`。
- 展开解决方案视图中 `GZSkinsX.Appx.Contracts` 项目的文件列表，找到 `Appx/AppxContext.Services.g.tt` T4 模板文件将其打开，手动保存以从模板中生成必要的代码。
- 等待包管理器还原依赖项，然后按 F5 编译运行！

## 贡献
如果感觉有哪些地方不对劲，或者如果您觉得缺少某些功能，欢迎提出意见或有关新功能的想法！
并且接受任何对 Bug 修复或功能改进的拉取请求。
