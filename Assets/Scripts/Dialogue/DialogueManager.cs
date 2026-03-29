using System.Collections;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private RectTransform dialoguePanelRect;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private UnityEngine.UI.Image characterPortrait;

    [Header("Animation")]
    [SerializeField] private float panelPopDuration = 0.3f;

    private DialogueData _currentData;
    private int _lineIndex;
    private Coroutine _typingCoroutine;
    private Coroutine _panelPopCoroutine;
    private bool _isTyping;
    private bool _isPanelAnimating;
    private bool _isActive;
    private bool _justEnded;

    public bool IsActive => _isActive;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void StartDialogue(DialogueData data)
    {
        if (dialoguePanel == null || dialogueText == null || nameText == null)
        {
            Debug.LogError("DialogueManager: UI references are missing. Ensure all UI is a child of the DialogueManager GameObject.");
            return;
        }
        nameText.text = data.characterName;
        characterPortrait.sprite = data.characterPortrait;
        _currentData = data;
        _lineIndex = 0;
        _isActive = true;
        dialogueText.text = "";
        dialoguePanel.SetActive(true);

        _isPanelAnimating = true;
        _panelPopCoroutine = UIAnimations.PopInPanel(this, dialoguePanelRect, panelPopDuration, () =>
        {
            _isPanelAnimating = false;
            _panelPopCoroutine = null;
            ShowLine();
        });
    }

    void Update()
    {
        _justEnded = false;

        if (!_isActive) return;

        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.E))
        {
            if (_isPanelAnimating)
            {
                if (_panelPopCoroutine != null) StopCoroutine(_panelPopCoroutine);
                _panelPopCoroutine = null;
                dialoguePanelRect.localScale = Vector3.one;
                _isPanelAnimating = false;
                ShowLine();
            }
            else if (_isTyping)
            {
                SkipTyping();
            }
            else
            {
                AdvanceLine();
            }
        }
    }

    private void ShowLine()
    {
        if (_typingCoroutine != null) StopCoroutine(_typingCoroutine);
        dialogueText.text = "";
        _typingCoroutine = StartCoroutine(TypeLine(_currentData.lines[_lineIndex]));
    }

    private IEnumerator TypeLine(string line)
    {
        _isTyping = true;
        float delay = 1f / _currentData.textSpeed;

        foreach (char c in line)
        {
            dialogueText.text += c;

            AudioManager.Instance?.PlayDialogueTyping(_currentData.typingSound);


            yield return new WaitForSeconds(delay);
        }

        _isTyping = false;
    }

    private void SkipTyping()
    {
        if (_typingCoroutine != null) StopCoroutine(_typingCoroutine);
        dialogueText.text = _currentData.lines[_lineIndex];
        _isTyping = false;
    }

    private void AdvanceLine()
    {
        _lineIndex++;

        if (_lineIndex >= _currentData.lines.Length)
        {
            EndDialogue();
            return;
        }

        ShowLine();
    }

    public void ForceClose()
    {
        if (!_isActive) return;
        if (_typingCoroutine != null) StopCoroutine(_typingCoroutine);
        _typingCoroutine = null;
        _isTyping = false;
        EndDialogue();
    }

    private void EndDialogue()
    {
        if (_panelPopCoroutine != null) StopCoroutine(_panelPopCoroutine);
        _panelPopCoroutine = null;
        _isPanelAnimating = false;

        _isActive = false;
        _justEnded = true;
        dialoguePanel.SetActive(false);
        dialogueText.text = "";

        PlayerController p = FindFirstObjectByType<PlayerController>();
        p?.ExitDialogue();
    }

    public bool JustEnded() => _justEnded;
}
