using UnityEngine;

public class Player : Unit
{
    private void Update()
    {

    }

    public override int Attack(Unit target)
    {
        if (target == null || target.IsDead()) return 0;

        // TODO; need to implement velocity based damage calculation
        // e.g. dmg = baseDmg * (currentVelocity / maxVelocity) or whatever we end up doing

        // Use base implementation which already applies the unit's base damage
        return base.Attack(target);
    }
}

