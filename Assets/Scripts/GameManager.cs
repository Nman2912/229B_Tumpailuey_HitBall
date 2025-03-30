using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public bool isGameEnded = false;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void EndGame(bool playerWin, GameObject winUI, GameObject loseUI, float delay = 3f)
    {
        if (isGameEnded) return;
        isGameEnded = true;

        if (playerWin)
        {
            if (winUI != null) winUI.SetActive(true);
        }
        else
        {
            if (loseUI != null) loseUI.SetActive(true);
        }

        StartCoroutine(LoadSceneAfterDelay("EndCredit", delay));
    }

    IEnumerator LoadSceneAfterDelay(string sceneName, float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneName);
    }
}
