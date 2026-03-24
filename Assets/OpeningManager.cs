using System.Collections;
using TMPro;
using UnityEngine;

public class OpeningManager : MonoBehaviour
{
    [SerializeField] private DialogueData openingDialogue;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private string worldSceneName = "World";

    private int _lineIndex;
    private bool _isTyping;
    private Coroutine _typingCoroutine;

    void Start()
    {
        if (openingDialogue == null || openingDialogue.lines.Length == 0)
        {
            LoadWorld();
            return;
        }

        dialogueText.text = "";
        ShowLine();
    }

    void Update()
    {
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
        _typingCoroutine = StartCoroutine(TypeLine(openingDialogue.lines[_lineIndex]));
    }

    private IEnumerator TypeLine(string line)
    {
        _isTyping = true;
        float delay = 1f / openingDialogue.textSpeed;

        foreach (char c in line)
        {
            dialogueText.text += c;
            AudioManager.Instance?.PlayDialogueTyping(openingDialogue.typingSound);
            yield return new WaitForSeconds(delay);
        }

        _isTyping = false;
    }

    private void SkipTyping()
    {
        if (_typingCoroutine != null) StopCoroutine(_typingCoroutine);
        _typingCoroutine = null;
        dialogueText.text = openingDialogue.lines[_lineIndex];
        _isTyping = false;
    }

    private void AdvanceLine()
    {
        _lineIndex++;
        if (_lineIndex >= openingDialogue.lines.Length)
        {
            LoadWorld();
            return;
        }
        ShowLine();
    }

    private void LoadWorld()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(worldSceneName);
    }
}
