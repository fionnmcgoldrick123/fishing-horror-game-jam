using System;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public class DayDialogueEntry
{
    [Tooltip("The day number this entry applies to (matches DayManager.CurrentDay).")]
    public int day;

    [Tooltip("DialogueData asset for Dr. Sturgeon on this day.")]
    public DialogueData drSturgeonDialogue;

    [Tooltip("DialogueData asset for Perch on this day.")]
    public DialogueData perchDialogue;

    [Tooltip("Dialogue shown on the day-transition screen when this day begins (used by DayLoader).")]
    public DialogueData transitionDialogue;

    [Tooltip("DialogueData asset for Car on this day.")]
    public DialogueData carDialogue;
}

public class DialogueSwappingManager : MonoBehaviour
{
    public static DialogueSwappingManager Instance { get; private set; }

    [Tooltip("One entry per day where dialogue should change. Days not listed keep whatever DialogueData is assigned directly on the NPCDialogueTrigger.")]
    [SerializeField] private DayDialogueEntry[] dayEntries;

    [Tooltip("Fallback transition dialogue used by DayLoader when no day-specific entry exists.")]
    [SerializeField] private DialogueData defaultTransitionDialogue;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void OnEnable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ApplyDialogueForCurrentDay();
    }

    /// <summary>
    /// Finds all NPCDialogueTrigger components in the active scene and assigns
    /// the correct DialogueData for the current day. Called automatically on every
    /// scene load, but you can also call it manually if needed.
    /// </summary>
    public void ApplyDialogueForCurrentDay()
    {
        int currentDay = DayManager.Instance != null ? DayManager.Instance.CurrentDay : 1;

        DayDialogueEntry entry = GetEntryForDay(currentDay);
        if (entry == null) return; // No override for this day — NPCs keep their default dialogue

        NPCDialogueTrigger[] triggers = FindObjectsByType<NPCDialogueTrigger>(FindObjectsSortMode.None);
        foreach (NPCDialogueTrigger trigger in triggers)
        {
            if (trigger.CharacterId == CharacterID.DrSturgeon && entry.drSturgeonDialogue != null)
                trigger.SetDialogueData(entry.drSturgeonDialogue);
            else if (trigger.CharacterId == CharacterID.Perch && entry.perchDialogue != null)
                trigger.SetDialogueData(entry.perchDialogue);
            else if (trigger.CharacterId == CharacterID.Car && entry.carDialogue != null)
                trigger.SetDialogueData(entry.carDialogue);
        }
    }

    /// <summary>
    /// Returns the transition DialogueData for the given day.
    /// Uses each day entry's transitionDialogue, falling back to defaultTransitionDialogue.
    /// </summary>
    public DialogueData GetTransitionDialogue(int day)
    {
        DayDialogueEntry entry = GetEntryForDay(day);
        if (entry != null && entry.transitionDialogue != null)
            return entry.transitionDialogue;
        return defaultTransitionDialogue;
    }

    private DayDialogueEntry GetEntryForDay(int day)
    {
        foreach (DayDialogueEntry entry in dayEntries)
        {
            if (entry.day == day) return entry;
        }
        return null;
    }
}

