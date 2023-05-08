using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BulletComponent : MonoBehaviour
{
    /// <summary>
    /// Combien de dommage fait-on?
    /// </summary>
    public int damage = 1;

    private void OnCollisionEnter(Collision collision)
    {
        //On essaie de prendre le HealthComponent de l'autre objet pour l'endommager
        HealthComponent health;
        if (collision.gameObject.TryGetComponent<HealthComponent>(out health))
            health.TakeDamage(damage);
        //On desactive la balle
        gameObject.SetActive(false);
    }

    //Lorsqu'une balle est desactivee, on remet a zero sa vitesse
    private void OnDisable()
    {
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
    }
}
