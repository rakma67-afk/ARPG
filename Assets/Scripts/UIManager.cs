using UnityEngine;
using UnityEngine.UI;
using TMPro; // ใช้บรรทัดนี้ถ้าคุณใช้ TextMeshPro (ถ้าใช้ Text ธรรมดาให้เปลี่ยนเป็น using UnityEngine.UI;)
using UnityEngine.SceneManagement; // จำเป็นสำหรับการรีสตาร์ทฉาก
using System.Collections; // จำเป็นสำหรับการใช้ Coroutine

public class UIManager : MonoBehaviour
{
    public PlayerController player; // ดึงข้อมูลตัวละครมาใช้
    [Header("Teleport Player")]
    public Vector3 merchantPosition = new Vector3(-100, 1, -100); // กำหนดตำแหน่งที่ต้องการให้ผู้เล่นไป

    [Header("Status Bars")]
    public Slider hpSlider;
    public TextMeshProUGUI hpText; // ช่องใส่ UI HP
    public Slider manaSlider;
    public Slider staminaSlider;
    public TextMeshProUGUI scoreText;

    [Header("Boss Stats")]
    public TextMeshProUGUI bossHealthText;
    public Slider bossHealthSlider;

    public GameObject bossPanel; // ช่องสำหรับใส่ Prefab ของบอส

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
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        if (player == null) return;

        // อัปเดตการแสดงผลของหลอด HP, Mana, Stamina ตลอดเวลา[cite: 3]
        hpSlider.value = player.currentHp;
        hpText.text = "HP : " + (int)player.currentHp + " / " + player.maxHp; // แสดงค่า HP ปัจจุบันและ Max HP
        manaSlider.value = player.currentMana;
        staminaSlider.value = player.currentStamina;

        scoreText.text = "Score : " + (int)player.score; // แสดงคะแนนสะสมของผู้เล่น

        if (levelText != null) levelText.text = "Level : " + player.level;
        if (moneyText != null) moneyText.text = "Money : " + player.money + "$";

        // ให้หลอดค่อยๆ ลดลงตามเวลา Cooldown ที่เหลืออยู่
        skill1Slider.value = player.skill1Timer;
        skill2Slider.value = player.skill2Timer;
        skill3Slider.value = player.skill3Timer;

        UpdatePlayerStats();
    }
    public void UpdatePlayerStats()
    {
        hpSlider.maxValue = player.maxHp;
        manaSlider.maxValue = player.maxMana;
        staminaSlider.maxValue = player.maxStamina;
    }
    public void TriggerGameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            StartCoroutine(FadeInPanel(gameOverPanel));
            StartCoroutine(Delay());
        }

        FindAnyObjectByType<SoundManager>().gameOver(); // เล่นเสียงเกมจบ
    }

    public void ___()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            StartCoroutine(FadeInPanel(gameOverPanel));
            StartCoroutine(_____());
        }
    }

    IEnumerator _____()
    {
        yield return new WaitForSecondsRealtime(1.5f);

        PlayGame();
    }

    private IEnumerator Delay()
    {
        // หน่วงเวลา 3 วิ โดยไม่สนใจ Time.timeScale
        yield return new WaitForSecondsRealtime(1.5f);

        Time.timeScale = 0f;
    }
    private IEnumerator FadeInPanel(GameObject panel)
    {
        CanvasGroup canvasGroup = panel.GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = panel.AddComponent<CanvasGroup>();

        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;

        float duration = 1.5f; // เวลาในการเฟด
        float t = 0f;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime; // ใช้ unscaled เพราะ Time.timeScale = 0
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, t / duration);
            yield return null;
        }

        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
    }
    public void TriggerVictory()
    {
        player.teleport(merchantPosition);
        if (bossPanel != null) bossPanel.SetActive(false);
        /*
        if (victoryPanel != null) victoryPanel.SetActive(true);
        Time.timeScale = 0f; // หยุดเวลาในเกมทั้งหมด
        */
    }

    public void UpdateBossHealth(float currentHealth, float maxHealth, bool isAggro)
    {
        if (isAggro)
        {
            if (bossPanel != null) bossPanel.SetActive(true); // แสดงแถบเลือดบอสเมื่อมีบอสอยู่
        }
        else
        {
            if (bossPanel != null) bossPanel.SetActive(false); // ซ่อนแถบเลือดบอสเมื่อไม่มีบอสอยู่
        }
        if (bossHealthSlider != null)
        {
            bossHealthSlider.maxValue = maxHealth;
            bossHealthSlider.value = currentHealth;
        }
        if (bossHealthText != null)
        {
            bossHealthText.text = "Boss HP : " + (int)currentHealth + " / " + maxHealth;
        }
        Debug.Log($"อัปเดตแถบเลือดบอส: {currentHealth} / {maxHealth}");
    }
    public void RestartGame()
    {
        Time.timeScale = 1f; // คืนค่าเวลาให้เดินปกติก่อนโหลดฉากใหม่
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // โหลดฉากปัจจุบันซ้ำ
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f; // คืนค่าเวลาให้เดินปกติก่อนโหลดฉากใหม่
        SceneManager.LoadScene(2);
    }

    public void PlayGame()
    {
        Time.timeScale = 1f; // คืนค่าเวลาให้เดินปกติก่อนโหลดฉากใหม่
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("เกมถูกปิดแล้ว"); // แสดงข้อความใน Console เพื่อยืนยันว่าฟังก์ชันทำงาน
    }
}