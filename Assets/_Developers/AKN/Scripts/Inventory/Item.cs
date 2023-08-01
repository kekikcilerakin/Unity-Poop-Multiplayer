using Unity.Netcode;
using UnityEngine;

public class Item : NetworkBehaviour
{
    [SerializeField] private ItemSO item;
    private SphereCollider sphereCollider;
    private Rigidbody rb;

    private Transform targetTransform;

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
        ParentToHandServerRpc(PlayerController.LocalInstance.GetNetworkObject());
    }

    public void UnparentFromHand()
    {
        targetTransform = null;
        transform.SetPositionAndRotation(transform.position, transform.rotation);
        ShowCollider();
    }

    public void SwapPosition(Vector3 newItemPosition)
    {
        targetTransform = null;
        transform.SetPositionAndRotation(newItemPosition, transform.rotation);
        ShowCollider();
    }

    private void LateUpdate()
    {
        if (targetTransform == null) return;

        transform.position = targetTransform.position;
        transform.rotation = targetTransform.rotation;
    }

    [ServerRpc(RequireOwnership = false)]
    private void ParentToHandServerRpc(NetworkObjectReference itemParentNetworkObjectReference)
    {
        ParentToHandClientRpc(itemParentNetworkObjectReference);
    }

    [ClientRpc]
    private void ParentToHandClientRpc(NetworkObjectReference itemParentNetworkObjectReference)
    {
        itemParentNetworkObjectReference.TryGet(out NetworkObject itemNetworkObject);
        PlayerController parent = itemNetworkObject.GetComponent<PlayerController>();

        targetTransform = PlayerController.LocalInstance.InventoryController.GetHandTransform();
        HideCollider();
    }
}