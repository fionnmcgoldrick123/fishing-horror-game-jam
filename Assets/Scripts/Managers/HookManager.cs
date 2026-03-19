using UnityEngine;
using UnityEngine.InputSystem;

public class HookManager : MonoBehaviour
{
    public static HookManager Instance { get; private set; }
    [SerializeField] private GameObject fishingMinigameUI;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    void Update()
    {
        //if q is pressed
        if (Keyboard.current.qKey.wasPressedThisFrame)
        {
            // enable game object for fishing minigame
            
        }
    }
}
