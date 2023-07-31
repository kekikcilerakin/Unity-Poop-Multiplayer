using UnityEngine;

public class PlayerVisual : MonoBehaviour
{
    private PlayerController playerController;

    private void Start()
    {
        playerController = transform.parent.GetComponent<PlayerController>();
        
            
    }
}