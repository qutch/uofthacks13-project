using UnityEngine;
using TMPro;

public class InteractionUI : MonoBehaviour
{
    [Header("UI Components")]
    public GameObject uiPanel; // The background panel
    public TextMeshProUGUI promptText; // The text element

    public void ShowPrompt(string message)
    {
        promptText.text = message;
        uiPanel.SetActive(true);
    }

    public void HidePrompt()
    {
        uiPanel.SetActive(false);
    }
}