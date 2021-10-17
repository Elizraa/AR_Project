using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Transform targetMarker;

    public GameObject prefabPick;

    [HideInInspector]
    public Queue<GameObject> pool = new Queue<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnHeart());
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = transform.position;
        pos.z = targetMarker.position.z;
        transform.position = pos;
    }

    IEnumerator SpawnHeart()
    {
        GameObject temp;
        while(true)
        {
            if (!Manager.instance.minigame)
            {
                yield return null;
                continue;
            }
            if (pool.Count == 0) 
            { 
                temp = Instantiate(prefabPick);
                temp.GetComponent<Pickup>().spawner = this;
            }
            else
            {
                temp = pool.Dequeue();
            }

            temp.SetActive(true);
            temp.transform.SetParent(transform);
            temp.transform.localPosition = new Vector3(Random.Range(-300f,300f), 0f, 0f);

            yield return new WaitForSeconds(2f);
        }
    }

    public void SpawnDestroyed(GameObject temp)
    {
        pool.Enqueue(temp);
    }
}
