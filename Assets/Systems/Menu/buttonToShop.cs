using UnityEngine;
using UnityEngine.SceneManagement;

public class buttonToShop : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    protected void OnTriggerStay2D(Collider2D collision)            // оновлюється при руху обєкта
    {
        if (collision.gameObject.tag == "Player")
            SceneManager.LoadScene("shop");
    }
}
