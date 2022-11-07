using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    private Player player;
    private PlayerInput pInput;

    private Transform plrCam;

    private void Awake()
    {
        player = GetComponent<Player>();
        pInput = GetComponent<PlayerInput>();
        plrCam = transform.Find("Head").Find("MainCamera");

        pInput.actions["LeftMouse"].started += On_MouseClick;
    }

    private void On_MouseClick(InputAction.CallbackContext obj)
    {
        Ray ray = new(plrCam.position, plrCam.forward);

        if (Physics.Raycast(ray, out RaycastHit hitInfo, 3f, ~LayerMask.GetMask("Terrain")) && Time.timeScale > 0f && !player.IsRunning)
        {
            if (!hitInfo.collider.gameObject) { return; }
            GameObject hit = hitInfo.collider.gameObject;

            if (hit.CompareTag("Page"))
            {
                hit.GetComponent<Page>().CollectPage();
            }
            
        }
    }
}
