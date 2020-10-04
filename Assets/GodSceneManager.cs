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

    [Header("Sounds")]
    [SerializeField] AudioClip ambientWindSound;
    [SerializeField] AudioClip wooshingWindSound;
    [SerializeField] AudioClip godSound;

    AudioSource mainSrc;
    AudioSource ambientSrc;

    Action callback;

    int shakeCounter = 0;
    bool shakeEffect = true;

    private void Awake()
    {
        AudioSource[] audioSources = GetComponents<AudioSource>();
        mainSrc = audioSources[0];
        ambientSrc = audioSources[1];
        ambientSrc.clip = ambientWindSound;
        ambientSrc.loop = true;
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
        mainSrc.PlayOneShot(wooshingWindSound);
        ambientSrc.Play();
        cameraManager.Set(entryCam);
        yield return new WaitForSeconds(entrySequenceTime);
        mainSrc.Stop();
        mainSrc.PlayOneShot(godSound);
        cameraManager.Set(godCam);
        shakeEffect = false;
        entryPS.Stop();

        fakePlayer.transform.position = godStartingPos.position;
        LTSeq sequence = LeanTween.sequence();
        sequence.append(LeanTween.move(fakePlayer, godFinalPos, 1.5f).setEase(LeanTweenType.easeOutCirc));
        sequence.append(LeanTween.move(fakePlayer, godStartingPos, 1.5f).setEase(LeanTweenType.easeInCirc).setOnComplete(EndSequence));   
    }

    void EndSequence() {
        mainSrc.Stop();
        ambientSrc.Stop();
        callback.Invoke();
        Debug.Log("Sequence complete");
    }
}
