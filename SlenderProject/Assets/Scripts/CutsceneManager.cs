using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using TMPro;

public class CutsceneManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI cutsceneText;
    [SerializeField]
    private Image blocker;
    [SerializeField]
    private AudioSource dramaSource;

    private const float CAR_SPEED = 19f;

    private float cutsceneTimer = 0f;
    private bool isDone = false;

    private void Awake()
    {
        //Application.targetFrameRate = 144;
        //<size=40>Dead Pixel Productions\n<size=25>Presents...
        //<size=50>Slender the Eight Pages\n<size=38>REWRITTEN
    }

    private void Update()
    {
        HandleEvents();
        SkipInput();

        Vector3 carMovement = CAR_SPEED * Time.deltaTime * Vector3.forward;
        transform.Translate(carMovement);
    }

    private void SkipInput()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame) { SceneManager.LoadScene(1); }
    }

    private void HandleEvents()
    {
        if (blocker.color.a > .3f && !isDone)
        {
            float newA = Mathf.Lerp(blocker.color.a, 0f, Time.deltaTime / 3.5f);
            Color newC = new(0, 0, 0, newA);
            blocker.color = newC;
        }
        else if (cutsceneText.color.a < .9f)
        {
            float newA = Mathf.Lerp(cutsceneText.color.a, 1f, Time.deltaTime / 1.5f);
            Color newC = new(1, 1, 1, newA);
            cutsceneText.color = newC;
        }
        else if (cutsceneTimer >= 3f && !isDone && !dramaSource.isPlaying)
        {
            dramaSource.Play();
            cutsceneText.SetText("<size=50>Slender the Eight Pages\n<size=38>REWRITTEN");
        }
        else if (cutsceneTimer >= 11f)
        {
            isDone = true;
            dramaSource.loop = false;
            float newA = Mathf.Lerp(blocker.color.a, 1f, Time.deltaTime / 1.9f);
            Color newC = new(0, 0, 0, newA);
            blocker.color = newC;

            if (newA >= .96f)
            {
                SceneManager.LoadScene(1);
            }
        }

        cutsceneTimer = cutsceneText.color.a >= .9f ? cutsceneTimer += Time.deltaTime : 0f;
    }
}
