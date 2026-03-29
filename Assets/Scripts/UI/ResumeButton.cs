using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ResumeButton : MonoBehaviour
{
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(() => PauseManager.Instance?.Resume());
    }
}
