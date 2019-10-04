using UnityEngine;
using UnityEngine.UI;

public class WinScreen : MonoBehaviour
{
    void Start()
    {
        var startGameButton = transform.FindDeepComponent<Button>("ContinueButton");
        startGameButton.onClick.AddListener(() =>
        {
            GameManager.LoadLevel("MainMenu");
        });
    }
}
