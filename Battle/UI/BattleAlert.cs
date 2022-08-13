using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

// Manager class for alerts in battle
// Used in conjuction with BattleAlert prefab
// Used for alerting with messages like "The Party Fled" or attack names like "Sweep" 
public class BattleAlert : MonoBehaviour {
    public TextMeshProUGUI message;
    public Image inner;

    public static BattleAlert Spawn(string text, GameObject canvas, UnityEvent endEvent = null) {
        Transform alertTransform = Instantiate(GameAsset.Instance.battleAlert, new Vector2(0f, 300f), Quaternion.identity);
        alertTransform.SetParent(canvas.transform, false);
        BattleAlert bAlert = alertTransform.gameObject.GetComponent<BattleAlert>();

        bAlert.Setup(text, endEvent);
        return bAlert;
    } // Spawn()

    private UnityEvent end;

    void Setup(string text, UnityEvent endEvent) {
        message.SetText(text);
        message.autoSizeTextContainer = true;
        end = endEvent;
        Vector2 textSize = message.GetPreferredValues(text);

        StartCoroutine(Die());
    } // Setup()

    // Kills game object after 1 second and invokes any end events
    // Bye bye :(
    IEnumerator Die() {
        yield return new WaitForSeconds(1.0f);
        
        if(end != null) end.Invoke();
        Destroy(gameObject);
    } // Die()
}
