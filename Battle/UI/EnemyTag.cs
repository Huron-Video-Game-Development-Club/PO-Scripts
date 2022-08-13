using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyTag : MonoBehaviour
{
    //Spawn an enemy tag in given GameObject enemy's specific relative tag location
    public static EnemyTag Spawn(Vector3 position, GameObject enemy)
    {
        Transform enemyTagObject = Instantiate(GameAsset.Instance.enemy_tag, position, Quaternion.identity);
        enemyTagObject.SetParent(enemy.transform, false);
        enemyTagObject.GetChild(0).localPosition = enemy.GetComponent<EnemyRuntime>().GetTagLocation();
        EnemyTag enemyTag = enemyTagObject.gameObject.GetComponent<EnemyTag>();

        enemyTag.Setup(enemy.GetComponent<EnemyRuntime>(), enemyTagObject.GetChild(0));
        return enemyTag;
    }

    //Reference variables for positioning and updating
    private EnemyRuntime enemy_ref;
    private Transform canvas_ref;

    //Variables that control what is shown on the tag
    public GameObject enemy_name_object;
    private TextMeshProUGUI enemy_name_text;

    public Image break_fill;

    private Vector2 break_gauge_pos;

    //While the scene is played, we are making sure we show text
    private void Awake()
    {
        enemy_name_text = enemy_name_object.GetComponent<TextMeshProUGUI>();
    }

    //Setup an enemy tag
    void Setup(EnemyRuntime enemy, Transform canvas)
    {
        enemy_ref = enemy;
        canvas_ref = canvas;
        enemy_name_text.SetText(enemy_ref.GetName());

        break_gauge_pos = new Vector2(break_fill.transform.position.x, break_fill.transform.position.y);
        float shieldRatio = (float)enemy_ref.GetShields() / enemy_ref.GetMaxShields();
        //Debug.Log("Shields " + enemy_ref.GetShields());
        float break_shift_dist = shieldRatio * break_fill.rectTransform.rect.width * canvas_ref.transform.localScale.x;
        break_fill.transform.position = new Vector2(break_gauge_pos.x - break_shift_dist, break_gauge_pos.y);
    }

    //Update Break Gauge Position
    public void UpdateGauge(bool noBreakUpdateInBreak)
    {
        float shieldRatio = (float)enemy_ref.GetShields() / enemy_ref.GetMaxShields();
        float break_shift_dist = shieldRatio * break_fill.rectTransform.rect.width * canvas_ref.transform.localScale.x;
        break_fill.transform.position = new Vector2(break_gauge_pos.x - break_shift_dist, break_gauge_pos.y);
        
        //Debug.Log("In Break?");
        //Debug.Log(enemy_ref.battleManager.InBreak());
        //Debug.Log("Should we update break status?");
        //Debug.Log(!noBreakUpdateInBreak);

        // Doesn't update break status if killed
        if(enemy_ref.hp <= 0 && !enemy_ref.Broke()) {
            // Ends Break
            if(BattleManager.Instance.InBreak()) {
                BattleManager.Instance.EndBreak();
            }
            return;
        }

        if (enemy_ref.GetShields() == 0 && !enemy_ref.Broke()) {
            enemy_ref.Break();
            BreakEffect.Spawn(new Vector3(0, 0, 0), BattleManager.Instance.canvas, BattleManager.Instance);
            BattleManager.Instance.SlowTime();
        } else if (enemy_ref.GetShields() == 0 && enemy_ref.Broke() && BattleManager.Instance.InBreak() && noBreakUpdateInBreak == false) {
            BattleManager.Instance.EndBreak();
        } else if (enemy_ref.GetShields() != 0 && !enemy_ref.Broke() && BattleManager.Instance.InBreak() && noBreakUpdateInBreak == false) {
            BattleManager.Instance.EndBreak();
        }


    }
}
