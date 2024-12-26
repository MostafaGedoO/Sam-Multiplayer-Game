using System;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField codeInputFeild;

    private Allocation allocation;
    private JoinAllocation joinAllocation;
    
    public string JoinCode {  get; private set; }


    private async void Start()
    {
        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    public async void StartHostAsync()
    {
        allocation = await RelayService.Instance.CreateAllocationAsync(4);

        JoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

        Debug.Log(JoinCode);

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

    public async void Join()
    {
        try
        {
            joinAllocation =  await RelayService.Instance.JoinAllocationAsync(codeInputFeild.text);
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
