using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapHitbox : MonoBehaviour, IHittable
{
    [SerializeField] private Tilemap m_Tilemap;

    public void Hit(float damage, Vector3 hitPosition, Vector3 normal)
    {
        var closeCell = hitPosition - (normal * m_Tilemap.cellSize.x / 2);
        Vector3Int cellPosition = m_Tilemap.WorldToCell(closeCell);
        
        m_Tilemap.SetTile(cellPosition, null);
    }
}
