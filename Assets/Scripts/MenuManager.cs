using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("Final01"); // ชื่อ Scene ที่ใช้เล่นเกม
    }
}
