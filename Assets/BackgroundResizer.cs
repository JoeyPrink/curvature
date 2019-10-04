using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundResizer : MonoBehaviour {
    [SerializeField]
    private Transform background;
    
    
    
    void Update()
    {
        background.localScale = new Vector3(Screen.width*20/(float) Screen.height, 20, 1);    
    }
}
