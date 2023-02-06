using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space)
        || Input.GetKeyDown(KeyCode.Return)
        || Input.GetKeyDown(KeyCode.Return)
        || Input.GetMouseButtonDown(0))
            LoadGame();
    }

    void LoadGame()
    {
        SceneManager.LoadScene("ForestPlayScene");
    }
}
