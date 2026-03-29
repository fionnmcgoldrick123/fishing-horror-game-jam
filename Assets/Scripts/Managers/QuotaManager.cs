using UnityEngine;

public class QuotaManager : MonoBehaviour
{
    public static QuotaManager Instance { get; private set; }

    [SerializeField] private float startingQuota = 1000;
    [SerializeField] private float quotaIncreaseRate = 2f; 

    public float currentQuota { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            currentQuota = startingQuota;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void IncreaseQuota()
    {
        currentQuota *= quotaIncreaseRate;
    }

    public void Reset()
    {
        currentQuota = startingQuota;
    }

    public bool CanAffordQuota()
    {
        return FishInventory.Instance != null && FishInventory.Instance.TotalValue >= currentQuota;
    }

    public void PayQuota()
    {
        if (FishInventory.Instance != null)
            FishInventory.Instance.SpendMoney((int)currentQuota);
    }

    public void CheckIfQuotaMet()
    {
        if (FishInventory.Instance.TotalValue >= currentQuota)
        {
            Debug.Log("Quota met! Advancing to next day.");
        }
        else
        {
            GameOverManager.Instance.TriggerGameOver();
        }
    }
}