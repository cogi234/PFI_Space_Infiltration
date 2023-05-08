using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    [SerializeField] float timer;
    [SerializeField] bool deactivate;
    float actualTimer;

    private void OnEnable()
    {
        actualTimer = timer;
    }

    void Update()
    {
        actualTimer -= Time.deltaTime;
        if (actualTimer <= 0)
        {
            if (deactivate)
                gameObject.SetActive(false);
            else
                Destroy(gameObject);
        }
    }
}
