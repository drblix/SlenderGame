using System.Collections;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private AudioSource spookSource;

    [SerializeField]
    private TextMeshProUGUI pageDisplay;

    [SerializeField]
    private AudioClip[] noises;

    private const int PAGES_NEEDED = 8;
    private int currentPages = 0;
    public int CurrentPages { get { return currentPages; } }

    public IEnumerator PageAdded(GameObject page)
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
}
