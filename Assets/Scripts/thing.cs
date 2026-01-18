using UnityEngine;
using UnityEngine.InputSystem;

public class thing : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public ElevenLabsSTTManager manager;
    public InputAction interactAction;
    public InputAction interactAction2;
    void Start()
    {
        interactAction.Enable();
        interactAction2.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        if (interactAction.WasPressedThisFrame())
        {
            Debug.Log("StartRecording triggered");
            manager.StartRecording() ;
        }
        if (interactAction2.WasPressedThisFrame())
        {
            Debug.Log("StopRecordingAndTranscribe triggered");
            manager.StopRecordingAndTranscribe(
                (text) => Debug.Log("Transcribed: " + text),
                (err) => Debug.LogError("STT Error: " + err)
            );
        }
    }
}
