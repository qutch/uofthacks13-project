using TMPro;
using UnityEngine;

public class CollectibleWord : MonoBehaviour
{
    [Header("UI Configuration")]
    public GameObject uiFloatingPrompt; // Drag your Child Canvas here
    public TMP_Text uiText;

    [Header("Word Data")]
    public string word;
    public string translatedWord;
    
    [SerializeField] private AIManager aiManager;
    [SerializeField] private ElevenLabsManager elevenLabsManager;
    [SerializeField] JournalEntryManager journalEntryManager;

    private void Start()
    {
        // Ensure the prompt is hidden when the game starts
        if (uiFloatingPrompt != null)
        {
            uiFloatingPrompt.SetActive(false);

            aiManager.TranslateText(word, (translatedResult) =>
            {
                // 1. Store the text
                translatedWord = translatedResult;
                Debug.Log($"Translation received: {translatedWord}");
            });
        }
    }

    // 1. SHOW UI when Player enters
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check for Player tag to avoid enemies triggering the UI
        if (other.CompareTag("Player"))
        {
            if (uiFloatingPrompt != null) 
                uiFloatingPrompt.SetActive(true);

            // Tell the player script: "I am the word you can collect now"
            PlayerController controller = other.GetComponent<PlayerController>();
            if (controller != null)
            {
                // You must add this method to your PlayerController (see below)
                controller.SetActiveWord(this);
            }
        }
    }

    // 2. HIDE UI when Player leaves
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (uiFloatingPrompt != null) 
                uiFloatingPrompt.SetActive(false);

            // Tell the player script: "You walked away, clear the reference"
            PlayerController controller = other.GetComponent<PlayerController>();
            if (controller != null)
            {
                controller.ClearActiveWord();
            }
        }
    }

    // 3. EXECUTE LOGIC (Called by PlayerController when 'E' is pressed)
    public void Interact(PlayerController controller)
    {
        uiText.text = translatedWord + " : " + word;
        journalEntryManager.CreateEntry(word, translatedWord);
        Debug.Log($"Interacted with {word}!");

        if (controller != null && controller.numWords < controller.maxWords)
        {
            controller.addVocabWord(word);

            // 2. NOW call ElevenLabs with the correct text
            if (elevenLabsManager != null)
            {
                elevenLabsManager.GenerateVoiceOver(translatedWord, "JBFqnCBsd6RMkjVDRZzb");
            }
            
            // Destroy the item (and the UI attached to it)
            Destroy(gameObject);
        }
    }
}