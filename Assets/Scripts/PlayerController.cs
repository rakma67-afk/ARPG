using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    private NavMeshAgent agent;
    private Camera mainCamera;

    Animator animator;

    [Header("Player Stats (ตาม GDD)")]
    public float maxHp = 100f;
    public float maxMana = 100f;
    public float maxStamina = 100f;
    public float baseSpeed = 10f;
    public float armor = 0f;
    public float normalAttackDamage = 10f; // พลังโจมตีปกติ

    [Header("Combat Settings")]
    public float attackRange = 2f; // ระยะที่ดาบฟันถึง
    public float attackCooldown = 1f; // ความเร็วในการฟัน (1 วิ/ครั้ง)
    private float lastAttackTime = 0f;
    private GameObject targetEnemy = null; // เป้าหมายที่ถูกล็อคไว้

    [Header("Current Status")]
    public float currentStamina;
    public float currentMana;
    private bool isRolling = false;
    private bool isDashing = false; // สำหรับเช็กสถานะสกิล 1
    public float currentHp;
    public int level = 1;
    public float currentExp = 0f;
    public float maxExp = 250f; // Exp ที่ต้องการเพื่ออัปเลเวล
    public int money = 0; // เงินสำหรับใช้จ่าย

    [Header("Stamina & Roll Settings")]
    public float rollCost = 15f;
    public float staminaRegenRate = 15f;
    public float staminaRegenDelay = 1f;
    private float lastRollTime;
    [Header("Skill 1: Dash (A)")]
    public float skill1ManaCost = 5f; //[cite: 2]
    public float skill1Cooldown = 7f; //[cite: 2]
    public float skill1Timer = 0f;
    public float dashSpeedMultiplier = 2f; // เร่งความเร็วขึ้น 2 เท่า
    public float dashDuration = 3f; // เป็นเวลา 3 วินาที
    [Header("Skill 2: Slashing Wave (S)")]
    public float skill2ManaCost = 15f; //[cite: 2]
    public float skill2Cooldown = 12f; //[cite: 2]
    public float skill2Damage = 50f; //[cite: 2]
    public float skill2Timer = 0f;
    public float waveRange = 18f; // ระยะคลื่นดาบด้านหน้า
    [Header("Skill 3: Sweeping Edge (D)")]
    public float skill3ManaCost = 10f; //[cite: 2]
    public float skill3Cooldown = 10f; //[cite: 2]
    public float skill3Damage = 25f; //[cite: 2]
    public float skill3Timer = 0f;
    public float sweepRadius = 8f; // รัศมีโจมตีรอบตัว

    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        mainCamera = Camera.main;

        agent.speed = baseSpeed;
        currentStamina = maxStamina;
        currentMana = maxMana; // ตั้งค่าเริ่มต้นมานา
        currentHp = maxHp;
    }

    void Update()
    {
        if (!agent.pathPending)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    animator.SetBool("moving", false);
                }
            }
        }

        if (isRolling) return;

        UpdateSkillCooldowns();

        HandleInput();
        HandleCombatMovement(); // จัดการการวิ่งไปหาศัตรู
        HandleRoll();
        HandleSkills(); // ตรวจสอบการกดใช้สกิล
        RegenerateStamina();
    }

    void UpdateSkillCooldowns()
    {
        // นับเวลาถอยหลัง Cooldown ของแต่ละสกิล
        if (skill1Timer > 0) skill1Timer -= Time.deltaTime;
        if (skill2Timer > 0) skill2Timer -= Time.deltaTime;
        if (skill3Timer > 0) skill3Timer -= Time.deltaTime;
    }
    void HandleSkills()
    {
        // Skill 1: Dash to speed up (ปุ่ม A)[cite: 2, 3]
        if (Input.GetKeyDown(KeyCode.A) && skill1Timer <= 0 && currentMana >= skill1ManaCost)
        {
            StartCoroutine(Skill1DashRoutine());
        }

        // Skill 2: Slashing wave (ปุ่ม S)[cite: 2, 3]
        if (Input.GetKeyDown(KeyCode.S) && skill2Timer <= 0 && currentMana >= skill2ManaCost)
        {
            UseSkill2SlashingWave();
            animator.SetTrigger("skill2");
        }

        // Skill 3: Sweeping edge (ปุ่ม D)[cite: 2, 3]
        if (Input.GetKeyDown(KeyCode.D) && skill3Timer <= 0 && currentMana >= skill3ManaCost)
        {
            UseSkill3SweepingEdge();
            animator.SetTrigger("skill3");
        }
    }
    IEnumerator Skill1DashRoutine()
    {
        currentMana -= skill1ManaCost;
        skill1Timer = skill1Cooldown;
        isDashing = true;

        Debug.Log("ใช้ Skill 1: Dash! (มานาเหลือ: " + currentMana + ")");
        agent.speed = baseSpeed * dashSpeedMultiplier; // เพิ่มความเร็ว

        yield return new WaitForSeconds(dashDuration);

        agent.speed = baseSpeed; // คืนค่าความเร็วปกติ
        isDashing = false;
    }
    void UseSkill2SlashingWave()
    {
        currentMana -= skill2ManaCost;
        skill2Timer = skill2Cooldown;

        Debug.Log("ใช้ Skill 2: Slashing wave! สร้างความเสียหาย: " + skill2Damage);
        agent.ResetPath(); // หยุดเดินเพื่อใช้สกิล

        // หันหน้าไปทางเมาส์เพื่อให้ปล่อยคลื่นดาบได้ตรงทิศ
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 targetDir = hit.point - transform.position;
            targetDir.y = 0;
            transform.rotation = Quaternion.LookRotation(targetDir);
        }

        // โจมตีศัตรูที่อยู่ด้านหน้า (ใช้ OverlapSphere เช็กศัตรูในระยะด้านหน้า)
        Vector3 waveCenter = transform.position + transform.forward * (waveRange / 2);
        Collider[] hitColliders = Physics.OverlapSphere(waveCenter, waveRange / 2);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Enemy"))
            {
                EnemyStats stats = hitCollider.GetComponent<EnemyStats>();
                if (stats != null) stats.TakeDamage(skill2Damage);

                BossStats bossStats = hitCollider.GetComponent<BossStats>();
                if (bossStats != null) bossStats.TakeDamage(skill2Damage);
            }
        }
    }
    void UseSkill3SweepingEdge()
    {
        currentMana -= skill3ManaCost;
        skill3Timer = skill3Cooldown;

        Debug.Log("ใช้ Skill 3: Sweeping edge! สร้างความเสียหายรอบตัว: " + skill3Damage);
        agent.ResetPath();

        // โจมตีศัตรูรอบตัว (กวาดดาบ 360 องศา)
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, sweepRadius);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Enemy"))
            {
                EnemyStats stats = hitCollider.GetComponent<EnemyStats>();
                if (stats != null) stats.TakeDamage(skill3Damage);
                BossStats bossStats = hitCollider.GetComponent<BossStats>();
                if (bossStats != null) bossStats.TakeDamage(skill3Damage);
            }
        }
    }

    void HandleInput()
    {
        // ใช้เมาส์ซ้าย (M1)
        if (Input.GetMouseButton(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                animator.SetBool("moving", true);
                // ถ้าคลิกโดนศัตรู
                if (hit.collider.CompareTag("Enemy"))
                {
                    targetEnemy = hit.collider.gameObject; // ล็อคเป้า
                }
                // ถ้าคลิกโดนพื้น
                else if (hit.collider.CompareTag("Ground"))
                {
                    targetEnemy = null; // ยกเลิกการล็อคเป้า
                    agent.SetDestination(hit.point); // เดินไปที่พื้น
                    
                    Debug.Log("ตำแหน่ง "+ agent.transform.position);
                    Debug.Log("hit "+ hit.point);
                }
            }
        }
    }

    void HandleCombatMovement()
    {
        // ถ้ามีเป้าหมายเป็นศัตรู (และศัตรูยังไม่ตาย)
        if (targetEnemy != null)
        {
            float distance = Vector3.Distance(transform.position, targetEnemy.transform.position);

            // ถ้าอยู่ในระยะฟัน
            if (distance <= attackRange)
            {
                agent.ResetPath(); // สั่งให้หยุดเดิน

                // เช็ก Cooldown การโจมตี
                if (Time.time >= lastAttackTime + attackCooldown)
                {
                    Attack();
                }
            }
            else
            {
                // ถ้าอยู่นอกระยะ ให้วิ่งเข้าไปหาศัตรู
                agent.SetDestination(targetEnemy.transform.position);
            }
        }
    }

    void Attack()
    {
        lastAttackTime = Time.time;

        // หันหน้าหาศัตรูตอนฟัน
        transform.LookAt(targetEnemy.transform.position);

        if (animator != null)
        {
            animator.SetTrigger("attack");
        }
        // เรียกฟังก์ชันเสียเลือดของศัตรู
        EnemyStats stats = targetEnemy.GetComponent<EnemyStats>();
        if (stats != null)
        {
            stats.TakeDamage(normalAttackDamage);
        }
        else
        {
            BossStats bossStats = targetEnemy.GetComponent<BossStats>();
            if (bossStats != null)
            {
                bossStats.TakeDamage(normalAttackDamage);
            }
        }
    }

    void HandleRoll()
    {
        if (Input.GetKeyDown(KeyCode.Space) && currentStamina >= rollCost)
        {
            targetEnemy = null; // ยกเลิกเป้าหมายถ้ากลิ้งหลบ
            StartCoroutine(RollRoutine());
            animator.SetTrigger("dodge");
        }
    }

    void RegenerateStamina()
    {
        if (Time.time >= lastRollTime + staminaRegenDelay)
        {
            if (currentStamina < maxStamina)
            {
                currentStamina += staminaRegenRate * Time.deltaTime;
                if (currentStamina > maxStamina) currentStamina = maxStamina;
            }
        }
    }

    IEnumerator RollRoutine()
    {
        isRolling = true;
        currentStamina -= rollCost;
        lastRollTime = Time.time;

        float originalSpeed = agent.speed;
        float originalAcceleration = agent.acceleration;

        agent.speed = 30f;
        agent.acceleration = 100f;

        Vector3 rollDirection = transform.forward;
        if (agent.velocity.sqrMagnitude > 0.1f) rollDirection = agent.velocity.normalized;

        Vector3 rollTarget = transform.position + rollDirection * 5f;
        agent.SetDestination(rollTarget);

        yield return new WaitForSeconds(0.3f);

        agent.speed = originalSpeed;
        agent.acceleration = originalAcceleration;
        agent.ResetPath();
        isRolling = false;
    }
    public void TakeDamage(float damage)
    {
        // คำนวณหักลบเกราะก่อน (ถ้าผู้เล่นซื้ออัปเกรดเกราะมาแล้ว)
        float finalDamage = Mathf.Max(damage - armor, 1f);
        currentHp -= finalDamage;
        Debug.Log($"ผู้เล่นถูกโจมตี! เลือดเหลือ: {currentHp}");

        if (currentHp <= 0)
        {
            Debug.Log("ผู้เล่นตาย!");
            // ในอนาคตเราจะใส่ระบบ Game Over ที่นี่
            UIManager uiManager = FindObjectOfType<UIManager>();
            if (uiManager != null)
            {
                uiManager.TriggerGameOver();
            }
            animator.SetTrigger("dead");
            agent.isStopped = true; // สั่งให้ NavMeshAgent เบรกกะทันหัน
            this.enabled = false; // ปิดสคริปต์ตัวนี้ทิ้ง เพื่อไม่ให้รับคำสั่งเมาส์หรือสกิลใดๆ อีก
            
        }
        else
        {
            animator.SetTrigger("hit");
        }
    }
    public void GainExpAndMoney(float expAmount, int moneyAmount)
    {
        money += moneyAmount;
        currentExp += expAmount;
        Debug.Log($"ได้รับ Exp: {expAmount} และเงิน: {moneyAmount} (เงินรวม: {money})");

        if (currentExp >= maxExp)
        {
            LevelUp(); // สังหารศัตรูเพื่อเลื่อนขั้น
        }
    }

    void LevelUp()
    {
        level++;
        currentExp -= maxExp;
        maxExp *= 1.2f; // เลเวลต่อไปใช้ Exp เยอะขึ้น 20%

        // ฟื้นฟูสถานะเมื่อเลเวลอัป
        currentHp = maxHp;
        currentMana = maxMana;
        currentStamina = maxStamina;

        Debug.Log($"Level Up! ตอนนี้คุณเลเวล {level} แล้ว");
    }
}