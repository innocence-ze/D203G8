using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BGM : MonoBehaviour
{
    private static BGM _instance;
    public static BGM Instance { get { return _instance; } }
    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(this.gameObject); return;
        }
        else
        {
            _instance = this;
        }
    }

    public AudioClip start, angry, sadness;
    AudioSource audioSource;

    int lastSceneIndex;

    enum sceneType
    {
        start,
        angry,
        sadness,
    }

    sceneType st = sceneType.start;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        DontDestroyOnLoad(gameObject);
        lastSceneIndex = SceneManager.GetActiveScene().buildIndex;
    }

    private void Update()
    {
        transform.position = Camera.main.transform.position;
        var scene = SceneManager.GetActiveScene();
        var curIndex = scene.buildIndex;
        if (curIndex != lastSceneIndex)
        {
            char c = scene.name[0];
            if (c == 'A')
            {
                if (st != sceneType.angry)
                {
                    st = sceneType.angry;
                    audioSource.clip = angry;
                    audioSource.Play();
                }
            }
            else if(c == 'B')
            {
                if (st != sceneType.sadness)
                {
                    st = sceneType.sadness;
                    audioSource.clip = sadness;
                    audioSource.Play();
                }
            }
            else
            {
                if (st != sceneType.start)
                {
                    st = sceneType.start;
                    audioSource.clip = start;
                    audioSource.Play();
                }
            }
            lastSceneIndex = curIndex;
        }
    }

}
