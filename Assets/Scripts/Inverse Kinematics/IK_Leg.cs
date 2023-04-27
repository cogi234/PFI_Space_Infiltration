using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IK_Leg : MonoBehaviour
{
    [Header("Joints")]
    public IK_Joint hipElevation;
    public IK_Joint hipSwing;
    public IK_Joint knee;
    public IK_Joint ankle;
    public Transform foot;
    [Header("Target")]
    public Vector3 target;

    private float length1, length2, length3, totalLength;


    /// <summary>
    /// On enregistre les longueurs des parties de la jambe et de la jambe au complet
    /// </summary>
    private void Awake()
    {
        length1 = Vector3.Distance(hipElevation.transform.position, knee.transform.position);
        length2 = Vector3.Distance(knee.transform.position, ankle.transform.position);
        length3 = Vector3.Distance(ankle.transform.position, foot.position);
        totalLength = length1 + length2 + length3;
    }
}
