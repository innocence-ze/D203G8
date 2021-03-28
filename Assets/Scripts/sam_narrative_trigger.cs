using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class sam_narrative_trigger : MonoBehaviour
{
    public int narrativeIndex;
    private string docName = "subtitles.txt";
    private string[] subtitleLines;

    public Text mySubtitles;
    public GameObject myTextBox;

    private bool alreadyShown = false;

    private void Awake()
    {
        myTextBox.SetActive(false);
    }

    void Start()
    {
        StreamReader sr = new StreamReader(Application.dataPath + "/" + docName);
        string docContents = sr.ReadToEnd();
        sr.Close();

        subtitleLines = docContents.Split("\n"[0]);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            if (!alreadyShown)
            {
                alreadyShown = true;
                mySubtitles.text = subtitleLines[narrativeIndex];
                myTextBox.SetActive(true);
            }
        }
    }
}
