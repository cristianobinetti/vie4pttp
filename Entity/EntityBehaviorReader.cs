using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;

public class EntityBehaviorReader : NetworkBehaviour {

//INST VAR

    //macros
    private const string DIRECTORY_PATH = @"C:\\xampp\\htdocs\\AssetBundles\\Entity";
    private const string FILE_NAME = "Behavior";
    private const string TXT_EXTENTION = ".txt";

    private const int FEATURES_NUMBER = 5;

    private const string NATURE_TITLE = "NATURE";
    private const string NATURE_VALUE_0 = "defensive";
    private const string NATURE_VALUE_1 = "passive";
    private const string NATURE_VALUE_2 = "aggressive";

    private const string DIMENSION_TITLE = "DIMENSION";
    private const string DIMENSION_VALUE_0 = "tiny";
    private const string DIMENSION_VALUE_1 = "avarage";
    private const string DIMENSION_VALUE_2 = "big";

    private const string MOVEMENT_TITLE = "MOVMENT";
    private const string MOVEMENT_VALUE_0 = "fast";
    private const string MOVEMENT_VALUE_1 = "avarage";
    private const string MOVEMENT_VALUE_2 = "slow";

    private const string VISION_TITLE = "VISION";
    private const string VISION_VALUE_0 = "small";
    private const string VISION_VALUE_1 = "avarage";
    private const string VISION_VALUE_2 = "wide";

    private const string FLYING_TITLE = "FLYING";
    private const string FLYING_VALUE_0 = "no";
    private const string FLYING_VALUE_1 = "yes";

    private const string VOID_VALUE = "VOID";

    //variables
    [HideInInspector]
    public List<int[]> behaviorCodeList;
    [HideInInspector]
    public bool areBehaviorReady = false;

//UNITY METH

    void Start() {

        if (this.isServer) {

            behaviorCodeList = new List<int[]>();

            DirectoryInfo entityDir = new DirectoryInfo(DIRECTORY_PATH);
            DirectoryInfo[] entityAssetBundles = entityDir.GetDirectories();
     
            foreach (DirectoryInfo entity in entityAssetBundles) { behaviorCodeList.Add(readFile(DIRECTORY_PATH + "\\" + entity.Name + "\\" + FILE_NAME + TXT_EXTENTION)); }

            areBehaviorReady = true;
        }
    }

    private int[] readFile(string file_path) {

        int[] behaviorCode = new int[FEATURES_NUMBER];
        for (int i = 0; i < FEATURES_NUMBER; i++) behaviorCode[i] = 0;

        if (File.Exists(file_path)) {

            StreamReader inputStream = new StreamReader(file_path);

            string fileLine;
            int lineCounter = 0;

            while (!inputStream.EndOfStream) {

                fileLine = inputStream.ReadLine();

                if (lineCounter == 0) behaviorCode[lineCounter] = fetureReader(fileLine, NATURE_TITLE, NATURE_VALUE_0, NATURE_VALUE_1, NATURE_VALUE_2);
                if (lineCounter == 1) behaviorCode[lineCounter] = fetureReader(fileLine, DIMENSION_TITLE, DIMENSION_VALUE_0, DIMENSION_VALUE_1, DIMENSION_VALUE_2);
                if (lineCounter == 2) behaviorCode[lineCounter] = fetureReader(fileLine, MOVEMENT_TITLE, MOVEMENT_VALUE_0, MOVEMENT_VALUE_1, MOVEMENT_VALUE_2);
                if (lineCounter == 3) behaviorCode[lineCounter] = fetureReader(fileLine, VISION_TITLE, VISION_VALUE_0, VISION_VALUE_1, VISION_VALUE_2);
                if (lineCounter == 4) behaviorCode[lineCounter] = fetureReader(fileLine, FLYING_TITLE, FLYING_VALUE_0, FLYING_VALUE_1, VOID_VALUE);

                lineCounter++;
            }

            inputStream.Close();
        }

        else Debug.Log("File not found");

        return behaviorCode;
    }

//METH

    private int fetureReader(string featureLine, string feature, string value0, string value1, string value2) {

        if (featureLine.Contains(feature)) {

            if (featureLine.Contains(value0)) return 0;

            else if (featureLine.Contains(value1)) return 1;

            else if (featureLine.Contains(value2)) return 2;

            else return 0;
        }

        else return 0;
    }
}