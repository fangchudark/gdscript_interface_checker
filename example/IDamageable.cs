using GDScriptInterface.Abstractions;
using Godot;

public interface IDamageable
{
    bool IsDead { get; }
    void TakeDamage(int damage);
}


[GenerateGDScriptInterfaceMapping(typeof(IDamageable))]
public static partial class IDamageableGDScriptMapping
{

}