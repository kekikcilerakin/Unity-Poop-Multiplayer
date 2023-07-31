using System;
using UnityEngine;

namespace Poop.Player.Inventory
{
    public class InventoryController : MonoBehaviour
    {
        [SerializeField] private Item itemInHand;
        public event EventHandler<OnItemInHandChangedEventArgs> OnItemInHandChanged;
        public class OnItemInHandChangedEventArgs : EventArgs
        {
            public ItemSO ItemInHand;
        }

        [SerializeField] private Transform handTransform;

        public void SetItemInHand(Item item)
        {
            if (item == null)
            {
                itemInHand = null;

                OnItemInHandChanged?.Invoke(this, new OnItemInHandChangedEventArgs
                {
                    ItemInHand = null
                });

                return;
            }

            if (itemInHand == null)
            {
                itemInHand = item;
                itemInHand.ParentToHand();

                OnItemInHandChanged?.Invoke(this, new OnItemInHandChangedEventArgs
                {
                    ItemInHand = itemInHand.GetItemSO()
                });

                return;
            }

            //Swap Items
            Vector3 newItemPosition = item.transform.position;

            itemInHand.SwapPosition(newItemPosition);
            itemInHand = item;
            item.ParentToHand();

            OnItemInHandChanged?.Invoke(this, new OnItemInHandChangedEventArgs
            {
                ItemInHand = itemInHand.GetItemSO()
            });

        }

        public void DropItem()
        {
            itemInHand.UnparentFromHand();
            SetItemInHand(null);
        }

        public Item GetItemInHand()
        {
            return itemInHand;
        }

        public Transform GetHandTransform()
        {
            return handTransform;
        }
    }
}
