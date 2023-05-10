using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    [SerializeField] Transform leftDoor, rightDoor;
    [SerializeField] Vector3 leftMovement, rightMovement;
    [SerializeField] float speed = 1;
    [SerializeField] float openRadius;

    public float openness = 0;
    Vector3 leftClosed, leftOpen, rightClosed, rightOpen;

    private void Awake()
    {
        leftClosed = leftDoor.localPosition;
        rightClosed = rightDoor.localPosition;

        leftOpen = leftClosed + leftMovement;
        rightOpen = rightClosed + rightMovement;
    }

    private void Update()
    {
        //Si le joueur est assez proche, on ouvre
        if (Physics.CheckSphere(transform.position, openRadius, LayerMask.GetMask("Player")))
            openness = Mathf.Clamp(openness + (Time.deltaTime * speed), 0, 1);
        else //Sinon, on ferme
            openness = Mathf.Clamp(openness - (Time.deltaTime * speed), 0, 1);

        //On applique ca sur les portes
        leftDoor.localPosition = Vector3.Lerp(leftClosed, leftOpen, openness);
        rightDoor.localPosition = Vector3.Lerp(rightClosed, rightOpen, openness);
    }
}
