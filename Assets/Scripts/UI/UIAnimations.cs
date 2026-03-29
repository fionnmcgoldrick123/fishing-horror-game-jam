using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public static class UIAnimations
{
    public static Coroutine CountUp(MonoBehaviour host, TMPro.TextMeshProUGUI text, int targetValue, float duration, Action onComplete = null, Action onTick = null)
    {
        return host.StartCoroutine(CountUpRoutine(text, targetValue, duration, onComplete, onTick));
    }

    private static IEnumerator CountUpRoutine(TMPro.TextMeshProUGUI text, int targetValue, float duration, Action onComplete, Action onTick)
    {
        float elapsed = 0f;
        int lastValue = -1;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            int current = Mathf.RoundToInt(Mathf.Lerp(0, targetValue, t));
            if (current != lastValue)
            {
                text.text = current.ToString();
                onTick?.Invoke();
                lastValue = current;
            }
            yield return null;
        }
        text.text = targetValue.ToString();
        onComplete?.Invoke();
    }

    public static Coroutine PopInSequence(MonoBehaviour host, Image[] images, int count, Color activeColor, Color inactiveColor, float interval, float punchDuration, Action onComplete = null, Action onEachStar = null)
    {
        return host.StartCoroutine(PopInSequenceRoutine(images, count, activeColor, inactiveColor, interval, punchDuration, onComplete, onEachStar));
    }

    private static IEnumerator PopInSequenceRoutine(Image[] images, int count, Color activeColor, Color inactiveColor, float interval, float punchDuration, Action onComplete, Action onEachStar)
    {
        for (int i = 0; i < images.Length; i++)
        {
            images[i].color = inactiveColor;
            images[i].transform.localScale = Vector3.zero;
        }

        for (int i = 0; i < images.Length; i++)
        {
            if (i < count)
            {
                images[i].color = activeColor;
                onEachStar?.Invoke();
                yield return PopScale(images[i].transform, punchDuration);
                yield return new WaitForSeconds(interval);
            }
            else
            {
                images[i].color = inactiveColor;
                images[i].transform.localScale = Vector3.one;
            }
        }

        onComplete?.Invoke();
    }

    private static IEnumerator PopScale(Transform t, float duration)
    {
        float elapsed = 0f;
        float overshoot = 1.3f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float norm = Mathf.Clamp01(elapsed / duration);

            float scale;
            if (norm < 0.6f)
                scale = Mathf.Lerp(0f, overshoot, norm / 0.6f);
            else
                scale = Mathf.Lerp(overshoot, 1f, (norm - 0.6f) / 0.4f);

            t.localScale = new Vector3(scale, scale, scale);
            yield return null;
        }
        t.localScale = Vector3.one;
    }

    public static void FinishPopIn(Image[] images, int count, Color activeColor, Color inactiveColor)
    {
        for (int i = 0; i < images.Length; i++)
        {
            if (i < count)
            {
                images[i].color = activeColor;
                images[i].transform.localScale = Vector3.one;
            }
            else
            {
                images[i].color = inactiveColor;
                images[i].transform.localScale = Vector3.one;
            }
        }
    }

    public static void FinishCountUp(TMPro.TextMeshProUGUI text, int targetValue)
    {
        text.text = targetValue.ToString();
    }

    public static Coroutine Typewriter(MonoBehaviour host, TMPro.TextMeshProUGUI text, string fullText, float charsPerSecond, Action onComplete = null, Action onChar = null)
    {
        return host.StartCoroutine(TypewriterRoutine(text, fullText, charsPerSecond, onComplete, onChar));
    }

    private static IEnumerator TypewriterRoutine(TMPro.TextMeshProUGUI text, string fullText, float charsPerSecond, Action onComplete, Action onChar)
    {
        text.text = "";
        float interval = 1f / charsPerSecond;
        for (int i = 0; i < fullText.Length; i++)
        {
            text.text = fullText.Substring(0, i + 1);
            onChar?.Invoke();
            yield return new WaitForSeconds(interval);
        }
        text.text = fullText;
        onComplete?.Invoke();
    }

    public static void FinishTypewriter(TMPro.TextMeshProUGUI text, string fullText)
    {
        text.text = fullText;
    }

    public static Coroutine PopInPanel(MonoBehaviour host, RectTransform panel, float duration, Action onComplete = null)
    {
        return host.StartCoroutine(PopInPanelRoutine(panel, duration, onComplete));
    }

    private static IEnumerator PopInPanelRoutine(RectTransform panel, float duration, Action onComplete)
    {
        float elapsed = 0f;
        float overshoot = 1.1f;
        panel.localScale = Vector3.zero;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float norm = Mathf.Clamp01(elapsed / duration);
            float scale;
            if (norm < 0.6f)
                scale = Mathf.Lerp(0f, overshoot, norm / 0.6f);
            else
                scale = Mathf.Lerp(overshoot, 1f, (norm - 0.6f) / 0.4f);
            panel.localScale = new Vector3(scale, scale, scale);
            yield return null;
        }
        panel.localScale = Vector3.one;
        onComplete?.Invoke();
    }
}
