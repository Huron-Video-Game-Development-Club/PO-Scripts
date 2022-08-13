using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectLayerSorter : MonoBehaviour
{
    public int baseSortingOrder = 1000;
    
    [SerializeField]
    private int offset = 0;

    [SerializeField]
    private bool inanimateObject = true;
    
    /*private SpriteRenderer renderer;

    void Awake() {
        renderer = gameObject.GetComponent<SpriteRenderer>();
    }*/

    void LateUpdate() {
        gameObject.GetComponent<SpriteRenderer>().sortingOrder = (int)(baseSortingOrder - (5*transform.position.y - offset));
        if(inanimateObject) {
            Destroy(this);
        }
    }
}
