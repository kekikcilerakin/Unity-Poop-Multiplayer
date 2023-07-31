using Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera TPSCamera;
    public Transform PlayerCameraRoot;

    [SerializeField] private float topClamp = 70.0f;
    [SerializeField] private float bottomClamp = -30.0f;

    private bool isCameraInitialized = false;

    #region Cached Variables
    private float cinemachineTargetY;
    private float cinemachineTargetX;
    #endregion

    public void Initialize()
    {
        cinemachineTargetY = PlayerCameraRoot.transform.rotation.eulerAngles.y;
        TPSCamera.Follow = PlayerCameraRoot;
        TPSCamera.LookAt = PlayerCameraRoot;

        isCameraInitialized = true;
    }

    private void LateUpdate()
    {
        if (!isCameraInitialized) return;

        HandleRotate();
    }

    private void HandleRotate()
    {
        //Don't multiply mouse input by Time.deltaTime;
        //float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;
        float deltaTimeMultiplier = 1.0f;

        cinemachineTargetY += InputManager.Instance.Look.x * deltaTimeMultiplier;
        cinemachineTargetX += InputManager.Instance.Look.y * deltaTimeMultiplier;

        cinemachineTargetY = ClampAngle(cinemachineTargetY, float.MinValue, float.MaxValue);
        cinemachineTargetX = ClampAngle(cinemachineTargetX, bottomClamp, topClamp);

        PlayerCameraRoot.transform.rotation = Quaternion.Euler(cinemachineTargetX, cinemachineTargetY, 0.0f);
    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }
}
