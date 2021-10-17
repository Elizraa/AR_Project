using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    // This script is still in development

    [Tooltip("The Object AR Target")]
    public Transform targetMarker;

    [Tooltip("Prefab to spawn")]
    public GameObject prefabPick;

    // pool for queue and reuse gameObject
    [HideInInspector]
    public Queue<GameObject> pool = new Queue<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        // Start spawning
        StartCoroutine(SpawnHeart());
    }

    // Update is called once per frame
    void Update()
    {
        // Always follow target in z-axis
        Vector3 pos = transform.position;
        pos.z = targetMarker.position.z;
        transform.position = pos;
    }

    /// <summary>
    /// Spawning object
    /// </summary>
    IEnumerator SpawnHeart()
    {
        // To store temporary gameObject
        GameObject temp;
        while(true)
        {
            if (!Manager.instance.minigame) // Only spawn when state is minigame
            {
                yield return null;
                continue;
            }
            if (pool.Count == 0) // Spawn new gameObject if queue is zero
            { 
                temp = Instantiate(prefabPick);
                temp.GetComponent<Pickup>().spawner = this;
            }
            else // Use the existing gameObject in queue
            {
                temp = pool.Dequeue();
            }

            // Set position
            temp.SetActive(true);
            temp.transform.SetParent(transform);
            temp.transform.localPosition = new Vector3(Random.Range(-300f,300f), 0f, 0f);

            yield return new WaitForSeconds(2f);
        }
    }

    /// <summary>
    /// Add GameObject to the pool in this component
    /// </summary>
    /// <param name="temp">The GameObject to add to the pool</param>
    public void SpawnDestroyed(GameObject temp)
    {
        pool.Enqueue(temp);
    }
}
