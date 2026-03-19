using System.Collections;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip typingSound;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private UnityEngine.UI.Image characterPortrait;

    private PlayerController player;

    private DialogueData _currentData;
    private int _lineIndex;
    private Coroutine _typingCoroutine;
    private bool _isTyping;
    private bool _isActive;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        player = FindFirstObjectByType<PlayerController>();
    }

    public void StartDialogue(DialogueData data)
    {
        nameText.text = data.characterName;
        characterPortrait.sprite = data.characterPortrait;
        _currentData = data;
        _lineIndex = 0;
        _isActive = true;
        dialoguePanel.SetActive(true);
        ShowLine();
    }

    void Update()
    {
        if (!_isActive) return;

        if (Input.GetMouseButtonDown(0))
        {
            if (_isTyping)
            {
                // First click: skip to end of current line
                SkipTyping();
            }
            else
            {
                // Second click: advance to next line
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

            if (_currentData.typingSound != null && audioSource != null)
                audioSource.PlayOneShot(_currentData.typingSound);


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

    private void EndDialogue()
    {
        _isActive = false;
        dialoguePanel.SetActive(false);
        dialogueText.text = "";
        player.ExitDialogue();
    }
}
