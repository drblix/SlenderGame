using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Page : MonoBehaviour
{
    private GameManager gameManager;

    private AudioSource source;
    private BoxCollider bCollider;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        source = GetComponent<AudioSource>();
        bCollider = GetComponent<BoxCollider>();
    }

    public void CollectPage()
    {
        if (!bCollider.enabled) { return; }
        bCollider.enabled = false;

        source.Play();
        StartCoroutine(gameManager.PageAdded(gameObject));
        //Destroy(gameObject, source.clip.length);

        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }
}
