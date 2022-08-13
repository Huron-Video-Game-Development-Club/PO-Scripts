using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LockedArea : MonoBehaviour {
    public Item necessaryItem;
    public UnityEvent LockedEvent;
    public UnityEvent UnlockedEvent;

    void OnTriggerEnter2D() {
        if(Party.Instance.inventory.HasKeyItem(necessaryItem)) {
            UnlockedEvent.Invoke();
        } else {
            LockedEvent.Invoke();
        }
    }
}
