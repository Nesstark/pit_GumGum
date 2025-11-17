using UnityEngine;

public class DamageForwarder : MonoBehaviour
{
    private EnemyCombat owner;

    public void Init(EnemyCombat combatOwner)
    {
        owner = combatOwner;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (owner == null) return;

        Unit targetUnit = other.GetComponent<Unit>();
        if (targetUnit != null && !targetUnit.IsDead())
        {
            owner.Attack(targetUnit);
        }
    }
}
