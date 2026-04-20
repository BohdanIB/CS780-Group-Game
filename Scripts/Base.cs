using Godot;

using System;

public partial class Base : Node2D
{
    [Export] public int MaxHealth = 100;
    public int CurrentHealth { get; private set; }

    public event Action<int> BaseDamaged;
    public event Action BaseDestroyed;

    public override void _Ready()
    {
        CurrentHealth = MaxHealth;
        var area = GetNode<Area2D>("Area2D");
        area.BodyEntered += OnBodyEntered;
    }

    private void OnBodyEntered(Node2D body)
    {
        if (body is Enemy enemy)
        {
            TakeDamage(5);
            enemy.QueueFree();
        }
    }

    private void TakeDamage(int amount)
    {
        CurrentHealth -= amount;
        BaseDamaged?.Invoke(CurrentHealth);

        if (CurrentHealth <= 0)
            BaseDestroyed?.Invoke();
    }
}
