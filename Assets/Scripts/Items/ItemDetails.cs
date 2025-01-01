using UnityEngine;

[CreateAssetMenu(menuName = "Item Details/Default Item", fileName = "NewDefaultItem")]
public class ItemDetails : ScriptableObject
{
    [field: SerializeField] public int ID { get; private set; }
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public Item ItemPrefab { get; private set; }

    public virtual void AffectPlayer(Player player) { }
}
