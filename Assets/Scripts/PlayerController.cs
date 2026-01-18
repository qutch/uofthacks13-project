using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public InputAction MoveAction;
    Rigidbody2D rigidbodyElement;
    Vector2 movement;

    [Header("Visuals")]
    public SpriteRenderer spriteRenderer;
    public Sprite spriteUp;
    public Sprite spriteDown;
    public Sprite spriteLeft;
    public Sprite spriteRight;

    public int maxWords = 10;
    private string[] learnedVocabulary;
    private int numWordsLearned = 0;
    public string[] vocabList { get { return learnedVocabulary; } }
    public int numWords { get { return numWordsLearned; } }
    
    private Camera mainCam;
    private CollectibleWord activeWord;
    private NPCWalker activeNPC;

    void Start()
    {
        mainCam = Camera.main;
        
        MoveAction.Enable();
        rigidbodyElement = GetComponent<Rigidbody2D>();
        learnedVocabulary = new string[maxWords];

        // Automatic fallback if you forget to assign the SpriteRenderer
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    // These methods allow the CollectibleWord script to register itself
    public void SetActiveWord(CollectibleWord wordScript)
    {
        activeWord = wordScript;
    }
    
    public void SetActiveNPC(NPCWalker npc)
    {
        activeNPC = npc;
    }
    
    public void ClearActiveNPC()
    {
        activeNPC = null;
    }

    public void ClearActiveWord()
    {
        activeWord = null;
    }

    void Update()
    {
        movement = MoveAction.ReadValue<Vector2>();
        UpdateSprite(movement);
    
        // Check for 'E' press
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            // Priority: Talk to NPC first, if no NPC, try picking up word
            if (activeNPC != null)
            {
                activeNPC.Interact();
            }
            else if (activeWord != null)
            {
                activeWord.Interact(this);
            }
        }
    }
    
    void LateUpdate()
    {
        // Rotate the UI to look at the camera
        transform.LookAt(transform.position + mainCam.transform.rotation * Vector3.forward,
            mainCam.transform.rotation * Vector3.up);
    }

    void UpdateSprite(Vector2 moveInput)
    {
        if (moveInput.magnitude > 0.1f)
        {
            // Vertical Movement
            if (Mathf.Abs(moveInput.y) > Mathf.Abs(moveInput.x))
            {
                spriteRenderer.flipX = false; // Reset flip when moving up/down
                if (moveInput.y > 0) spriteRenderer.sprite = spriteUp;
                else spriteRenderer.sprite = spriteDown;
            }
            // Horizontal Movement
            else 
            {
                if (moveInput.x > 0) 
                {
                    spriteRenderer.sprite = spriteRight;
                    spriteRenderer.flipX = false; // Face Right
                }
                else 
                {
                    spriteRenderer.sprite = spriteRight; // Use Right sprite...
                    spriteRenderer.flipX = true;  // ...but flip it!
                }
            }
        }
    }

    void FixedUpdate()
    {
        Vector2 newPosition = (Vector2)rigidbodyElement.position + movement * 3.0f * Time.deltaTime;
        rigidbodyElement.MovePosition(newPosition);
    }

    public void addVocabWord(string word)
    {
        if (numWordsLearned < maxWords)
        {
            // FIXED: Set the word first, then increment the counter
            learnedVocabulary[numWordsLearned] = word;
            Debug.Log("Learned: " + learnedVocabulary[numWordsLearned]);
            numWordsLearned += 1;
        }
    }
}