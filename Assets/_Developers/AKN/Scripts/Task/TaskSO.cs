using Poop.Player.Inventory;
using UnityEngine;

namespace Poop
{
    [CreateAssetMenu(menuName = "Poop/Task")]
    public class TaskSO : ScriptableObject
    {
        public string taskName;
        public Transform prefab;
        public ItemSO requiredItem;
    }
}
