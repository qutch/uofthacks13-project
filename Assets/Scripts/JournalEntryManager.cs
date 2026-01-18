using TMPro;
using UnityEngine;

public class JournalEntryManager : MonoBehaviour
{
    public Transform ParentObject;
    public GameObject JournalPrefab;
    
    // Make sure you drag your Korean-supporting Font Asset here in the Inspector
    [SerializeField] TMP_FontAsset textFont; 
    
    public void CreateEntry(string word, string translatedWord)
    {
        GameObject newEntry = Instantiate(JournalPrefab, ParentObject);
        WordEntryLogic entryLogic = newEntry.GetComponent<WordEntryLogic>();
        
        // 1. Force the font to be the one you set up in the Inspector
        if (textFont != null)
        {
            entryLogic.SetFont(textFont);
        }

        // 2. Set the data
        entryLogic.SetWord(word);
        entryLogic.SetTranslatedWord(translatedWord);
        
        // 3. Update the display (Now safe because Awake has run)
        entryLogic.UpdateDisplay();
    }
}