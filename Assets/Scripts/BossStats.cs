using UnityEngine;
using UnityEngine.AI;

public class BossStats : MonoBehaviour
{
    Animator animator;

    [Header("Boss Stats (ตาม GDD)")]
    public string bossName = "Boss";
    public float health = 500f; //
    public float armor = 20f; //[cite: 3]
    public float damage = 19f; //[cite: 3]
    public float speed = 5f; //[cite: 3]
    public int minionSpawnCount = 5;
    public float expReward = 200f;
    public int moneyReward = 100;

    [Header("AI Settings")]
    public Transform player;
    public float aggroRange = 10f; // ระยะมองเห็น
    public float attackRange = 3f; // ระยะฟันของบอสจะกว้างกว่าลูกน้อง
    public float attackCooldown = 2f; // บอสโจมตีช้ากว่าลูกน้องเล็กน้อย
    private float lastAttackTime;
    private bool isAggroed = false;

    [Header("Minion Spawning")]
    public GameObject minionPrefab; // ช่องสำหรับใส่ Prefab ลูกน้อง
    public float spawnInterval = 10f; // เรียก Minion ทุกๆ 10 วินาที
    private float spawnTimer;

    private NavMeshAgent agent;

    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        if (agent != null) agent.speed = speed;

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }

        spawnTimer = spawnInterval; // เริ่มนับเวลาถอยหลัง
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // ถ้าผู้เล่นเข้าใกล้บอส ให้บอสตื่น
        if (distanceToPlayer <= aggroRange && !isAggroed)
        {
            WakeUpBoss();
        }

        // ถ้าบอสตื่นแล้ว (Aggro)
        if (isAggroed)
        {
            animator.SetBool("agro", true);

            // --- ระบบต่อสู้ ---
            if (distanceToPlayer <= attackRange)
            {
                agent.ResetPath(); // หยุดเดินเพื่อตี
                if (Time.time >= lastAttackTime + attackCooldown)
                {
                    AttackPlayer();
                }
            }
            else
            {
                agent.SetDestination(player.position); // เดินตาม
            }

            // --- ระบบเรียก Minion ---
            spawnTimer -= Time.deltaTime;
            if (spawnTimer <= 0)
            {
                SpawnMinion();
                spawnTimer = spawnInterval; // รีเซ็ตเวลา 10 วินาทีใหม่
            }
        }
    }

    public void TakeDamage(float damageAmount)
    {
        // ถ้าบอสถูกตีจากระยะไกล ให้ตื่นทันที
        if (!isAggroed)
        {
            WakeUpBoss();
        }

        // คำนวณดาเมจผ่านเกราะ บอสมีเกราะ 100 ทำให้โจมตีปกติแทบไม่เข้า
        float finalDamage = Mathf.Max(damageAmount - armor, 1f);
        health -= finalDamage;
        Debug.Log($"{bossName} โดนฟัน! เลือดเหลือ: {health}");

        if (health <= 0)
        {
            Die();
        }
    }

    void WakeUpBoss()
    {
        isAggroed = true;
        Debug.Log("บอสถูกปลุกแล้ว! ระวังลูกน้องมันด้วย!");
    }

    void AttackPlayer()
    {
        lastAttackTime = Time.time;
        transform.LookAt(player.position);
        animator.SetTrigger("hit");
        Debug.Log($"{bossName} ทุบผู้เล่น! สร้างความเสียหาย {damage}");

        PlayerController pController = player.GetComponent<PlayerController>();
        if (pController != null) pController.TakeDamage(damage);
    }

    void SpawnMinion()
    {
        if (minionPrefab != null)
        {
            int bonusSpawns = 0;
            PlayerController pController = player.GetComponent<PlayerController>();

            // ถ้าดึงข้อมูลผู้เล่นได้ ให้นำเลเวลมาหาร 5 เพื่อเป็นจำนวนลูกน้องที่เพิ่มขึ้น
            if (pController != null)
            {
                bonusSpawns = pController.level / 5;
            }

            // จำนวนที่เรียกจริง = ค่าพื้นฐาน + โบนัสเลเวล
            int totalSpawns = minionSpawnCount + bonusSpawns;

            for (int i = 0; i < totalSpawns; i++)
            {
                Vector2 randomCircle = Random.insideUnitCircle.normalized * 3f;
                Vector3 spawnPos = new Vector3(transform.position.x + randomCircle.x, transform.position.y, transform.position.z + randomCircle.y);

                Instantiate(minionPrefab, spawnPos, Quaternion.identity);
            }
            Debug.Log("บอสเรียก Minion ออกมาช่วยแล้ว!");
        }
    }

    void Die()
    {
        Debug.Log($"{bossName} ถูกสังหาร! ชัยชนะเป็นของคุณ!"); // ตามเป้าหมายเกม สังหารบอสเพื่อชัยชนะ
        PlayerController pController = player.GetComponent<PlayerController>();
        if (pController != null)
        {
            // คำนวณตัวคูณโบนัส (ถ้าเลเวล 1-4 จะได้ 0, เลเวล 5-9 จะได้ 1, เลเวล 10-14 จะได้ 2)
            int levelBonusMultiplier = pController.level / 5;

            // สมมติว่าทุกๆ 5 เลเวล จะได้ Exp เพิ่มขึ้นทีละ 50% (ปรับตัวเลข 0.5f ได้ตามต้องการ)
            float finalExpReward = expReward + (expReward * 0.5f * levelBonusMultiplier);

            pController.GainExpAndMoney(finalExpReward, moneyReward);
        }

        UIManager uiManager = FindObjectOfType<UIManager>();
        if (uiManager != null)
        {
            uiManager.TriggerVictory();
        }
        Destroy(gameObject);
    }
}