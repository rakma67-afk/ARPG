using UnityEngine;
using TMPro; // ใช้บรรทัดนี้ถ้าคุณใช้ TextMeshPro (ถ้าใช้ Text ธรรมดาให้เปลี่ยนเป็น using UnityEngine.UI;)

public class NPCMerchant : MonoBehaviour
{
    public float interactRange = 3f;
    private PlayerController player;
    UIManager uiManager;

    [Header("UI Shop")]
    public GameObject shopPanel; // นำ NPCShopPanel มาใส่ช่องนี้
    public TextMeshProUGUI DisplayResult; // ช่องสำหรับแสดงผลลัพธ์การซื้อขาย

    [Header("Shop Prices")]
    public int weaponUpgradeCost = 200;
    public int armorUpgradeCost = 200;
    public int buffCost = 50;

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

            if (randomRoll <= 20) { tier = "Bad"; attackBonus = 2f; } // สุ่มการอัพเกรด[cite: 1]
            else if (randomRoll <= 60) { tier = "Good"; attackBonus = 5f; }
            else if (randomRoll <= 90) { tier = "Great"; attackBonus = 10f; }
            else { tier = "Legendary"; attackBonus = 25f; }

            player.normalAttackDamage += attackBonus;
            player.skill2Damage += attackBonus;
            player.skill3Damage += attackBonus;
            DisplayResult.text = $"Tier [{tier}] Attack +{attackBonus}"; // อัพเกรดอาวุธที่ NPC[cite: 1]
            Debug.Log($"อัปเกรดอาวุธสำเร็จ! [{tier}] พลังโจมตี +{attackBonus}"); // อัพเกรดอาวุธที่ NPC[cite: 1]
        }
        else
        {
            DisplayResult.text = "Not enough money!"; // แจ้งผู้เล่นว่ามีเงินไม่พอ
            Debug.Log("เงินไม่พอ!"); // แจ้งผู้เล่นว่ามีเงินไม่พอ
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

            if (randomRoll <= 20) { tier = "Bad"; armorBonus = 1f; }
            else if (randomRoll <= 60) { tier = "Good"; armorBonus = 3f; }
            else if (randomRoll <= 90) { tier = "Great"; armorBonus = 5f; }
            else { tier = "Legendary"; armorBonus = 15f; }

            player.armor += armorBonus;
            DisplayResult.text = $"Tier [{tier}] Armor +{armorBonus}"; // อัพเกรดชุดเกราะที่ NPC[cite: 1]
            Debug.Log($"อัปเกรดเกราะสำเร็จ! [{tier}] เกราะ +{armorBonus}"); // อัพเกรดชุดเกราะที่ NPC[cite: 1]
        }
        else
        {
            DisplayResult.text = "Not enough money!"; // แจ้งผู้เล่นว่ามีเงินไม่พอ
            Debug.Log("เงินไม่พอ!"); // แจ้งผู้เล่นว่ามีเงินไม่พอ
        }
    }

    public void BuyRecoveryBuff()
    {
        if (player.money >= buffCost)
        {
            player.money -= buffCost;
            player.maxHp += 50f;
            player.maxMana += 50f;
            player.maxStamina += 50f;
            DisplayResult.text = $"HP +50, MP +50, SP +50"; // เขาขายบัฟฟื้นฟู[cite: 1]
            Debug.Log("ซื้อบัฟฟื้นฟูสำเร็จ!"); // เขาขายบัฟฟื้นฟู[cite: 1]
        }
        else
        {
            DisplayResult.text = "Not enough money!"; // แจ้งผู้เล่นว่ามีเงินไม่พอ
            Debug.Log("เงินไม่พอ!"); // แจ้งผู้เล่นว่ามีเงินไม่พอ
        }
    }

    
}