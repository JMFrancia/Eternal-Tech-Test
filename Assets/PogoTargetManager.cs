using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PogoTargetManager : MonoBehaviour
{

    public static PogoTarget ActiveTarget { get; private set; }

    public List<PogoTarget> targets;
    CenterOfMass world;
    
    private void Awake()
    {
        world = GameObject.FindGameObjectWithTag("World").GetComponent<CenterOfMass>();
        targets = new List<PogoTarget>(GetComponentsInChildren<PogoTarget>());
        targets.Sort(CompareTargetHeight);
        ActiveTarget = targets[0];
        for (int n = 1; n < targets.Count; n++) {
            targets[n].gameObject.SetActive(false);
        }
    }

    public void NextTarget() {
        targets.RemoveAt(0);
        if (targets.Count > 0)
        {
            ActiveTarget = targets[0];
            ActiveTarget.gameObject.SetActive(true);
        }
    }

    int CompareTargetHeight(PogoTarget a, PogoTarget b) {
    {
            float heightA = (a.transform.position - world.transform.position).magnitude;
            float heightB = (b.transform.position - world.transform.position).magnitude;
            return heightA.CompareTo(heightB);
    }
}
}
