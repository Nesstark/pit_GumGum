using Unity.IO.LowLevel.Unsafe;
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

        int dmg = base.Attack(target); // base dmg for now, can remove when we implement velocity stuff
        return target.TakeDamage(dmg);
    }
}
