using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SoundSpawning : NetworkBehaviour {

//INST VAR

    [SerializeField]
    private GameObject notePlaceholder;
    private GameObject assetBundleManager;
    private AssetLoader assetLoader;

//METH

    private void Start() {

        assetBundleManager = GameObject.Find("Asset Bundle Manager");
        assetLoader = assetBundleManager.GetComponent<AssetLoader>();
    }

    public GameObject SpawnLogic(int soundIndex, Vector3 spawnPosition, Quaternion spawnRotation) {

        if (this.isServer) {

            GameObject placeholderSpawned;

            placeholderSpawned = this.ServerInstantiatePlaceholder(spawnPosition);
            this.RpcClientServerInstantiateAsset(soundIndex, spawnPosition, spawnRotation);

            return placeholderSpawned;
        }

        return null;
    }

    //server spawn placeholder
    [Server]
    private GameObject ServerInstantiatePlaceholder(Vector3 spawnPosition) {

        GameObject placeholder;

        placeholder = Instantiate(notePlaceholder);
        placeholder.transform.position = spawnPosition;

        return placeholder;
    }

    //client and server intantiate asset
    [ClientRpc]
    private void RpcClientServerInstantiateAsset(int soundIndex, Vector3 spawnPosition, Quaternion spawnRotation) {

        GameObject sound;
        sound = Instantiate(assetLoader.soundAssetList[soundIndex].getAsset(), spawnPosition, spawnRotation);

        if (sound.GetComponent<AudioSource>() == null) sound.AddComponent<AudioSource>();
        sound.GetComponent<AudioSource>().spatialBlend = 1;
        sound.GetComponent<AudioSource>().playOnAwake = true;
        sound.GetComponent<AudioSource>().loop = true;
    }
}