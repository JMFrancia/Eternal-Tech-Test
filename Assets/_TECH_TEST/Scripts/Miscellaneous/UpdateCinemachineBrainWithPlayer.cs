using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Cinemachine;

[RequireComponent(typeof(CinemachineBrain))]
public class UpdateCinemachineBrainWithPlayer : MonoBehaviour
{
    CinemachineBrain cinemachineBrain;

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


    void Awake()
    {
        cinemachineBrain = GetComponent<CinemachineBrain>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null)
            return;

        cinemachineBrain.m_WorldUpOverride = player.transform;
    }
}
