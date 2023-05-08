using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    [SerializeField] Rigidbody associatedRigidbody;
    [SerializeField] float maxRaycastDistance = 200f;
    [SerializeField] float shootForce;

    Camera camera;

    private void Awake()
    {
        camera = Camera.main;
    }

    private void Update()
    {
        //On fait un raycast pour voir si la camera pointe vers un point precis. On ignore player pour pas tirer sur nous meme ou sur nos balles
        RaycastHit hit;
        if (Physics.Raycast(camera.transform.position, camera.transform.forward, out hit, maxRaycastDistance, ~LayerMask.GetMask("Player")))
        {
            //Si oui, on oriente le gun vers ce point
            transform.rotation = Quaternion.LookRotation(hit.point - transform.position, transform.up) * Quaternion.Euler(90,0,0);

        } else
        {
            //Si non, on oriente le gun dans la meme direction que la camera
            transform.rotation = camera.transform.rotation * Quaternion.Euler(90, 0, 0);
        }
    }
}
