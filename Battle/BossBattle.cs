using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Child of BattleManager specifically meant for boss battles
// Starts the battle slightly differently
public class BossBattle : BattleManager {
    // Start is called before the first frame update
    // Considering enemies are already present, don't try to spawn them
    protected override void Start() {
        MainVCam.Instance.EnterBattle();
        CameraController.Instance.EnterBattle();
        for (int i = 0; i < enemies.Count; ++i) {
            enemies[i].GetComponent<EnemyRuntime>().ResetMoveGauge();
            EnemyTag eTag = EnemyTag.Spawn(new Vector3(0, 0, 0), enemies[i]);
            Debug.Log(eTag);
            enemies[i].GetComponent<EnemyRuntime>().SetEnemyTag(eTag);
            enemies[i].GetComponent<EnemyRuntime>().SetButton(ButtonWrapper.Spawn(new Vector3(0, 0, 0), enemies[i]));
            if (i > 0) {
                enemies[i].GetComponent<EnemyRuntime>().ButtonSetRightAndUp(enemies[i - 1].GetComponent<EnemyRuntime>());
                enemies[i - 1].GetComponent<EnemyRuntime>().ButtonSetLeftAndDown(enemies[i].GetComponent<EnemyRuntime>());
            }
        }

        chain = 0;
        input_box.SetActive(false);
        eventSystem.SetActive(false);
        battleState = Battle_States.WAIT;

        for(int i = 0; i < Party.Instance.current_party.Count; ++i) {
            party.Add(Party.Instance.current_party[i]);
            
            party[i].transform.position = new Vector2(3f + (i * 0.8f), 1f - (1.4f * i));
            party[i].GetComponent<CharacterRuntime>().ResetMoveGauge();
            Debug.Log("RESET");
            Vector3 tag_spawn_pos = new Vector3(-100, 350 - (125 * i));
            party[i].GetComponent<CharacterRuntime>().SetCharTag(TagUI.Spawn(tag_spawn_pos, party[i], canvas));
            party[i].GetComponent<CharacterRuntime>().SetButton(ButtonWrapper.Spawn(new Vector3(0, 0, 0), party[i]));

            if(i > 0) {
                party[i].GetComponent<CharacterRuntime>().ButtonSetRightAndUp(party[i - 1].GetComponent<CharacterRuntime>());
                party[i - 1].GetComponent<CharacterRuntime>().ButtonSetLeftAndDown(party[i].GetComponent<CharacterRuntime>());
            }
        }
    }
}
