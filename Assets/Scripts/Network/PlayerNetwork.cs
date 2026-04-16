using System;
using System.Collections;
using UI;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Network
{
    public class PlayerNetwork : NetworkBehaviour
    {
        [SerializeField] private GameObject model;

        public Action PlayerDied;
    
        // Ник должен быть виден всем клиентам, но менять его может только сервер.
        public NetworkVariable<FixedString32Bytes> Nickname = new(
            default,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server
        );

        // HP тоже читает каждый клиент, но изменяется только на сервере.
        public NetworkVariable<int> HP = new(
            100,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server
        );
    
        public NetworkVariable<int> Ammo = new(
            10,
            NetworkVariableReadPermission.Owner,
            NetworkVariableWritePermission.Server
        );
    
        public NetworkVariable<bool> IsAlive = new(
            true,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server
        );

        public override void OnNetworkSpawn()
        {
            if (IsOwner)
            {
                // Только владелец отправляет на сервер свой локально введенный ник.
                SubmitNicknameServerRpc(ConnectionUI.PlayerNickname);
            }
        
            if (IsServer)
            {
                transform.position = new Vector3(
                    Random.Range(-3f, 3f),
                    0.4f,
                    0
                );
            }
        
            HP.OnValueChanged += OnHpChanged;
            IsAlive.OnValueChanged += OnIsAliveChanged;
        }

        public override void OnNetworkDespawn()
        {
            HP.OnValueChanged -= OnHpChanged;
            IsAlive.OnValueChanged -= OnIsAliveChanged;
        }

        private void OnHpChanged(int oldValue, int newValue)
        {
            if (!IsServer) return;
            if (newValue <= 0 && IsAlive.Value)
            {
                IsAlive.Value = false;
                PlayerDied?.Invoke();
                StartCoroutine(RespawnRoutine());
            }
        }
    
        [ServerRpc(InvokePermission = RpcInvokePermission.Everyone)]
        private void SubmitNicknameServerRpc(string nickname)
        {
            // Сервер нормализует ник и записывает итоговое значение в NetworkVariable.
            string safeValue = string.IsNullOrWhiteSpace(nickname) ? $"Player_{OwnerClientId}" : nickname.Trim();
            Nickname.Value = safeValue;
        }
    
        private IEnumerator RespawnRoutine()
        {
            yield return new WaitForSeconds(3);

            Vector3 spawnPos = PlayerSpawner.Instance.GetSpawnPosition();

            transform.position = spawnPos;

            HP.Value = 100;
            Ammo.Value = 10;
            yield return null;
            IsAlive.Value = true;
        }
    
        private void OnIsAliveChanged(bool prev, bool next)
        {
            if (model == null)
            {
                Debug.LogError("Model not assigned!");
                return;
            }

            model.SetActive(next);
        }
    }
}