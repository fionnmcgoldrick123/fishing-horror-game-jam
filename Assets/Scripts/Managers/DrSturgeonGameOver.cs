using UnityEngine;

public class DrSturgeonGameOver : MonoBehaviour
{
    [Tooltip("The position Dr. Sturgeon teleports to when the jumpscare triggers.")]
    [SerializeField] private Vector3 jumpscarePosition;

    public void MoveToJumpscarePosition()
    {
        Debug.Log($"[DrSturgeonGameOver] Moving to position: {jumpscarePosition}");
        transform.position = jumpscarePosition;
        Debug.Log($"[DrSturgeonGameOver] After move, actual position is: {transform.position}");
    }
}
