using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    private Player player;
    private GameManager gameManager;

    private Canvas canvas;
    [SerializeField]
    private Canvas gameEventCanvas;

    [SerializeField]
    private GameObject homeMenu;
    [SerializeField]
    private GameObject optionsMenu;

    [SerializeField]
    private TextMeshProUGUI sensDisplay;
    [SerializeField]
    private TextMeshProUGUI disDisplay;
    [SerializeField]
    private Toggle headBobbing;
    [SerializeField]
    private Toggle invertMouse;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        player = FindObjectOfType<Player>();
        canvas = GetComponent<Canvas>();

        canvas.enabled = false;
        homeMenu.SetActive(true);
        optionsMenu.SetActive(false);

        sensDisplay.SetText(Mathf.RoundToInt(PlayerSettings.mouseSensitivity).ToString());
        disDisplay.SetText(PlayerSettings.mapObjDraw.ToString());

        headBobbing.isOn = PlayerSettings.headBobbing;
        invertMouse.isOn = PlayerSettings.invertMouse;
    }

    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame && !gameManager.gameOver) { ToggleMenu(); }
    }

    public void ToggleMenu()
    {
        canvas.enabled = !canvas.enabled;
        gameEventCanvas.enabled = !gameEventCanvas.enabled;
        player.ToggleAmbience(!canvas.enabled);

        if (canvas.enabled)
        {
            Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 0;

            foreach (AudioSource source in FindObjectsOfType<AudioSource>())
            {
                if (source.isPlaying)
                {
                    source.Pause();
                }
            }
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Time.timeScale = 1;

            foreach (AudioSource source in FindObjectsOfType<AudioSource>())
            {
                source.UnPause();
            }
        }
    }

    public void ToggleInvert(bool state) => PlayerSettings.invertMouse = state;
    public void ToggleBob(bool state) => PlayerSettings.headBobbing = state;
    public void ChangeSens(float newS)
    {
        PlayerSettings.mouseSensitivity = newS;
        sensDisplay.SetText(Mathf.RoundToInt(PlayerSettings.mouseSensitivity).ToString());
    }

    public void ChangeObjDis(float newN)
    {
        player.SetObjDistance(newN);
        disDisplay.SetText(PlayerSettings.mapObjDraw.ToString());
    }

    // will go to the main menu at some point
    public void QuitGame() => Application.Quit();
}
