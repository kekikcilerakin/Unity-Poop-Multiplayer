using UnityEngine;

public class TaskVisual : MonoBehaviour
{
    private Task task;
    private Outline outline;

    [SerializeField] private float progress = 0;

    private void Start()
    {
        task = transform.parent.GetComponent<Task>();
        outline = GetComponent<Outline>();

        PlayerController.Instance.InventoryController.OnItemInHandChanged += InventoryController_OnItemInHandChanged;
        task.OnActivePlayerChanged += Task_OnActivePlayerChanged;
    }

    private void Task_OnActivePlayerChanged(object sender, Task.OnActivePlayerChangedEventArgs e)
    {
        Debug.Log($"ActivePlayerChanged to {e.ActivePlayer}");
        if (e.ActivePlayer == null)
        {
            progress = 0;
            Debug.Log($"Progress set to {progress}");
        }
    }

    private void HandleProgress()
    {
        if (task.GetIsTaskCompleted()) return;

        if (progress > task.GetCompleteTime()) return;

        if (!task.GetActivePlayer()) return;

        progress += Time.deltaTime;

        Debug.Log((progress / task.GetCompleteTime()).ToString("P0"));

        if (progress > task.GetCompleteTime())
        {
            Debug.Log("Task Completed");
            task.SetIsTaskCompleted(true);
            HideOutline();
            PlayerController.Instance.InventoryController.GetItemInHand().gameObject.layer = 0;
            PlayerController.Instance.InventoryController.DropItem();

            //D�ZENLENECEK
        }
    }

    private void Update()
    {
        HandleProgress();
    }

    private void InventoryController_OnItemInHandChanged(object sender, InventoryController.OnItemInHandChangedEventArgs e)
    {
        if (task.GetRequiredItem() == e.ItemInHand && !task.GetIsTaskCompleted())
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