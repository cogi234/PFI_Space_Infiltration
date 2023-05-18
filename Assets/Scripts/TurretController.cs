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
    bool wasVisible = false;
    Quaternion rotation = Quaternion.identity;
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
    [SerializeField] float rotationSpeed;
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
            // Vise le joueur
            var rotation = Quaternion.LookRotation(player.transform.position - sphere.position);
            sphere.rotation = Quaternion.RotateTowards(sphere.rotation, rotation, rotationSpeed * Time.deltaTime);
        }
        else if (wasVisible)
        {
            // replace l'orientation vers la rotation si la tourrel n'aurait pas détecté le joueur
            if (rotation == Quaternion.identity)
            {
                rotation = sphere.localRotation;
                elapsedTime = 0f;
            }
            elapsedTime += Time.deltaTime;
            if (elapsedTime < timeToRotate)
            {
                sphere.localRotation = Quaternion.Slerp(rotation, Quaternion.Euler(futurRotation), elapsedTime / timeToRotate);
            }
            else
            {
                var temp = futurRotation;
                futurRotation = sphere.localEulerAngles - targetRotation;
                sphere.localEulerAngles = temp;
                targetRotation = -targetRotation;
                rotation = Quaternion.identity;
                elapsedTime = 0f;
                wasVisible = false;
            }
        }
        else
        {
            // Le mouvement normal de la tourrel
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
    // la couroutine et la fonction fieldOFViewCheck sert a changer la variable isVisible
    // isVisible est vrai si le joueur est visible
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
                {
                    if (IsVisible)
                        wasVisible = true;
                    IsVisible = false;
                }
                
            }
            else
            {
                if (IsVisible)
                    wasVisible = true;
                IsVisible = false;
            }
        }
        else if (IsVisible)
        {
            IsVisible = false;
            wasVisible = true;
        }
    }
}
