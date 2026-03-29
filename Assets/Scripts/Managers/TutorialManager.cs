using System.Collections;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private GameObject warningPanel;
    [SerializeField] private RectTransform warningPanelRect;

    [Header("Animation")]
    [SerializeField] private float warningAnimationDuration = 0.5f;
    [SerializeField] private AnimationType animationType = AnimationType.ScalePopIn;

    [Header("Scene")]
    [SerializeField] private string openingSceneName = "Opening";

    private int stage = 0;
    private bool isAnimating = false;

    public enum AnimationType { ScalePopIn, FadeIn, SlideInUp }

    void Start()
    {
        tutorialPanel?.SetActive(true);
        warningPanel?.SetActive(false);
        stage = 0;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isAnimating)
        {
            if (stage == 0)
                OnTutorialClick();
            else if (stage == 1)
                OnWarningClick();
        }
    }

    private void OnTutorialClick()
    {
        tutorialPanel?.SetActive(false);
        warningPanel?.SetActive(true);

        StartCoroutine(AnimateWarningPanel());
        stage = 1;
    }

    private void OnWarningClick()
    {
        MainMenuMusic.Instance?.DestroyMusic();
        global::SceneManager.Instance.LoadScene(openingSceneName);
    }

    private IEnumerator AnimateWarningPanel()
    {
        isAnimating = true;

        if (warningPanelRect != null)
        {
            switch (animationType)
            {
                case AnimationType.ScalePopIn:
                    yield return PopInScale(warningPanelRect);
                    break;
                case AnimationType.FadeIn:
                    yield return FadeIn(warningPanel);
                    break;
                case AnimationType.SlideInUp:
                    yield return SlideInUp(warningPanelRect);
                    break;
            }
        }

        isAnimating = false;
    }

    private IEnumerator PopInScale(RectTransform rect)
    {
        rect.localScale = Vector3.zero;
        float elapsed = 0f;
        float overshoot = 1.1f;

        while (elapsed < warningAnimationDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / warningAnimationDuration);

            float scale;
            if (t < 0.6f)
                scale = Mathf.Lerp(0f, overshoot, t / 0.6f);
            else
                scale = Mathf.Lerp(overshoot, 1f, (t - 0.6f) / 0.4f);

            rect.localScale = new Vector3(scale, scale, 1f);
            yield return null;
        }

        rect.localScale = Vector3.one;
    }

    private IEnumerator FadeIn(GameObject panel)
    {
        CanvasGroup canvasGroup = panel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = panel.AddComponent<CanvasGroup>();

        canvasGroup.alpha = 0f;
        float elapsed = 0f;

        while (elapsed < warningAnimationDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(elapsed / warningAnimationDuration);
            yield return null;
        }

        canvasGroup.alpha = 1f;
    }

    private IEnumerator SlideInUp(RectTransform rect)
    {
        Vector3 startPos = rect.localPosition;
        Vector3 endPos = startPos;
        startPos.y -= 500f;
        rect.localPosition = startPos;

        float elapsed = 0f;

        while (elapsed < warningAnimationDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / warningAnimationDuration);
            rect.localPosition = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }

        rect.localPosition = endPos;
    }
}
