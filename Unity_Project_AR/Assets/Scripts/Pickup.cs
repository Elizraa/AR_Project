using UnityEngine;

public class Pickup : MonoBehaviour
{
    // This script is still in development

    [Tooltip("The owner of this object")]
    public Spawner spawner;

    /// <summary>
    /// Disable object and send it to the pool to queue for spawning
    /// </summary>
    public void Disable()
    {
        gameObject.SetActive(false);
        spawner.SpawnDestroyed(gameObject);
    }
}
