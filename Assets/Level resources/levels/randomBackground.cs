using UnityEngine;
using UnityEngine.UI;

public class RandomBackground : MonoBehaviour
{
    [Header("Assign UI Image used as background")]
    public Image backgroundImage;

    [Header("List of possible background sprites")]
    public Sprite[] backgrounds;

    void Start()
    {
        if (backgrounds.Length == 0)
        {
            Debug.LogWarning("No backgrounds assigned to RandomBackground.");
            return;
        }

        if (backgroundImage == null)
        {
            Debug.LogWarning("No Image component assigned to RandomBackground.");
            return;
        }

        // Pick a random sprite
        Sprite randomSprite = backgrounds[Random.Range(0, backgrounds.Length)];

        // Set it as background
        backgroundImage.sprite = randomSprite;
    }
}
