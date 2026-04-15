using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerView : NetworkBehaviour
{
    [SerializeField] private PlayerNetwork playerNetwork;
    [SerializeField] private TMP_Text nicknameText;
    [SerializeField] private TMP_Text hpText;

    public override void OnNetworkSpawn()
    {
        // Подписываемся на изменения только после сетевого спавна объекта.
        playerNetwork.Nickname.OnValueChanged += OnNicknameChanged;
        playerNetwork.HP.OnValueChanged += OnHpChanged;

        // Сразу рисуем текущее состояние, чтобы UI не ждал первого сетевого события.
        OnNicknameChanged(default, playerNetwork.Nickname.Value);
        OnHpChanged(0, playerNetwork.HP.Value);
    }

    public override void OnNetworkDespawn()
    {
        // Отписка обязательна, чтобы не оставлять "висячие" обработчики.
        playerNetwork.Nickname.OnValueChanged -= OnNicknameChanged;
        playerNetwork.HP.OnValueChanged -= OnHpChanged;
    }

    private void OnNicknameChanged(FixedString32Bytes oldValue, FixedString32Bytes newValue)
    {
        nicknameText.text = newValue.ToString();
    }

    private void OnHpChanged(int oldValue, int newValue)
    {
        hpText.text = $"HP: {newValue}";
    }
}