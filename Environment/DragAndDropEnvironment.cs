using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Animations;
using UnityEngine.UI;
using UnityEngine.AI;


public class DragAndDropEnvironment : MonoBehaviour, IDragHandler, IEndDragHandler {

//INST VAR

    private GameObject assetBundleManager;
    private AssetLoader assetLoader;

    private GameObject enviromentSpawnManager;
    private EnviromentSpawning enviromentSpawning;

    private GameObject hierarchyGUI;

    private BoxCollider EnvironmentCollider;
    private Camera overHeadCamera;
    Ray overHeadCameraRay;
    RaycastHit overHeadCameraRayHit;

    private Vector3 dragPosition, dropPosition;

    [SerializeField]
    public Texture2D cursorCorrectTexture;
    [SerializeField]
    public Texture2D cursorWrongTexture;
    [SerializeField]
    private CursorMode cursorMode = CursorMode.ForceSoftware;
    private Vector2 hotSpot = Vector2.zero;

    private List<Asset> enviromentAssetList;

    private int spawnIndex;

    private bool areAssetsReaded = false;

    private NavMeshSurface tinySurface;
    private NavMeshSurface avarageSurface;
    private NavMeshSurface bigSurface;
    private NavMeshSurface flyingSurface;

//UNITY METH

    void Start() {
        
        assetBundleManager = GameObject.Find("Asset Bundle Manager");
        assetLoader = assetBundleManager.GetComponent<AssetLoader>();

        enviromentSpawnManager = GameObject.Find("Enviroment Spawn Manager");
        enviromentSpawning = enviromentSpawnManager.GetComponent<EnvironmentSpawning>();

        hierarchyGUI = GameObject.Find("Hierarchy");

        overHeadCamera = GameObject.Find("Over Head Camera").GetComponent<Camera>();
        EnvironmentCollider = GameObject.FindGameObjectWithTag("Environment").GetComponent<BoxCollider>();

        tinySurface = GameObject.Find("Tiny Surface").GetComponent<NavMeshSurface>();
        avarageSurface = GameObject.Find("Avarage Surface").GetComponent<NavMeshSurface>();
        bigSurface = GameObject.Find("Big Surface").GetComponent<NavMeshSurface>();
        flyingSurface = GameObject.Find("Flying Surface").GetComponent<NavMeshSurface>();
    }

    void FixedUpdate() {

        //client and server load the asset from the asset loader
        if (assetLoader.areAssetsLoaded && !this.areAssetsReaded) {

            this.enviromentAssetList = assetLoader.enviromentAssetList;
            areAssetsReaded = true;
        }
    }

    public void OnDrag(PointerEventData eventData) {

        if (areAssetsReaded) {

            dragPosition = Input.mousePosition;
            dragPosition.z = overHeadCamera.transform.position.y;

            Cursor.SetCursor(cursorCorrectTexture, hotSpot, cursorMode);
            dropPosition = overHeadCamera.ScreenToWorldPoint(dragPosition);

            //out of boundaries
            if (!EnvironmentCollider.bounds.Contains(dropPosition)) { Cursor.SetCursor(cursorWrongTexture, hotSpot, cursorMode); }
        }
    }

    public void OnEndDrag(PointerEventData eventData) {

        if (areAssetsReaded) {

            dragPosition = Input.mousePosition;
            dragPosition.z = overHeadCamera.transform.position.y; //distance of the plane from the camera
            dropPosition = overHeadCamera.ScreenToWorldPoint(dragPosition);

            spawnIndex = int.Parse(this.transform.parent.name.Substring(this.transform.parent.name.Length - 1));

            //spawn in boundaries
            if (EnvironmentCollider.bounds.Contains(dropPosition)) {

                GameObject oldEnviroment = GameObject.FindGameObjectWithTag("Environment");
                Destroy(oldEnviroment);

                GameObject enviromentSpawned;
                enviromentSpawned = enviromentSpawning.SpawnLogic(spawnIndex, dropPosition, Quaternion.identity);

                enviromentSpawned.name = enviromentAssetList[spawnIndex].getAssetName();
                setEnviromentAttributes(enviromentSpawned);
                rebakeEnviroment();
            }
            
            Cursor.SetCursor(default, hotSpot, cursorMode);
        }
    }

    //METH

    private static void setEnviromentAttributes(GameObject enviroment)
    {

        enviroment.tag = "Environment";

        if (enviroment.GetComponent<BoxCollider>() == null) enviroment.AddComponent<BoxCollider>();
        enviroment.GetComponent<BoxCollider>().size = new Vector3(250, 1,250);
    }

    private void rebakeEnviroment () {

        this.tinySurface.BuildNavMesh();
        this.avarageSurface.BuildNavMesh();
        this.bigSurface.BuildNavMesh();
        this.flyingSurface.BuildNavMesh();
    }
}