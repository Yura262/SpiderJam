using UnityEngine;
using UnityEngine.SceneManagement;

public class buttonMenu : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public string reactToTag = "DeadPlayer";
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    protected void OnTriggerStay2D(Collider2D collision)            // оновлюється при руху обєкта
    {
        if (collision.gameObject.tag == reactToTag)
            SceneManager.LoadScene("menu");
    }
}
