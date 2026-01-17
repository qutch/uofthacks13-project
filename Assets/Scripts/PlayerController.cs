using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerController : MonoBehaviour
{
    public InputAction MoveAction;
    Rigidbody2D rigidbodyElement;
    Vector2 movement;

    public int maxWords = 10;
    private string[] learnedVocabulary;
    private int numWordsLearned = 0;
    public string[] vocabList { get { return learnedVocabulary; }}
    public int numWords { get { return numWordsLearned; }}
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        MoveAction.Enable();
        rigidbodyElement = GetComponent<Rigidbody2D>();
        learnedVocabulary = new string[maxWords];
    }

    // Update is called once per frame
    void Update()
    {
        movement = MoveAction.ReadValue<Vector2>();
    }
    
    void FixedUpdate()
    {
        Vector2 newPosition = (Vector2)rigidbodyElement.position + movement*3.0f*Time.deltaTime;
        rigidbodyElement.MovePosition(newPosition);
    }

    public void addVocabWord(string word)
    {
        if (numWordsLearned < maxWords)
        {
            numWordsLearned += 1;
            learnedVocabulary[numWordsLearned] = word;
            Debug.Log(learnedVocabulary[numWordsLearned]);
        }
    }
}
