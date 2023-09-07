using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class FPCameraMovementServer : NetworkBehaviour {

//INST VAR

    private const string CLIENT_TAG = "Client";
    private const string SERVER_TAG = "Server";

    private const string PLAYER_LABEL_TAG = "Player Label";

    private const string SERVER_CAMERA_MARK = "Server First Person Camera - ";

    private GameObject[] clients;
    private int clientsPastLength = 0;
    private Transform[] clientsTransform;

    private bool areClientsFound = false;
    private bool updateClients = false;
    private bool updateClientsFlag = false;
    private int updateDelay = 3;

    private string clientLabelName;
    private int clientCameraIndex = 0;

    private Vector3 updatedCameraPosition = Vector3.zero;

    [SerializeField]
    private GameObject playerLabelPrefab;
    [SerializeField]
    private GameObject playerLightPrefab;

    private Color[] colorList = { Color.red, Color.green, Color.blue, Color.white, Color.yellow, Color.cyan, Color.magenta };
    private int colorIndex = 0;

    private Camera FirstPersonCamera;
    private Ray ray;
    private RaycastHit hit;

    //UNITY METH

    private void Start() {

        //server camera
        if (this.isServer) {

            Debug.Log(SERVER_CAMERA_MARK + "finding all the clients");
            FirstPersonCamera = this.GetComponent<Camera>();
        }
    }

    void FixedUpdate() {

        if (this.isServer) {

            this.OnClickLabel();
            StartCoroutine(UpdateClients());
        }
    }

    void LateUpdate() {

        //server camera
        if (this.isServer) {

            //finding all the clients
            if (!areClientsFound || updateClients) {

                //check if update is worth
                if (updateClients) {

                    updateClients = false;
                    if (clients.Length != GameObject.FindGameObjectsWithTag(CLIENT_TAG).Length) { updateClientsFlag = true; }
                }

                if (!areClientsFound || updateClientsFlag) {

                    updateClientsFlag = false;
                    clients = GameObject.FindGameObjectsWithTag(CLIENT_TAG);

                    //no clients have been found
                    if (clients.Length <= 0) {

                        Debug.Log(SERVER_CAMERA_MARK + "no clients has been found");
                    }

                    //some clients have been found
                    else {

                        Debug.Log(SERVER_CAMERA_MARK + clients.Length + " clients have been found");
                        this.FindingClients();

                        //add client info only to new connected clients
                        if (clientsPastLength < clients.Length) {

                            int numNewClients = clients.Length - clientsPastLength;
                            int newClientIndex = 0;
                            
                            for (int i = 0; i < numNewClients; i++) {

                                newClientIndex = clients.Length - (i + 1);
                                setClientInfo(clients[newClientIndex].GetComponent<Transform>(), newClientIndex + 1);
                            }
                        }
                    }

                    clientsPastLength = clients.Length;
                }
            }

            //all the clients have been found
            else {

                if (clientCameraIndex < clientsTransform.Length) {

                    this.CameraFollowingClient(clientCameraIndex);
                }
            }
        }
    }

//METH

    private IEnumerator UpdateClients () {

        yield return new WaitForSeconds(updateDelay);

        if (this.areClientsFound) { this.updateClients = true; }
    }

    private void FindingClients() {

        clientsTransform = new Transform[clients.Length];

        for (int i = 0; i < clients.Length; i++) {

            clientsTransform[i] = clients[i].GetComponent<Transform>();
        } 

        areClientsFound = true;
        updateClients = false;
        updateClientsFlag = false;
    }

    private void CameraFollowingClient (int i) {

        updatedCameraPosition = clientsTransform[i].position;
        updatedCameraPosition.y += 2f;

        this.transform.position = updatedCameraPosition;
        this.transform.rotation = clientsTransform[i].rotation;
    }


    private void OnClickLabel () {

        if (Input.GetMouseButtonDown(0)) {

            ray = FirstPersonCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit)) {
 
                if (hit.transform.CompareTag(PLAYER_LABEL_TAG)) {

                    clientLabelName = hit.transform.name;

                    if (int.TryParse(clientLabelName.Substring(clientLabelName.Length - 1), out clientCameraIndex)) {

                        clientCameraIndex -= 1;
                    }
                }
            }
        }
    }

    private void setClientInfo (Transform clientTransform, int clientIndex) {

        GameObject newLabel;
        RectTransform newLabelTransform;
        string labelName = "Player" + " " + clientIndex; 
  
        newLabel = Instantiate(playerLabelPrefab);
        newLabelTransform = newLabel.GetComponent<RectTransform>();
        newLabel.transform.parent = GameObject.Find("PlayerListContent").transform;
        newLabelTransform.localPosition = new Vector3(newLabelTransform.position.x, newLabelTransform.position.y, 0);
        newLabelTransform.localScale = new Vector3(1, 1, 1);
        newLabel.name = labelName;
        newLabel.GetComponentInChildren<Text>().text = labelName;

        GameObject newLight;

        newLight = Instantiate(playerLightPrefab, new Vector3(clientTransform.position.x, clientTransform.position.y + 10, clientTransform.position.z), Quaternion.Euler(90, 0, 0));
        newLight.GetComponent<PlayerLightFollowing>().clientTransform = clientTransform;

        if (colorIndex >= colorList.Length) colorIndex = 0;

        newLabel.GetComponent<RawImage>().color = colorList[colorIndex];
        newLight.GetComponent<Light>().color = colorList[colorIndex];
        colorIndex++;
    }
}

