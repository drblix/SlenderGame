using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PauseMenu : MonoBehaviour
{
    private Canvas canvas;

    [SerializeField]
    private RectTransform homeMenu;
    [SerializeField]
    private RectTransform optionsMenu;

    private void Awake()
    {
        canvas = GetComponent<Canvas>();
    }

    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame) { ToggleMenu(); }
    }

    public void ToggleMenu()
    {
        canvas.enabled = !canvas.enabled;

        if (canvas.enabled)
        {
            Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 0;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Time.timeScale = 1;
        }

        homeMenu.anchoredPosition = new Vector2(0f, 0f);
        optionsMenu.anchoredPosition = new Vector2(800f, 0f);
    }
}
