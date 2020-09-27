using System;
using System.Collections;
using UnityEngine;

public class GodSceneManager : MonoBehaviour
{
    [SerializeField] GameObject fakePlayer;
    [SerializeField] Transform godStartingPos;
    [SerializeField] Transform godFinalPos;
    [SerializeField] CameraManager cameraManager;
    [SerializeField] Camera godCam;

    [Header("Entry scene")]
    [SerializeField] Camera entryCam;
    [SerializeField] Transform entryPosition;
    [SerializeField] ParticleSystem entryPS;
    [SerializeField] float entryShakeDist = .5f;
    [SerializeField] int shakeFreq = 3;
    [SerializeField] float entrySequenceTime = 2f;

    Action callback;

    int shakeCounter = 0;
    bool shakeEffect = true;

    private void Start()
    {
        //BeginSequence();
    }

    private void Update()
    {
        if (shakeEffect)
        {
            fakePlayer.transform.position = entryPosition.position;
            if (++shakeCounter % shakeFreq == 0)
            {
                fakePlayer.transform.position += UnityEngine.Random.insideUnitSphere * entryShakeDist;
            }
        }
    }

    public void BeginSequence(Action callback) {
        StartCoroutine(PlaySequence());
        this.callback = callback;
    }

    IEnumerator PlaySequence()
    {
        shakeEffect = true;
        entryPS.Play();
        cameraManager.Set(entryCam);
        yield return new WaitForSeconds(entrySequenceTime);
        cameraManager.Set(godCam);
        shakeEffect = false;
        entryPS.Stop();

        fakePlayer.transform.position = godStartingPos.position;
        LTSeq sequence = LeanTween.sequence();
        sequence.append(LeanTween.move(fakePlayer, godFinalPos, 1.5f).setEase(LeanTweenType.easeOutCirc));
        sequence.append(LeanTween.move(fakePlayer, godStartingPos, 1.5f).setEase(LeanTweenType.easeInCirc).setOnComplete(EndSequence));   
    }

    void EndSequence() {
        callback.Invoke();
        Debug.Log("Sequence complete");
    }
}
