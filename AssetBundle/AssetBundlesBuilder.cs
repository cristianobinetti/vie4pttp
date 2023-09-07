using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class AssetBundlesBuilder : Editor {

    [MenuItem("Assets/Build AssetBundles")]
    private static void BuildMyAssetBundles() {

        BuildPipeline.BuildAssetBundles (

            @"C:\Assets\AssetBundles",
            BuildAssetBundleOptions.ChunkBasedCompression, 
            BuildTarget.StandaloneWindows64
        );
    }
}
