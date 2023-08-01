using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] private ItemSO item;
    private SphereCollider sphereCollider;
    private Rigidbody rb;

    [SerializeField] bool hasItemBeenUsed = false;

    private void Start()
    {
        if (item == null) Debug.LogError("Item can't be null!");

        sphereCollider = GetComponent<SphereCollider>();
        rb = GetComponent<Rigidbody>();
    }

    public ItemSO GetItemSO()
    {
        return item;
    }

    public bool GetHasItemBeenUsed()
    {
        return hasItemBeenUsed;
    }

    public void SetHasItemBeenUsed(bool value)
    {
        hasItemBeenUsed = value;
    }

    public void Interact()
    {
        //if (hasItemBeenUsed) return;

        PlayerController.LocalInstance.InventoryController.SetItemInHand(this);
    }

    public void ShowCollider()
    {
        sphereCollider.enabled = true;
    }

    public void HideCollider()
    {
        sphereCollider.enabled = false;
    }

    public void ParentToHand()
    {
        transform.SetParent(PlayerController.LocalInstance.InventoryController.GetHandTransform());
        transform.localPosition = Vector3.zero;
        HideCollider();
        rb.isKinematic = true;

    }

    public void UnparentFromHand()
    {
        transform.SetParent(null);
        transform.SetPositionAndRotation(transform.position, transform.rotation);
        ShowCollider();
        rb.isKinematic = false;
    }

    public void SwapPosition(Vector3 newItemPosition)
    {
        transform.SetParent(null);
        transform.SetPositionAndRotation(newItemPosition, transform.rotation);
        ShowCollider();
        rb.isKinematic = false;
    }
}