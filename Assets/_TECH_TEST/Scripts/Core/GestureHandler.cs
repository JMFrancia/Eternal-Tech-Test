using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GestureEventData
{
    public delegate void GestureEnded(GestureEventData dat);
    public static event GestureEnded onEnd;

    public InputHandler input;

    public InputEventData[] touches = new InputEventData[] { };

    float spread0, spread1;
    Vector3 center0, center1;
    Vector3 delta0, delta1;

    public float spreadDelta = 0f;
    public Vector3 centerDelta = Vector3.zero;
    public Vector3 deltaDelta = Vector3.zero;

    public float lifetime;

    bool touching = false;

    public int touchCount
    {
        get
        {
            if (touches == null)
                return 0;

            return touches.Length;
        }
    }

    public Vector3 center
    {
        get
        {
            if (touches == null || touches.Length == 0)
                return Vector3.zero;

            Vector3 average = Vector3.zero;
            foreach (InputEventData touch in touches)
                average += touch.position;

            average /= touchCount;
            return average;
        }
    }

    public float spread
    {
        get
        {
            if (touches == null || touches.Length == 0)
                return 0f;

            Vector3 _center = center;
            float averagedist = 0f;
            foreach (InputEventData touch in touches)
                averagedist += Vector3.Distance(_center, touch.position);

            averagedist /= touchCount;
            return averagedist;
        }
    }


    public Vector3 minDelta
    {
        get
        {
            if (touches == null || touches.Length == 0)
                return Vector3.zero;

            Vector3 min = touches[0].delta;
            foreach (InputEventData touch in touches)
            {
                if (touch.delta.magnitude < min.magnitude)
                    min = touch.delta;
            }

            return min;
        }
    }

    public Vector3 maxDelta
    {
        get
        {
            if (touches == null || touches.Length == 0)
                return Vector3.zero;

            Vector3 max = touches[0].delta;
            foreach (InputEventData touch in touches)
            {
                if (touch.delta.magnitude > max.magnitude)
                    max = touch.delta;
            }

            return max;
        }
    }

    public Vector3 delta
    {
        get
        {
            if (touches == null || touches.Length == 0)
                return Vector3.zero;

            Vector3 averagedelta = Vector3.zero;
            foreach (InputEventData touch in touches)
                averagedelta += touch.delta;

            averagedelta /= touchCount;
            return averagedelta;
        }
    }

    public float angle
    {
        get
        {
            if (touches == null || touches.Length == 0)
                return 0f;

            float averageangle = 0f;
            foreach (InputEventData touch in touches)
                averageangle += touch.angle;

            averageangle /= touchCount;
            return averageangle;
        }
    }

    public void Update()
    {
        if (touches == null || touches.Length == 0)
        {
            if (touching)
            {
                touching = false;
            }
            return;
        }
        if (!touching)
        {
            touching = true;
            EventManager.TriggerEvent("StartTouch");
        }

        for (int i = 0; i < touches.Length; i++)
            UpdateTouch(i);


        // Update changes of core values per frame
        center1 = center;
        centerDelta = (center1 - center0);
        center0 = center1;


        spread1 = spread;
        spreadDelta = (spread1 - spread0);
        spread0 = spread1;


        delta1 = delta;
        deltaDelta = (delta1 - delta0);
        delta0 = delta;

        lifetime += Time.deltaTime;
    }

    void SetTouch(int index)
    {
        var touch = touches[index];

        // Mobile input
#if !UNITY_EDITOR && (UNITY_IOS || UNITY_ANDROID)

                var t = Input.GetTouch(index);

                touch.position = t.position;
                touch.delta = Vector3.zero;

            // Editor/desktop input
#else

        touch.position = input.Position;
        touch.delta = input.Delta;

#endif
    }

    void UpdateTouch(int index)
    {

        var touch = touches[index];

        // Mobile input
#if !UNITY_EDITOR && (UNITY_IOS || UNITY_ANDROID)

                var t = Input.GetTouch(index);

                touch.position = t.position;
                touch.delta = t.deltaPosition;

                if(t.phase == TouchPhase.Began)
                    touch.state = InputEventData.State.Began;

                else if(t.phase == TouchPhase.Stationary || t.phase == TouchPhase.Moved)
                    touch.state = InputEventData.State.Continue;

                else if(t.phase == TouchPhase.Ended || t.phase == TouchPhase.Canceled)
                    touch.state = InputEventData.State.Cancel;

            // Editor/desktop input
#else

        string id = touch.id;

        if (id == "pinch_anchor")
        {
            if (Input.GetKey(KeyCode.Z))
                touch.state = InputEventData.State.Continue;
            else if (Input.GetKeyUp(KeyCode.Z))
                touch.state = InputEventData.State.Cancel;
        }

        else if (id == "swipe_anchor")
        {
            touch.position = input.Position;
            touch.delta = input.Delta;

            if (Input.GetKey(KeyCode.X))
                touch.state = InputEventData.State.Continue;
            else if (Input.GetKeyUp(KeyCode.X))
                touch.state = InputEventData.State.Cancel;
        }

        else if (id == "touch")
        {
            touch.position = input.Position;
            touch.delta = input.Delta;

            if (Input.GetMouseButton(0))
                touch.state = InputEventData.State.Continue;
            else if (Input.GetMouseButtonUp(0))
                touch.state = InputEventData.State.Cancel;
        }

#endif


        touch.Update(Time.deltaTime); // Increment time touch has been active
    }

    public GestureEventData(InputHandler input) { this.input = input; touches = new InputEventData[] { }; }
    public GestureEventData(InputHandler input, InputEventData[] touches)
    {
        this.input = input;
        this.touches = touches; // Assign all input data to gesture

        if (touches.Length > 0)
        {
            for (int i = 0; i < touches.Length; i++)
                SetTouch(i);
        }

        // Initialize starting values
        center0 = center1 = center;
        delta0 = delta1 = delta;
        spread0 = spread1 = spread;
    }
}

