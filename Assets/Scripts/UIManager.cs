using UnityEngine;
using UnityEngine.UI;
using TMPro; // ใช้บรรทัดนี้ถ้าคุณใช้ TextMeshPro (ถ้าใช้ Text ธรรมดาให้เปลี่ยนเป็น using UnityEngine.UI;)
using UnityEngine.SceneManagement; // จำเป็นสำหรับการรีสตาร์ทฉาก

public class UIManager : MonoBehaviour
{
    public PlayerController player; // ดึงข้อมูลตัวละครมาใช้

    [Header("Status Bars")]
    public Slider hpSlider;
    public Slider manaSlider;
    public Slider staminaSlider;

    [Header("Skill Cooldown Sliders")]
    public Slider skill1Slider;
    public Slider skill2Slider;
    public Slider skill3Slider;

    [Header("Progression Texts")]
    public TextMeshProUGUI levelText; // ช่องใส่ UI เลเวล
    public TextMeshProUGUI moneyText; // ช่องใส่ UI เงิน

    [Header("End Game UI")]
    public GameObject gameOverPanel;
    public GameObject victoryPanel;

    void Start()
    {
        // ตั้งค่าแถบสูงสุด (Max Value) ให้ตรงกับ GDD
        if (player != null)
        {
            hpSlider.maxValue = player.maxHp;
            manaSlider.maxValue = player.maxMana;
            staminaSlider.maxValue = player.maxStamina;

            skill1Slider.maxValue = player.skill1Cooldown;
            skill2Slider.maxValue = player.skill2Cooldown;
            skill3Slider.maxValue = player.skill3Cooldown;
        }

        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (victoryPanel != null) victoryPanel.SetActive(false);
    }

    void Update()
    {
        if (player == null) return;

        // อัปเดตการแสดงผลของหลอด HP, Mana, Stamina ตลอดเวลา[cite: 3]
        hpSlider.value = player.currentHp;
        manaSlider.value = player.currentMana;
        staminaSlider.value = player.currentStamina;

        if (levelText != null) levelText.text = "Level: " + player.level;
        if (moneyText != null) moneyText.text = "Money: " + player.money;

        // ให้หลอดค่อยๆ ลดลงตามเวลา Cooldown ที่เหลืออยู่
        skill1Slider.value = player.skill1Timer;
        skill2Slider.value = player.skill2Timer;
        skill3Slider.value = player.skill3Timer;
    }
    public void TriggerGameOver()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        Time.timeScale = 0f; // หยุดเวลาในเกมทั้งหมด
    }
    public void TriggerVictory()
    {
        if (victoryPanel != null) victoryPanel.SetActive(true);
        Time.timeScale = 0f; // หยุดเวลาในเกมทั้งหมด
    }
    public void RestartGame()
    {
        Time.timeScale = 1f; // คืนค่าเวลาให้เดินปกติก่อนโหลดฉากใหม่
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // โหลดฉากปัจจุบันซ้ำ
    }
}