# CheckInterface

English readme available at [Readme_en.md](Readme_en.md).

CheckInterface 是一个基于 Godot 4.4（支持 GDScript 和 C#）的示例项目，演示了如何在 Godot 中检查 GDScript 脚本是否实现了指定的“接口”方法集合。
该项目适用于需要在运行时动态判断脚本是否符合某种接口规范的场景。

## 主要功能

- **接口检查**：通过 `IsImplementedInterface`（C#）或 `is_implemented_interface`（GDScript）方法，判断某个 GDScript 是否实现了指定方法签名集合。
- **支持属性 getter/setter 检查**：可检测属性访问器方法（如 `get_xxx`/`set_xxx`）。
- **支持参数类型、数量、返回值类型等多维度匹配**。

## 目录结构

这个仓库包含了用 GDScript 和 C# 实现的 CheckInterface，以及一个用于运行它们的示例 Godot 项目。

对于 GDScript 实现，需要 2 个文件：
- [`gds_interface_checker.gd`](src/gds_interface_checker.gd) / [`GDSInterfaceChecker`](src/gds_interface_checker.gd):GDScript的 `is_implemented_interface` 实现 .
- [`gds_method_info.gd`](src/gds_method_info.gd) / [`GDSMethodInfo`](src/gds_method_info.gd): 用于构建和存储接口验证规则的辅助类。

对于 C# 实现：
- [`GDScriptInterface`](.GDScriptInterface) 用于辅助构建GDScript 接口映射的源代码生成器实现
- [`Main.cs`](Main.cs)：包含 `InterfaceChecker.IsImplementedInterface()` 的示例用法。
- [`IDamageable.cs`](example/IDamageable.cs)：用 C# 编写的示例接口，以及源代码生成器的使用示例。
- [`Enemy.cs`](example/Enemy.cs)：实现 `IDamageable` 接口的示例 C# 类。

Godot 项目文件：
- `main.tscn`：主要场景，附加了 `main.gd`.
- [`main.gd`](main.gd): 演示 GDScript 实现。验证 `enemy.gd` 是否实现某些接口规则并打印结果。
- [`enemy.gd`](enemy.gd)： 实现 IDamageable 接口的示例 GDScript (见 [`IDamageable.cs`](example/IDamageable.cs)).
- `project.godot`, 相关的元数据文件。

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
var script = GD.Load<GDScript>("res://example/enemy.gd");
List<GDScriptMethodInfo> @interface = [
    // take_damage(int damage) -> void
    new GDScriptMethodInfo(
        Name: "take_damage",
        Args: [
            new GDScriptPropertyInfo(
                Name: "damage", // Interface checks will not compare parameter names
                ClassName: "",
                Type: Variant.Type.Int,
                Hint: PropertyHint.None,
                HintString: "",
                Usage: PropertyUsageFlags.None
            )
        ],
        DefaultArgs: [],
        Flags: MethodFlags.Normal,
        Return: new GDScriptPropertyInfo(
            Name: "",
            ClassName: "",
            Type: Variant.Type.Nil,
            Hint: PropertyHint.None,
            HintString: "",
            Usage: PropertyUsageFlags.Default
        )
    ),

    // property getter
    new GDScriptMethodInfo(
        Name: "@is_dead_getter",
        Args: [],
        DefaultArgs: [],
        Flags: MethodFlags.Normal,
        Return: new GDScriptPropertyInfo(
            Name: "",
            ClassName: "",
            Type: Variant.Type.Bool,
            Hint: PropertyHint.None,
            HintString: "",
            Usage: PropertyUsageFlags.None
        )
    )
];
GD.Print(InterfaceChecker.IsImplementedInterface(script, @interface));
```

#### 源代码生成器

C#支持使用源码生成器自动映射
- 在静态类定义上添加特性 `[GenerateGDScriptInterfaceMapping(typeof(IMyInterface))]` （参见 [example/IDamageable.cs](example/IDamageable.cs)）。
- 编译后，源码生成器会在同名静态类（例如 `IDamageableGDScriptMapping`）中生成映射常量。

[GenerateGDScriptInterfaceMapping](.GDScriptInterface/GDScriptInterface.Abstractions/GenerateGDScriptInterfaceMappingAttribute.cs) 特性的使用需要满足以下条件:

- 只能使用于静态分部类(`static partial class`)
- 构造时提供的类型必须是接口
- 提供的接口不能是泛型接口
- 提供的接口不能具有泛型方法
- 提供的接口不能包含索引器
- 提供的接口中所有成员的返回值和参数必须是Variant兼容类型

源码生成器位于 [.GDScriptInterface/GDScriptInterface.SourceGenerator](.GDScriptInterface/GDScriptInterface.SourceGenerator)。
若在 C# 项目中启用该生成器，编译时会自动生成对应的 GDScript 接口映射辅助代码（无需手动编写映射数组）。

```csharp
public interface IDamageable
{
    bool IsDead { get; }
    void TakeDamage(int damage);
}


[GenerateGDScriptInterfaceMapping(typeof(IDamageable))]
public static partial class IDamageableGDScriptMapping
{
    
}

// 对于 C#，您可以使用源生成器来生成 GDScriptMethodInfo
List<GDScriptMethodInfo> @interface = [IDamageableGDScriptMapping.IsDeadGetter, IDamageableGDScriptMapping.TakeDamage];
GD.Print(InterfaceChecker.IsImplementedInterface(script,@interface));
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
