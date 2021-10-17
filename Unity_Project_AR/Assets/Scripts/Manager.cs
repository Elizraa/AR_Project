using System.Collections;
using UnityEngine;

public class Manager : MonoBehaviour
{
    [Tooltip("Singleton")]
    public static Manager instance;

    [Tooltip("Wether the marker is detected or not")]
    public bool detected;

    [Tooltip("Wether it is minigame mode or not")]
    public bool minigame;

    [Tooltip("How many seconds the object stay appear to be declared as detected ")]
    public float deltaTimeDetected = 0.5f;

    [Tooltip("App openend")]
    private bool firstOpened = true;

    public AudioSource audioSource;

    public AudioClip detectedSound, notDetectedSound, click, capture, pickup;

    [Tooltip("How many particle to brust out at a time")]
    public int particleEmit = 25;
    public ParticleSystem particle;
    public Material[] particleEffect;

    [Tooltip("Minigame canvas")]
    public GameObject canvasMinigame; 

    // To store coroutine for deltaTimeDetected function
    private Coroutine varCoroDetected = null;

    [Tooltip("The UI To Guide User What To Scan")]
    public GameObject guideScan;

    [Tooltip("Button to Enter the minigame")]
    public GameObject enterMinigameButton;

    [Tooltip("The Main AR Object Target")]
    public ObjectHandler mainObject;

    private void Awake()
    {
        // Make Singleton
        if (instance == null)
            instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        canvasMinigame.SetActive(false);
    }

    /// <summary>
    /// Function for handle detected declared by vuforia
    /// </summary>
    public void onDetected()
    {
        varCoroDetected = StartCoroutine(onDetectedWaits());
    }

    /// <summary>
    /// Function for handle detected declared after modified
    /// </summary>
    IEnumerator onDetectedWaits()
    {
        float timeElapsed = 0f;
        guideScan.SetActive(false);
        while (timeElapsed < deltaTimeDetected) // Wait till object really detected
        {
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        detected = true;
        audioSource.PlayOneShot(detectedSound);
        ParticleEmit();
        varCoroDetected = null;
    }

    /// <summary>
    /// Function for handle notDetected declared by vuforia
    /// </summary>
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

    /// <summary>
    /// Function to start brust particle when object detected
    /// </summary>
    public void ParticleEmit()
    {
        particle.GetComponent<ParticleSystemRenderer>().material = particleEffect[Random.Range(0, particleEffect.Length)];
        particle.Emit(particleEmit);
    }

    /// <summary>
    /// Function to start minigame
    /// </summary>
    public void Minigame()
    {
        PlayButtonClickSound();
        minigame = true;
        mainObject.ResetSize();
        canvasMinigame.SetActive(true);
        enterMinigameButton.SetActive(false);
    }

    /// <summary>
    /// Function to back home
    /// </summary>
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
