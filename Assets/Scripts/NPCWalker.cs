using UnityEngine;
using System.Collections;

public class NPCWalker : MonoBehaviour
{
    public float moveSpeed = 3f;
    public Vector2[] walkPattern; // Define points like (0,1), (1,0), etc.
    private int currentStep = 0;
    private bool isMoving = false;

    void Update()
    {
        if (!isMoving && walkPattern.Length > 0)
        {
            StartCoroutine(MoveToNextPoint());
        }
    }

    IEnumerator MoveToNextPoint()
    {
        isMoving = true;

        // Calculate the target position based on the next pattern step
        Vector3 startPos = transform.position;
        Vector3 targetPos = startPos + (Vector3)walkPattern[currentStep];

        float percent = 0;
        while (percent < 1f)
        {
            percent += Time.deltaTime * moveSpeed;
            transform.position = Vector3.Lerp(startPos, targetPos, percent);
            yield return null;
        }

        // Cycle to the next step in your pattern
        currentStep = (currentStep + 1) % walkPattern.Length;
        
        yield return new WaitForSeconds(1f); // Wait 1 second before moving again
        isMoving = false;
    }
}