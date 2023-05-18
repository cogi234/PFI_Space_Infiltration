using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject deathMessage;
    [SerializeField] GameObject victoryMessage;
    [SerializeField] GameObject startMessage;
    [Header("Movement")]
    [SerializeField] float walkingSpeed;
    [SerializeField] float walkingHeight;
    [SerializeField] float floorRotationSpeed;
    [SerializeField] float mouseSensibility;

    [Header("Raycasts")]
    [SerializeField] float raycastDistance;
    [SerializeField] Vector3[] raycastPositions;
    [SerializeField] Vector3[] raycastDirections;
    [SerializeField] float sideToFloorNormalRatio = 0.25f;

    [Header("Jumping")]
    [SerializeField] float jumpSpeed;
    [SerializeField] float jumpNoStickTime = 1f;
    float jumpTimer = 0;

    Vector3 velocity;
    public Vector3 Velocity { get => velocity; }

    Rigidbody rb;
    CameraController cam;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        cam = GetComponentInChildren<CameraController>();

        StartCoroutine(StartMessageCoroutine());
    }

    private void Update()
    {
        //Si le collider est kinematique, c'est qu'on est entrain de marcher sur quelque chose.
        //Sinon, on est entrain de flotter dans le vide
        if (rb.isKinematic)
        {
            if (Input.GetButtonDown("Jump"))
            {
                Jump();
            }
        }

        if (!pauseMenu.activeInHierarchy && Input.GetButtonDown("Cancel"))
            pauseMenu.SetActive(true);
    }

    private void FixedUpdate()
    {
        //On tourne selon la souris
        transform.Rotate(new Vector3(0, Input.GetAxis("Mouse X") * mouseSensibility * Time.fixedDeltaTime), Space.Self);

        if (jumpTimer > 0)
            jumpTimer -= Time.fixedDeltaTime;

        //Si le collider est kinematique, c'est qu'on est entrain de marcher sur quelque chose.
        //Sinon, on est entrain de flotter dans le vide
        if (rb.isKinematic)
        {

            //On change notre vitesse selon les touches de mouvement
            velocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized * walkingSpeed;

            //On bouge selon notre vitesse
            transform.Translate(velocity * Time.fixedDeltaTime, Space.Self);


            //On utilise un SphereCast pour s'orienter selon notre plancher
            RaycastHit hit;
            if (Physics.SphereCast(transform.position, 0.25f, -transform.up, out hit, walkingHeight * 2, LayerMask.GetMask("Default")))
            {
                Vector3 normal = hit.normal;
                Vector3 sideNormals = Vector3.zero;
                //On va utiliser des raycasts vers les cotes pour tourner vers les murs
                for (int i = 0; i < raycastPositions.Length; i++)
                {
                    RaycastHit sideHit;
                    if (Physics.Raycast((Vector3)(transform.localToWorldMatrix * raycastPositions[i]) + transform.position, transform.localToWorldMatrix * raycastDirections[i], out sideHit, raycastDistance * 5, ~LayerMask.GetMask("Player")))
                    {
                        if (sideHit.distance < raycastDistance)
                        {
                            //Si on touche quelque chose dans cette direction
                            sideNormals += sideHit.normal * ((raycastDistance - sideHit.distance) / raycastDistance);
                        }
                    }
                }
                //On devie la normale un peu vers les cotes touches
                normal += sideNormals.normalized * sideToFloorNormalRatio;
                normal = normal.normalized;


                //On tourne pour rester les pieds sur le sol
                Quaternion targetRotation = Quaternion.LookRotation(Vector3.Cross(transform.right, normal), normal);
                //J'utilise Lerp pour garder le changement lisse
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, floorRotationSpeed * Time.fixedDeltaTime);

                //Si la difference entre la hauteur desiree et la hauteur en ce moment est trop haute ou basse, on ajuste
                if (Mathf.Abs(walkingHeight - hit.distance) > 0.01f)
                    transform.Translate(Vector3.up * (walkingHeight - hit.distance));
                
            } else
            {
                //Si on ne trouve pas de plancher, on flotte dans le vide
                rb.isKinematic = false;
                rb.velocity = transform.localToWorldMatrix * velocity;
                velocity = Vector3.zero;
                cam.takeInput = false;
            }
        } else
        {
            //Si on flotte, on veut pouvoir tourner notre perso dans les directions qu'on veut
            transform.Rotate(new Vector3(Input.GetAxis("Mouse Y") * mouseSensibility * Time.fixedDeltaTime, 0), Space.Self);

            //On essaie de detecter le sol juste si on a attendu assez
            if (jumpTimer <= 0)
            {
                //On utilise un SphereCast pour voir si on est de retour sur une surface
                RaycastHit hit;
                if (Physics.SphereCast(transform.position, 0.25f, -transform.up, out hit, walkingHeight, ~LayerMask.GetMask("Player")))
                {
                    //On se remet en mode marcher
                    rb.isKinematic = true;
                    cam.takeInput = true;

                    //On change notre vitesse selon les touches de mouvement
                    velocity = new Vector3(Input.GetAxis("Horizontal") * walkingSpeed, 0, Input.GetAxis("Vertical") * walkingSpeed).normalized;

                    //On bouge selon notre vitesse
                    transform.Translate(velocity * Time.fixedDeltaTime, Space.Self);

                    //On tourne pour rester les pieds sur le sol
                    Quaternion targetRotation = Quaternion.LookRotation(Vector3.Cross(transform.right, hit.normal), hit.normal);
                    //J'utilise Lerp pour garder le changement lisse
                    transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 0.5f);

                    //Si la difference entre la hauteur desiree et la hauteur en ce moment est trop haute ou basse, on ajuste
                    if (Mathf.Abs(walkingHeight - hit.distance) > 0.01f)
                        transform.Translate(Vector3.up * (walkingHeight - hit.distance));
                }
            }
        }
    }

    private void Jump()
    {
        //On se met en mode flotter
        rb.isKinematic = false;
        rb.velocity = transform.localToWorldMatrix * (velocity + new Vector3(0, jumpSpeed));
        velocity = Vector3.zero;
        cam.takeInput = false;
        jumpTimer = jumpNoStickTime;
    }

    public void OnDeath()
    {
        Time.timeScale = 0;
        StartCoroutine(DeathCoroutine());
    }

    IEnumerator DeathCoroutine()
    {
        deathMessage.SetActive(true);
        yield return new WaitForSecondsRealtime(5);
        SceneManager.LoadScene("Level1");
    }

    public void Victory()
    {
        Time.timeScale = 0;
        StartCoroutine(VictoryCoroutine());
    }

    IEnumerator VictoryCoroutine()
    {
        victoryMessage.SetActive(true);
        yield return new WaitForSecondsRealtime(5);
        SceneManager.LoadScene("MainMenu");
    }

    IEnumerator StartMessageCoroutine()
    {
        startMessage.SetActive(true);
        yield return new WaitForSecondsRealtime(5);
        startMessage.SetActive(false);
    }
}
