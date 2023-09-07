using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetServer : NetworkDiscovery {

    void Start() {

        Application.runInBackground = true;
        startServer();
    }

    public void startServer() {

        if (Initialize()) {

            Debug.Log("Broadcast port: " + this.broadcastPort + " is avaible");

            if (StartAsServer()) Debug.Log("Starts sending broadcast messages");
            else Debug.Log("Fail to sending broadcast messages");

            if (NetworkManager.singleton.StartHost() != null) Debug.Log("Connect as Host");
            else Debug.Log("Fail to connect as Host");
        }

        else Debug.Log("Broadcast port: " + this.broadcastPort + " is not avaible");
    }
}
