using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ObjectPooling : MonoBehaviour
{
    [SerializeField] LayerMask poolObjects;
    [SerializeField] float poolRadius;
    List<GameObject> closedObjects;
    List<GameObject> openObjects;
    Collider[] nearbyColliders, farColliders;
    [SerializeField] GameObject player;

    private void Awake()
    {
        closedObjects = new List<GameObject>();
        openObjects = new List<GameObject>();

        foreach (GameObject obj in Object.FindObjectsOfType(typeof(GameObject)))
        {
            if (obj.layer == 10)
            {
                closedObjects.Add(obj);
                obj.SetActive(false);
            }
        }

       
       
    }

    private void Start()
    {
        CheckForStaticMeshes();
    }

    private void Update()
    {
        CheckForStaticMeshes();
    }

    void CheckForStaticMeshes()
    {
        nearbyColliders = Physics.OverlapSphere(player.transform.position, poolRadius, poolObjects);
        farColliders = Physics.OverlapSphere(player.transform.position, poolRadius + 5, poolObjects);

        foreach (Collider col in nearbyColliders)
        {

            if (closedObjects.Contains(col.gameObject))
                closedObjects.Remove(col.gameObject); 

            if (!openObjects.Contains(col.gameObject))
            {
                openObjects.Add(col.gameObject);
                col.gameObject.SetActive(true);
            }          
        }

        foreach (Collider col in farColliders)
        {
            if (nearbyColliders.Contains(col))
                return;

            if (closedObjects.Contains(col.gameObject))
                return;

            openObjects.Remove(col.gameObject);
            closedObjects.Add(col.gameObject);
            col.gameObject.SetActive(true);




        }
    }
}
