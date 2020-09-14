using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

using UnityEngine.UI;

public class MovementJoystick : FloatingJoystick
{
    PointerEventData sim = new PointerEventData(EventSystem.current);

    bool m_ready = true;
    public bool ready
    {
        get
        {
            return m_ready;
        }
        set
        {
            m_ready = value;
        }
    }

    float maxRadius = 1f;
    float maxThreshold = .5f;

    public float MaxThreshold
    {
        get { return maxThreshold; }
        set { maxThreshold = value; }
    }

    [SerializeField] AnimationCurve strengthCurve;

    public bool reachedExtents
    {
        get
        {
            return (Direction.magnitude) >= maxRadius;
        }
    }

    public override Vector2 Radius
    {
        get
        {
            return (background.sizeDelta / 2) * maxRadius;
        }
    }

    public override float Strength
    {
        get
        {
            return strengthCurve.Evaluate(ClampedDirection.magnitude);
        }
    }

    public enum State { Inactive, Inner, InnerAndOuter, Outer };
    public State m_state = State.Inactive;

    public int state
    {
        get
        {
            if (m_active)
            {
                float mag = Direction.magnitude + .001f;

                if (mag < maxThreshold)
                    return 1;
                //else if (mag < maxRadius)
                //return 2;

                return 2;
            }
            else
                return 0;
        }
    }

    bool m_active = false;
    public bool active
    {
        get
        {
            return m_active;
        }
        set
        {
            if (m_active != value)
            {
                if (value)
                {
                    PointerEventData dat = ConstructPointerEventFromInput();
                    base.OnPointerDown(dat);
                }
                else
                    base.OnPointerUp(null);
            }
            m_active = value;
        }
    }

    public RectTransform threshold;

    [System.Serializable]
    public struct LookState
    {
        public Sprite[] states;

        [HideInInspector]
        public Image image;

        public void Evaluate(int state)
        {
            state -= 1;

            if (state < 0 || state > (states.Length - 1))
                return;

            var spr = states[state];

            image.enabled = (spr != null);
            image.sprite = spr;
        }
    }
    public LookState backgroundState;
    public LookState handleState;
    public LookState thresholdState;


    protected override void HandleInput(float magnitude, Vector2 normalised, Vector2 radius, Camera cam, float extents = 1f)
    {
        base.HandleInput(magnitude, normalised, radius, cam, maxRadius);
    }

    protected override void Start()
    {
        base.Start();

        thresholdState.image = threshold.GetComponent<Image>();
        backgroundState.image = background.GetComponent<Image>();
        handleState.image = handle.GetComponent<Image>();
    }



    void Update()
    {

        sim.position = _Input.Position;
        active = HandleActive();// (Gestures.GetHold() && ready);



        threshold.localScale = Vector3.one * maxThreshold; // Update threshold size

        int st = state;
        if (st > 0)
        {
            backgroundState.Evaluate(st);
            thresholdState.Evaluate(st);
            handleState.Evaluate(st);
        }

        m_state = (State)st;

        if (active)
            OnDrag(sim);
    }

    bool HandleActive()
    {
        if (!ready)
            return false;

        if (!active)
        {
            if (Gestures.numberOfTouches > 1)
                return false;
        }

        return Gestures.GetGlobalHold();
    }


    PointerEventData ConstructPointerEventFromInput()
    {
        PointerEventData dat = new PointerEventData(EventSystem.current);
        dat.position = _Input.Position;

        return dat;
    }
}