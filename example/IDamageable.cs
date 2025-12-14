
using GDScriptInterface.Abstractions;

public interface IDamageable
{
    bool IsDead { get; }
    void TakeDamage(int damage);
}


[GenerateGDScriptInterfaceMapping(typeof(IDamageable))]
public static partial class IDamageableGDScriptMapping
{
    
}