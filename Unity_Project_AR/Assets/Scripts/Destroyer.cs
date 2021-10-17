using UnityEngine;

public class Destroyer : MonoBehaviour
{
    // This script is still in development

    [Tooltip("The Object AR Target")]
    public Transform targetMarker;

    // Update is called once per frame
    void Update()
    {
        // Always follow target in z-axis
        Vector3 pos = transform.position;
        pos.z = targetMarker.position.z;
        transform.position = pos;
    }

    private void OnTriggerEnter(Collider other)
    {
        other.GetComponent<Pickup>().Disable();
    }
}
