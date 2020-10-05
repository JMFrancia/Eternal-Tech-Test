using System;
using System.Collections;
using UnityEngine;


/*
 * Manages the cut scene when you hit the highest bounce
 */
public class GodCutsceneManager : MonoBehaviour
{
    [SerializeField] GameObject fakePlayer;
    [SerializeField] CameraManager cameraManager;
       
    [Header("Entry scene")]
    [SerializeField] Camera entryCam;
    [SerializeField] Transform entryPosition;
    [SerializeField] ParticleSystem entryPS;
    [SerializeField] float entryShakeDist = .5f;
    [SerializeField] int shakeFreq = 3;
    [SerializeField] float entrySequenceTime = 2f;

    [Header("God scene")]
    [SerializeField] Camera godCam;
    [SerializeField] ParticleSystem haloPS;
    [SerializeField] Transform cloudParent;
    [SerializeField] Transform godStartingPos;
    [SerializeField] Transform godFinalPos;

    [Header("Sounds")]
    [SerializeField] AudioClip ambientWindSound;
    [SerializeField] AudioClip wooshingWindSound;
    [SerializeField] AudioClip godSound;

    AudioSource mainSrc;
    AudioSource ambientSrc;

    Action callback;

    int shakeCounter = 0;
    bool shakeEffect = true;
    Vector3 originalCloudPos;

    private void Awake()
    {
        AudioSource[] audioSources = GetComponents<AudioSource>();
        mainSrc = audioSources[0];
        ambientSrc = audioSources[1];
        ambientSrc.clip = ambientWindSound;
        ambientSrc.loop = true;
        originalCloudPos = cloudParent.localPosition;
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
        haloPS.transform.Rotate(0f, 0f, -.25f);
    }

    public void BeginCutscene(Action callback) {
        StartCoroutine(PlayCutscene());
        this.callback = callback;
    }

    IEnumerator PlayCutscene()
    {
        //Entry sequence
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

        //God sequence
        fakePlayer.transform.position = godStartingPos.position;
        LeanTween.moveLocalZ(cloudParent.gameObject, 223f, 3f);
        LTSeq sequence = LeanTween.sequence();
        sequence.append(LeanTween.move(fakePlayer, godFinalPos, 1.5f).setEase(LeanTweenType.easeOutCirc));
        sequence.append(LeanTween.move(fakePlayer, godStartingPos, 1.5f).setEase(LeanTweenType.easeInCirc).setOnComplete(EndSequence));   
    }

    void EndSequence() {
        mainSrc.Stop();
        ambientSrc.Stop();
        cloudParent.localPosition = originalCloudPos;
        callback.Invoke();
    }
}
