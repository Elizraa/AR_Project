using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectHandler : MonoBehaviour
{
    public float degreePerSecond = 15f;
    public float zoomSpeed = 0.01f;
    public float rotateSpeed = 0.5f;

    Touch touchZero, touchOne;
    float prevTouchDeltaMag;
    Vector3 prevTouchPos;

    public Transform particleObject;

    Vector3 defaultSize, defaultRotation;

    public FloatVariable score;

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
        if(Input.touchCount == 1 && Manager.instance.detected)
        {
            touchZero = Input.GetTouch(0);
            prevTouchPos = touchZero.position - touchZero.deltaPosition;
        }

        if (Input.touchCount == 2 && Manager.instance.detected && !Manager.instance.minigame)
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
        if(Input.touchCount == 1 && Manager.instance.detected)
        {
            Vector2 deltaTouch = touchZero.position - (Vector2)prevTouchPos;
            Vector3 temp = transform.localEulerAngles;
            temp.x += deltaTouch.x * rotateSpeed;
            temp.y = defaultRotation.y;
            temp.z = defaultRotation.z;
            transform.localEulerAngles = temp;
        }

        if (Input.touchCount == 2 && Manager.instance.detected && !Manager.instance.minigame)
        {
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            // Find the difference in the distances between each frame.
            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

            Vector3 zoom = new Vector3(zoomSpeed, -zoomSpeed, zoomSpeed * 0.1f);

            if (deltaMagnitudeDiff > 0)
            {
                transform.localScale = transform.localScale - zoom;
                particleObject.localScale = particleObject.localScale - zoom;
            }
            else if (deltaMagnitudeDiff < 0)
            {
                transform.localScale = transform.localScale + zoom;
                particleObject.localScale = particleObject.localScale + zoom;
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
            if (!Manager.instance.detected || Manager.instance.minigame)
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

    public void ResetSize()
    {
        transform.localScale = defaultSize;
        transform.localRotation = Quaternion.Euler(defaultRotation);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name);
        Manager.instance.PlayHeartPickupSound();
        score.value++;
        other.GetComponent<Pickup>()?.Disable();
    }
}
