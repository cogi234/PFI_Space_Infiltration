using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    [SerializeField] Rigidbody associatedRigidbody;
    [SerializeField] Transform endPoint;
    AudioSource sfx;
    ParticleSystem muzzleFlash;
    [SerializeField] float maxRaycastDistance = 200f;
    [SerializeField] float shootForce;
    [SerializeField] int bulletDamage;
    /// <summary>
    /// Le nombre de degres d'imprecision
    /// </summary>
    [SerializeField] float inaccuracy;
    [SerializeField] float shootInterval = 1f;
    float shootTimer = 0;

    ObjectPool bulletPool;
    Camera cam;

    private void Awake()
    {
        cam = Camera.main;
        bulletPool = GameObject.Find("BulletPool").GetComponent<ObjectPool>();
        muzzleFlash = endPoint.GetComponent<ParticleSystem>();
        sfx = endPoint.GetComponent<AudioSource>();
    }

    private void Update()
    {
        //On fait un raycast pour voir si la camera pointe vers un point precis et qu'on est pas en mode viser. On ignore player pour pas tirer sur nous meme ou sur nos balles
        RaycastHit hit;
        if (!Input.GetButton("Aim") && Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, maxRaycastDistance, ~LayerMask.GetMask("Player")))
        {
            //Si oui, on oriente le gun vers ce point
            transform.rotation = Quaternion.LookRotation(hit.point - transform.position, transform.up) * Quaternion.Euler(90,0,0);

        } else
        {
            //Si non, on oriente le gun dans la meme direction que la camera
            transform.rotation = cam.transform.rotation * Quaternion.Euler(90, 0, 0);
        }

        //Pour faire tirer le joueur a une certaine vitesse
        if (shootTimer > 0)
        {
            shootTimer -= Time.deltaTime;
        } else if (Input.GetButton("Shoot"))
        {
            
            shootTimer += shootInterval;
            Shoot();
        }
    }

    public void Shoot()
    {
        sfx.Play();
        muzzleFlash.Play();
        //Je prends une balle
        GameObject bullet = bulletPool.GetElement();
        //Je lui met la layer Player
        bullet.layer = 6;
        bullet.SetActive(true);
        //Je la met au bon endroit
        bullet.transform.position = endPoint.position;
        //Je lui met le dommage desirer
        bullet.GetComponent<BulletComponent>().damage = bulletDamage;
        //Je la propulse
        Quaternion innacuracyRotation = Quaternion.Euler(Random.Range(-0.5f * inaccuracy, 0.5f * inaccuracy), Random.Range(-0.5f * inaccuracy, 0.5f * inaccuracy), Random.Range(-0.5f * inaccuracy, 0.5f * inaccuracy));
        Vector3 forceDirection = innacuracyRotation * transform.up;
        bullet.GetComponent<Rigidbody>().velocity = Vector3.zero;
        bullet.GetComponent<Rigidbody>().AddForce(forceDirection * shootForce, ForceMode.Impulse);
        //Je met une force inverse sur le joueur
        associatedRigidbody.AddForce(-forceDirection * shootForce, ForceMode.Impulse);

    }
}
