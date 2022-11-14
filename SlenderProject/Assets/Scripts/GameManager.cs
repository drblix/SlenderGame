using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private RawImage staticVid;
    [SerializeField]
    private Image headshotImg;
    [SerializeField]
    private Image blockerImg;
    [SerializeField]
    private AudioSource harshStatic;

    [SerializeField]
    private AudioSource spookSource;

    [SerializeField]
    private TextMeshProUGUI pageDisplay;

    [SerializeField]
    private AudioClip[] noises;

    private const int PAGES_NEEDED = 8;

    // starts at 0
    private int currentPages = 8;
    public int CurrentPages { get { return currentPages; } }

    private bool gameOver = false;
    public bool GameOver { get { return gameOver; } }

    public IEnumerator PageAdded()
    {
        // add one to current pages
        // display ui element that shows current pages out of pages needed
        currentPages++;
        pageDisplay.SetText(currentPages + " / " + PAGES_NEEDED + " Pages");
        pageDisplay.gameObject.SetActive(true);

        yield return new WaitForSeconds(2f);

        pageDisplay.gameObject.SetActive(false);

        if (CurrentPages == 1)
        {
            StartCoroutine(PlaySound(noises[0], 4));
        }
    }

    private IEnumerator PlaySound(AudioClip clip, int repeatNum)
    {
        spookSource.clip = clip;

        for (int i = 0; i < repeatNum; i++)
        {
            spookSource.Play();
            yield return new WaitForSeconds(clip.length);
        }

        spookSource.Stop();
    }

    public IEnumerator PlayerDeath()
    {
        gameOver = true;
        staticVid.color = Color.white;
        print("Game over!");
        yield return new WaitForSeconds(1.8f);
        blockerImg.gameObject.SetActive(true);
        blockerImg.color = Color.black;
        staticVid.color = Color.clear;
        harshStatic.Stop();
        yield return new WaitForSeconds(.25f);
        headshotImg.color = Color.white;
        blockerImg.color = Color.clear;
        staticVid.color = Color.white;
        harshStatic.Play();
        yield return new WaitForSeconds(.3f);
        headshotImg.color = Color.clear;
        staticVid.color = Color.clear;
        blockerImg.color = Color.black;
        harshStatic.Stop();
        yield return new WaitForSeconds(.2f);
        headshotImg.color = Color.white;
        blockerImg.color = Color.clear;
        staticVid.color = Color.white;
        harshStatic.Play();
        yield return new WaitForSeconds(.15f);
        headshotImg.color = Color.clear;
        staticVid.color = Color.clear;
        blockerImg.color = Color.black;
        harshStatic.Stop();
        yield return new WaitForSeconds(.3f);
        headshotImg.color = Color.white;
        blockerImg.color = Color.clear;
        staticVid.color = Color.white;
        harshStatic.Play();
    }
}
