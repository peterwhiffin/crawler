using UnityEngine;

[CreateAssetMenu(menuName = "Item Details/Healing Item", fileName = "NewDefaultItem")]
public class HealingItemDetails : ItemDetails
{

    [field: SerializeField] public float HealAmount { get; private set; }

    public override void AffectPlayer(Player player)
    {
        base.AffectPlayer(player);

        player.Stats.ApplyHeal(HealAmount);
    }
}
