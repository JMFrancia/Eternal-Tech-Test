using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages all in-game cameras
///     - Ensure one camera used at a time
///     - Switch between camera views
/// </summary>

public class CameraManager : MonoBehaviour
{
    new Camera camera;

    Camera mainCamera;

    [SerializeField] GameCamera defaultCamera;
    [SerializeField] GameCamera previousCamera, currentCamera;

    void Awake()
    {
        mainCamera = Camera.main;

        Initialize();
    }

    void Initialize()
    {
        Camera[] cameras = FindObjectsOfType<Camera>();
        foreach (Camera c in cameras)
        {
            if (c != mainCamera && c.tag != "Hidden")
                c.gameObject.SetActive(false);
            else
                camera = c;
        }


        GameCamera[] gameCameras = GetComponentsInChildren<GameCamera>(true);
        foreach (GameCamera c in gameCameras)
        {
            if (c == defaultCamera) Enable(c);
            else Disable(c);
        }
    }


    void Enable(GameCamera camera){
        if(camera == null) return;
             
        camera.gameObject.SetActive(true);
        camera.Enable();

        UpdateCinemachineBrainWorldUpOverride(camera.worldUpOverride);

        GameCamera @ref = (camera != currentCamera)? currentCamera:null;
        onUpdateActiveCamera(camera);

        if (@ref != null)
            Disable(@ref);
    }

    void Disable(GameCamera camera){
        if (camera == null) return;

        camera.gameObject.SetActive(false);
        if (camera == currentCamera)
            onUpdateActiveCamera(null);
    }

    void onUpdateActiveCamera(GameCamera camera)
    {
        previousCamera = currentCamera;
        currentCamera = camera;

        Debug.LogFormat("previous: {0}  curr: {1}", (previousCamera == null)? "NULL": previousCamera.name, (currentCamera == null)? "NULL": currentCamera.name);
    }


    public void Set(Camera camera){
        if(this.camera != camera){
            if(this.camera != null && this.camera.tag != "Hidden")
                this.camera.gameObject.SetActive(false);
            this.camera = camera;

            if (this.camera != null && this.camera.tag != "Hidden")
                this.camera.gameObject.SetActive(true);
        }
    }

    public void Set(GameCamera camera){
        if(camera == null)
            return;

        Enable(camera);    
    }

    // Reset to default camera
    public void Reset(){
        Enable(defaultCamera);
    }

    public void Revert()
    {
        Enable(previousCamera);
    }

    void UpdateCinemachineBrainWorldUpOverride(Transform up)
    {
        if (mainCamera == null) return;

        Cinemachine.CinemachineBrain brain = mainCamera.GetComponent<Cinemachine.CinemachineBrain>();
        if (brain == null) return;

        brain.m_WorldUpOverride = up;
    }
}
