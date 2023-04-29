using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class IK_Leg : MonoBehaviour
{
    //Les informations necessaires pour chaque joint
    [Header("Joints")]
    public Transform hipSwing;
    public float hipSwingRestAngle;
    private ArticulationBody hipSwingArticulation;
    public Transform hipElevation;
    public float hipElevationRestAngle;
    private ArticulationBody hipElevationArticulation;
    public Transform knee;
    public float kneeRestAngle;
    private ArticulationBody kneeArticulation;
    public Transform ankle;
    public float ankleRestAngle;
    private ArticulationBody ankleArticulation;
    //Le point de contact avec le sol
    public Transform endPoint;
    //Les longueurs de la jambe
    private float length1, length2, length3, totalLength;

    //Vers quel point veut-on placer le pieds
    [SerializeField] Vector3 target;
    Vector3 targetNormal;

    [SerializeField]private bool tryingToReachTarget;


    /// <summary>
    /// On enregistre les longueurs des parties de la jambe et de la jambe au complet
    /// </summary>
    private void Awake()
    {
        //Aller chercher les references
        hipElevationArticulation = hipElevation.GetComponent<ArticulationBody>();
        hipSwingArticulation = hipSwing.GetComponent<ArticulationBody>();
        kneeArticulation = knee.GetComponent<ArticulationBody>();
        ankleArticulation = ankle.GetComponent<ArticulationBody>();

        //Calculer les longueurs de la jambe
        length1 = Vector3.Distance(hipElevation.position, knee.position);
        length2 = Vector3.Distance(knee.position, ankle.position);
        length3 = Vector3.Distance(ankle.position, endPoint.position);
        totalLength = length1 + length2;

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
                tryingToReachTarget = false;
                GoToRest();
            }
        } else
        {
            GoToRest();
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
        if (hipSwingAngle <= hipSwingArticulation.xDrive.upperLimit && hipSwingAngle >= hipSwingArticulation.xDrive.lowerLimit)
            hipSwingArticulation.SetDriveTarget(ArticulationDriveAxis.X, hipSwingAngle);
        else
            return false;
        

        //2D kinematics
        //On tourne la cible pour qu'elle soit devant nous, ce qui nous permet de juste prendre compte des coordonnees y et z. On peut donc utiliser les equations 2D.
        Vector3 virtualTarget;
        {
            float distance = localTarget.magnitude;
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
        //Debug.Log($"a:{a}, b:{b}, c:{c}\nalpha:{alpha}, beta:{beta}, Aprime:{Aprime}\nVirtual target:{virtualTarget}\nA:{A}, B:{B}");
        //Si il y a quelque chose d'invalide dans les resultats, on retourne false
        if (float.IsNaN(alpha) || float.IsNaN(beta) || float.IsNaN(A) || float.IsNaN(Aprime) || float.IsNaN(B))
            return false;

        //Hip Elevation
        float hipElevationAngle = A;
        //Si l'angle cible est en dehors des limites de la hanche, on retourne false
        if (hipElevationAngle <= hipElevationArticulation.yDrive.upperLimit && hipElevationAngle >= hipElevationArticulation.yDrive.lowerLimit)
            hipElevationArticulation.SetDriveTarget(ArticulationDriveAxis.Y, hipElevationAngle);
        else
            return false;

        //Knee
        float kneeAngle = -B;
        //Si l'angle cible est en dehors des limites du genoux, on retourne false
        if (kneeAngle <= kneeArticulation.yDrive.upperLimit && kneeAngle >= kneeArticulation.yDrive.lowerLimit)
            kneeArticulation.SetDriveTarget(ArticulationDriveAxis.Y, kneeAngle);
        else
            return false;

        return true;
    }

    /// <summary>
    /// Dire a la jambe a aller a sa position de repos
    /// </summary>
    private void GoToRest()
    {
        hipSwingArticulation.SetDriveTarget(ArticulationDriveAxis.X, hipSwingRestAngle);
        hipElevationArticulation.SetDriveTarget(ArticulationDriveAxis.Y, hipElevationRestAngle);
        kneeArticulation.SetDriveTarget(ArticulationDriveAxis.Y, kneeRestAngle);
        ankleArticulation.SetDriveTarget(ArticulationDriveAxis.Y, ankleRestAngle);
        ankleArticulation.SetDriveTarget(ArticulationDriveAxis.Z, ankleRestAngle);
    }
}
