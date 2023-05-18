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

    [SerializeField] GameObject sparkPrefab;

    private void OnCollisionEnter(Collision collision)
    {
        //On essaie de prendre le HealthComponent de l'autre objet pour l'endommager
        HealthComponent health;
        if (collision.gameObject.TryGetComponent<HealthComponent>(out health))
            health.TakeDamage(damage);
        //On fait apparaitre une etincelle
        GameObject.Instantiate(sparkPrefab, collision.contacts[0].point, Quaternion.LookRotation(collision.contacts[0].normal, transform.up));
        //On desactive la balle
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        gameObject.SetActive(false);
    }
}
