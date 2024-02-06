using Cinemachine;
using UnityEngine;

public class Swapping_UI_Layers : MonoBehaviour{
    public GameObject inventoryLayer;
    public CinemachineVirtualCamera virtualCamera;

    [SerializeField] public GameObject pickUpImage;

    private bool inventoryVisible = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleInventory();
        }

        
    }

    private void ToggleInventory()
    {
        inventoryVisible = !inventoryVisible;
        inventoryLayer.SetActive(inventoryVisible);
        if (!inventoryVisible)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            EnableCameraRotation();
            PlayerMovement.canMove = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            DisableCameraRotation();
            PlayerMovement.canMove = false;
        }
    }

    public void EnableCameraRotation()
    {
        if (virtualCamera != null)
        {
            CinemachinePOV pov = virtualCamera.GetCinemachineComponent<CinemachinePOV>();
            if (pov != null)
            {
                pov.m_HorizontalAxis.m_MaxSpeed = 300f; // Adjust the speed if needed
                pov.m_VerticalAxis.m_MaxSpeed = 300f; // Adjust the speed if needed
            }
        }
    }

    public void DisableCameraRotation()
    {
        if (virtualCamera != null)
        {
            CinemachinePOV pov = virtualCamera.GetCinemachineComponent<CinemachinePOV>();
            if (pov != null)
            {
                pov.m_HorizontalAxis.m_MaxSpeed = 0f; // Set speed to 0 to disable rotation
                pov.m_VerticalAxis.m_MaxSpeed = 0f; // Set speed to 0 to disable rotation
            }
        }
    }
}
