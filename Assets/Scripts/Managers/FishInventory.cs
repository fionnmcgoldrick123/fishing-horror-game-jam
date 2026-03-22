using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FishInventory : MonoBehaviour
{
    public static FishInventory Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI scoreText;

    [Header("Count-Up Animation")]
    [SerializeField] private float countUpDuration = 0.6f;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip scoreTickClip;

    private readonly List<FishScriptableObject> caughtFish = new List<FishScriptableObject>();
    private int totalValue;
    private Coroutine countUpCoroutine;

    public IReadOnlyList<FishScriptableObject> CaughtFish => caughtFish;
    public int TotalValue => totalValue;

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

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
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

    public int SellAll()
    {
        int sold = 0;
        for (int i = caughtFish.Count - 1; i >= 0; i--)
        {
            if (!caughtFish[i].isUpgradeCurrency)
            {
                sold += caughtFish[i].value;
                caughtFish.RemoveAt(i);
            }
        }
        totalValue = RecalculateValue();
        UpdateScoreText();
        return sold;
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

    private int RecalculateValue()
    {
        int val = 0;
        foreach (var f in caughtFish)
            val += f.value;
        return val;
    }

    private IEnumerator CountUpRoutine(int from, int to)
    {
        float elapsed = 0f;
        while (elapsed < countUpDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / countUpDuration);
            int current = (int)Mathf.Lerp(from, to, t);
            if (scoreText != null)
                scoreText.text = $"${current}";

            if (audioSource != null && scoreTickClip != null)
                audioSource.PlayOneShot(scoreTickClip);

            yield return null;
        }

        if (scoreText != null)
            scoreText.text = $"${to}";
        countUpCoroutine = null;
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
            scoreText.text = $"${totalValue}";
    }
}
