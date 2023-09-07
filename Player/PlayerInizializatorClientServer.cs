using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerInizializatorClientServer : NetworkBehaviour {

//INST VAR

    private const string PLAYER_TAG = "Player";
    private const string CLIENT_TAG = "Client";
    private const string SERVER_TAG = "Server";

    private readonly Vector3 SERVER_SPAWN_POSITION = new Vector3(0, 100, 0);

    private GameObject[] players;

    [SyncVar]
    [HideInInspector]
    public bool isPlayerServer = false;
    private bool isServerFound = false;

    private int updateDelay = 0;

//UNITY METH

    void Start() {

        //local player is a client
        if (this.isLocalPlayer && !this.isServer) this.InizializeClient();

        //local player is the server
        else if (this.isLocalPlayer)  this.InizializeServer();
    }

    void FixedUpdate() {

        //after the server is found the update delay can slow down to 3 sec.
        if (isServerFound) updateDelay = 3;

        StartCoroutine(UpdatePlayers());
    }

//METH

    //Inizialization
    private void InizializeClient() {

        this.tag = CLIENT_TAG;

        Debug.Log("Player Client joined");
    }

    private void InizializeServer() {

        this.tag = SERVER_TAG;

        this.isPlayerServer = true;
        this.isServerFound = true;

        this.DeactivateServer(this.gameObject);

        Debug.Log("Player Server joined");
    }

    private void DeactivateServer(GameObject server) {

        server.GetComponent<Transform>().position = SERVER_SPAWN_POSITION;
        server.transform.GetChild(0).gameObject.SetActive(false);
        server.transform.GetChild(1).gameObject.SetActive(false);
        server.GetComponent<Rigidbody>().isKinematic = true;
        server.GetComponent<CapsuleCollider>().enabled = false;
        server.GetComponent<Animator>().enabled = false;
        server.GetComponent<NetworkTransform>().enabled = false;
        server.GetComponent<NetworkAnimator>().enabled = false;
    }

    //Updating
    private IEnumerator UpdatePlayers() {

        yield return new WaitForSeconds(updateDelay);

        //local player is a client
        if (this.isLocalPlayer && !this.isServer) {

            //find the server
            this.FindServer();

            //find the clients
            this.FindClients();
        }

        //local player is the server
        else if (this.isLocalPlayer) {

            //find the clients
            this.FindClients();
        }
    }

    private void FindClients() {

        if (isServerFound) {

            players = GameObject.FindGameObjectsWithTag(PLAYER_TAG);

            for (int i = 0; i < players.Length; i++) players[i].tag = CLIENT_TAG;
        }
    }

    private void FindServer() {

        if (!isServerFound) {

            players = GameObject.FindGameObjectsWithTag(PLAYER_TAG);

            for (int i = 0; i < players.Length; i++) {

                if (players[i].GetComponent<PlayerInizializatorClientServer>().isPlayerServer) {

                    players[i].tag = SERVER_TAG;

                    this.DeactivateServer(players[i]);

                    isServerFound = true;
                }
            }
        }
    }
}

