using System;
using UnityEngine;

/*
 * Manages pogo targets, setting their heights and controlling which one is active
 */
public class PogoTargetManager : MonoBehaviour
{
    public static PogoTargetManager Instance { get; private set; }

    PogoTarget activeTarget;
    PogoTarget[] targets;

    float baseHeight = 8f;
    int targetCounter = 0;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else {
            Destroy(this);
        }
    }

    public void NextTarget() {
        activeTarget.gameObject.SetActive(false);
        activeTarget = targets[++targetCounter % targets.Length];
        activeTarget.gameObject.SetActive(true);
    }

    public void SetActive(bool active) {
        //Lazy load used to solve race condition. Not ideal, but good enough for now
        if (targets == null) {
            Initialize();
        }

        Debug.Log("Set Active called: " + active);

        if (active)
        {
            Debug.Log("Activating " + targets[0]);
            activeTarget = targets[0];
            activeTarget.gameObject.SetActive(true);
        }
        else if(activeTarget != null) { 
            activeTarget.gameObject.SetActive(false);
        }
    }

    void Initialize()
    {
        targets = GetComponentsInChildren<PogoTarget>();

        Vector3 worldCenter = GameObject.FindGameObjectWithTag("World").GetComponent<CenterOfMass>().transform.position;
        for (int n = 0; n < targets.Length; n++)
        {
            targets[n].transform.position += (targets[n].transform.position - worldCenter).normalized * (baseHeight * Mathf.Pow(1.6f, n + 2));
            targets[n].gameObject.SetActive(false);
        }
    }
}