using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class IK_Leg : MonoBehaviour
{
    //Les informations necessaires pour chaque joint
    [Header("Joints")]
    [SerializeField] Transform hipSwing;
    [SerializeField] float hipSwingRestAngle;
    HingeJoint hipSwingArticulation;
    [SerializeField] Transform hipElevation;
    [SerializeField] float hipElevationRestAngle;
    HingeJoint hipElevationArticulation;
    [SerializeField] Transform knee;
    [SerializeField] float kneeRestAngle;
    HingeJoint kneeArticulation;
    [SerializeField] Transform ankle;
    [Header("Other parameters")]
    //Le point de contact avec le sol
    [SerializeField] Transform endPoint;
    //Le joueur
    [SerializeField] PlayerController player;
    //Le temps pendant lequel la jambe reste au repos avant de reessayer de trouver une surface
    [SerializeField] float restTime = 1;
    float restTimer = 0;

    //Les longueurs de la jambe
    float length1, length2, length3, totalLength;

    //Vers quel point veut-on placer le pieds
    Vector3 target;
    Vector3 targetNormal;

    bool tryingToReachTarget;


    /// <summary>
    /// On enregistre les longueurs des parties de la jambe et de la jambe au complet
    /// </summary>
    private void Awake()
    {
        //Aller chercher les references
        hipElevationArticulation = hipElevation.GetComponent<HingeJoint>();
        hipSwingArticulation = hipSwing.GetComponent<HingeJoint>();
        kneeArticulation = knee.GetComponent<HingeJoint>();

        //Calculer les longueurs de la jambe
        length1 = Vector3.Distance(hipElevation.position, knee.position);
        length2 = Vector3.Distance(knee.position, ankle.position);
        length3 = Vector3.Distance(ankle.position, endPoint.position);
        totalLength = length1 + length2;

        //On fait disparaitre le curseur
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        //On met les jambes en mode repos
        GoToRest();
    }

    private void Update()
    {
        if (tryingToReachTarget)
        {
            //On essaie d'atteindre la cible
            if (!CalculateAngles())
            {
                //Si la jambe n'est plus capable d'atteindre la cible, on met la jambe dans sa position de repos et on arrete d'essayer de l'atteindre
                GoToRest();
            }
        } else if (restTimer <= 0)
        {
            FindTarget(player.transform.localToWorldMatrix * player.Velocity);
        } else
        {
            restTimer -= Time.deltaTime;
        }
        
    }

    /// <summary>
    /// On calcule les angles des articulations pour atteindre la cible
    /// </summary>
    /// <returns>Est-ce que l'objectif est atteignable</returns>
    public bool CalculateAngles()
    {
        //On transforme la position de la cible en position locale a la jambe
        Vector3 localTarget = transform.worldToLocalMatrix * (target - transform.position);
        
        //Hip Swing
        float hipSwingAngle = -Mathf.Rad2Deg * Mathf.Atan2(localTarget.z, localTarget.x) + 90;
        if (hipSwingAngle > 180)
            hipSwingAngle -= 360;
        //Si l'angle cible est en dehors des limites de la hanche, on retourne false
        if (hipSwingAngle <= hipSwingArticulation.limits.max && hipSwingAngle >= hipSwingArticulation.limits.min)
        {
            JointSpring temp = hipSwingArticulation.spring;
            temp.targetPosition = hipSwingAngle;
            hipSwingArticulation.spring = temp;
        }
        else
            return false;
        

        //2D kinematics
        //On tourne la cible pour qu'elle soit devant nous, ce qui nous permet de juste prendre compte des coordonnees y et z. On peut donc utiliser les equations 2D.
        Vector3 virtualTarget;
        {
            float distance = localTarget.magnitude;
            if (distance > length1 + length2)
                return false;
            Vector3 temp = localTarget;
            temp.x = 0;
            virtualTarget = temp.normalized * distance;
        }
        //Pythagore (Voir https://www.alanzucconi.com/2018/05/02/ik-2d-1/ pour les equations sur lesquelles je me suis baser)
        float a = length2;
        float b = virtualTarget.magnitude;
        float c = length1;
        float alpha = Mathf.Acos(((b * b) + (c * c) - (a * a)) / (2 * b * c)) * Mathf.Rad2Deg;
        float beta = Mathf.Acos(((a * a) + (c * c) - (b * b)) / (2 * a * c)) * Mathf.Rad2Deg;
        float Aprime = Mathf.Atan(virtualTarget.y / virtualTarget.z) * Mathf.Rad2Deg;
        float A = alpha + Aprime;
        float B = 180 - beta;
        //Si il y a quelque chose d'invalide dans les resultats, on retourne false
        if (float.IsNaN(alpha) || float.IsNaN(beta) || float.IsNaN(A) || float.IsNaN(Aprime) || float.IsNaN(B))
            return false;

        //Hip Elevation
        float hipElevationAngle = A;
        //Si l'angle cible est en dehors des limites de la hanche, on retourne false
        if (hipElevationAngle <= hipElevationArticulation.limits.max && hipElevationAngle >= hipElevationArticulation.limits.min)
        {
            JointSpring temp = hipElevationArticulation.spring;
            temp.targetPosition = hipElevationAngle;
            hipElevationArticulation.spring = temp;
        }
        else
            return false;

        //Knee
        float kneeAngle = B;
        //Si l'angle cible est en dehors des limites du genoux, on retourne false
        if (kneeAngle <= kneeArticulation.limits.max && kneeAngle >= kneeArticulation.limits.min)
        {
            JointSpring temp = kneeArticulation.spring;
            temp.targetPosition = kneeAngle;
            kneeArticulation.spring = temp;
        }
        else
            return false;

        //Ankle
        ankle.rotation = Quaternion.LookRotation(-targetNormal, transform.up);


        return true;
    }


    [Header("Raycasts")]
    //A quels angles est-ce que les raycast peuvent etre faits
    [SerializeField] Vector2 raycastDefaultAngle;
    [SerializeField] Vector2 raycastMinAngle;
    [SerializeField] Vector2 raycastMaxAngle;
    //Combien d'essais de raycasts va-t-on faire avant d'abandonner. Plus ce nombre est haut, plus la performance est affectee
    [SerializeField] int numRaycastTries = 1;
    [SerializeField] float logisticGrowthRate;

    /// <summary>
    /// On utilise des raycasts pour trouver le prochain point ou la patte peut marcher
    /// </summary>
    public void FindTarget(Vector3 parentMovement)
    {
        //On met le mouvement du parent en terme du point de reference local
        Vector3 localParentMovement = (transform.worldToLocalMatrix * parentMovement);

        //Selon la direction du mouvement, on modifie un peu l'angle du raycast, pour bouger les pattes vers le devant lorsqu'on avance, par exemple.
        Vector2 angleOffset = new Vector2();
        //J'utilise la fonction logistique pour changer la vitesse en offset d'angle   https://en.wikipedia.org/wiki/Logistic_function
        //Pour le x
        if (localParentMovement.z > 0)
        {
            float L = (raycastMinAngle.x - raycastDefaultAngle.x) * 2;
            angleOffset.x = ((1 / (1 + Mathf.Exp(-logisticGrowthRate * localParentMovement.z))) - 0.5f) * L;
        } else if (localParentMovement.z < 0)
        {
            float L = (raycastMaxAngle.x - raycastDefaultAngle.x) * 2;
            angleOffset.x = ((1 / (1 + Mathf.Exp(-logisticGrowthRate * -localParentMovement.z))) - 0.5f) * L;
        }
        //Pour le y
        if (localParentMovement.x > 0)
        {
            float L = (raycastMaxAngle.y - raycastDefaultAngle.y) * 2;
            angleOffset.y = ((1 / (1 + Mathf.Exp(-logisticGrowthRate * localParentMovement.x))) - 0.5f) * L;
        } else if (localParentMovement.x < 0)
        {
            float L = (raycastMinAngle.y - raycastDefaultAngle.y) * 2;
            angleOffset.y = ((1 / (1 + Mathf.Exp(-logisticGrowthRate * -localParentMovement.x))) - 0.5f) * L;
        }

        //On raycast un certain nombre de fois pour trouver un endroit ou mettre le pieds
        for (int i = 0; i < numRaycastTries; i++)
        {
            Vector2 raycastAngle = Vector2.Lerp(raycastDefaultAngle + angleOffset, raycastDefaultAngle, (float)i / (float)(numRaycastTries - 1));
            //On convertit les angles en un vecteur de direction, a utiliser pour le raycast.
            Vector3 raycastWorldDirection = transform.localToWorldMatrix * (Quaternion.Euler(raycastAngle) * Vector3.forward);

            //Lancer le raycast (On evite la layer Player)
            RaycastHit hit;
            if (Physics.Raycast(transform.position, raycastWorldDirection, out hit, totalLength, ~LayerMask.GetMask("Player")))
            {
                //Si le raycast s'est rendu, on choisi cette cible. Le point de la cible est le point ou la cheville va.
                //On enregistre aussi ensuite la normale de la surface, pour pouvoir orienter le pieds.
                targetNormal = hit.normal;
                target = hit.point + hit.normal * length3;
                tryingToReachTarget = true;
                //Si on a trouver un endroit ou mettre le pieds, on arrete le loop et on retourne.
                return;
            }
        }
    }

    /// <summary>
    /// Dire a la jambe a aller a sa position de repos
    /// </summary>
    public void GoToRest()
    {
        tryingToReachTarget = false;
        restTimer = restTime;
        //Hip swing
        {
            JointSpring temp = hipSwingArticulation.spring;
            temp.targetPosition = hipSwingRestAngle;
            hipSwingArticulation.spring = temp;
        }
        //Hip elevation
        {
            JointSpring temp = hipElevationArticulation.spring;
            temp.targetPosition = hipElevationRestAngle;
            hipElevationArticulation.spring = temp;
        }
        //Knee
        {
            JointSpring temp = kneeArticulation.spring;
            temp.targetPosition = kneeRestAngle;
            kneeArticulation.spring = temp;
        }
        //Ankle
        ankle.localEulerAngles = Vector3.zero;
    }
}
