using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectHandler : MonoBehaviour
{
    bool firstOpened = true;
    bool onScreen = false;

    public float degreePerSecond;

    void Start()
    {
        StartCoroutine(Rotate(90/degreePerSecond));
    }

    IEnumerator Rotate(float duration)
    {
        float timeElapsed = 0;
        Quaternion startRotation = transform.rotation;
        Quaternion targetRotation = transform.rotation * Quaternion.Euler(0, 90, 0);

        while (timeElapsed < duration)
        {
            Vector3 temp = Quaternion.Slerp(startRotation, targetRotation, timeElapsed / duration).eulerAngles;
            temp.x = 0;
            temp.z = 0;
            transform.localRotation = Quaternion.Euler(temp);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        transform.rotation = targetRotation;
        StartCoroutine(Rotate(90/degreePerSecond));
    }
}
