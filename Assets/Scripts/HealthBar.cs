using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] HealthComponent healthComponent;
    Slider slider;

    private void Awake()
    {
        slider = GetComponent<Slider>();
        healthComponent.onDamage.AddListener(UpdateHealth);
        healthComponent.onHeal.AddListener(UpdateHealth);

        slider.minValue = 0;
        slider.maxValue = healthComponent.maxHealth;

        UpdateHealth(0, healthComponent.startingHealth);
    }

    public void UpdateHealth(int nothing, int health)
    {
        slider.value = health;
    }
}
