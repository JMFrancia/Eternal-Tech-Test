using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PogoTarget : MonoBehaviour
{
    [SerializeField] float rotationSpeed = 1f;

    PogoTargetManager manager;
    CenterOfMass world;

    private void Awake()
    {
        manager = GetComponentInParent<PogoTargetManager>();
        world = GameObject.FindGameObjectWithTag("World").GetComponent<CenterOfMass>();
        transform.LookAt((world.transform.position - transform.position));
        transform.Rotate(90f, 0f, 0f);
    }

    private void Update()
    {
        transform.Rotate(0f, rotationSpeed, 0f);
    }

    private void OnDestroy()
    {
        manager.NextTarget();
    }
}
