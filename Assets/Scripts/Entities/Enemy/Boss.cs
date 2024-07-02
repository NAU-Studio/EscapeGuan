using System;

using EscapeGuan.Entities;

public abstract class Boss : Entity
{
    public abstract string BossName { get; }
    public abstract string BossDescription { get; }

    public override int InventoryLength => throw new NotImplementedException();
}
