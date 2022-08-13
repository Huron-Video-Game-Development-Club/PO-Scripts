using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VictoryScreen : MonoBehaviour {
    public static VictoryScreen Spawn(int expGain, int goldGain, GameObject canvas) {
        Transform transform = Instantiate(GameAsset.Instance.victoryScreen, new Vector2(0f, 0f), Quaternion.identity);
        transform.SetParent(canvas.transform, false);
        VictoryScreen vScreen = transform.gameObject.GetComponent<VictoryScreen>();

        vScreen.Setup(expGain, goldGain);
        return vScreen;
    }

    public TextMeshProUGUI goldNum;
    public TextMeshProUGUI expNum;
    
    List<VictoryTag> expTags; 
    bool finished;

    void Setup(int expGain, int goldGain) {
        AudioManager.Instance.Stop(BattleSceneManager.Instance.GetBattleMusicName());
        AudioManager.Instance.PlayNoNext("Victory");

        expTags = new List<VictoryTag>();

        goldNum.SetText(goldGain.ToString());
        expNum.SetText(expGain.ToString());

        for(int i = 0; i < Party.Instance.current_party.Count; ++i) {
            int idx = i;
            expTags.Add(VictoryTag.Spawn(idx, expGain, this.transform.GetChild(0).gameObject));
            expTags[expTags.Count - 1].StartEXPGain();
        }

        finished = false;
    }

    void Update() {
        if(!finished) {
            finished = true;
            foreach(VictoryTag tag in expTags) {
                if(tag.Finished()) {
                    continue;
                }

                finished = false;
                break;
            }
        }

        if(Input.GetKeyDown(KeyCode.Space)) {
            if(finished) {
                AudioManager.Instance.Stop("Victory");
                if(BattleSceneManager.Instance != null)
                    BattleSceneManager.Instance.EndBattle();
            } else {
                foreach(VictoryTag tag in expTags) {
                    tag.SkipEXPGain();
                }
            }
        }
    }
}