public class GestureHandler : Singleton<GestureHandler>
{
    InputHandler input;

    [SerializeField]
    GestureEventData gesture;



    int touches_0 = 0, touches_1 = 0;



    float tapThreshold = .167f;
    float touchLifetime = 0f;
    int consecutiveTaps = 0;
    public int tapMoveThreshold = 87;

    // All measure in points (iOS)

    int minimumTouchesForSwipe = 1;
    int maximumTouchesForSwipe = 1;

    int maximumSpreadForSwipe = 128; // px
    public int swipeThreshold = 300; // pt


    int minimumTouchesForPinch = 2;
    int maximumTouchesForPinch = 2;
    public int pinchThreshold = 1; // px


    public InputEventData[] touches { get { return (gesture == null) ? null : gesture.touches; } }
    public int numberOfTouches { get { return (gesture == null) ? 0 : gesture.touchCount; } }
    public Vector3 center { get { return (gesture == null) ? Vector3.zero : gesture.center; } }
    public Vector3 delta { get { return (gesture == null) ? Vector3.zero : gesture.delta; } }
    public float spread { get { return (gesture == null) ? 0f : gesture.spread; } }


    private void Start()
    {
        input = InputHandler.Instance;
    }

    void Update()
    {
        float dt = Time.deltaTime;
        var touches = 0;

#if UNITY_EDITOR

        if (Input.GetKey(KeyCode.Z) || Input.GetKeyUp(KeyCode.Z)) // Pinch anchor
            ++touches;
        else if (Input.GetKey(KeyCode.X) || Input.GetKeyUp(KeyCode.X)) // Swipe anchor
            ++touches;

        if (Input.GetMouseButton(0) || Input.GetMouseButtonUp(0)) // Last touch
            ++touches;

#else
                touches = Input.touchCount;
#endif

        touches_1 = touches;
        if (touches > 0)
        {
            touchLifetime += Time.deltaTime;
        }
        if (touches_1 != touches_0)
        {
            if (touches > 0)
            {
                var activeTouches = FetchTouches();
                if (gesture == null)
                    gesture = new GestureEventData(input, activeTouches);
                else
                {
                    gesture.input = input;
                    gesture.touches = activeTouches;
                }
            }
            else
            {
                ClearTouches();
                EventManager.TriggerEvent("ReleaseTouch", touchLifetime);
                touchLifetime = 0f;
            }
        }
        touches_0 = touches_1;


        if (gesture != null)
            gesture.Update(); // Update active gesture

        if (GetTap())
        {
            EventManager.TriggerEvent("Tap");
        }
    }

    InputEventData[] FetchTouches()
    {
        List<InputEventData> events = new List<InputEventData>();

#if UNITY_EDITOR

        if (Input.GetKey(KeyCode.Z))
            events.Add(new InputEventData("pinch_anchor", input.Position));
        else if (Input.GetKey(KeyCode.X))
            events.Add(new InputEventData("swipe_anchor", input.Position));

        if (Input.GetMouseButton(0))
            events.Add(new InputEventData("touch", input.Position));

#else

                int touches = Input.touchCount;
                Touch touch;

                for(int i = 0; i < touches; i++){
                    touch = Input.GetTouch(i);
                    events.Add(new InputEventData("touch_" + i, touch.position));
                }

#endif

        return events.ToArray();
    }

    void ClearTouches()
    {
        gesture = null;
    }

    #region Tap

    public bool GetTap(int touchId = 0)
    {
        if (numberOfTouches > touchId)
        {
            InputEventData touch = gesture.touches[touchId];

            float sp = Screening.PixelsToPoints(touch.delta.magnitude) / Time.deltaTime;

            if (touch.state == InputEventData.State.Cancel && touch.timer <= tapThreshold && sp < tapMoveThreshold)
                return true;
        }

        return false;
    }

