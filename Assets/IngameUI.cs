using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IngameUI : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var restartLevelButton = transform.FindDeepComponent<Button>("RestartLevelButton");
        restartLevelButton.onClick.AddListener(() =>
        {
            // simply reload the whole scene
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        });

        var selectLevelButton = transform.FindDeepComponent<Button>("LevelSelectButton");
        selectLevelButton.onClick.AddListener(() =>
        {
            // force disable grab input
            var im = transform.Find("/GameManager").GetComponent<InputManager>();
            im.Disable();

            // load and display level select overlay
            var p = Resources.Load<GameObject>("Prefabs/LevelSelectOverlay");
            var levelSelectOverlay = GameObject.Instantiate(p, GameObject.Find("IngameUI").transform);
            var backButton = levelSelectOverlay.transform.FindDeepComponent<Button>("BackButton");
            backButton.onClick.AddListener(() => {
                GameObject.Destroy(levelSelectOverlay.gameObject);
                im.Enable();
            });

            // set level select buttons based on level array
            var levels = transform.FindDeep("Levels");
            levels.DestroyChildren();
            for(int i = 0;i < GameManager.LevelNames.Length;i++)
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
