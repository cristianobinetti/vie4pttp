using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetManager : MonoBehaviour {

    [SerializeField]
    private GameObject netServer;
    [SerializeField]
    private GameObject netClient;

    void Start() {

	#if UNITY_STANDALONE

		this.netServer.SetActive(true);
		this.netClient.SetActive(false);

	#endif

	#if UNITY_ANDROID

		this.netServer.SetActive(false);
		this.netClient.SetActive(true);

	#endif
    }
}

