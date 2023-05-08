using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public bool takeInput = true;

    [SerializeField] Transform targetTransform;
    [SerializeField] Transform gunTransform;
    [SerializeField] float mouseSensibility;
    [SerializeField] float idealDistance;
    [SerializeField] float pitchOffset;
    [SerializeField] float minAngle;
    [SerializeField] float maxAngle;
    [SerializeField] float distanceFromCollision;
    [SerializeField] float defaultAngle;

    float angleFromTarget = 25;
    LayerMask layerMask;

    Vector3 targetPosition;
    Quaternion targetRotation;

    private void Awake()
    {
        layerMask = ~LayerMask.GetMask("Player");
    }

    void LateUpdate()
    {
        if (Input.GetButtonDown("Aim"))
            angleFromTarget = 0;
        if (Input.GetButton("Aim"))
        {
            //Quand on vise on se met en premiere personne
            targetPosition = gunTransform.position;
            angleFromTarget += mouseSensibility * -Input.GetAxis("Mouse Y") * Time.deltaTime;

            if (!takeInput)
                angleFromTarget = defaultAngle;

            Quaternion rotation = Quaternion.Euler(angleFromTarget, 0, 0);
            Vector3 raycastWorldDirection = targetTransform.localToWorldMatrix * (rotation * -Vector3.forward);
            targetRotation = Quaternion.LookRotation(-raycastWorldDirection, transform.up) * Quaternion.Euler(pitchOffset, 0, 0);

        } else
        {
            //En mode camera troisieme personne
            angleFromTarget += mouseSensibility * -Input.GetAxis("Mouse Y") * Time.deltaTime;
            angleFromTarget = Mathf.Clamp(angleFromTarget, minAngle, maxAngle);

            if (!takeInput)
                angleFromTarget = defaultAngle;

            Quaternion rotation = Quaternion.Euler(angleFromTarget, 0, 0);
            Vector3 raycastWorldDirection = targetTransform.localToWorldMatrix * (rotation * -Vector3.forward);

            RaycastHit hit;
            if (Physics.Raycast(targetTransform.position, raycastWorldDirection, out hit, idealDistance, layerMask))
            {
                targetPosition = hit.point - raycastWorldDirection.normalized * distanceFromCollision;
            }
            else
            {
                targetPosition = targetTransform.position + (raycastWorldDirection.normalized * idealDistance);
            }

            targetRotation = Quaternion.LookRotation(-raycastWorldDirection, transform.up) * Quaternion.Euler(pitchOffset, 0, 0);
        }


        //On bouge la camera doucement vers la position et rotation necessaire
        transform.position = Vector3.Lerp(transform.position, targetPosition, 0.75f);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 0.75f);
    }
}
