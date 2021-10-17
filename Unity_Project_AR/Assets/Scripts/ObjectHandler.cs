using System.Collections;
using UnityEngine;

public class ObjectHandler : MonoBehaviour
{
    [Tooltip("How many degree to rotate in automatically mode")]
    public float degreePerSecond = 15f;

    [Tooltip("How much scalling done when pinching object")]
    public float zoomSpeed = 0.01f;

    [Tooltip("How many degree to rotate in dragging mode")]
    public float rotateSpeed = 0.5f;

    Touch touchZero, touchOne;
    float prevTouchDeltaMag;
    Vector3 prevTouchPos;

    [Tooltip("Particle to scale with this object")]
    public Transform particleObject;

    Vector3 defaultSize, defaultRotation;

    [Tooltip("Score in minigame")]
    public FloatVariable score;

    void Start()
    {
        // Get the default size to use it later when resettting or maintaining variable
        defaultSize = transform.localScale;
        defaultRotation = new Vector3(90f, -90f, 90f);

        // Start the automatically rotate mode
        StartCoroutine(Rotate(90 / degreePerSecond));
    }

    void Update()
    {
        // Detecting first touch position for rotate object
        if(Input.touchCount == 1 && Manager.instance.detected)
        {
            touchZero = Input.GetTouch(0);
            prevTouchPos = touchZero.position - touchZero.deltaPosition;
        }

        // Detetcting touches for scalling object
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
        // For calculating the difference position between frame for each touches that exist
        // Rotating
        if(Input.touchCount == 1 && Manager.instance.detected)
        {
            Vector2 deltaTouch = touchZero.position - (Vector2)prevTouchPos;

            // Get the current rotation object value
            Vector3 temp = transform.localEulerAngles;

            // Only rotate on x-axis
            temp.x += deltaTouch.x * rotateSpeed;
            temp.y = defaultRotation.y;
            temp.z = defaultRotation.z;

            // Apply
            transform.localEulerAngles = temp;
        }

        // Scalling
        if (Input.touchCount == 2 && Manager.instance.detected && !Manager.instance.minigame)
        {
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            // Find the difference in the distances between each frame.
            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

            // Different axis will have different scale value
            Vector3 zoom = new Vector3(zoomSpeed, -zoomSpeed, zoomSpeed * 0.1f);

            // Zoom out
            if (deltaMagnitudeDiff > 0)
            {
                transform.localScale = transform.localScale - zoom;
                particleObject.localScale = particleObject.localScale - zoom;
            }

            // Zoom in
            else if (deltaMagnitudeDiff < 0)
            {
                transform.localScale = transform.localScale + zoom;
                particleObject.localScale = particleObject.localScale + zoom;
            }
        }
    }


    /// <summary>
    /// Rotate gameObject 90 degree from the current rotation value
    /// </summary>
    /// <param name="duration">The duration it take to rotate</param>
    IEnumerator Rotate(float duration)
    {
        float timeElapsed = 0;
        float startRotation = transform.localEulerAngles.x;
        float targetRotation = startRotation + 90f;

        while (timeElapsed < duration)
        {
            if (Input.touchCount == 1) // Will not rotate automatically if manual rotate started
            {
                break;
            }
            if (!Manager.instance.detected || Manager.instance.minigame) // Will not rotate if object not detected or in minigame state
            {
                yield return null;
                continue;
            }
            float temp = Mathf.LerpAngle(startRotation, targetRotation, timeElapsed / duration); // Lerp
            transform.localRotation = Quaternion.Euler(new Vector3(temp, defaultRotation.y, defaultRotation.z)); // Applu
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        if (Input.touchCount == 1)
        {
            while (Input.touchCount == 1) // Wait until manual rotate stopped
            {
                yield return null;
            }
            StartCoroutine(Rotate(90 / degreePerSecond)); // Recursive
            yield break;
        }
        transform.localRotation = Quaternion.Euler(targetRotation, defaultRotation.y, defaultRotation.z);
        StartCoroutine(Rotate(90 / degreePerSecond)); // Recursive
    }

    /// <summary>
    /// Reset object to the default value size and rotation
    /// </summary>
    public void ResetSize()
    {
        transform.localScale = defaultSize;
        transform.localRotation = Quaternion.Euler(defaultRotation);
    }

    private void OnTriggerEnter(Collider other)
    {
        Manager.instance.PlayHeartPickupSound();
        score.value++;
        other.GetComponent<Pickup>()?.Disable();
    }
}
