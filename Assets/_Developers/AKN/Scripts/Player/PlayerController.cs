using System;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum PlayerType
{
    None = 0,
    Principal = 1,
    Student = 2,
}

public enum PlayerState
{
    None = 0,
    DoingTask = 1,
}

public class PlayerController : NetworkBehaviour
{
    public static PlayerController Instance { get; private set; }

    public InventoryController InventoryController { get; private set; }

    [SerializeField] private CameraController cameraController;

    [SerializeField] private Item highlightedItem;
    public event EventHandler<OnHighlightedItemChangedEventArgs> OnHighlightedItemChanged;
    public class OnHighlightedItemChangedEventArgs : EventArgs
    {
        public Item HighlightedItem;
    }

    [SerializeField] private Task highlightedTask;
    public event EventHandler<OnHighlightedTaskChangedEventArgs> OnHighlightedTaskChanged;
    public class OnHighlightedTaskChangedEventArgs : EventArgs
    {
        public Task HighlightedTask;
    }

    [SerializeField] private PlayerType playerType;
    [SerializeField] private PlayerState playerState;

    private CharacterController characterController;

    [SerializeField] private GameObject principalVisual;
    [SerializeField] private GameObject studentVisual;
    private bool isPlayerInitialized = false;

    [Tooltip("Principal = 1.7, Student = 1")]
    [SerializeField] private float walkSpeed = 0.0f;
    [Tooltip("Principal = 3, Student = 2.8")]
    [SerializeField] private float runSpeed = 0.0f;

    [SerializeField] private LayerMask interactableMask;
    [SerializeField] private float itemInteractDistance = 6f;
    [SerializeField] private float taskInteractDistance = 6f;

    #region Constant Variables
    private const float Gravity = -9.81f;
    private const float SpeedChangeRate = 10.0f;
    private const float SpeedOffset = 0.1f;
    private const float InputMagnitude = 1.0f;
    private const float RotationSmoothness = 0.1f;
    #endregion

    #region Cached Variables
    private float speed;
    private float targetRotation;
    private float rotationVelocity;
    #endregion

    private void InitializePlayer()
    {
        Instance = this;

        characterController = GetComponent<CharacterController>();
        InventoryController = GetComponent<InventoryController>();
        cameraController = GetComponent<CameraController>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        InputManager.Instance.OnItemInteractAction += InputManager_OnItemInteractAction;
        InputManager.Instance.OnItemDropAction += InputManager_OnItemDropAction;

        InputManager.Instance.OnTaskInteractStartedAction += InputManager_OnTaskInteractStartRequestedAction;
        InputManager.Instance.OnTaskInteractCanceledAction += InputManager_OnTaskInteractCancelRequestedAction;

        principalVisual.SetActive(false);
        studentVisual.SetActive(false);

        if (playerType == PlayerType.Principal)
        {
            principalVisual.SetActive(true);
            walkSpeed = 1.7f;
            runSpeed = 3.0f;
            cameraController.PlayerCameraRoot = principalVisual.transform.GetChild(0);
        }
        else if (playerType == PlayerType.Student)
        {
            studentVisual.SetActive(true);
            walkSpeed = 1.0f;
            runSpeed = 2.8f;
            cameraController.PlayerCameraRoot = studentVisual.transform.GetChild(0);
        }

        if (walkSpeed == 0.0f || runSpeed == 0.0f) Debug.LogWarning("Walk and run speed are not set.");

        if (!isLocalPlayer)
        {
            cameraController.TPSCamera.gameObject.SetActive(false);
        }

        cameraController.Initialize();
        isPlayerInitialized = true;
    }

    private void InputManager_OnItemDropAction(object sender, EventArgs e)
    {
        if (!InventoryController.GetItemInHand()) return;

        InventoryController.DropItem();
    }

    private void InputManager_OnItemInteractAction(object sender, EventArgs e)
    {
        if (highlightedItem == null) return;

        highlightedItem.Interact();
        SetHighlightedItem(null);
    }

    private void InputManager_OnTaskInteractStartRequestedAction(object sender, EventArgs e)
    {
        if (InventoryController.GetItemInHand() == null)
        {
            Debug.Log($"You need an item to start interacting with tasks");
            return;
        }
        if (highlightedTask == null)
        {
            Debug.Log($"You are not in range of any task.");
            return;
        }
        if (highlightedTask.GetActivePlayer())
        {
            Debug.Log($"{highlightedTask} is already in progress by {highlightedTask.GetActivePlayer()}");
            return;
        }
        if (highlightedTask.GetRequiredItem() != InventoryController.GetItemInHand().GetItemSO())
        {
            Debug.Log($"Task requires {highlightedTask.GetRequiredItem()} but you have {InventoryController.GetItemInHand().GetItemSO()}");
            return;
        }

        playerState = PlayerState.DoingTask;
        Debug.Log($"PlayerState set to {playerState}");
        highlightedTask.SetPlayer(this);
        InputManager.Instance.DisableMovement();
    }

