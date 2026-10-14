using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject bossPrefab;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnBoss()
    {
        if (bossPrefab != null)
        {
            Instantiate(bossPrefab, transform.position, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("Boss Prefab is not assigned in the Spawner.");
        }
    }
}
