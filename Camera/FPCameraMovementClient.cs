using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class FPCameraMovementClient : NetworkBehaviour {

//INST VAR

    private const string CLIENT_TAG = "Client";
    private const string SERVER_TAG = "Server";

    private const string CLIENT_CAMERA_MARK = "Client First Person Camera - ";

    private GameObject[] clients;
    private Transform localClientTransform;

    private Vector3 updatedCameraPosition = Vector3.zero;

    private bool isLocalClientFound = false;

//UNITY METH

    void LateUpdate() {

        //client camera
        if (!this.isServer) {

            //finding the local client
            if (!isLocalClientFound) {

                clients = GameObject.FindGameObjectsWithTag(CLIENT_TAG);

                //no players has been found
                if (clients.Length <= 0) {

                    Debug.Log(CLIENT_CAMERA_MARK + "no clients has been found");
                }

                //some players have been found
                else {

                    Debug.Log(CLIENT_CAMERA_MARK + "some clients have been found");
                    this.FindingLocalClient();
                }
            }

            //local client is found
            else {

                this.CameraFollowingLocalClient();
            }
        }
    }

//METH

    private void FindingLocalClient() {

        for (int i = 0; i < clients.Length; i++) {

            //the client is the local client
            if (clients[i].GetComponent<NetworkIdentity>().isLocalPlayer) {

                localClientTransform = clients[i].GetComponent<Transform>();

                isLocalClientFound = true;
                Debug.Log(CLIENT_CAMERA_MARK + "local client is found");
            }
        }
    }

    private void CameraFollowingLocalClient() {

        updatedCameraPosition = localClientTransform.position;
        updatedCameraPosition.y += 2f;

        this.transform.position = updatedCameraPosition;
        this.transform.rotation = localClientTransform.rotation;
    }
}

