using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PCEffect : MonoBehaviour
{
    AudioSource source;

    public AudioClip attack1, attack2, attack3, dash, death, hit, jump, left, right, landing;

    public ParticleSystem fallPS, jumpPS, walkPS;

    public void PlayAttackAudio(int i)
    {
        if (i == 1) source.PlayOneShot(attack1);
        if (i == 2) source.PlayOneShot(attack2);
        if (i == 3) source.PlayOneShot(attack3);
    }

    public void PlayDashAudio() => source.PlayOneShot(dash);
    public void PlayDeathAudio() => source.PlayOneShot(death);
    public void PlayHitAudio() => source.PlayOneShot(dash);
    public void PlayJumpAudio() => source.PlayOneShot(dash);
    public void PlayLeftFootAudio() => source.PlayOneShot(left);
    public void PlayRightFootAudio() => source.PlayOneShot(right);
    public void PlayLandingAudio() => source.PlayOneShot(landing);

    public void ShakeCame() => Camera.main.DOShakePosition(0.3f,100,50);

    public void PlayFallPS() => fallPS.Play();
    public void PlayJumpPS() => jumpPS.Play();
    public void PlayWalkPS() => walkPS.Play();
    public void StopWalkPS() => walkPS.Stop();

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
