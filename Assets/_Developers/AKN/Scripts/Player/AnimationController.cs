using UnityEngine;

public class AnimationController : MonoBehaviour
{
    private const float AnimationBlendSpeed = 0.1f;
    private Animator animator;
    private PlayerController player;

    private int verticalHash;

    private void Start()
    {
        animator = GetComponent<Animator>();
        player = GetComponentInParent<PlayerController>();

        verticalHash = Animator.StringToHash("Vertical");
    }

    private void Update()
    {
        animator.SetFloat(verticalHash, player.GetMoveAmount(), AnimationBlendSpeed, Time.deltaTime);
    }
}