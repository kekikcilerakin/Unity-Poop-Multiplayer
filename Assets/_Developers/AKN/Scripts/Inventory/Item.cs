using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] private ItemSO item;
    private SphereCollider sphereCollider;
    private Rigidbody rb;

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

    public void Interact()
    {
        PlayerController.Instance.InventoryController.SetItemInHand(this);
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
        transform.SetParent(PlayerController.Instance.InventoryController.GetHandTransform());
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