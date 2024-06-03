using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageDisplay : MonoBehaviour
{
    private const float DISAPPEAR_TIMER_MAX = 1f;
    
    private TextMeshPro textMesh;
    private float disappearTimer;
    private Color textColor;
    private Vector3 displacement;

    private void Awake()
    {
        textMesh = transform.GetComponent<TextMeshPro>();
    }

    public void setValue(int amount, Color color)
    {
        textMesh.SetText(amount.ToString());
        textColor = color;
        disappearTimer = DISAPPEAR_TIMER_MAX;
        displacement = new Vector3(0, 5, 0);
    }


    public static DamageDisplay create(int damage, Vector3 position, Color color)
    {
        DamageDisplay popup = Instantiate(GuiManager.instance.damageNumberPrefab, position, Quaternion.identity).GetComponent<DamageDisplay>();
        popup.setValue(damage, color);

        return popup;
    }

    private void Update()
    {
        if(disappearTimer > DISAPPEAR_TIMER_MAX * .5f)
        {
            float increaseScale = 1f;
            transform.localScale += Vector3.one * increaseScale * Time.deltaTime;
        }
        else
        {
            float increaseScale = 1f;
            transform.localScale -= Vector3.one * increaseScale * Time.deltaTime;
        }

        
        disappearTimer -= Time.deltaTime;

        if(disappearTimer < 0)
        {
            float disappearSpeed = 3f;
            textColor.a -= disappearSpeed * Time.deltaTime;
            textMesh.color = textColor;

            if(textColor.a < 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
