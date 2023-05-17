using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretController : MonoBehaviour
{
    [Header("Vision")]
    [SerializeField] float radius;
    [SerializeField] float angle;
    [HideInInspector] public bool IsVisible;
    [SerializeField] LayerMask obstructionMask;
    [SerializeField] LayerMask targetMask;

    // Rotation turret
    [Header("Movement")]
    [SerializeField] Vector3 targetRotation;
    [SerializeField] float timeToRotate;
    float elapsedTime = 0f;
    Vector3 futurRotation;

    Transform barrel;
    GameObject player;
    Transform sphere;

    // turret Attacks
    [Header("Shooting")]
    [SerializeField] float shootInterval;
    float shootTimer = 0f;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        obstructionMask = LayerMask.GetMask("Default");
        var transforms = GetComponentsInChildren<Transform>();
        foreach (var i in transforms)
        {
            if (i.gameObject.name == "Barrel")
                barrel = i;
            if (i.gameObject.name == "Sphere")
                sphere = i;
        }
        futurRotation = targetRotation;
        StartCoroutine(FOV());
    }
    void Update()
    {
        if (IsVisible)
        {

        }
        else
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime < timeToRotate)
            {
                sphere.Rotate(targetRotation * (Time.deltaTime / timeToRotate));
            }
            else
            {
                var temp = futurRotation;
                futurRotation = sphere.localEulerAngles - targetRotation;
                sphere.localEulerAngles = temp;
                targetRotation = -targetRotation;
                elapsedTime = 0f;
            }
        }

    }
    // Source: https://www.youtube.com/watch?v=j1-OyLo77ss
    IEnumerator FOV()
    {
        WaitForSeconds wait = new WaitForSeconds(0.2f);
        while (true)
        {
            yield return wait;

            FieldOfViewCheck();
        }
    }
    void FieldOfViewCheck()
    {
        Collider[] rangeChecks = Physics.OverlapSphere(sphere.position, radius, targetMask);

        if (rangeChecks.Length != 0)
        {
            Transform target = rangeChecks[0].transform;
            Vector3 directionToTarget = (target.position - sphere.position).normalized;

            if (Vector3.Angle(sphere.forward, directionToTarget) < angle / 2)
            {
                float distanceToTarget = Vector3.Distance(sphere.position, player.transform.position);

                if (!Physics.Raycast(sphere.position, directionToTarget, distanceToTarget, obstructionMask))
                    IsVisible = true;
                else
                    IsVisible = false;
            }
            else
            {
                IsVisible = false;
            }
        }
        else if (IsVisible)
            IsVisible = false;
    }
}
