using UnityEngine;

/// <summary>
/// Attach this to the Dr. Sturgeon NPC GameObject in the World scene.
/// Set Jumpscare Position in the inspector to wherever you want him to teleport.
/// GameOverManager finds this component automatically when World loads.
/// </summary>
public class DrSturgeonGameOver : MonoBehaviour
{
    [Tooltip("The position Dr. Sturgeon teleports to when the jumpscare triggers.")]
    [SerializeField] private Vector3 jumpscarePosition;

    public void MoveToJumpscarePosition()
    {
        transform.position = jumpscarePosition;
    }
}
