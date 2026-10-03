using UnityEngine;

public class NPCMerchant : MonoBehaviour
{
    public float interactRange = 3f;
    private PlayerController player;

    [Header("UI Shop")]
    public GameObject shopPanel; // นำ NPCShopPanel มาใส่ช่องนี้

    [Header("Shop Prices")]
    public int weaponUpgradeCost = 50;
    public int armorUpgradeCost = 50;
    public int buffCost = 20;

    void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.GetComponent<PlayerController>();

        if (shopPanel != null) shopPanel.SetActive(false);
    }

    void Update()
    {
        if (player == null || shopPanel == null) return;

        // เช็กระยะ ถ้าเดินเข้าใกล้ให้เปิดหน้าต่าง ถ้าออกห่างให้ปิดหน้าต่าง
        if (Vector3.Distance(transform.position, player.transform.position) <= interactRange)
        {
            if (!shopPanel.activeSelf) shopPanel.SetActive(true);
        }
        else
        {
            if (shopPanel.activeSelf) shopPanel.SetActive(false);
        }
    }

    // เปลี่ยนเป็น public เพื่อให้ปุ่มบน Canvas สามารถกดเรียกใช้งานได้
    public void TryUpgradeWeapon()
    {
        if (player.money >= weaponUpgradeCost)
        {
            player.money -= weaponUpgradeCost;
            int randomRoll = Random.Range(1, 101);
            float attackBonus = 0f;
            string tier = "";

            if (randomRoll <= 20) { tier = "แย่"; attackBonus = 2f; } // สุ่มการอัพเกรด[cite: 1]
            else if (randomRoll <= 60) { tier = "ธรรมดา"; attackBonus = 5f; }
            else if (randomRoll <= 90) { tier = "ดี"; attackBonus = 10f; }
            else { tier = "เลิศ"; attackBonus = 25f; }

            player.normalAttackDamage += attackBonus;
            Debug.Log($"อัปเกรดอาวุธสำเร็จ! [{tier}] พลังโจมตี +{attackBonus}"); // อัพเกรดอาวุธที่ NPC[cite: 1]
        }
    }

    public void TryUpgradeArmor()
    {
        if (player.money >= armorUpgradeCost)
        {
            player.money -= armorUpgradeCost;
            int randomRoll = Random.Range(1, 101);
            float armorBonus = 0f;
            string tier = "";

            if (randomRoll <= 20) { tier = "แย่"; armorBonus = 1f; }
            else if (randomRoll <= 60) { tier = "ธรรมดา"; armorBonus = 3f; }
            else if (randomRoll <= 90) { tier = "ดี"; armorBonus = 5f; }
            else { tier = "เลิศ"; armorBonus = 15f; }

            player.armor += armorBonus;
            Debug.Log($"อัปเกรดเกราะสำเร็จ! [{tier}] เกราะ +{armorBonus}"); // อัพเกรดชุดเกราะที่ NPC[cite: 1]
        }
    }

    public void BuyRecoveryBuff()
    {
        if (player.money >= buffCost)
        {
            player.money -= buffCost;
            player.currentHp = player.maxHp;
            player.currentMana = player.maxMana;
            player.currentStamina = player.maxStamina;
            Debug.Log("ซื้อบัฟฟื้นฟูสำเร็จ!"); // เขาขายบัฟฟื้นฟู[cite: 1]
        }
    }
}