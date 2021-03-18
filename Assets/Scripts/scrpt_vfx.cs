using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * This is a work in progress script for the random particles flowing around on the screen
 * -> to be added: scaling down instead of destroy,
 * --> stretch: running on a separate thread
 * Date created: 18/3/21
 * By: Sam
 */

public class scrpt_vfx : MonoBehaviour
{
    public GameObject myAngerParticle;
    public GameObject mySadnessParticle;

    private GameObject currentParticle;

    private bool scalingUp = false;
    private Queue<GameObject> particlesToScale = new Queue<GameObject>();
    private Queue<GameObject> particlesOnScreen = new Queue<GameObject>();

    void Start()
    {
        for (int i = 0; i <= Random.Range(8, 12); i++)
        {
            //creating a number of particles on screen when the game starts
            currentParticle = Instantiate(myAngerParticle, Camera.main.ViewportToWorldPoint(new Vector3(Random.value, Random.value, Random.Range(1f, 7f))), Quaternion.identity);

            float scale = Random.Range(0.35f, 1f);
            currentParticle.transform.localScale = new Vector3(scale, scale, 0);

            particlesOnScreen.Enqueue(currentParticle);
        }
        StartCoroutine(ParticlesSpawner());
        StartCoroutine(ParticlesDelete());
    }

    private IEnumerator ParticlesSpawner()
    {
        //spawning a new particle every few seconds
        while (true)
        {
            if (particlesOnScreen.Count <= 24)
            {
                NewParticle();
            }
            yield return new WaitForSeconds(Random.Range(3, 6));
        }
    }

    private void NewParticle()
    {
        //creating a particle and adding it to the queue to be scaled up to the desired size
        currentParticle = Instantiate(myAngerParticle, Camera.main.ViewportToWorldPoint(new Vector3(Random.value, Random.value, Random.Range(1f, 7f))), Quaternion.identity);
        currentParticle.transform.localScale = Vector3.zero;
        particlesToScale.Enqueue(currentParticle);

    }

    private void Update()
    {
        //checking if the game isn't scaling any particle at the moment
        if (particlesToScale.Count > 0 && !scalingUp)
        {
            StartCoroutine(ParticlesScaleUp(Random.Range(0.35f, 1f), particlesToScale.Dequeue()));
            scalingUp = true;
        }
    }

    private IEnumerator ParticlesScaleUp(float maxScale, GameObject particle)
    {
        //scaling the newly created particle up to the desired size
        Vector3 addition = new Vector3(0.005f, 0.005f, 0f);
        Vector3 currentScale = Vector3.zero;
        while (currentScale.x < maxScale)
        {
            currentScale += addition;
            particle.transform.localScale = currentScale;
            yield return new WaitForEndOfFrame();
        }
        particlesOnScreen.Enqueue(particle);
        scalingUp = false;
        yield break;
    }

    private IEnumerator ParticlesDelete()
    {
        //deleting the particle that was on screen the longest every few seconds
        GameObject lastParticle;
        while (true)
        {
            if (particlesOnScreen.Count > 13)
            {
                lastParticle = particlesOnScreen.Dequeue();
                Destroy(lastParticle);
            }
            yield return new WaitForSeconds(Random.Range(4, 6));
        }
    }
}
