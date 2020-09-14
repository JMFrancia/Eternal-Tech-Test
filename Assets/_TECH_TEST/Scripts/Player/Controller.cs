using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;

namespace Player
{

    /// <summary>
    /// Controller for player, drives all behaviors
    /// </summary>

    public class Controller : MonoBehaviour
    {
        Animations animations;
        public Animations Animation { get { return animations; } }

        Movement movement;
        public Movement Movement { get { return movement; } }

        Appearance appearance;
        public Appearance Appearance { get { return appearance; } }

        Sounds sounds;
        public Sounds Sounds { get { return sounds; } }



        [SerializeField] bool m_self = false;
        public bool self
        {
            get
            {
                return m_self;
            }
            set
            {
                m_self = value;
                UpdateActiveAttributesFromLocalInstance();
            }
        }

        public Avatar avatar
        {
            get
            {
                return appearance.Avatar;
            }
        }

        public Attribute<Controller>[] attributes;

        public enum State
        {
            Static,
            Normal,
            Posing
        }
        public State state = State.Normal, previousState = State.Normal;
        public State ActiveState
        {
            set
            {
                if (value != state)
                {
                    previousState = state;
                    state = value;
                }
                //animations.Reset();
            }
        }

        public void SetState(string state)
        {
            if (state == "normal")
                ActiveState = State.Normal;
            else if (state == "static")
                ActiveState = State.Static;
            else if (state == "posing")
                ActiveState = State.Posing;
        }

        public void RevertState()
        {
            ActiveState = previousState;
        }

        public bool Static
        {
            get { return state == State.Static; }
        }

        public bool Posing
        {
            get { return state == State.Posing; }
        }


        private void Awake()
        {
            animations = GetComponent<Animations>();
            movement = GetComponent<Movement>();
            appearance = GetComponent<Appearance>();
            sounds = GetComponent<Sounds>();

            attributes = GetComponentsInChildren<Attribute<Controller>>();
        }

        void Start()
        {
            self = true;
        }

        private void Update()
        {
            if (!self) return;

            // Play sounds if moving
            if (movement != null && movement.CurrentMoveState != Movement.MoveState.idle && movement.canMove && movement.isGrounded)
            {
                float step = movement.Step;
                sounds.Step(step);
            }
        }

        void UpdateActiveAttributesFromLocalInstance()
        {
            foreach (Attribute<Controller> attr in attributes)
                attr.Active = self;

            animations.enabled = self;
            sounds.enabled = self;
        }
    }
}
