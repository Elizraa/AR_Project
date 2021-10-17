using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    public static Manager instance;

    public bool detected;
    public bool minigame;
    public float deltaTimeDetected = 0.5f;

    private bool firstOpened = true;

    public AudioSource audioSource;

    public AudioClip detectedSound, notDetectedSound, click, capture, pickup;

    public int particleEmit = 25;
    public ParticleSystem particle;
    public Material[] particleEffect;

    public GameObject canvasMinigame; 

    private Coroutine varCoroDetected = null;

    public GameObject guideScan;

    public GameObject enterMinigameButton;

    public ObjectHandler mainObject;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        canvasMinigame.SetActive(false);
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
        guideScan.SetActive(false);
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
        guideScan.SetActive(true);
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

    public void Minigame()
    {
        PlayButtonClickSound();
        minigame = true;
        mainObject.ResetSize();
        canvasMinigame.SetActive(true);
        enterMinigameButton.SetActive(false);
    }

    public void Home()
    {
        PlayButtonClickSound();
        minigame = false;
        canvasMinigame.SetActive(false);
        enterMinigameButton.SetActive(true);
    }

    public void PlayButtonClickSound()
    {
        audioSource.PlayOneShot(click);
    }

    public void PlayButtonCaptureSound()
    {
        audioSource.PlayOneShot(capture);
    }

    public void PlayHeartPickupSound()
    {
        audioSource.PlayOneShot(pickup);
    }
}
