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
        [Header("Pogo")]
        public AudioClip[] boings;
        public AudioClip megaBoing;
        public AudioClip badBoing;
        public AudioClip goodBoing;

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

        public void RegularBounce() {
            PlayRandomSoundRandomPitch(boings, 1f);
        }

        public void BadBounce() {
            PlaySound(badBoing, 1f);
        }

        public void GoodBounce()
        {
            PlaySound(goodBoing, 1f);
        }

        public void GodBounce()
        {
            PlaySound(megaBoing, 1f);
        }
    }
}
