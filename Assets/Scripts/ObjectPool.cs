using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    /// <summary>
    /// L'objet qui est contenu par la pool
    /// </summary>
    [SerializeField] private GameObject poolable;
    /// <summary>
    /// La liste des objets dans la pool
    /// </summary>
    private List<GameObject> pool = new List<GameObject>();


    public GameObject GetElement()
    {
        //Si on ne trouve pas d'objet disponible a donner, on en cree un nouveau
        GameObject gameObject = pool.Find(obj => obj.activeInHierarchy == false);
        if (gameObject == null)
        {
            gameObject = Instantiate(poolable, transform);
            pool.Add(gameObject);
        }
        return gameObject;
    }
}
