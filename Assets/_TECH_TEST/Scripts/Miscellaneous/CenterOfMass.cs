using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CenterOfMass : MonoBehaviour
{
    public float gravPullMax, gravPullMin = 0;
    public float distForPullMax, distForPullMin;

    public Transform @base;
    Collider baseCollider;
    GameObject spawner = null;

    void Start()
    {
        spawner = new GameObject("spawner");
        spawner.transform.parent = transform;
    }

    public float radius
    {
        get
        {
            if (baseCollider == null)
                baseCollider = @base.GetComponent<Collider>();

            return baseCollider.bounds.extents.x;
        }
    }

    public float GetGravPull(Vector3 pos)
    {
        float dist = Vector3.Distance(pos, transform.position);

        dist = dist.RemapNRB(distForPullMax * transform.localScale.magnitude, distForPullMin * transform.localScale.magnitude, 1f, 0f);

        return EasingFunction.EaseInQuad(gravPullMin, gravPullMax, dist);
    }

    public void GetPositionOnWorld(Vector3 origin, float ang, float heightAngle, LayerMask mask, out Vector3 position, out Vector3 normal)
    {
        bool self = (origin == transform.position);

        Vector3 dir = (origin - transform.position);
        float dist = dir.magnitude;

        float rad = Mathf.Max(radius, dist);

        var x = rad * Mathf.Cos(ang) * Mathf.Sin(heightAngle);
        var y = rad * (Mathf.Cos(heightAngle) - ( self? 0f:1f ));
        var z = rad * Mathf.Sin(ang) * Mathf.Sin(heightAngle);

        /* * * * * * * * * * */

        if (!self)
        {
            spawner.transform.position = origin;
            spawner.transform.up = dir.normalized;
        }
        else
        {
            spawner.transform.position = transform.position;
            spawner.transform.rotation = transform.rotation;
        }

        var ray_origin = spawner.transform.TransformPoint(x, y, z);
        var ray_dir = (transform.position - ray_origin).normalized;

        var ray = new Ray(ray_origin, ray_dir);
        var hit = new RaycastHit();

        if (Physics.Raycast(ray, out hit, rad * 2f, mask.value))
        {
            position = hit.point;
            normal = hit.normal;
        }
        else
            throw new System.Exception("Unable to find point on world!");
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green - new Color(0, 0, 0, 0.5f);
        Gizmos.DrawSphere(transform.position, distForPullMax * transform.localScale.magnitude);
        Gizmos.color -= new Color(0, 0, 0, 0.3f);
        Gizmos.DrawSphere(transform.position, distForPullMin * transform.localScale.magnitude);
    }
}
