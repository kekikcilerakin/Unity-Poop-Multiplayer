
using UnityEngine;

public class ItemVisual : MonoBehaviour
{
    private Item item;
    private Outline outline;

    private void Start()
    {
        item = transform.parent.GetComponent<Item>();
        outline = GetComponent<Outline>();

        PlayerController.Instance.OnHighlightedItemChanged += PlayerController_OnSelectedItemChanged;
    }

    private void PlayerController_OnSelectedItemChanged(object sender, PlayerController.OnHighlightedItemChangedEventArgs e)
    {
        if (e.HighlightedItem == item)
        {
            ShowOutline();
        }
        else
        {
            HideOutline();
        }
    }

    private void ShowOutline()
    {
        outline.enabled = true;
    }

    private void HideOutline()
    {
        outline.enabled = false;
    }
}
