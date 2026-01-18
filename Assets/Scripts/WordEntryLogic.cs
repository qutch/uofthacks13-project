using UnityEngine;
using TMPro;

public class WordEntryLogic : MonoBehaviour
{
    // FIX: Use [SerializeField] so you can drag the component in the Inspector.
    // This bypasses the need for Awake() entirely.
    [SerializeField] private TMP_Text self; 
    
    [SerializeField] private string word = "";
    [SerializeField] private string translatedWord = "";

    // REMOVED Awake(). It is dangerous for objects spawned as disabled.

    public void SetWord(string newWord)
    {
        word = newWord;
    }

    public void SetTranslatedWord(string newTranslatedWord)
    {
        translatedWord = newTranslatedWord;
    }
    
    public void SetFont(TMP_FontAsset font)
    {
        // Fallback: If you forgot to drag it in, try to find it now.
        // GetComponent works here even if the object is disabled.
        if (self == null) self = GetComponent<TextMeshProUGUI>();

        if (self != null && font != null)
        {
            self.font = font;
        }
    }

    public void UpdateDisplay()
    { 
        // Fallback: Ensure we have the reference before trying to use it
        if (self == null) self = GetComponent<TextMeshProUGUI>();

        if (self != null)
        {
            self.text = $"{translatedWord} : {word}";
        }
    }
}