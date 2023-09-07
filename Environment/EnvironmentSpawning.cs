using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class EnvironmentSpawning : NetworkBehaviour {

//INST VAR

    private GameObject assetBundleManager;
    private AssetLoader assetLoader;

//METH

    private void Start() {

        assetBundleManager = GameObject.Find("Asset Bundle Manager");
        assetLoader = assetBundleManager.GetComponent<AssetLoader>();
    }

    public GameObject SpawnLogic(int soundIndex, Vector3 spawnPosition, Quaternion spawnRotation) {

        if (this.isServer) {

            GameObject enviromentSpawned;

            enviromentSpawned = this.ServerInstantiateEnviroment(soundIndex, spawnPosition, spawnRotation);
            this.RpcClientInstantiateAsset(soundIndex, spawnPosition, spawnRotation);

            return enviromentSpawned;
        }

        return null;
    }

    //server spawn placeholder
    [Server]
    private GameObject ServerInstantiateEnviroment (int enviromentIndex, Vector3 spawnPosition, Quaternion spawnRotation) {

        GameObject enviroment;
        enviroment = Instantiate(assetLoader.enviromentAssetList[enviromentIndex].getAsset(), spawnPosition, spawnRotation, GameObject.Find("Environment").transform);

        return enviroment;
    }

    //client and server intantiate asset
    [ClientRpc]
    private void RpcClientInstantiateAsset(int enviromentIndex, Vector3 spawnPosition, Quaternion spawnRotation) {

        if (!this.isServer) {

            GameObject enviroment;
            enviroment = Instantiate(assetLoader.enviromentAssetList[enviromentIndex].getAsset(), spawnPosition, spawnRotation, GameObject.Find("Environment").transform);
        }
    }
}