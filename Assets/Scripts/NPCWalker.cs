using UnityEngine;
using System.Collections;

public class NPCWalker : MonoBehaviour
{
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

    void Start()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
    }

    void Update()
    {
        if (!isMoving && walkPattern.Length > 0)
        {
            StartCoroutine(MoveRoutine());
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