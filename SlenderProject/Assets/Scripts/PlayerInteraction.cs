using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    private PlayerInput pInput;

    private Transform plrCam;

    private void Awake()
    {
        pInput = GetComponent<PlayerInput>();
        plrCam = transform.Find("Head").Find("MainCamera");

        pInput.actions["LeftMouse"].started += On_MouseClick;
    }

    private void On_MouseClick(InputAction.CallbackContext obj)
    {
        Ray ray = new(plrCam.position, plrCam.forward);

        if (Physics.Raycast(ray, out RaycastHit hitInfo, 8f, ~LayerMask.GetMask("Terrain")))
        {
            if (hitInfo.collider)
            {
                print(hitInfo.collider.name);
            }
        }
    }
}
