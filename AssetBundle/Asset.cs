using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asset : MonoBehaviour {

//TYPE

    public enum Category { Entity, Enviroment, Sound }

//INST VAR

    private GameObject asset;
    private string assetName;
    

//CONST

    public Asset () {

        this.asset = null;
        this.assetName = "";
    }

    public Asset(GameObject asset, string assetName) {

        this.asset = asset;
        this.assetName = assetName;
    }

//GET

    public GameObject getAsset() {

        return this.asset;
    }

    public string getAssetName () {

        return this.assetName;
    }

//SETTER

    public void setAsset(GameObject newAsset) {

        this.asset = newAsset;
    }

    public void setAssetName (string newAssetName) {

        this.assetName = newAssetName;
    }
}

