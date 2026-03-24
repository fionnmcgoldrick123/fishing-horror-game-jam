using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FishInventory : MonoBehaviour
{
    public static FishInventory Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI moneyText;

    [Header("Count-Up Animation")]
    [SerializeField] private float countUpDuration = 0.6f;

    private readonly List<FishScriptableObject> caughtFish = new List<FishScriptableObject>();
    private int totalValue;
    private Coroutine countUpCoroutine;

    public IReadOnlyList<FishScriptableObject> CaughtFish => caughtFish;
    public int TotalValue => totalValue;



    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public int EyeballCount
    {
        get
        {
            int count = 0;
            foreach (var f in caughtFish)
                if (f.isUpgradeCurrency) count++;
            return count;
        }
    }

    private void Start()
    {
        UpdateScoreText();
    }

    /// <summary>Called by DayUI each scene load to re-bind the scene's money text.</summary>
    public void SetMoneyText(TextMeshProUGUI text)
    {
        moneyText = text;
        UpdateScoreText();
    }

    public void AddFish(FishScriptableObject fish)
    {
        caughtFish.Add(fish);
        int previousValue = totalValue;
        totalValue += fish.value;

        if (countUpCoroutine != null)
            StopCoroutine(countUpCoroutine);
        countUpCoroutine = StartCoroutine(CountUpRoutine(previousValue, totalValue));
    }

    public int GetFishCount() => caughtFish.Count;

    public void SpendMoney(int amount)
    {
        int previousValue = totalValue;
        totalValue = Mathf.Max(0, totalValue - amount);

        if (countUpCoroutine != null)
            StopCoroutine(countUpCoroutine);
        countUpCoroutine = StartCoroutine(CountUpRoutine(previousValue, totalValue));
    }

    public void SpendEyeball()
    {
        for (int i = caughtFish.Count - 1; i >= 0; i--)
        {
            if (caughtFish[i].isUpgradeCurrency)
            {
                caughtFish.RemoveAt(i);
                return;
            }
        }
    }

    // private int RecalculateValue()
    // {
    //     int val = 0;
    //     foreach (var f in caughtFish)
    //         val += f.value;
    //     return val;
    // }

    private IEnumerator CountUpRoutine(int from, int to)
    {
        float elapsed = 0f;
        int lastValue = from - 1;
        while (elapsed < countUpDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / countUpDuration);
            int current = (int)Mathf.Lerp(from, to, t);
            if (current != lastValue)
            {
                if (moneyText != null)
                    moneyText.text = $"${current}";

                if (current > from)
                    AudioManager.Instance?.PlayScoreTick();

                lastValue = current;
            }
            yield return null;
        }

        if (moneyText != null)
            moneyText.text = $"${to}";
        countUpCoroutine = null;
    }

    private void UpdateScoreText()
    {
        if (moneyText != null)
            moneyText.text = $"${totalValue}";
    }
}
