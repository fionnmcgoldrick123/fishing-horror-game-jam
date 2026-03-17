using UnityEngine;

public class Camera : MonoBehaviour
{

    [SerializeField] private Transform playerTransform;
    [SerializeField] private Vector3 offset;
    [SerializeField] private float smoothTime = 0.3f;
    private Vector3 velocity = Vector3.zero;

    // fixing z position to -10
    

    void LateUpdate()
    {
        if (playerTransform != null)
        {
            Vector3 targetPosition = playerTransform.position + offset;
            targetPosition.z = -10f; 
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
        }
    }
}
