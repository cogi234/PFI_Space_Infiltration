using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MassCalculator : MonoBehaviour
{
    [SerializeField] float density = 1;
    [SerializeField] Shape shape;
    [SerializeField] Rigidbody rb;

    public void CalculateMass()
    {
        float scale = transform.lossyScale.y * transform.lossyScale.x * transform.lossyScale.z;
        float mult = scale * density;

        switch (shape)
        {
            case Shape.Sphere:
                {
                    rb.mass = mult * ((4 / 3) * Mathf.PI / Mathf.Pow(0.5f, 3));
                    break;
                }
            case Shape.Cube:
                {
                    rb.mass = mult;
                    break;
                }
            case Shape.Capsule:
                {
                    rb.mass = mult * (Mathf.PI * Mathf.Pow(0.5f, 2) * (2 - (2 / 3) * 0.5f));
                    break;
                }
            case Shape.Cylinder:
                {
                    rb.mass = mult * (Mathf.PI * Mathf.Pow(0.5f, 2) * 2);
                    break;
                }
            default:
                break;
        }
    }

    public enum Shape { Sphere, Cube, Capsule, Cylinder}
}
