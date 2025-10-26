using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class LevelHighscoreDisplay : MonoBehaviour
{
    [SerializeField] private string levelName;
    [SerializeField] private TMP_Text highscoreText;

    private void Start()
    {
        if (levelName=="")
            levelName = SceneManager.GetActiveScene().name;
        float score = SurvivalTimer.GetHighscore(levelName);
        if (score > 0)
            highscoreText.text = $"Best Time: {score:F0}s";
        else
            highscoreText.text = "Best Time: --";
    }
}
