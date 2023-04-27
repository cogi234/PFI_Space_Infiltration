using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IK_Joint : MonoBehaviour
{
    public Axis rotationAxis;
    public float minAngle;
    public float maxAngle;
    public float maxRotationSpeed;
    public float targetAngle;

    private float initialAngle;


    /// <summary>
    /// On enregistre la position initiale et on la garde dans les limites
    /// </summary>
    private void Awake()
    {
        Vector3 initialRotation = transform.localEulerAngles;

        switch (rotationAxis)
        {
            case Axis.X:
                {
                    initialAngle = transform.localRotation.eulerAngles.x;
                    if (initialAngle > maxAngle)
                    {
                        initialAngle = maxAngle;
                        initialRotation.x = maxAngle;
                    } else if (initialAngle < minAngle)
                    {
                        initialAngle = minAngle;
                        initialRotation.x = minAngle;
                    }
                    break;
                }
            case Axis.Y:
                {
                    initialAngle = transform.localRotation.eulerAngles.y;
                    if (initialAngle > maxAngle)
                    {
                        initialAngle = maxAngle;
                        initialRotation.y = maxAngle;
                    }
                    else if (initialAngle < minAngle)
                    {
                        initialAngle = minAngle;
                        initialRotation.y = minAngle;
                    }
                    break;
                }
            case Axis.Z:
                {
                    initialAngle = transform.localRotation.eulerAngles.z;
                    if (initialAngle > maxAngle)
                    {
                        initialAngle = maxAngle;
                        initialRotation.z = maxAngle;
                    }
                    else if (initialAngle < minAngle)
                    {
                        initialAngle = minAngle;
                        initialRotation.z = minAngle;
                    }
                    break;
                }
        }
        targetAngle = initialAngle;

        transform.localEulerAngles = initialRotation;
    }

    /// <summary>
    /// Bouger le joint vers sa rotation desiree
    /// </summary>
    private void Update()
    {
        Vector3 currentEuler = transform.localEulerAngles;

        switch (rotationAxis)
        {
            case Axis.X:
                {
                    float currentAngle = currentEuler.x;
                    if (currentAngle != targetAngle)
                    {
                        float rotationAmount = Mathf.Min(maxRotationSpeed * Time.deltaTime, Mathf.Abs(currentAngle - targetAngle)) * Mathf.Sign(targetAngle - currentAngle);
                        currentEuler.x += rotationAmount;
                        transform.localEulerAngles = currentEuler;
                    }
                    break;
                }
            case Axis.Y:
                {
                    float currentAngle = currentEuler.y;
                    if (currentAngle != targetAngle)
                    {
                        float rotationAmount = Mathf.Min(maxRotationSpeed * Time.deltaTime, Mathf.Abs(currentAngle - targetAngle)) * Mathf.Sign(targetAngle - currentAngle);
                        currentEuler.y += rotationAmount;
                        transform.localEulerAngles = currentEuler;
                    }
                    break;
                }
            case Axis.Z:
                {
                    float currentAngle = currentEuler.z;
                    if (currentAngle != targetAngle)
                    {
                        float rotationAmount = Mathf.Min(maxRotationSpeed * Time.deltaTime, Mathf.Abs(currentAngle - targetAngle)) * Mathf.Sign(targetAngle - currentAngle);
                        currentEuler.z += rotationAmount;
                        transform.localEulerAngles = currentEuler;
                    }
                    break;
                }
        }

    }


    public enum Axis { X, Y, Z }
}
