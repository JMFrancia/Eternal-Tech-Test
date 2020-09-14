using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Cinemachine;

/// <summary>
/// Base class for game cameras in scene
/// </summary>

[RequireComponent(typeof(CinemachineVirtualCamera))]
public class GameCamera : MonoBehaviour
{
    CameraManager manager;

    public Transform worldUpOverride;

    // Connect to Cinemachine camera
    protected new CinemachineVirtualCamera camera;
                protected CinemachineTransposer transposer;
                protected CinemachineComposer composer;

    protected CinemachineFreeLook cameraFreeLook;

    [SerializeField] bool respondToCharacterScale = false;

    #region Fetch Player

    Player.Controller m_player = null;
    public virtual Player.Controller player
    {
        get
        {
            if (m_player == null)
                m_player = FindObjectOfType<Player.Controller>();

            return m_player;
        }
    }

    #endregion

    #region Monobehaviour callbacks

    protected virtual void Awake() {
        manager = FindObjectOfType<CameraManager>();

        camera = GetComponent<CinemachineVirtualCamera>();
        cameraFreeLook = GetComponent<CinemachineFreeLook>();
    }

    protected virtual void Start(){
        if (camera != null)
        {
            transposer = camera.GetCinemachineComponent<CinemachineTransposer>();
            composer = camera.GetCinemachineComponent<CinemachineComposer>();
        }
    }

    public virtual void Enable()
    { 
    
    }

    public virtual void Disable(){
        Debug.Log("Disabled " + name);
        gameObject.SetActive(false);
    }

    #endregion
}
