using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HitArea : MonoBehaviour
{
    public Slider HPBar;

    float maxHP = 100.0f;
    float currentHP;

    private void Start()
    {
        currentHP = maxHP; 
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EnemyAttackRange"))
        {
            float damage = 10.0f;
            currentHP -= damage;
            UpdateHPBar();
        }
    }

    void UpdateHPBar()
    {
        float hpRoll = (float)currentHP / maxHP;
        HPBar.value = hpRoll;
    }
}
