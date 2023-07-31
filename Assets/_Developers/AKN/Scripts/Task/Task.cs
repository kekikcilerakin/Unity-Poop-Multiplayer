
using System;
using UnityEngine;

public class Task : MonoBehaviour
{
    [SerializeField] private ItemSO requiredItem;
    [SerializeField] private float completeTime;

    [SerializeField] private bool isTaskCompleted;

    [SerializeField] private PlayerController activePlayer;
    public event EventHandler<OnActivePlayerChangedEventArgs> OnActivePlayerChanged;
    public class OnActivePlayerChangedEventArgs : EventArgs
    {
        public PlayerController ActivePlayer;
    }

    public ItemSO GetRequiredItem()
    {
        return requiredItem;
    }

    public float GetCompleteTime()
    {
        return completeTime;
    }

    public PlayerController GetActivePlayer()
    {
        return activePlayer;
    }

    public bool GetIsTaskCompleted()
    {
        return isTaskCompleted;
    }

    public void SetIsTaskCompleted(bool value)
    {
        isTaskCompleted = value;
    }

    public void SetPlayer(PlayerController player)
    {
        this.activePlayer = player;

        if (player == null)
        {
            this.activePlayer = null;
        }

        OnActivePlayerChanged?.Invoke(this, new OnActivePlayerChangedEventArgs
        {
            ActivePlayer = activePlayer
        });
    }
}