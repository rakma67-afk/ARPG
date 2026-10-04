using UnityEngine;
using UnityEngine.AI;

public class EnemyStats : MonoBehaviour
{
    Animator animator;

    [Header("Stats (ตาม GDD)")]
    public string enemyName = "Minion";
    public float health = 50f; //[cite: 2]
    public float armor = 0f; //[cite: 2]
    public float damage = 9f; //[cite: 3]
    public float speed = 15f; //[cite: 2]
    public float expReward = 30f;
    public int moneyReward = 20;

    [Header("AI Settings")]
    public Transform player;
    public float aggroRange = 8f; // ระยะที่ศัตรูจะเริ่มมองเห็นผู้เล่น
    public float attackRange = 2f; // ระยะโจมตี
    public float attackCooldown = 1.5f;
    private float lastAttackTime;
    private bool isAggroed = false; // ตัวแปรเช็กว่าศัตรูโกรธหรือยัง

    private NavMeshAgent agent;

    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        if (agent != null) agent.speed = speed;

        // ค้นหาผู้เล่นอัตโนมัติจาก Tag "Player"
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // เงื่อนไขที่ 1: ถ้าผู้เล่นเดินเข้ามาในระยะมองเห็น ให้โกรธ (Aggro)
        if (distanceToPlayer <= aggroRange)
        {
            isAggroed = true;
        }

        // ถ้าโกรธแล้ว ให้เริ่มทำงาน (วิ่งตาม/โจมตี)
        if (isAggroed)
        {
            animator.SetBool("agro", true);
            if (distanceToPlayer <= attackRange)
            {
                agent.ResetPath(); // หยุดเดินเมื่อเข้าสู่ระยะฟัน

                if (Time.time >= lastAttackTime + attackCooldown)
                {
                    AttackPlayer();
                }
            }
            else
            {
                // ถ้าอยู่นอกระยะฟัน ให้วิ่งตามผู้เล่น
                agent.SetDestination(player.position);
            }
        }
    }

    public void TakeDamage(float damageAmount)
    {
        // เงื่อนไขที่ 2: ถ้าถูกโจมตี (เช่นโดนสกิลยิงไกล) ให้โกรธและหันมาวิ่งไล่ทันที
        isAggroed = true;

        float finalDamage = Mathf.Max(damageAmount - armor, 1f);
        health -= finalDamage;
        Debug.Log($"{enemyName} โดนฟัน! เลือดเหลือ: {health}");

        if (health <= 0)
        {
            Die();
        }
    }

    void AttackPlayer()
    {
        lastAttackTime = Time.time;
        transform.LookAt(player.position); // หันหน้าหาผู้เล่นตอนตี
        animator.SetTrigger("hit");
        Debug.Log($"{enemyName} โจมตีผู้เล่น! สร้างความเสียหาย {damage}");

        // ส่งดาเมจไปที่สคริปต์ PlayerController
        PlayerController pController = player.GetComponent<PlayerController>();
        if (pController != null)
        {
            pController.TakeDamage(damage);
        }
    }

    void Die()
    {
        Debug.Log($"{enemyName} ถูกสังหารแล้ว!");
        PlayerController pController = player.GetComponent<PlayerController>();
        if (pController != null)
        {
            // คำนวณตัวคูณโบนัส (ถ้าเลเวล 1-4 จะได้ 0, เลเวล 5-9 จะได้ 1, เลเวล 10-14 จะได้ 2)
            int levelBonusMultiplier = pController.level / 5;

            // สมมติว่าทุกๆ 5 เลเวล จะได้ Exp เพิ่มขึ้นทีละ 50% (ปรับตัวเลข 0.5f ได้ตามต้องการ)
            float finalExpReward = expReward + (expReward * 0.5f * levelBonusMultiplier);

            pController.GainExpAndMoney(finalExpReward, moneyReward);
        }
        Destroy(gameObject);
    }
}