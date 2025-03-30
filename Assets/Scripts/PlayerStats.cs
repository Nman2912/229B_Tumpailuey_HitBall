using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHP = 3;         // จำนวน HP สูงสุด (ควรตรงกับจำนวนหัวใจ)
    public int currentHP;

    [Header("UI Settings")]
    public GameObject[] heartIcons;   // Array ของ GameObject ที่แสดงหัวใจ (เช่น Heart1, Heart2, Heart3)

    void Start()
    {
        // ตั้งค่า HP เริ่มต้นและอัปเดต UI
        currentHP = maxHP;
        UpdateHearts();
    }

    // Method สำหรับลด HP โดยไม่ต้องระบุค่า (ค่า default = 1)
    public void TakeDamage()
    {
        TakeDamage(1f);
    }

    // ลด HP ด้วยค่าความเสียหายที่รับเข้ามา
    public void TakeDamage(float dmg)
    {
        currentHP -= Mathf.RoundToInt(dmg);
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);
        Debug.Log("💥 Player took damage! Current HP: " + currentHP);
        UpdateHearts();

        if (currentHP <= 0)
        {
            Debug.Log("☠️ Player is dead!");
            // สามารถเพิ่มระบบ Game Over, Reset, หรือ Respawn ได้ที่นี่
        }
    }

    // ฟังก์ชัน UpdateHearts() จะทำให้หัวใจแต่ละดวงแสดงหรือซ่อนตาม HP ที่เหลือ
    void UpdateHearts()
    {
        // ตรวจสอบให้แน่ใจว่า Array มีความยาวเท่ากับ maxHP (เช่น 3 ดวง)
        for (int i = 0; i < heartIcons.Length; i++)
        {
            // ถ้า i น้อยกว่า currentHP ให้แสดงหัวใจนั้น, ไม่เช่นนั้นซ่อน
            bool isActive = (i < currentHP);
            heartIcons[i].SetActive(isActive);
            Debug.Log("Heart " + i + " active: " + isActive);
        }
    }

    // ฟังก์ชัน ResetHealth() ใช้ในการคืนค่า HP และอัปเดต UI หัวใจ
    public void ResetHealth()
    {
        currentHP = maxHP;
        UpdateHearts();
    }
}
