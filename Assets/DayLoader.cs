using System.Collections;
using TMPro;
using UnityEngine;

/// <summary>
/// Place this in the Shop scene alongside your overlay Canvas.
/// Call StartTransition() when the player meets quota.
///
/// Sequence:
///   1. Fade the CanvasGroup overlay in.
///   2. Show the upcoming day number on dayText.
///   3. Play the day-complete sound.
///   4. Type each dialogue line (click / E skips typing; click / E on a finished line advances).
///   5. After the last line is advanced, pay quota, increment the day, and load World.
/// </summary>
public class DayLoader : MonoBehaviour
{
    [Header("Overlay")]
    [Tooltip("CanvasGroup of the full-screen overlay image. Starts hidden.")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float fadeInDuration = 0.8f;

    [Header("Day Text")]
    [Tooltip("Displays the upcoming day number, e.g. 'Day 2'.")]
    [SerializeField] private TextMeshProUGUI dayText;

    [Header("Dialogue")]
    [Tooltip("TextMeshProUGUI that the typewriter text is written into.")]
    [SerializeField] private TextMeshProUGUI dialogueText;
    [Tooltip("Last-resort fallback if DialogueSwappingManager has no dialogue for this day.")]
    [SerializeField] private DialogueData fallbackDialogue;

    [Header("Audio")]
    [Tooltip("One-shot clip played once the overlay is fully visible.")]
    [SerializeField] private AudioClip dayCompleteSound;

    private bool _isActive;
    private bool _skipRequested;
    private bool _isTyping;
    private DialogueData _activeDialogue;

    private void Awake()
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (!_isActive) return;
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.E))
            _skipRequested = true;
    }

    /// <summary>
    /// Starts the full day-end transition. Ignored if already running.
    /// </summary>
    public void StartTransition()
    {
        if (_isActive) return;
        StartCoroutine(TransitionRoutine());
    }

    private IEnumerator TransitionRoutine()
    {
        _isActive = true;
        _skipRequested = false;

        // Show the NEXT day number before MeetQuota increments the counter
        int nextDay = DayManager.Instance != null ? DayManager.Instance.CurrentDay + 1 : 1;
        if (dayText != null)
            dayText.text = $"Day {nextDay}";

        // Look up dialogue from DialogueSwappingManager; fall back to local fallbackDialogue
        if (DialogueSwappingManager.Instance != null)
            _activeDialogue = DialogueSwappingManager.Instance.GetTransitionDialogue(nextDay);
        if (_activeDialogue == null)
            _activeDialogue = fallbackDialogue;

        // Fade overlay in
        canvasGroup.gameObject.SetActive(true);
        canvasGroup.alpha = 0f;
        float elapsed = 0f;
        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(elapsed / fadeInDuration);
            yield return null;
        }
        canvasGroup.alpha = 1f;

        // Play day-complete sound
        if (dayCompleteSound != null)
            AudioManager.Instance?.PlayClipGetLength(dayCompleteSound);

        // Run dialogue lines
        if (_activeDialogue != null && _activeDialogue.lines != null && _activeDialogue.lines.Length > 0 && dialogueText != null)
        {
            dialogueText.text = "";
            for (int i = 0; i < _activeDialogue.lines.Length; i++)
            {
                yield return TypeLine(_activeDialogue.lines[i]);
                // Line is fully shown — wait for a fresh click/E to advance
                _skipRequested = false;
                yield return new WaitUntil(() => _skipRequested);
            }
        }

        // Pay quota, advance day counter, load World
        if (TimeOfDayManager.Instance != null)
            TimeOfDayManager.Instance.MeetQuota();
        else
            UnityEngine.SceneManagement.SceneManager.LoadScene("World");

        _isActive = false;
    }

    private IEnumerator TypeLine(string line)
    {
        _isTyping = true;
        _skipRequested = false;
        dialogueText.text = "";

        float charDelay = (_activeDialogue != null && _activeDialogue.textSpeed > 0f) ? 1f / _activeDialogue.textSpeed : 0.033f;
        foreach (char c in line)
        {
            if (_skipRequested)
            {
                // Jump straight to full line; clear flag so the outer loop waits for a NEW press
                dialogueText.text = line;
                _skipRequested = false;
                break;
            }
            dialogueText.text += c;
            AudioManager.Instance?.PlayDialogueTyping(_activeDialogue != null ? _activeDialogue.typingSound : null);
            yield return new WaitForSeconds(charDelay);
        }

        _isTyping = false;
    }
}


