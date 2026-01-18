using System;
using UnityEngine;
using System.Collections;

public class NPCWalker : MonoBehaviour
{
    [Header("UI Configuration")]
    public GameObject uiFloatingPrompt;
    
    [Header("Movement Settings")]
    public float moveSpeed = 3f;
    public float waitTime = 1f;
    public Vector2[] walkPattern; 

    [Header("Visuals")]
    public SpriteRenderer spriteRenderer;
    public Sprite spriteUp;
    public Sprite spriteDown;
    public Sprite spriteLeft;
    public Sprite spriteRight;

    private int currentStep = 0;
    private bool isMoving = false;
    private bool isBusy = false;
    
    [SerializeField] AIManager aiManager;
    [SerializeField] ElevenLabsManager elevenLabsManager;

    private String translatedText = "";

    void Start()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        // Hide UI at start
        if (uiFloatingPrompt != null)
            uiFloatingPrompt.SetActive(false);
    }

    void Update()
    {
        if (!isMoving && walkPattern.Length > 0 && !isBusy)
        {
            StartCoroutine(MoveRoutine());
        }
    }
    
    // 1. Show UI on approach
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (uiFloatingPrompt != null) uiFloatingPrompt.SetActive(true);
            
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.SetActiveNPC(this); // Register this NPC with the player
            }
        }
    }
    
    // 2. Hide UI on exit
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (uiFloatingPrompt != null) uiFloatingPrompt.SetActive(false);
            
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.ClearActiveNPC();
            }

            // Optional: If the player runs away, stop being busy so we can walk again
            isBusy = false; 
        }
    }

    // 3. Called by PlayerController when 'E' is pressed
    public void Interact()
    {
        if (!isBusy)
        {
            isBusy = true;
            aiManager.TranslateText("Hello! Do you like coffee?", (translatedResult) =>
            {
                // 1. Store the text
                translatedText = translatedResult;
                Debug.Log($"Translation received: {translatedText}");

                // 2. NOW call ElevenLabs with the correct text
                if (elevenLabsManager != null)
                {
                    elevenLabsManager.GenerateVoiceOver(translatedText, "JBFqnCBsd6RMkjVDRZzb");
                }
            });
        }
        else
        {
            // End Conversation (Toggle off)
            aiManager.TranslateText("Goodbye!", (translatedResult) =>
            {
                // 1. Store the text
                translatedText = translatedResult;
                Debug.Log($"Translation received: {translatedText}");

                // 2. NOW call ElevenLabs with the correct text
                if (elevenLabsManager != null)
                {
                    elevenLabsManager.GenerateVoiceOver(translatedText, "JBFqnCBsd6RMkjVDRZzb");
                }
            });
            isBusy = false; // Allows NPC to walk again
        }
    }

    IEnumerator MoveRoutine()
    {
        isMoving = true;

        Vector2 moveDirection = walkPattern[currentStep];
        UpdateSpriteDirection(moveDirection);

        Vector3 startPos = transform.position;
        Vector3 targetPos = startPos + (Vector3)moveDirection;

        float percent = 0;
        while (percent < 1f)
        {
            percent += Time.deltaTime * moveSpeed;
            transform.position = Vector3.Lerp(startPos, targetPos, percent);
            yield return null; 
        }

        transform.position = targetPos;
        currentStep = (currentStep + 1) % walkPattern.Length;
        yield return new WaitForSeconds(waitTime);
        
        isMoving = false;
    }

    void UpdateSpriteDirection(Vector2 dir)
    {
        if (spriteRenderer == null) return;

        // Reset flip by default so it doesn't carry over from the previous move
        spriteRenderer.flipX = false;

        if (dir.y > 0) 
        {
            spriteRenderer.sprite = spriteUp;
        }
        else if (dir.y < 0) 
        {
            spriteRenderer.sprite = spriteDown;
        }
        else if (dir.x > 0) 
        {
            spriteRenderer.sprite = spriteRight;
            // Ensure it's not flipped when going right
            spriteRenderer.flipX = false;
        }
        else if (dir.x < 0) 
        {
            // If you have a specific Left sprite, use it
            if (spriteLeft != null) 
            {
                spriteRenderer.sprite = spriteLeft;
            }
            // If you want to use the Right sprite but flipped:
            else if (spriteRight != null)
            {
                spriteRenderer.sprite = spriteRight;
                spriteRenderer.flipX = true;
            }
        }
    }
}