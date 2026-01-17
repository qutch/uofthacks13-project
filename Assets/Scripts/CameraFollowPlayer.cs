using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour
{
    [SerializeField] private Transform targetPlayer;
    [SerializeField] private float smoothSpeed = 5f;
    [SerializeField] private Vector2 offset;
    [SerializeField] private Vector2 minMapBounds;
    [SerializeField] private Vector2 maxMapBounds;
    private float initialZ;

    private void Start()
    {
        initialZ = transform.position.z;
    }

    private void LateUpdate()
    {
        if (targetPlayer == null) return;

        Vector3 desiredPosition = new Vector3(
            Mathf.Clamp(targetPlayer.position.x + offset.x, minMapBounds.x, maxMapBounds.x),
            Mathf.Clamp(targetPlayer.position.y + offset.y, minMapBounds.y, maxMapBounds.y),
            initialZ
            );

        Vector3 smoothedPosition = Vector3.Lerp(
            transform.position,
            desiredPosition,
            smoothSpeed * Time.deltaTime
        );
        

        transform.position = smoothedPosition;
    }
}