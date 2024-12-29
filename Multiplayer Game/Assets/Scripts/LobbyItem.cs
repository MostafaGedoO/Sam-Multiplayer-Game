using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI lobbyName;
    [SerializeField] private Button joinLobbyButton;

    public void InizilizeLobby(Lobby _lobby)
    {
        lobbyName.text = _lobby.Name;
        joinLobbyButton.onClick.AddListener(() =>
        {
            LobbyManager.Instance.Join(_lobby);
        });
    }
}
