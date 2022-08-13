using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasFitter : MonoBehaviour {
    Canvas canvas;
    
    // Start is called before the first frame update
    void Start() {
        canvas = GetComponent<Canvas>();
        if(canvas == null || CameraController.Instance == null) return;

        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = CameraController.Instance.gameObject.GetComponent<Camera>();
        canvas.planeDistance = 1;
        canvas.sortingLayerName = "Canvas";
    }
}
