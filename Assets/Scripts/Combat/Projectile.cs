using System.Collections;
using Network;
using Unity.Netcode;
using UnityEngine;

namespace Combat
{
    public class Projectile : NetworkBehaviour
    {
        [SerializeField] private float _speed = 18f;
        [SerializeField] private int _damage = 20;

        private bool _isDestroying;

        private void Update()
        {
            transform.Translate(Vector3.forward * (_speed * Time.deltaTime));
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!IsServer || _isDestroying) return;
            
            if (!NetworkObject.IsSpawned) return;

            var target = other.GetComponent<PlayerNetwork>();
            
            if (target == null)
            {
                DestroyProjectile();
                return;
            }

            if (target.OwnerClientId == OwnerClientId) return;

            int newHp = Mathf.Max(0, target.HP.Value - _damage);
            target.HP.Value = newHp;

            DestroyProjectile();
        }

        private void DestroyProjectile()
        {
            if (_isDestroying) return;
            _isDestroying = true;
            
            if (NetworkObject != null && NetworkObject.IsSpawned)
            {
                NetworkObject.Despawn(destroy: true);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}