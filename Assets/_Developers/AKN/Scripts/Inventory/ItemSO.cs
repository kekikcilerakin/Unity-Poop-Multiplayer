using UnityEngine;

namespace Poop.Player.Inventory
{
    [CreateAssetMenu(menuName = "Poop/Item")]
    public class ItemSO : ScriptableObject
    {
        public string itemName;
        public Transform prefab;
    }
}