    public bool GetGlobalHold()
    {
        if (gesture != null)
            return gesture.lifetime > tapThreshold;

        return false;
    }

    public bool GetHold(int touchId = 0)
    {
        if (numberOfTouches > touchId)
        {
            InputEventData touch = gesture.touches[touchId];
            if (touch.state == InputEventData.State.Continue && touch.timer > tapThreshold)
                return true;
        }

        return false;
    }
    public bool GetHoldAnyFinger()
    {
        for (int i = 0; i < numberOfTouches; i++)
        {
            if (GetHold(i)) return true;
        }
        return false;
    }

    public Vector3 Tap
    {
        get
        {
            if (GetTap())
                return center;

            return Vector3.zero;
        }
    }

    public Vector3 Hold
    {
        get
        {
            if (GetHold())
                return center;

            return Vector3.zero;
        }
    }

    #endregion

    #region Drag

    public bool GetDrag(int numTouches = 1)
    {
        return (numberOfTouches == numTouches);
    }

    public Vector3 GetDragStrength(int numTouches = 1)
    {
        Vector3 drag = Vector3.zero;

        if (GetDrag(numTouches))
        {
            foreach (InputEventData touch in gesture.touches)
                drag += touch.delta;
            drag /= numberOfTouches;
        }

        return drag;
    }


    #endregion

    #region Swipe
    public bool GetAnySwipe()
    {
        return (GetSwipeVertical() || GetSwipeHorizontal());
    }

    public float GetVerticalSwipe()
    {
        if (isSwipeReady)
        {
            float dy_pt = Screening.PixelsToPoints(delta.y) / Time.deltaTime;
            float ang_offset = Mathf.Sin(gesture.angle);

            if (spread <= maximumSpreadForSwipe && Mathf.Abs(ang_offset) >= Mathf.Sin(Mathf.PI / 4f))
                return dy_pt;
        }

        return 0f;
    }

    public bool GetSwipeVertical()
    {
        return Mathf.Abs(GetVerticalSwipe()) >= swipeThreshold;
    }

    public bool GetSwipeVerticalUp()
    {
        return GetVerticalSwipe() >= swipeThreshold;
    }

    public bool GetSwipeVerticalDown()
    {
        return -GetVerticalSwipe() >= swipeThreshold;
    }

    public float GetHorizontalSwipe()
    {

        if (isSwipeReady)
        {
            float dx_pt = Screening.PixelsToPoints(delta.x) / Time.deltaTime;
            float ang_offset = Mathf.Sin(gesture.angle);

            if (spread <= maximumSpreadForSwipe && Mathf.Abs(ang_offset) < Mathf.Sin(Mathf.PI / 4f))
                return dx_pt;
        }
        return 0f;
    }

    public bool GetSwipeHorizontal()
    {
        return Mathf.Abs(GetHorizontalSwipe()) >= swipeThreshold;
    }

    public bool GetSwipeHorizontalRight()
    {
        return GetHorizontalSwipe() >= swipeThreshold;
    }

    public bool GetSwipeHorizontalLeft()
    {
        return -GetHorizontalSwipe() >= swipeThreshold;
    }


    public bool isSwipeReady
    {
        get
        {
            if (gesture == null) return false;
            float magnitude = Screening.PixelsToPoints(gesture.minDelta.magnitude) / Time.deltaTime;
            return (numberOfTouches >= minimumTouchesForSwipe && numberOfTouches <= maximumTouchesForSwipe) && (magnitude >= swipeThreshold);
        }
    }

    public Vector3 Swipe
    {
        get
        {
            if (GetAnySwipe())
            {
                float dt = Time.deltaTime;
                return new Vector3(Screening.PixelsToPoints(delta.x) / dt, Screening.PixelsToPoints(delta.y) / dt, 0f);
            }

            return Vector3.zero;
        }
    }

    public float SwipeVertical
    {
        get
        {
            if (GetSwipeVertical())
            {
                return Screening.PixelsToPoints(delta.y) / Time.deltaTime;
            }

            return 0f;
        }
    }

    public float SwipeHorizontal
    {
        get
        {
            if (GetSwipeHorizontal())
            {
                return Screening.PixelsToPoints(delta.x) / Time.deltaTime;
            }

            return 0f;
        }
    }

    #endregion


    #region Tap



    #endregion

    #region Pinch

    public bool GetPinch()
    {

        if (isPinchReady)
            return (Mathf.Abs(gesture.spreadDelta) >= pinchThreshold);

        return false;
    }

    public bool isPinchReady
    {
        get
        {
            return (numberOfTouches >= minimumTouchesForPinch && numberOfTouches <= maximumTouchesForPinch);
        }
    }

    public float Pinch
    {
        get
        {
            if (GetPinch())
                return gesture.spreadDelta;

            return 0f;
        }
    }

    #endregion


}
