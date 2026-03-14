using UnityEngine;
using UnityEngine.SceneManagement;

public class Book_Navigation_Script : MonoBehaviour
{
    public string sceneToLoad;

    void OnMouseDown()
    {
        Debug.Log("Loading scene: " + sceneToLoad);
        SceneManager.LoadScene(sceneToLoad);
    }
}