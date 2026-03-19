using UnityEngine;
using UnityEngine.InputSystem;

public class HookManager : MonoBehaviour
{
    public static HookManager Instance { get; private set; }
    [SerializeField] private GameObject fishingMinigameUI;
    [SerializeField] private PlayerFishingState fishingStateManager;

    // event for fishing
    public event System.Action OnCatchingStarted;

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
        if(Input.GetKeyDown(KeyCode.Q) && fishingStateManager.isWaiting())
        {
            TriggerFishBite();
        }
    }

    public void TriggerFishBite()
    {
        Debug.Log("Fish bite triggered!");
        fishingMinigameUI.SetActive(true);
        OnCatchingStarted?.Invoke();
    }
}
