using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class EntityDummy : NetworkBehaviour {

//INST VAR

    [HideInInspector]
    public GameObject dummy;
    private Animator dummyAnimator;

    private Vector3 placeholderInitialPosition;
    private Vector3 placeholderPreviousPosition;
    private Vector3 placeholderActualPosition;

    private bool assetIsAttached = false;

//UNITY METH

    private void Start() {

        placeholderInitialPosition = this.transform.position;
        placeholderPreviousPosition = placeholderInitialPosition; 
    }

    void FixedUpdate() {

        if (assetIsAttached) {

            placeholderActualPosition = this.transform.position;

            dummy.transform.position = this.transform.position;
            dummy.transform.rotation = this.transform.rotation;

            //placeholder is moving horizontally -> walk
            if (placeholderActualPosition.x != placeholderPreviousPosition.x || placeholderActualPosition.z != placeholderPreviousPosition.z) {

                dummyAnimator.SetBool("walk", true);
            }

            //placeholder is not moving horizontally -> idle || jump
            else {

                if (placeholderActualPosition.y > placeholderPreviousPosition.y) {

                    dummyAnimator.SetBool("walk", false);
                    dummyAnimator.SetTrigger("jump");
                } 

                else dummyAnimator.SetBool("walk", false);
            }
            
            placeholderPreviousPosition = placeholderActualPosition;
        }

        else if (dummy != null) {

            dummyAnimator = dummy.GetComponent<Animator>();
            assetIsAttached = true;
        }
    }
}

