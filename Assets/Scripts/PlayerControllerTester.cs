using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControllerTester : MonoBehaviour
{
    public InputAction MoveAction;
    Rigidbody2D rigidbodyElement;
    Vector2 movement;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        MoveAction.Enable();
        rigidbodyElement = GetComponent<Rigidbody2D>();
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
}
