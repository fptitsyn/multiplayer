using System;
using Unity.Netcode;
using UnityEngine;

public class NetworkTest : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            NetworkManager.Singleton.StartHost();
            Debug.Log("StartHost");
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            NetworkManager.Singleton.StartClient();
            Debug.Log("StartClient");
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            NetworkManager.Singleton.Shutdown();
            Debug.Log("Shutdown");
        }
    }
}
