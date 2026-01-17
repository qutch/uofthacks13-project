using UnityEngine;

public class CollectibleWord : MonoBehaviour
{
    public string word;
    
    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("collion detected");

        PlayerController controller = other.GetComponent<PlayerController>();
        
        if (controller != null && controller.numWords < controller.maxWords)
        {
            controller.addVocabWord(word);
            Destroy(gameObject);
        }
    }
    
}
