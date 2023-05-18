using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageOverlay : MonoBehaviour
{
    [SerializeField] float maxAlpha = 0.3f;
    [SerializeField] float fadeSpeed = 0.1f;
    [SerializeField] float alphaPerDamage = 0.1f;

    Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    private void Update()
    {
        //On baisse l'alpha graduellement
        image.color = new Color(image.color.r, image.color.g, image.color.b, Mathf.Max(0, image.color.a - (fadeSpeed * Time.deltaTime)));
    }

    public void TakeDamage(int dmg, int health)
    {
        //On augmente l'alpha selon le dommage pris
        image.color = new Color(image.color.r, image.color.g, image.color.b, Mathf.Min(maxAlpha, image.color.a + (alphaPerDamage * dmg)));
    }
}
