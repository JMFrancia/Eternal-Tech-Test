using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Abstract animation class for objects, interfaces with attached animator controller
/// </summary>

public abstract class AnimationHandler : MonoBehaviour
{

    [SerializeField] protected Animator animator;
    public Animator Animator {
        get{ return animator; }
        set{ 
            bool events_flag = (animator != value);

            animator = value;
            if (events_flag)
                RegisterAnimationEvents();
        }
    }

    [System.Serializable]
    public struct CustomAnimationEvent {
        public string clip;
        public float time;
        public string param;
        public string function;
    }

    [SerializeField] List<CustomAnimationEvent> events = new List<CustomAnimationEvent>();


    protected virtual void Awake()
    {
        if(animator == null)
            animator = GetComponent<Animator>();
    }

    // Resets all animator parameters to default state
    public void Reset(){
        if(animator == null) return;
        animator.SetTrigger("reset");
    }

    #region Getters and setters for animator parameters

    public virtual bool GetBool(string b) { if(animator == null) return false; return animator.GetBool(b); }
    public virtual void SetBool(string b){ if(animator == null) return; animator.SetBool(b, true); }
    public virtual void SetBool(string b, bool value){ if(animator == null) return; animator.SetBool(b, value); }
    public virtual void ResetBool(string b) { if(animator == null) return; animator.SetBool(b, false); }

    public virtual int GetInt(string i) { if(animator == null) return -1; return animator.GetInteger(i); }
    public virtual void SetInt(string i) { if(animator == null) return; animator.SetInteger(i, 1); }
    public virtual void SetInt(string i, int value){ if(animator == null) return; animator.SetInteger(i, value); }
    public virtual void ResetInt(string i){ if(animator == null) return; animator.SetInteger(i, -1); }

    public virtual void SetTrigger(string t){ if(animator == null) return; animator.SetTrigger(t); }
    public virtual void ResetTrigger(string t){ if(animator == null) return; animator.ResetTrigger(t); }

    #endregion

    // Checks if animation is currently active
    public bool IsAnimationActive(string animation) { 
        if(animator == null)
            return false;

        return animator.GetCurrentAnimatorStateInfo(0).IsName(animation);
    }

    #region Animation events

    public virtual void ReceiveAnimationEvent(string e){
        // Do nothing
    }

    public void RegisterAnimationEvents(){
        foreach(CustomAnimationEvent e in events){
            foreach(AnimationClip clip in animator.runtimeAnimatorController.animationClips){
                if(clip.name == e.clip){
                    AnimationEvent ae = new AnimationEvent();
                                   ae.time = e.time;
                                   ae.stringParameter = e.param;
                                   ae.functionName = e.function;

                                   clip.AddEvent(ae);

                    break;
                }
            }
        }
    }

    #endregion

    //Sets animator speed 
    public float Speed
    {
        get
        {
            if(animator == null)
                return 0f;

            return animator.speed;
        }

        set
        {
            if(animator == null)
                return;

            animator.speed = value;
        }
    }

    
}
