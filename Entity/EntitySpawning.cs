using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Animations;

public class EntitySpawning : NetworkBehaviour {

//INST VAR

    [SerializeField]
    private GameObject entityPlaceholder;
    private GameObject assetBundleManager;
    private AssetLoader assetLoader;

//METH

    private void Start() {

        assetBundleManager = GameObject.Find("Asset Bundle Manager");
        assetLoader = assetBundleManager.GetComponent<AssetLoader>();
    }

    public GameObject SpawnLogic (int entityIndex, Vector3 spawnPosition, Quaternion spawnRotation) {

        if (this.isServer) {

            GameObject placeholderSpawned;

            placeholderSpawned = this.ServerSpawnPlaceholder(spawnPosition);
            this.RpcClientServerInstantiateAsset(placeholderSpawned, entityIndex, spawnPosition, spawnRotation);

            return placeholderSpawned;
        }

        return null;
    }

    //server spawn placeholder
    [Server]
    private GameObject ServerSpawnPlaceholder(Vector3 spawnPosition) {

        GameObject placeholder;

        placeholder = Instantiate(entityPlaceholder);
        placeholder.transform.position = spawnPosition;

        NetworkServer.Spawn(placeholder);

        return placeholder;
    }

    //client and server intantiate asset
    [ClientRpc]
    private void RpcClientServerInstantiateAsset(GameObject placeholderSkeleton, int entityIndex, Vector3 spawnPosition, Quaternion spawnRotation) {

        GameObject entity;
        entity = Instantiate(assetLoader.entityAssetList[entityIndex].getAsset(), spawnPosition, spawnRotation);

        if (entity.GetComponent<ScaleConstraint>() != null) entity.GetComponent<ScaleConstraint>().enabled = false;
        entity.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

        placeholderSkeleton.GetComponent<EntityDummy>().dummy = entity;
    }
}