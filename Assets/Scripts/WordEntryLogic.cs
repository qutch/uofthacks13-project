using UnityEngine;
using TMPro;

public class WordEntryLogic : MonoBehaviour
{
    private TMP_Text self;
    [SerializeField] private string word = "";
    [SerializeField] private string translatedWord = "";

    // Awake is called immediately when the object is initialized
    void Awake()
    {
        self = GetComponent<TextMeshProUGUI>();
    }

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
        if (self != null && font != null)
        {
            self.font = font;
        }
    }

    public void UpdateDisplay()
    { 
        if (self != null)
        {
            self.text = $"{translatedWord} : {word}";
        }
    }
}