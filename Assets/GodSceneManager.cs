using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GodSceneManager : MonoBehaviour
{
    [SerializeField] GameObject fakePlayer;
    [SerializeField] Transform godStartingPos;
    [SerializeField] Transform godFinalPos;
    [SerializeField] Camera godCam;

    [Header("Entry scene")]
    [SerializeField] Camera entryCam;
    [SerializeField] Transform entryPosition;
    [SerializeField] float entryShakeDist = .5f;
    [SerializeField] int shakeFreq = 3;

    int shakeCounter = 0;
    bool shakeEffect = true;

    private void Update()
    {
        fakePlayer.transform.position = entryPosition.position;
        if (shakeEffect && ++shakeCounter % shakeFreq == 0)
        {
            fakePlayer.transform.position += Random.insideUnitSphere * entryShakeDist;
        }
    }

}
