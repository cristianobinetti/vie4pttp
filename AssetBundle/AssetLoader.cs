using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;

public class AssetLoader : NetworkBehaviour {

    //INST VAR

    private const string LOCAL_FOLDER_PATH = "C:\\xampp\\htdocs\\AssetBundles";

    [SyncVar] private SyncListString entityDirectoryInfo = new SyncListString();
    [SyncVar] private SyncListString enviromentDirectoryInfo = new SyncListString();
    [SyncVar] private SyncListString soundDirectoryInfo = new SyncListString();

    [SyncVar] private bool entityInfoAreAvaible = false;
    [SyncVar] private bool enviromentInfoAreAvaible = false;
    [SyncVar] private bool soundInfoAreAvaible = false;

    [HideInInspector]
    public List<Asset> entityAssetList;
    [HideInInspector]
    public List<Asset> enviromentAssetList;
    [HideInInspector]
    public List<Asset> soundAssetList;

    private bool entityAreLoaded = false;
    private bool enviromentAreLoaded = false;
    private bool soundAreLoaded = false;

    [SyncVar] private string networkFolderPath;
    private AssetBundle loadedAssetBundle;

    [SyncVar] private bool isNetworkFolderPathAcquired = false;
    private bool isAssetBundleLoaded = false;

    [HideInInspector]
    public bool areAssetsLoaded = false;

    //UNITY METH

    private void Start() { StartCoroutine(MainCoroutine()); }

    //METH

    private IEnumerator MainCoroutine() {

        //Waiting Network Folder Path acquisition
        this.GetNetworkFolderPath();
        yield return new WaitUntil(() => isNetworkFolderPathAcquired == true);

        //Waiting Category Directory Info acquisition
        this.GetDirectoryInfo(Asset.Category.Entity);
        this.GetDirectoryInfo(Asset.Category.Enviroment);
        this.GetDirectoryInfo(Asset.Category.Sound);
        yield return new WaitUntil(() => entityInfoAreAvaible == true && enviromentInfoAreAvaible == true && soundInfoAreAvaible == true);


        //Waiting Category Asset acquisition
        StartCoroutine(readCategoryDir(Asset.Category.Entity));
        StartCoroutine(readCategoryDir(Asset.Category.Enviroment));
        StartCoroutine(readCategoryDir(Asset.Category.Sound));
        yield return new WaitUntil(() => entityAreLoaded == true && enviromentAreLoaded == true && soundAreLoaded == true);

        areAssetsLoaded = true;
    }

    private IEnumerator readCategoryDir(Asset.Category category) {

        List<string> directoryInfo = new List<string>();
        List<Asset> categoryAssetList = new List<Asset>();

        AssetBundle assetBundle;
        GameObject asset;

        string categoryDirPath = this.networkFolderPath + "/" + category.ToString();
        directoryInfo = this.ExportDirectoryInfo(category);

        foreach (string info in directoryInfo) {

            //load the asset bundle -> server_path/category/asset_bundle_dir/asset_bundle_name
            StartCoroutine(LoadAssetBundle(categoryDirPath + "/" + info + "/" + info));
            yield return new WaitUntil(() => isAssetBundleLoaded == true);

            assetBundle = loadedAssetBundle;

            //load the asset from the loaded asset bundle
            if (assetBundle != null) {

                asset = LoadAsset(assetBundle, info);

                if (asset != null) {

                    categoryAssetList.Add(new Asset(asset, info));
                }
            }
        }

        this.Return(category, categoryAssetList);
    }


    private void GetNetworkFolderPath() {

        //the server get his network folder path and synchronize it
        if (this.isServer) {

            string hostName, localIP;

            hostName = System.Net.Dns.GetHostName();
            localIP = System.Net.Dns.GetHostEntry(hostName).AddressList[1].ToString();

            this.networkFolderPath = "http://" + localIP + "/AssetBundles";
            this.isNetworkFolderPathAcquired = true;
        }
    }


    private void GetDirectoryInfo(Asset.Category category) {

        //the server get his local directory info and synchronize it
        if (this.isServer) {

            List<string> directoryInfo = new List<string>();

            string categoryDirPath = LOCAL_FOLDER_PATH + "\\" + category.ToString();

            DirectoryInfo categoryDir = new DirectoryInfo(categoryDirPath);
            DirectoryInfo[] categoryAssetBundles = categoryDir.GetDirectories();

            foreach (DirectoryInfo info in categoryAssetBundles) { directoryInfo.Add(info.Name); }

            this.ImportDirectoryInfo(category, directoryInfo);
        }
    }

    private void ImportDirectoryInfo(Asset.Category category, List<string> directoryInfo) {

        if (category == Asset.Category.Entity) {

            foreach (string info in directoryInfo) entityDirectoryInfo.Add(info);
            entityInfoAreAvaible = true;
        }

        else if (category == Asset.Category.Enviroment) {

            foreach (string info in directoryInfo) enviromentDirectoryInfo.Add(info);
            enviromentInfoAreAvaible = true;
        }

        else if (category == Asset.Category.Sound) {

            foreach (string info in directoryInfo) soundDirectoryInfo.Add(info);
            soundInfoAreAvaible = true;
        }
    }

    private List<string> ExportDirectoryInfo(Asset.Category category) {

        List<string> directoryInfo = new List<string>();

        if (category == Asset.Category.Entity) { foreach (string info in entityDirectoryInfo) directoryInfo.Add(info); }

        if (category == Asset.Category.Enviroment) { foreach (string info in enviromentDirectoryInfo) directoryInfo.Add(info); }

        if (category == Asset.Category.Sound) { foreach (string info in soundDirectoryInfo) directoryInfo.Add(info); }

        return directoryInfo;
    }


    private IEnumerator LoadAssetBundle(string assetBundlePath) {

        loadedAssetBundle = null;
        isAssetBundleLoaded = false;

        UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle(assetBundlePath);
        yield return www.SendWebRequest();

        //network error
        if (www.isNetworkError || www.isHttpError) { Debug.Log("Network Request failed: " + www.error); }

        //network success
        else {

            loadedAssetBundle = DownloadHandlerAssetBundle.GetContent(www);
            Debug.Log("Network Request approved");

            if (loadedAssetBundle == null) {

                Debug.Log("AssetBundle is Null");
            }

            else {

                Debug.Log("AssetBundle succesfully loaded:\n" + assetBundlePath);
            }
        }

        isAssetBundleLoaded = true;
    }

    private GameObject LoadAsset(AssetBundle assetBundle, string assetName) {

        GameObject asset = assetBundle.LoadAsset<GameObject>(assetName);

        if (asset == null) {

            Debug.Log("Failed to load Asset: " + assetName);
        }

        else {

            Debug.Log("Asset: " + assetName + " succesfully loaded");
        }

        return asset;
    }


    private void Return(Asset.Category category, List<Asset> categoryAssetList) {

        if (category == Asset.Category.Entity) {

            entityAssetList = categoryAssetList;
            entityAreLoaded = true;
        }

        else if (category == Asset.Category.Enviroment) {

            enviromentAssetList = categoryAssetList;
            enviromentAreLoaded = true;
        }

        else if (category == Asset.Category.Sound) {

            soundAssetList = categoryAssetList;
            soundAreLoaded = true;
        }
    }
}

