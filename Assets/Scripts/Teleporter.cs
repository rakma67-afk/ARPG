using UnityEngine;

public class Teleporter : MonoBehaviour
{
    public float interactRange = 3f;
    private PlayerController player;

    public GameObject exitGate;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.GetComponent<PlayerController>();

    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(transform.position, player.transform.position) <= interactRange)
        {
            exitGate.SetActive(true);
            if (Input.GetKeyDown(KeyCode.E))
            {
                player.teleport(new Vector3(0, 1, -20)); // ย้ายผู้เล่นไปยังตำแหน่งที่กำหนด)
                FindAnyObjectByType<Spawner>().SpawnBoss(); // เรียกใช้ฟังก์ชัน SpawnBoss() ของสคริปต์ Spawner
            }
        }
        else
        {
            exitGate.SetActive(false);
        }
    }
}
