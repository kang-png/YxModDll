# YxMod

YxMod 是一个基于 C# 的 Unity 注入模组框架，适用于《人类一败涂地》。该项目旨在提供强大而灵活的功能扩展能力，包括玩家属性控制、自定义 UI、音频卸载优化、多人生效等。

YxMod is a C# modding framework for *Human: Fall Flat*. It enables custom gameplay logic, runtime UI, memory optimizations, and multiplayer-compatible enhancements via Harmony patches and UnityDoorstop injection.

---

## 🚀 功能特色 Features

- 🔧 Harmony 补丁支持（Prefix / Postfix / Transpiler）
- 🎮 玩家能力扩展（飞行、超级跳跃、抓力控制、物理参数等）
- 🖥️ 内置 IMGUI UI 控制面板
- 🧼 卸载未使用资源，减少内存占用
- 🤝 支持多人游戏联动逻辑（如扩展 `NetGame`、`NetPlayer` 等）
- 🔄 自动更新机制（基于 github release）
- 🪶 插件式架构，支持功能注册与动态启用/禁用

---

## 🧠 开发指南 Development

- 语言：C#
- 框架：.NET Framework 4.6.2
- 核心依赖：
  - `Assembly-CSharp.dll`（游戏主程序集）
  - `UnityEngine.dll`
  - `HarmonyLib`（方法注入）
- 构建类型：Class Library（DLL）

推荐使用 Visual Studio 开发，并以 UnityDoorstop 注入为载体运行。

---

## 🤝 致谢 Thanks

- [HarmonyX](https://github.com/BepInEx/HarmonyX) - 方法注入支持
- [UnityDoorstop](https://github.com/NeighTools/UnityDoorstop) - DLL 注入工具
- Human: Fall Flat 社区玩家和测试者

---

## 📜 许可 License

本项目采用 MIT License 开源协议，允许自由使用、修改与分发。

---

## 📬 联系方式 Contact

如有问题或建议，欢迎通过以下方式反馈：

- 提交 GitHub Issue
- QQ群：385272989

