using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class IntroTextController : MonoBehaviour
{
    [TextArea(3, 10)]
    public string[] lines;

    public float typingSpeed = 0.04f;
    public float lineDelay = 1.5f;

    public string nextSceneName = "MainScene";

    private TextMeshProUGUI text;
    private bool isTyping = false;

    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        text.text = "";
        StartCoroutine(PlayIntro());
    }

    IEnumerator PlayIntro()
    {
        foreach (string line in lines)
        {
            yield return StartCoroutine(TypeLine(line));
            yield return new WaitForSeconds(lineDelay);
        }

        // Wait for player input
        text.text += "\n\nPress any key to continue";
        yield return new WaitUntil(() => (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame));

        SceneManager.LoadScene(nextSceneName);
    }

    IEnumerator TypeLine(string line)
    {
        isTyping = true;
        text.text = "";

        foreach (char c in line)
        {
            text.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }
}

