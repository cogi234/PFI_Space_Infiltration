using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IK_Leg : MonoBehaviour
{
    [Header("Joints")]
    public Transform hip;
    public Transform knee;
    public Transform ankle;
    public Transform foot;
    [Header("Target")]
    public Vector3 target;
    public Transform targetTransform;

    private float length1, length2, length3, totalLength;
    private bool attached = false;
    private bool tryingToReachTarget = true;


    /// <summary>
    /// On enregistre les longueurs des parties de la jambe et de la jambe au complet
    /// </summary>
    private void Awake()
    {
        length1 = Vector3.Distance(hip.position, knee.position);
        length2 = Vector3.Distance(knee.position, ankle.position);
        length3 = Vector3.Distance(ankle.position, foot.position);
        totalLength = length1 + length2 + length3;
    }

    private void Update()
    {
        target = targetTransform.position;
        CalculateAngles();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns>Est-ce que l'objectif est atteignable</returns>
    public bool CalculateAngles()
    {
        Vector3 localTarget = transform.worldToLocalMatrix * (target - transform.position);
        float hipSwingAngle = (90 - Mathf.Rad2Deg * Mathf.Atan2(localTarget.z - hip.localPosition.z, localTarget.x - hip.localPosition.x));
        //Debug.Log($"Local target: {localTarget}\nAngle: {hipSwingAngle}");


        return true;
    }
}
