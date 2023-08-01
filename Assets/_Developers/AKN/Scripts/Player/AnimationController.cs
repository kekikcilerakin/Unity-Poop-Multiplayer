using Unity.Netcode;
using UnityEngine;

public class AnimationController : NetworkBehaviour
{
    private Animator animator;
    private PlayerController playerController;

    #region Constant Variables
    private const float AnimationBlendSpeed = 0.1f;
    #endregion

    #region Cached Variables
    private int verticalHash;
    #endregion

    private void Awake()
    {
        animator = GetComponent<Animator>();
        playerController = GetComponentInParent<PlayerController>();

        verticalHash = Animator.StringToHash("Vertical");
    }

    private void Update()
    {
        if (!IsOwner) return;

        animator.SetFloat(verticalHash, playerController.GetMoveAmount(), AnimationBlendSpeed, Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.E))
        {
            animator.SetTrigger("TakeItem");
        }
    }
}