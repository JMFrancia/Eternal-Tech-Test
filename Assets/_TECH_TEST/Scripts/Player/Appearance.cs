using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

using UnityEngine.Events;

namespace Player
{

    /// <summary>
    /// Handles player appearance, driven by character selected
    /// </summary>

    public class Appearance : Attribute<Controller>
    {
        public GameObject pr_character;
        
        [SerializeField] GameObject characterObject;
        [SerializeField] Avatar avatar;


        public Avatar Avatar
        {
            get
            {
                return avatar;
            }
        }


        [SerializeField] Vector3 characterScale = Vector3.one;

        public Vector3 normalizedCharacterScale
        {
            get
            {
                return characterScale;
            }
        }


        protected override void onActive()
        {
            SelectCharacter();
        }

        public void ChangeAvatar(GameObject newCharacterObject, float scale) {
            pr_character = newCharacterObject;
            characterScale = Vector3.one * scale;
            SelectCharacter();
        }

        // Instantiate character prefab from character instance
        public void SelectCharacter()
        {
            if (characterObject != null)
            {
                GameObject.Destroy(characterObject);
            }

            characterObject = Instantiate(pr_character) as GameObject;
            avatar = characterObject.GetComponent<Avatar>();

            characterObject.transform.position = transform.position;
            characterObject.transform.rotation = transform.rotation;
            characterObject.transform.localScale = characterScale;

            controller.Movement.Sub = characterObject.transform; // Update movement sub-transform with character

            SetAnimator();
        }

        // Bind to animator controller attached to prefab
        public void SetAnimator()
        {
            Animator animator = characterObject.GetComponent<Animator>();

            //set player animator 
            if (animator != null)
            {
                controller.Animation.Animator = animator;
                controller.Movement.characterAnimator = animator;
            }
            //set by grabbing animator directly from character GO
            else
            {
                controller.Animation.Animator = null;
                controller.Movement.characterAnimator = null;
            }
        }

    }

}

