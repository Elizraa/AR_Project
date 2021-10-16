using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    public static Manager instance;

    public bool detected;

    private bool firstOpened = true;

    public AudioSource audioSource;

    public AudioClip detectedSound, notDetectedSound;

    public int particleEmit = 25;
    public ParticleSystem particle;
    public Material[] particleEffect;

    public float deltaTimeDetected = 0.5f;

    Coroutine varCoroDetected = null;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void onDetected()
    {
        varCoroDetected = StartCoroutine(onDetectedWaits());
    }

    IEnumerator onDetectedWaits()
    {
        float timeElapsed = 0f;
        while (timeElapsed < deltaTimeDetected)
        {
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        detected = true;
        audioSource.PlayOneShot(detectedSound);
        ParticleEmit();
        varCoroDetected = null;
    }

    public void offDetected()
    {
        if(firstOpened)
        {
            firstOpened = false;
            return;
        }
        if (varCoroDetected != null)
        {
            StopCoroutine(varCoroDetected);
            return;
        }
        audioSource.PlayOneShot(notDetectedSound);
        detected = false;
    }

    public void ParticleEmit()
    {
        particle.GetComponent<ParticleSystemRenderer>().material = particleEffect[Random.Range(0, particleEffect.Length)];
        particle.Emit(particleEmit);
    }
}
