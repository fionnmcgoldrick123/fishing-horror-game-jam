using UnityEngine;
using UnityEngine.UI;

public class MainMenuButtonHandler : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private string sceneToLoad = "Opening";

    void Start()
    {
        startButton.onClick.AddListener(OnStartButtonClicked);
    }

    private void OnStartButtonClicked()
    {
        SceneManager.Instance.LoadScene("Opening");
    }
}
