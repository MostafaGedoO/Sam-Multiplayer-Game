using System;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{
    public static LobbyManager Instance { get; private set; }

    [SerializeField] private TMP_InputField lobbyNameField;

    private Allocation allocation;
    private JoinAllocation joinAllocation;

    private string joinCode;


    private async void Start()
    {
        Instance = this;
        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    public async void StartHostAsync()
    {
        allocation = await RelayService.Instance.CreateAllocationAsync(4);

        joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

        Debug.Log(joinCode);

        CreateLobbyOptions _options = new CreateLobbyOptions();
        _options.IsPrivate = false;
        _options.Data = new System.Collections.Generic.Dictionary<string, DataObject>()
        {
            {"JoinCode" , new DataObject(DataObject.VisibilityOptions.Member,joinCode) }
        };

        Lobby _lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyNameField.text,4,_options);

        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(
          allocation.RelayServer.IpV4,
          (ushort)allocation.RelayServer.Port,
          allocation.AllocationIdBytes,
          allocation.Key,
          allocation.ConnectionData
      );

        NetworkManager.Singleton.StartHost();
        NetworkManager.Singleton.SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
    }

    public async void Join(Lobby _lobby)
    {

        Lobby _joinLobby  = await LobbyService.Instance.JoinLobbyByIdAsync( _lobby.Id );
        joinCode = _joinLobby.Data["JoinCode"].Value;

        try
        {
            joinAllocation =  await RelayService.Instance.JoinAllocationAsync(joinCode);
        }
        catch(Exception ex)
        {
            Debug.LogException(ex);
        }

        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(
            joinAllocation.RelayServer.IpV4,
            (ushort)joinAllocation.RelayServer.Port,
            joinAllocation.AllocationIdBytes,
            joinAllocation.Key,
            joinAllocation.ConnectionData,
            joinAllocation.HostConnectionData
        );

        NetworkManager.Singleton.StartClient();
    }
}
