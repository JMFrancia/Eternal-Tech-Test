using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{

    /// <summary>
    /// Handles player sounds, extends Audio Handler
    /// </summary>

    public class Sounds : AudioHandler
    {
        [Header("Movement")]
        public AudioClip[] footsteps;
        public AudioClip jump;
    
        float footstepTimer;
        [SerializeField] float footstepInterval = 1f;

        [Header("Inventory")]
        public AudioClip collect;

        protected override void Awake() {
            base.Awake();
        }
        
        public void Step(float step){
            footstepTimer += Time.deltaTime * step;

            // Passed footestep threshold, play footstep sound at random pitch
            if (footstepTimer > footstepInterval)
            {
                PlayRandomSoundRandomPitch(footsteps, 1f);
                footstepTimer = 0;
            }
        }
    }
}
