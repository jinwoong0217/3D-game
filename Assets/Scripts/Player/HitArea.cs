using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HitArea : MonoBehaviour
{
    public Slider HPBar;
    public Canvas DieCanvas;

    float maxHP = 100.0f;
    float currentHP;

    private void Start()
    {
        currentHP = maxHP; 
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EnemyAttackRange"))  // 적의 공격범위 콜라이더를 태그로 지정하여 찾게함
        {
            float damage = 10.0f;
            currentHP -= damage;
            UpdateHPBar();
        }
    }

    void UpdateHPBar()
    {
        float hpRoll = (float)currentHP / maxHP;  // 실시간으로 Hp체력을 업데이트해줌
        HPBar.value = hpRoll;
        if(currentHP < 0.1f )
        {
            DieCanvas.gameObject.SetActive(true);
            Time.timeScale = 0;
        }
    }
}
