using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : NetworkBehaviour
{
    [SerializeField] private PlayerNetwork playerNetwork;
    [SerializeField] private int damage = 10;

    private InputAction _attackAction;

    private void Start()
    {
        _attackAction = InputSystem.actions.FindAction("Attack");
    }

    private void Update()
    {
        if (!IsOwner) return;

        if (_attackAction.triggered)
        {
            TryAttack();
        }
    }

    private void TryAttack()
    {
        PlayerNetwork target = FindTarget();

        if (!target)
        {
            Debug.Log("No target");
            return;
        }

        DealDamageServerRpc(target.NetworkObjectId, damage);
    }

    private PlayerNetwork FindTarget()
    {
        PlayerNetwork[] players = FindObjectsByType<PlayerNetwork>(FindObjectsSortMode.None);

        foreach (var p in players)
        {
            if (p != playerNetwork)
                return p;
        }

        return null;
    }

    [ServerRpc]
    private void DealDamageServerRpc(ulong targetObjectId, int damage)
    {
        if (!NetworkManager.SpawnManager.SpawnedObjects.TryGetValue(targetObjectId, out NetworkObject targetObject))
            return;

        PlayerNetwork targetPlayer = targetObject.GetComponent<PlayerNetwork>();

        if (!targetPlayer || targetPlayer == playerNetwork)
            return;

        int newHp = Mathf.Max(0, targetPlayer.hp.Value - damage);
        targetPlayer.hp.Value = newHp;
    }
}