using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Attach to a manager GameObject in the GameOver scene.
/// Plays a DialogueData typewriter sequence (no portrait), then reveals a
/// Main Menu button when finished.
///
/// Scene setup:
///   - Assign a DialogueData asset (characterName and portrait are ignored).
///   - Point dialogueText at a TextMeshProUGUI in the scene.
///   - Point mainMenuButton at a Button that is DISABLED by default.
///   - Set mainMenuSceneName to match your main menu scene name.
/// </summary>
public class GameOverScreenManager : MonoBehaviour
{
    [Header("Dialogue")]
    [Tooltip("Dialogue asset to read lines and typing speed from. Portrait is ignored.")]
    [SerializeField] private DialogueData dialogue;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [Tooltip("Optional: override typing sound. If not set, uses the one from DialogueData.")]
    [SerializeField] private AudioClip typingSound;

    [Header("UI")]
    [Tooltip("The button shown after dialogue ends. Disable it in the scene by default.")]
    [SerializeField] private Button mainMenuButton;
    [Tooltip("Duration of the button pop-in animation.")]
    [SerializeField] private float buttonPopDuration = 0.4f;

    [Header("Circle Flicker")]
    [Tooltip("The circle Image to flicker. Leave empty if you don't want flickering.")]
    [SerializeField] private Image circleImage;
    [Tooltip("Colors to cycle through (at least 2).")]
    [SerializeField] private Color[] flickerColors = new Color[] { Color.white, Color.red };
    [Tooltip("Seconds per color before switching.")]
    [SerializeField] private float flickerDuration = 0.2f;
    [Tooltip("Start flickering automatically when the scene loads.")]
    [SerializeField] private bool startFlickering = true;

    private Coroutine _flickerCoroutine;

    [Header("Scene")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private int _lineIndex;
    private bool _isTyping;
    private bool _dialogueDone;
    private Coroutine _typingCoroutine;

    private void Start()
    {
        if (mainMenuButton != null)
        {
            mainMenuButton.gameObject.SetActive(false);
            // Start with scale 0 so the pop animation works
            RectTransform btnRect = mainMenuButton.GetComponent<RectTransform>();
            if (btnRect != null)
                btnRect.localScale = Vector3.zero;
            mainMenuButton.onClick.AddListener(GoToMainMenu);
        }

        if (startFlickering && circleImage != null && flickerColors.Length > 0)
            _flickerCoroutine = StartCoroutine(FlickerCircle());

        if (dialogue == null || dialogue.lines.Length == 0)
        {
            ShowMainMenuButton();
            return;
        }

        dialogueText.text = "";
        ShowLine();
    }

    private void Update()
    {
        if (_dialogueDone) return;

        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.E))
        {
            if (_isTyping)
                SkipTyping();
            else
                AdvanceLine();
        }
    }

    private void ShowLine()
    {
        if (_typingCoroutine != null) StopCoroutine(_typingCoroutine);
        dialogueText.text = "";
        _typingCoroutine = StartCoroutine(TypeLine(dialogue.lines[_lineIndex]));
    }

    private IEnumerator TypeLine(string line)
    {
        _isTyping = true;
        float delay = dialogue.textSpeed > 0f ? 1f / dialogue.textSpeed : 0.033f;

        foreach (char c in line)
        {
            dialogueText.text += c;
            
            // Use override sound if set, otherwise use sound from DialogueData
            AudioClip soundToPlay = typingSound != null ? typingSound : dialogue.typingSound;
            AudioManager.Instance?.PlayDialogueTyping(soundToPlay);
            
            yield return new WaitForSeconds(delay);
        }

        _isTyping = false;
    }

    private void SkipTyping()
    {
        if (_typingCoroutine != null) StopCoroutine(_typingCoroutine);
        _typingCoroutine = null;
        dialogueText.text = dialogue.lines[_lineIndex];
        _isTyping = false;
    }

    private void AdvanceLine()
    {
        _lineIndex++;
        if (_lineIndex >= dialogue.lines.Length)
        {
            _dialogueDone = true;
            ShowMainMenuButton();
            return;
        }
        ShowLine();
    }

    private void ShowMainMenuButton()
    {
        if (mainMenuButton != null)
        {
            // Hide dialogue text
            dialogueText.text = "";
            
            // Pop in the button
            mainMenuButton.gameObject.SetActive(true);
            RectTransform btnRect = mainMenuButton.GetComponent<RectTransform>();
            if (btnRect != null)
            {
                btnRect.localScale = Vector3.zero;
                StartCoroutine(PopInButton(btnRect));
            }
        }
    }

    private System.Collections.IEnumerator PopInButton(RectTransform btnRect)
    {
        float elapsed = 0f;
        while (elapsed < buttonPopDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / buttonPopDuration);
            // Ease out: scale from 0 to 1 with a slight bounce
            float scale = t < 0.7f ? t / 0.7f : 1f + (t - 0.7f) / 0.3f * 0.1f;
            scale = Mathf.Clamp01(scale);
            btnRect.localScale = new Vector3(scale, scale, 1f);
            yield return null;
        }
        btnRect.localScale = Vector3.one;
    }

    private void GoToMainMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(mainMenuSceneName);
    }

    /// <summary>
    /// Cycles the circle image through the assigned flicker colors.
    /// </summary>
    private IEnumerator FlickerCircle()
    {
        int colorIndex = 0;
        while (true)
        {
            if (circleImage != null && flickerColors.Length > 0)
            {
                circleImage.color = flickerColors[colorIndex];
                colorIndex = (colorIndex + 1) % flickerColors.Length;
            }
            yield return new WaitForSeconds(flickerDuration);
        }
    }
}
