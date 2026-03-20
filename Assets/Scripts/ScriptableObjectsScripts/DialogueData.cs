using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogue", menuName = "Dialogue/Dialogue Data")]
public class DialogueData : ScriptableObject
{
    // string for name
    public string characterName;

    public Sprite characterPortrait;

    [TextArea(2, 6)]
    public string[] lines;

    [Tooltip("Characters revealed per second")]
    public float textSpeed = 30f;

    [Tooltip("Audio clip played while text is typing")]
    public AudioClip typingSound;
}
