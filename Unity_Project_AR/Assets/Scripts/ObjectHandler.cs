using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectHandler : MonoBehaviour
{
    public float degreePerSecond = 15f;
    public float zoomSpeed = 0.01f;
    public float rotateSpeed = 0.5f;

    public int particleEmit = 25;
    public ParticleSystem particle;
    public Material[] particleEffect;



    Touch touchZero, touchOne;
    float prevTouchDeltaMag;
    Vector3 prevTouchPos;

    Vector3 defaultSize, defaultRotation;

    void Start()
    {
        defaultSize = transform.localScale;
        defaultRotation = new Vector3(90f, -90f, 90f);
        StartCoroutine(Rotate(90 / degreePerSecond));
        //StartCoroutine(Rotation());
    }

    void Update()
    {
        //Rotate2(rotatespeed);
        if(Input.touchCount == 1)
        {
            touchZero = Input.GetTouch(0);
            prevTouchPos = touchZero.position - touchZero.deltaPosition;
        }

        if (Input.touchCount == 2 && Manager.instance.detected)
        {
            // Store both touches.
            touchZero = Input.GetTouch(0);
            touchOne = Input.GetTouch(1);

            // Find the position in the previous frame of each touch.
            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            // Find the magnitude of the vector (the distance) between the touches in each frame.
            prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;

        }
    }

    private void LateUpdate()
    {
        if(Input.touchCount == 1)
        {
            Vector2 deltaTouch = touchZero.position - (Vector2)prevTouchPos;
            Vector3 temp = transform.localEulerAngles;
            temp.x += deltaTouch.x * rotateSpeed;
            temp.y = defaultRotation.y;
            temp.z = defaultRotation.z;
            transform.localEulerAngles = temp;
        }

        if (Input.touchCount == 2 && Manager.instance.detected)
        {
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            // Find the difference in the distances between each frame.
            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

            Vector3 zoom = new Vector3(zoomSpeed, -zoomSpeed, zoomSpeed * 0.1f);

            if (deltaMagnitudeDiff > 0)
            {
                transform.localScale = transform.localScale - zoom;
            }
            else if (deltaMagnitudeDiff < 0)
            {
                transform.localScale = transform.localScale + zoom;
            }
        }
    }

    IEnumerator Rotate(float duration)
    {
        float timeElapsed = 0;
        float startRotation = transform.localEulerAngles.x;
        float targetRotation = startRotation + 90f;

        while (timeElapsed < duration)
        {
            if (Input.touchCount == 1)
            {
                break;
            }
            if (!Manager.instance.detected)
            {
                yield return null;
                continue;
            }
            float temp = Mathf.LerpAngle(startRotation, targetRotation, timeElapsed / duration);
            //localEuler = new Vector3(temp, defaultRotation.y, defaultRotation.z);
            transform.localRotation = Quaternion.Euler(new Vector3(temp, defaultRotation.y, defaultRotation.z));
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        if (Input.touchCount == 1)
        {
            while (Input.touchCount == 1)
            {
                yield return null;
            }
            StartCoroutine(Rotate(90 / degreePerSecond));
            yield break;
        }
        transform.localRotation = Quaternion.Euler(targetRotation, defaultRotation.y, defaultRotation.z);
        StartCoroutine(Rotate(90 / degreePerSecond));
    }

      public void ParticleEmit()
    {
        particle.GetComponent<ParticleSystemRenderer>().material = particleEffect[Random.Range(0, particleEffect.Length)];
        particle.Emit(particleEmit);
        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[particle.main.maxParticles];
        particle.GetParticles(particles);
        for(int i = 0; i < particles.Length; i++)
        {
            Vector3 temp = particles[i].velocity;
            temp.z = 0;
            particles[i].velocity = temp;
        }
    }
    

    public void ResetTransform()
    {
        transform.localScale = defaultSize;
        transform.localRotation = Quaternion.Euler(defaultRotation);
    }
}
