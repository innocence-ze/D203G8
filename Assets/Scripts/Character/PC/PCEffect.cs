using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PCEffect : MonoBehaviour
{
    AudioSource source;

    public AudioClip attack1, attack2, attack3, dash, death, hit, jump, walk;

    public void PlayAttackAudio(int i)
    {
        if(i == 0)  source.clip = attack1;
        if(i == 1)  source.clip = attack2;
        if(i == 2)  source.clip = attack3;
        source.Play();
        source.loop = false;
    }

    public void PlayDashAudio()
    {
        source.clip = dash;
        source.Play();
        source.loop = false;
    }

    public void PlayDeathAudio()
    {
        source.clip = death;
        source.Play();
        source.loop = false;
    }

    public void PlayHitAudio()
    {
        source.clip = hit;
        source.Play();
        source.loop = false;
    }

    public void PlayJumpAudio() 
    {
        source.clip = jump;
        source.Play();
        source.loop = false;
    }

    public void PlayWalkAudio()
    {
        source.clip = walk;
        source.Play();
        source.loop = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
