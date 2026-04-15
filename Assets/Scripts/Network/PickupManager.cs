using System;
using System.Collections;
using Objects;
using UI;
using Unity.Netcode;
using UnityEngine;

namespace Network
{
    public class PickupManager : MonoBehaviour
    {
        [SerializeField] private GameObject healthPickupPrefab;
        [SerializeField] private Transform[] spawnPoints;
        [SerializeField] private float respawnDelay = 10f;

        private void OnEnable()
        {
            ConnectionUI.HostStarted += OnHostStarted;
        }

        private void OnDisable()
        {
            ConnectionUI.HostStarted -= OnHostStarted;
        }

        private void OnHostStarted()
        {
            if (NetworkManager.Singleton != null &&
                NetworkManager.Singleton.IsServer)
            {
                SpawnAll();
            }
        }

        public void OnPickedUp(Vector3 position)
        {
            StartCoroutine(Respawn(position));
        }

        private IEnumerator Respawn(Vector3 pos)
        {
            yield return new WaitForSeconds(respawnDelay);
            SpawnPickup(pos);
        }

        private void SpawnPickup(Vector3 pos)
        {
            if (!healthPickupPrefab)
            {
                Debug.LogError("Pickup prefab not assigned!");
                return;
            }

            var go = Instantiate(healthPickupPrefab, pos, Quaternion.identity);

            go.GetComponent<FirstAid>().Init(this);
            go.GetComponent<NetworkObject>().Spawn();
        }

        private void SpawnAll()
        {
            foreach (var point in spawnPoints)
            {
                SpawnPickup(point.position);
                Debug.Log("SpawnAll called, points: " + spawnPoints.Length);
            }
        }
    }
}