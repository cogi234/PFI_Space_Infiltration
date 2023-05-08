using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretController : MonoBehaviour
{
    // Vision turret
    [SerializeField] float fieldOfViewXZ;
    [SerializeField] float fieldOfViewY;
    bool PlayerVisible = false;

    // Rotation turret
    [SerializeField] Vector3 rotation;
    [SerializeField] float TimeToRotate;
    Quaternion targetRotation;
    Quaternion initialRotation;
    float rotationDuration = 0f;

    Transform barrel;
    Transform player;
    Transform sphere;

    // turret Attacks
    [SerializeField] float shootInterval;
    float shootTimer = 0f;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        var transforms = GetComponentsInChildren<Transform>();
        foreach (var i in transforms)
        {
            if (i.gameObject.name == "Barrel")
                barrel = i;
            if (i.gameObject.name == "Sphere")
                sphere = i;
        }
        initialRotation = transform.rotation;
        targetRotation = Quaternion.Euler(rotation);
    }

    void Update()
    {
        rotationDuration += Time.deltaTime;

        if (rotationDuration < TimeToRotate)
        {
            float t = rotationDuration / TimeToRotate;
            sphere.rotation = Quaternion.Lerp(initialRotation, targetRotation, t);
        }
        else
        {
            sphere.rotation = targetRotation;
            rotationDuration = 0f;
            var temp = initialRotation;
            initialRotation = targetRotation;
            targetRotation = temp;
        }

    }
}