    private void InputManager_OnTaskInteractCancelRequestedAction(object sender, EventArgs e)
    {
        if (playerState != PlayerState.DoingTask)
        {
            Debug.LogWarning($"CancelInteract failed: playerState needs to be DoingTask to cancel an interact.");
            return;
        }

        playerState = PlayerState.None;
        Debug.Log($"PlayerState set to {playerState}");
        highlightedTask.SetPlayer(null);
        InputManager.Instance.EnableMovement();
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "Game")
        {
            if (!isPlayerInitialized)
            {
                InitializePlayer();
            }

            if (isOwned)
            {
                HandleMove();
                HandleRotate();
                HandleInteraction();
            }
        }
    }

    private void HandleMove()
    {
        float targetSpeed = InputManager.Instance.Run ? runSpeed : walkSpeed;
        if (InputManager.Instance.Move == Vector2.zero) targetSpeed = 0.0f;

        AccelerateSpeed(targetSpeed);

        Vector3 targetDirection = Quaternion.Euler(0.0f, targetRotation, 0.0f) * Vector3.forward;

        characterController.Move(targetDirection.normalized * (speed * Time.deltaTime) + new Vector3(0.0f, Gravity, 0.0f) * Time.deltaTime);
    }

    private void HandleRotate()
    {
        if (InputManager.Instance.Move == Vector2.zero) return;

        Vector3 lookDirection = new Vector3(InputManager.Instance.Move.x, 0.0f, InputManager.Instance.Move.y).normalized;

        targetRotation = Mathf.Atan2(lookDirection.x, lookDirection.z) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;
        float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref rotationVelocity, RotationSmoothness);

        transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
    }

    private void HandleInteraction()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        bool hitSomething = Physics.Raycast(ray, out RaycastHit hit, itemInteractDistance, interactableMask);
        Item hitItem = hit.collider?.GetComponent<Item>();
        Task hitTask = hit.collider?.GetComponent<Task>();

        if (hitItem != highlightedItem && hitItem != InventoryController.GetItemInHand() && playerType == PlayerType.Student)
        {
            SetHighlightedItem(hitItem);
        }
        else if (!hitSomething)
        {
            SetHighlightedItem(null);
        }

        if (hitTask != highlightedTask && playerType == PlayerType.Student)
        {
            SetHighlightedTask(hitTask);
        }
        else if (!hitSomething)
        {
            SetHighlightedTask(null);
        }
    }

    private void AccelerateSpeed(float targetSpeed)
    {
        float currentHorizontalSpeed = new Vector3(characterController.velocity.x, 0.0f, characterController.velocity.z).magnitude;

        if (currentHorizontalSpeed < targetSpeed - SpeedOffset || currentHorizontalSpeed > targetSpeed + SpeedOffset)
        {
            speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * InputMagnitude, Time.deltaTime * SpeedChangeRate);
            speed = Mathf.Round(speed * 1000f) / 1000f;
        }
        else
        {
            speed = targetSpeed;
        }
    }

    public float GetMoveAmount()
    {
        float horizontalInput = InputManager.Instance.Move.x;
        float verticalInput = InputManager.Instance.Move.y;

        float moveAmount = Mathf.Clamp01(Mathf.Abs(horizontalInput) + Mathf.Abs(verticalInput));

        if (InputManager.Instance.Run) return moveAmount *= 2f;
        else return moveAmount;
    }

    public PlayerType GetPlayerType()
    {
        return playerType;
    }

    private void SetHighlightedItem(Item highlightedItem)
    {
        this.highlightedItem = highlightedItem;

        OnHighlightedItemChanged?.Invoke(this, new OnHighlightedItemChangedEventArgs
        {
            HighlightedItem = highlightedItem
        });
    }

    private void SetHighlightedTask(Task highlightedTask)
    {
        this.highlightedTask = highlightedTask;

        OnHighlightedTaskChanged?.Invoke(this, new OnHighlightedTaskChangedEventArgs
        {
            HighlightedTask = highlightedTask
        });
    }
}