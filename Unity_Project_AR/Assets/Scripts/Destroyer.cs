using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyer : MonoBehaviour
{
    public Transform targetMarker;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = transform.position;
        pos.z = targetMarker.position.z;
        transform.position = pos;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name + "--Destroyer");
        other.GetComponent<Pickup>().Disable();
    }
}
