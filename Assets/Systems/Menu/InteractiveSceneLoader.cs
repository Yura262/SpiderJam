using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InteractiveSceneLoader : MonoBehaviour
{

    [Tooltip("Name of the scene to load")]
    public string sceneToLoad;

    [Tooltip("Time (in seconds) player must stay to load scene")]
    public float holdDuration = 2f;

    [Tooltip("UI Image with Fill method set to Radial 360")]
    public Image loadingCircle;


    private float holdTimer = 0f;
    private bool playerInside = false;

    private void Update()
    {
        if (playerInside)
        {
            holdTimer += Time.deltaTime;
            float fill = Mathf.Clamp01(holdTimer / holdDuration);

            if (loadingCircle)
            {
                loadingCircle.fillAmount = fill;
            }

            if (fill >= 1f)
            {
                LoadScene();
            }
        }
        else if (holdTimer > 0f)
        {
            // Smoothly reset the bar when leaving
            holdTimer = Mathf.MoveTowards(holdTimer, 0f, Time.deltaTime * 2);
            if (loadingCircle)
                loadingCircle.fillAmount = holdTimer / holdDuration;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
            if (loadingCircle)
            {
                //loadingCircle.fillAmount = 0;
                loadingCircle.gameObject.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
            if (loadingCircle)
                loadingCircle.gameObject.SetActive(false);
        }
    }

    private void LoadScene()
    {
        // Optional: prevent multiple triggers
        playerInside = false;

        if (loadingCircle)
            loadingCircle.fillAmount = 1f;

        SceneManager.LoadScene(sceneToLoad);
    }
}
