using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float walkingSpeed;
    [SerializeField] float walkingHeight;

    Vector3 velocity;
    public Vector3 Velocity { get => velocity; }

    Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        //Si le collider est kinematique, c'est qu'on est entrain de marcher sur quelque chose.
        //Sinon, on est entrain de flotter dans le vide
        if (rb.isKinematic)
        {
        } else
        {

        }
    }

    private void FixedUpdate()
    {
        //Si le collider est kinematique, c'est qu'on est entrain de marcher sur quelque chose.
        //Sinon, on est entrain de flotter dans le vide
        if (rb.isKinematic)
        {
            //On change notre vitesse selon les touches de mouvement
            velocity = new Vector3(Input.GetAxis("Horizontal") * walkingSpeed, 0, Input.GetAxis("Vertical") * walkingSpeed).normalized;

            //On bouge selon notre vitesse
            transform.Translate(velocity * Time.fixedDeltaTime, Space.Self);

            //On utilise un SphereCast pour s'orienter selon notre plancher
            RaycastHit hit;
            if (Physics.SphereCast(transform.position, 0.25f, -transform.up, out hit, walkingHeight * 2, ~LayerMask.GetMask("Player")))
            {
                //Si la difference entre la hauteur desiree et la hauteur en ce moment est trop haute ou basse, on ajuste
                if (Mathf.Abs(walkingHeight - hit.distance) > 0.01f)
                    transform.Translate(Vector3.up * (walkingHeight - hit.distance));

                transform.rotation = Quaternion.LookRotation(transform.forward, hit.normal);
                Debug.Log(hit.distance);

            } else
            {
                //Si on ne trouve pas de plancher, on flotte dans le vide
                rb.isKinematic = false;
                rb.velocity = velocity;
                velocity = Vector3.zero;
            }
        } else
        {

        }
    }

}
