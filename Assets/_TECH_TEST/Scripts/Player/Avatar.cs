using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{

    public class Avatar : MonoBehaviour
    {

        //For avatar w/ no animations
        public bool StaticAvatar { get; private set; } = false;

        public enum AnimationState
        {
            Movement,
            Pose
        }
        public AnimationState previousAnimationState = AnimationState.Movement, animationState = AnimationState.Movement;


        public enum MoveState
        {
            Idle,
            Sit,
            Walk,
            Run,
            Jump,
            Fall
        }
        public MoveState moveState = MoveState.Idle;
        public string previousPoseState = "", poseState = "";

        public Renderer meshRenderer;
        public Material normalMaterial, glowMaterial;

        public bool usePlayerScale = true;


        public bool glowing = false;

        bool reset = true;


        Animator animator;

        void Awake()
        {
            animator = GetComponent<Animator>();
            StaticAvatar = (animator == null);
        }

        void Start()
        {
            if (usePlayerScale)
                transform.localScale = FindObjectOfType<Controller>().Appearance.normalizedCharacterScale; // Update character scale on start

            normalMaterial = meshRenderer.material;
        }

        void Update()
        {
            EvaluateAnimationState();
        }


        #region Animations

        public void SetAnimationState(AnimationState state, bool self = false)
        {
            if (StaticAvatar)
                return;
            animationState = state;
        }

        public void SetMoveState(MoveState state, bool self = false)
        {
            if (StaticAvatar)
                return;
            SetAnimationState(AnimationState.Movement);
            moveState = state;
        }

        public void SetPose(string pose, bool self = false)
        {
            if (StaticAvatar)
                return;
            SetAnimationState(AnimationState.Pose);
            poseState = pose;
        }

        public void ClearPose()
        {
            if (StaticAvatar)
                return;
            if (!string.IsNullOrEmpty(previousPoseState))
                animator.SetBool(previousPoseState, false);
            if (!string.IsNullOrEmpty(poseState))
                animator.SetBool(poseState, false);

            previousPoseState = poseState = "";
        }

        void EvaluateAnimationState()
        {
            if (StaticAvatar)
                return;
            if (previousAnimationState != animationState)
            {
                if (previousAnimationState == AnimationState.Movement)
                {
                    ClearPose();

                    animator.SetBool("posing", true);
                    animator.SetTrigger("reset pose"); // Revert to posing root
                }
                else // Was posing
                {
                    animator.SetBool("posing", false);
                    animator.SetTrigger("reset pose"); // Revert to posing root
                    animator.SetTrigger("reset"); // Revert to movement root
                }

                previousAnimationState = animationState;
            }


            if (animationState == AnimationState.Movement)
            {
                animator.SetBool("posing", false);
                ClearPose(); // Ensure animotes are reset

                EvaluateMoveState();
            }
            else if (animationState == AnimationState.Pose)
            {
                animator.SetBool("posing", true);
                moveState = MoveState.Idle; // Ensure movement is reset

                EvaluatePoseState();
            }
        }

        void EvaluateMoveState()
        {
            switch (moveState)
            {
                case MoveState.Idle:
                    animator.SetBool("idle", true);
                    animator.SetBool("sitting", false);
                    animator.SetBool("walking", false);
                    animator.SetBool("running", false);
                    animator.SetBool("jumping", false);
                    animator.SetBool("falling", false);
                    break;
                case MoveState.Sit:
                    animator.SetBool("idle", false);
                    animator.SetBool("sitting", true);
                    animator.SetBool("walking", false);
                    animator.SetBool("running", false);
                    animator.SetBool("jumping", false);
                    animator.SetBool("falling", false);
                    break;
                case MoveState.Walk:
                    animator.SetBool("idle", false);
                    animator.SetBool("sitting", false);
                    animator.SetBool("walking", true);
                    animator.SetBool("running", false);
                    animator.SetBool("jumping", false);
                    animator.SetBool("falling", false);
                    break;
                case MoveState.Run:
                    animator.SetBool("idle", false);
                    animator.SetBool("sitting", false);
                    animator.SetBool("walking", false);
                    animator.SetBool("running", true);
                    animator.SetBool("jumping", false);
                    animator.SetBool("falling", false);
                    break;
                case MoveState.Jump:
                    animator.SetBool("idle", false);
                    animator.SetBool("sitting", false);
                    animator.SetBool("walking", false);
                    animator.SetBool("running", false);
                    animator.SetBool("jumping", true);
                    animator.SetBool("falling", false);
                    break;
                case MoveState.Fall:
                    animator.SetBool("idle", false);
                    animator.SetBool("sitting", false);
                    animator.SetBool("walking", false);
                    animator.SetBool("running", false);
                    animator.SetBool("jumping", false);
                    animator.SetBool("falling", true);
                    break;
                default:
                    break;
            }
        }

        void EvaluatePoseState()
        {
            if (previousPoseState != poseState)
            {
                if (!string.IsNullOrEmpty(previousPoseState))
                    animator.SetBool(previousPoseState, false);

                animator.SetTrigger("reset pose");
            }

            previousPoseState = poseState;
            animator.SetBool(poseState, true);
        }

        #endregion
    }

}
