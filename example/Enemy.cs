using Godot;

public partial class Enemy : CharacterBody2D, IDamageable
{
    public bool IsDead { get; private set; }

    public void TakeDamage(int damage)
    {
        
    }
}
