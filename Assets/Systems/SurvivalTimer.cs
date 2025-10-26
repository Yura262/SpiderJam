using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class SurvivalTimer : MonoBehaviour
{
    public static SurvivalTimer Instance;

    private float aliveTime;
    
    private bool isAlive = true;
    private string levelKey;
    public TMP_Text highscoreText;

    private void Start()
    {
        Instance = this;
        levelKey = "Highscore_" + UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        GameManager.Instance.playerHealth.OnDeath += OnPlayerDeath;
    }

    private void Update()
    {
        if (isAlive)
            aliveTime += Time.deltaTime;
            highscoreText.text = $"{aliveTime:F0}";

    }

    public void OnPlayerDeath(DamageType none)
    {
        if (!isAlive) return;
        isAlive = false;

        float highscore = PlayerPrefs.GetFloat(levelKey, 0);
        if (aliveTime > highscore)
        {
            PlayerPrefs.SetFloat(levelKey, aliveTime);
            PlayerPrefs.Save();
        }

        //Debug.Log($"Player survived {aliveTime:F2}s. Highscore: {PlayerPrefs.GetFloat(levelKey):F2}s");
    }

    public static float GetHighscore(string sceneName)
    {
        return PlayerPrefs.GetFloat("Highscore_" + sceneName, 0);
    }
}
