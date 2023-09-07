using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetClient : NetworkDiscovery {

    private bool isConnect = false;

    void Start() {

        startClient();
    }

    public void startClient() {

        if (Initialize()) {

            Debug.Log("Broadcast port: " + this.broadcastPort + " is avaible");

            if (StartAsClient()) Debug.Log("Starts listening for broadcasts messages");

            else {

                Debug.Log("Fail to listening broadcast messages");
                StopBroadcast();
            }
        }

        else Debug.Log("Broadcast port: " + this.broadcastPort + " is not avaible");
    }

    public override void OnReceivedBroadcast(string fromAddress, string data) {

        if (!isConnect) {
            
            NetworkManager.singleton.networkAddress = fromAddress;
            Debug.Log("A Server is found: " + fromAddress);

            if (NetworkManager.singleton.StartClient() != null) {

                Debug.Log("Connect as Client");
                isConnect = true;
            }

            else {

                Debug.Log("Fail to connect as Client");
                StopBroadcast();
            } 
        }

        else {

            StopBroadcast();
        }
    }
}
