using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var startGameButton = transform.FindDeepComponent<Button>("StartGameButton");
        startGameButton.onClick.AddListener(() =>
        {
            GameManager.LoadLevel(GameManager.LevelNames[0]);
        });

        var selectLevelButton = transform.FindDeepComponent<Button>("LevelSelectButton");
        selectLevelButton.onClick.AddListener(() =>
        {
            // load and display level select overlay
            var p = Resources.Load<GameObject>("Prefabs/LevelSelectOverlay");
            var levelSelectOverlay = GameObject.Instantiate(p, this.transform);
            var backButton = levelSelectOverlay.transform.FindDeepComponent<Button>("BackButton");
            backButton.onClick.AddListener(() => {
                GameObject.Destroy(levelSelectOverlay.gameObject);
            });

            // set level select buttons based on level array
            var levels = transform.FindDeep("Levels");
            levels.DestroyChildren();
            for (int i = 0; i < GameManager.LevelNames.Length; i++)
            {
                var levelName = GameManager.LevelNames[i];
                var levelSelectButtonWrapper = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/LevelSelectButton"), levels);
                var button = levelSelectButtonWrapper.transform.FindDeepComponent<Button>("Button");
                button.transform.GetComponentInChildren<TextMeshProUGUI>().text = $"Level {i + 1}";
                button.onClick.AddListener(() =>
                {
                    GameManager.LoadLevel(levelName);
                });
            }
        });
    }
}
