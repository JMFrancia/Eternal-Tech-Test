using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{

    /// <summary>
    /// Handles player animations, extends Animation Handler
    ///     - Updates animator speed and animations with movement
    /// </summary>

    public class Animations : AnimationHandler
    {
        Controller controller;

        [SerializeField] float minSpeed = 1f;
        [SerializeField] float maxSpeed = 16f;

        [SerializeField] float walkThreshold = 1f;
        [SerializeField] float runThreshold = 1f;

        bool reset = false;

        bool m_emoting = false;
        public bool Emoting
        {
            get
            {
                return m_emoting;
            }
            set
            {
                m_emoting = value;
                if (value)
                    EmoteParticleSys.Play();
            }
        }

        float timeBetweenTaps = 0f;
        int taps = 0;
        float triggerThreshold = .8f;


        protected override void Awake()
        {
            controller = GetComponent<Controller>();
        }

        private void Update()
        {
            if (!controller.Movement.canMove) return;

            doubleTap = false;
            if (GestureHandler.Instance.GetTap())
            {
                if (++taps == 2)
                {
                    //print("2 taps!");
                    //if (timeBetweenTaps <= triggerThreshold)
                    //{
                    StopCoroutine("Default");
                    StartCoroutine("Default");
                    doubleTap = true;
                    //}
                    taps = 0;
                }
                else
                    timeBetweenTaps = 0f;
            }
            else
            {
                timeBetweenTaps += Time.deltaTime;
                if (timeBetweenTaps > triggerThreshold)
                    taps = 0;
            }




            UpdateStep();

            if (controller.Movement.canMove) // Added check to ensure 
                UpdateSpeed();

            EmoteManager();
        }

        bool doubleTap;
        ParticleSystem m_emoteParticleSys;
        public ParticleSystem EmoteParticleSys
        {
            get
            {
                if (m_emoteParticleSys == null)
                    m_emoteParticleSys = GetComponentInChildren<ParticleSystem>();

                return m_emoteParticleSys;
            }
        }
        void EmoteManager()
        {
            if (Emoting && !EmoteParticleSys.isPlaying)
                Emoting = false;

            if (doubleTap && !Emoting)//if this player and double tap
                Emoting = true;
        }

        // Update speed of animations from movement speed
        void UpdateStep()
        {
            if (animator == null)
                return;

            //todo use rootmotion to make this match exactly
            if (controller.Movement.CurrentMoveState == Movement.MoveState.walking)
            {
                animator.speed = (controller.Movement.Step + 0.2f);
            }
            else if (controller.Movement.CurrentMoveState == Movement.MoveState.running)
            {
                animator.speed = controller.Movement.Step + 0.35f;
            }
            else
                animator.speed = 1f;
        }

        void UpdateSpeed()
        {
            if (controller.avatar == null)
                return;

            switch (controller.Movement.CurrentMoveState)
            {
                case Movement.MoveState.idle:
                    if (controller.Movement.Sitting)
                        controller.avatar.SetMoveState(Avatar.MoveState.Sit);
                    else
                        controller.avatar.SetMoveState(Avatar.MoveState.Idle);

                    break;
                case Movement.MoveState.walking:
                    controller.avatar.SetMoveState(Avatar.MoveState.Walk);
                    break;
                case Movement.MoveState.running:
                    controller.avatar.SetMoveState(Avatar.MoveState.Run);
                    break;
                case Movement.MoveState.jumping:
                    controller.avatar.SetMoveState(Avatar.MoveState.Jump);
                    break;
                case Movement.MoveState.falling:
                    controller.avatar.SetMoveState(Avatar.MoveState.Fall);
                    break;
            }
        }
    }
}


