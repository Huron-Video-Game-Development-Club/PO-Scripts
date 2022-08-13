using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageEffect : MonoBehaviour
{

    public static DamageEffect Spawn(Vector3 position, int damage)
    {
        Vector3 new_pos = new Vector3(position.x + 0.6f, position.y - 0.15f);
        Transform DamageEffectObject = Instantiate(GameAsset.Instance.damage_effect, new_pos, Quaternion.identity);
        DamageEffect damage_effect = DamageEffectObject.gameObject.GetComponent<DamageEffect>();
        damage_effect.Setup(damage);

        return damage_effect;
    }


    private TextMeshPro damage_text;
    private float dissapear_time;
    private Color text_color;
    private const float MAX_DISAPPEAR_TIME = 0.3f;
    void Awake()
    {
        damage_text = gameObject.GetComponent<TextMeshPro>();
    }

    public void Setup(int damage)
    {

        damage_text.SetText(damage.ToString());
        dissapear_time = MAX_DISAPPEAR_TIME;
        text_color = damage_text.color;
    }

    // Update is called once per frame
    public void Update()
    {
        dissapear_time -= Time.deltaTime;
        if (dissapear_time > 0.8f * MAX_DISAPPEAR_TIME)
        {
            float speed = 1f;
            transform.position += new Vector3(0, speed) * Time.deltaTime;
        }
        else if (dissapear_time > 0.35f * MAX_DISAPPEAR_TIME)
        {
            float speed = 1.3f;
            transform.position -= new Vector3(0, speed) * Time.deltaTime;
        }
        else if (dissapear_time > 0.1 * MAX_DISAPPEAR_TIME)
        {
            float speed = 1.5f;
            transform.position += new Vector3(0, speed) * Time.deltaTime;
        }
        else if(dissapear_time > 0 * MAX_DISAPPEAR_TIME)
        {
            float speed = 1.7f;
            transform.position -= new Vector3(0, speed) * Time.deltaTime;
        }
        

        if (dissapear_time < 0)
        {
            float fade_speed = 5f;
            text_color.a -= fade_speed * Time.deltaTime;
            damage_text.color = text_color;
            if(text_color.a <= 0)
            {
                //Debug.Log("Killing Pop-Up");
                Destroy(gameObject);
            }
        }
    }
}
