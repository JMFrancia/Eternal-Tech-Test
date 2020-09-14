using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class FollowCamera : GameCamera
{
    InputHandler input;
    GestureHandler gesture;

    Transform target;

    [SerializeField] bool networked = true;
    public bool recenterAngle = false, recenterHeight = false;

    [SerializeField] float angle, angleCenter, radius, height;

    [Range(0f, 1f)]
    public float heightCenter = 0.4f;
    public Vector3 lookOffset;

    [Range(0f, 1f)] public float lookSpeed = 0.2f;
    public float angleCenterSpeed = 0.01f;
    public float angleCenterLerpSpeed = 1f;
    public float heightLerpSpeed = 1f;

    private float t_height;
    private float t_angleCenter;
 
    [SerializeField] float def_height;
    [SerializeField] float def_angleCenter;

    [SerializeField] Transform m_angleTarget;
    public Transform angleTarget
    {
        get
        {
            return m_angleTarget;
        }
        set
        {
            m_angleTarget = value;
        }
    }

    #region Fetch Player

    Player.Controller m_player = null;
    public virtual Player.Controller player
    {
        get
        {
            if (m_player == null)
            {
                m_player = FindObjectOfType<Player.Controller>();
            }

            return m_player;
        }
    }

    #endregion

    protected override void Awake()
    {
        base.Awake();

        def_height = Mathf.Clamp(height, minHeight, maxHeight);
    }


    float minHeight = -0.2f, maxHeight = 5f;
    protected override void Start()
    {
        base.Start();

        input = InputHandler.Instance;
        gesture = GestureHandler.Instance;

        //this just turns off the cinemachine stuff that we dont need cause we're gonna be doing it all ourselves
        transposer.enabled = false;
        composer.enabled = false;

        t_height = def_height;
        t_angleCenter = def_angleCenter;
    }


    float realHeightCenter;
    private void Update()
    {
        if (player == null)
            return;

        target = player.transform;

        //sometimes things get messed up if angle number gets to big so this way it stays within the 360 range
        if (angle > 360f) angle -= 360f;
        if (angle < 0) angle += 360f;

        Vector3 delta = Vector3.zero;//this is the actual swipe direction that we use to move the camera around
        if (gesture.numberOfTouches > 1 && RaycastHandler.Instance.IsSafe)
        {
            delta = gesture.touches[1].delta / Time.deltaTime;
        }


        angle -= delta.x * player.Movement.camSpeedX * 0.01f;//angle moves the camera around horizontally

        t_height -= delta.y * player.Movement.camSpeedY * 0.01f;//height adjusts the camera's height in relation to the player
        t_height = Mathf.Clamp(t_height, minHeight, maxHeight);
        realHeightCenter = heightCenter.RemapNRB(0, 1, minHeight, maxHeight);

        radius = player.Movement.camRadius;//radius is how far away cam is from player


        height = Mathf.Lerp(height, t_height, Time.deltaTime * heightLerpSpeed);
        angleCenter = Mathf.Lerp(angleCenter, t_angleCenter, Time.deltaTime * angleCenterLerpSpeed);
    }

    Quaternion gravityAlignment = Quaternion.identity;
    Vector3 gravDir;
    float gravDirLerpVal, followSpeed;
    void FixedUpdate()
    {
        if (target == null) return;

        //this is mainly for a multiple planets setup, so camera is super smooth when changing gravity sources
        gravDirLerpVal = Mathf.Lerp(gravDirLerpVal, 0.2f, 0.1f);
        if (player.Movement.changingCOM)
            gravDirLerpVal = 0.05f;

        gravDir = Vector3.Lerp(gravDir, -target.up, gravDirLerpVal);//gets grav dir (smoothed so its all nice)

        //get the quaternion for this gravity direction, so that camera up = player up
        gravityAlignment = Quaternion.FromToRotation(gravityAlignment * Vector3.up, -gravDir) * gravityAlignment;

        if (recenterHeight)
        {
            if (player.Movement.CurrentMoveState != Player.Movement.MoveState.idle)
                t_height = Mathf.Lerp(t_height, realHeightCenter, 0.006f);//recenter with time
            else
                t_height = Mathf.Lerp(t_height, realHeightCenter, 0.0007f);
        }

        if (recenterAngle || (angleTarget != null && angleTarget != target))
        {
            float newAng = t_angleCenter;
            if (angleTarget != null)
            {
                //angle the camera to circle around player but aim towards angleTarget.
                //sorry for this complicated line- math is hard to explain but it just works ok?
                newAng = Vector3.SignedAngle(gravityAlignment * Vector3.left, Vector3.ProjectOnPlane(angleTarget.position - target.position, gravDir), gravDir);
            }
            angle = Mathf.LerpAngle(angle, newAng, angleCenterSpeed);
        }

        //this big complicated chunk of code calculates the camera's position
        float ang = angle * Mathf.Deg2Rad;
        Vector3 aa = new Vector3(Mathf.Cos(ang), 0, Mathf.Sin(ang));
        aa = gravityAlignment * aa;
        //Debug.DrawLine(target.transform.position, target.transform.position + (aa.normalized * 5f), Color.magenta, 10f);
        float distFromTarg = radius * height.Remap(minHeight, maxHeight, 0.4f, 1.6f);
        distFromTarg = Mathf.Max(distFromTarg, 15f);
        Vector3 pos = target.transform.position + (aa + target.transform.up * height).normalized * distFromTarg;
        followSpeed = Mathf.Lerp(followSpeed, Mathf.Clamp01(player.Movement.camFollowSpeed), 0.1f);
        if (player.Movement.changingCOM)
            followSpeed = Mathf.Clamp01(player.Movement.camFollowSpeed) / 4f;
        transform.position = Vector3.Lerp(transform.position, pos, followSpeed);
        //Debug.DrawLine(target.transform.position, target.transform.position + (zDir + target.transform.up * height).normalized * radius, Color.green, 10f);

        Vector3 targetLook = target.transform.position + transform.TransformVector(lookOffset);
        Quaternion lookAtT = Quaternion.LookRotation(targetLook - transform.position, -gravDir);
        transform.rotation = Quaternion.Lerp(transform.rotation, lookAtT, lookSpeed);

    }

    public void clearAngleTarget() { angleTarget = null; }

    public void AvatarCamera()
    {
        t_angleCenter = 112;
        t_height = 4;

        recenterAngle = true;
        recenterHeight = true;
    }

    public void DefaultCamera()
    {
        t_angleCenter = def_angleCenter;
        t_height = def_height;

        recenterAngle = false;
        recenterHeight = false;
    }

}