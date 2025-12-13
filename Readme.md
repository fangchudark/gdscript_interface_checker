# CheckInterface

English readme available at [Readme_en.md](Readme_en.md).

CheckInterface 是一个基于 Godot 4.4（支持 GDScript 和 C#）的示例项目，演示了如何在 Godot 中检查 GDScript 脚本是否实现了指定的“接口”方法集合。
该项目适用于需要在运行时动态判断脚本是否符合某种接口规范的场景。

## 主要功能

- **接口检查**：通过 `IsImplementedInterface`（C#）或 `is_implemented_interface`（GDScript）方法，判断某个 GDScript 是否实现了指定方法签名集合。
- **支持属性 getter/setter 检查**：可检测属性访问器方法（如 `get_xxx`/`set_xxx`）。
- **支持参数类型、数量、返回值类型等多维度匹配**。

## 目录结构

- `main.tscn`：主场景，挂载了主逻辑脚本。
- `main.gd`：GDScript 入口，包含接口检查逻辑的 GDScript 实现。
- `Main.cs`：C# 入口，包含接口检查逻辑的 C# 实现。
- `enemy.gd`：示例 GDScript，模拟实现了 `IDamageable` 接口。
- `Enemy.cs`：示例 C# 脚本，实现了 `IDamageable` 接口。
- `IDamageable.cs`：接口定义（C#）。
- 其他 Godot 工程配置文件。

## 用法说明

### GDScript 方式

在 `main.gd` 中：

```gdscript
var script = load("res://enemy.gd")
var interface : Array[MethodInfo] = [
    MethodInfo.new("take_damage", 1, [""], [TYPE_INT], "", TYPE_NIL, [], PROPERTY_USAGE_DEFAULT),
    MethodInfo.new("get_is_dead", 0, [], [], "", TYPE_BOOL, ["@is_dead_getter"])
]
print(is_implemented_interface(script, interface)) # 输出 true/false
```

### C# 方式

在 `Main.cs` 中：

```csharp
var script = GD.Load<GDScript>("res://enemy.gd");
List<GDScriptMethodInfo> @interface = [
    new("take_damage", 1, [""], [Variant.Type.Int], "", Variant.Type.Nil, ReturnFlags: PropertyUsageFlags.Default),
    new("get_is_dead", 0, [], [], "", Variant.Type.Bool, ["@is_dead_getter"]),
];
GD.Print(IsImplementedInterface(script, @interface)); // 输出 true/false
```

### 接口定义示例

`IDamageable.cs`:

```csharp
public interface IDamageable
{
    bool IsDead { get; }
    void TakeDamage(int damage);
}
```

`enemy.gd` 实现了该接口的等价方法。

## 适用场景

- 动态插件系统
- 脚本热加载与接口合规性校验
- 跨语言脚本互操作时的接口一致性检查

## 依赖

- Godot 4.4+
- .NET 8.0（如使用 C#）

## 运行方法

1. 使用 Godot 编辑器打开本项目文件夹。
2. 运行主场景 `main.tscn`。
3. 控制台将输出接口检查结果。

## 许可证

MIT
