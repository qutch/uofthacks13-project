using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuToGame : MonoBehaviour
{
    public void LoadScene(string intendedScene)
    {
        SceneManager.LoadScene(intendedScene);
    }
}
