using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelCompleteAnim : MonoBehaviour {
    private float counter = 0;
    private float initialScale = 1.5f;
    
    void Start() {
        transform.localScale = Vector3.one * initialScale;
    }

    void Update() {
        counter += Time.deltaTime*0.5f;
        float scale = Mathf.Lerp(initialScale, 1, Mathf.Pow(Mathf.Clamp01(counter), 0.75f));
        transform.localScale = Vector3.one*scale;
    }
}
