using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

public class CameraControl : MonoBehaviour
{
    GameObject player;
    Camera cammy;

    public PostProcessingProfile pp;
    public ColorGradingModel.Settings colorGrader;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        cammy = GetComponent<Camera>();

        colorGrader = pp.colorGrading.settings;

        ResetColor();
    }
    
    //sets random dancing colors 
    public void SetColor()
    {
        colorGrader.basic.temperature = Random.Range(-100f, 100f);
        colorGrader.basic.tint = Random.Range(-100f, 100f);

        pp.colorGrading.settings = colorGrader;
    }

    public void ResetColor()
    {
        colorGrader.basic.temperature = 0;
        colorGrader.basic.tint = 0;

        pp.colorGrading.settings = colorGrader;
    }
}
