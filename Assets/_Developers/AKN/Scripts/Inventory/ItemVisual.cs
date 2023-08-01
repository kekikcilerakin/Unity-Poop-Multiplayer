using System;
using UnityEngine;

public class ItemVisual : MonoBehaviour
{
    private Item item;
    private Outline outline;

    private void Start()
    {
        item = transform.parent.GetComponent<Item>();
        outline = GetComponent<Outline>();

        if (PlayerController.LocalInstance != null)
        {
            PlayerController.LocalInstance.OnHighlightedItemChanged += PlayerController_OnHighlightedItemChanged;
        }
        else
        {
            PlayerController.OnAnyPlayerSpawned += PlayerController_OnAnyPlayerSpawned;
        }

    }

    private void PlayerController_OnAnyPlayerSpawned(object sender, EventArgs e)
    {
        if (PlayerController.LocalInstance != null)
        {
            PlayerController.LocalInstance.OnHighlightedItemChanged -= PlayerController_OnHighlightedItemChanged;
            PlayerController.LocalInstance.OnHighlightedItemChanged += PlayerController_OnHighlightedItemChanged;
        }
    }

    private void PlayerController_OnHighlightedItemChanged(object sender, PlayerController.OnHighlightedItemChangedEventArgs e)
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