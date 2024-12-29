using Unity.Services.Lobbies;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Lobbies.Models;

public class LobbiesList : MonoBehaviour
{
    [SerializeField] private LobbyItem lobbyItemPrefab;
    [SerializeField] private Transform parentContainer;

    private void OnEnable()
    {
        RefreshLobbiesList();
    }

    public async void RefreshLobbiesList()
    {
        //Get All The Lobbies
        QueryLobbiesOptions _options = new QueryLobbiesOptions();
        _options.Count = 5;
        _options.Filters = new List<QueryFilter>() { new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0" ,QueryFilter.OpOptions.GT) };

        QueryResponse _lobbiesResponse =  await LobbyService.Instance.QueryLobbiesAsync(_options);

        foreach(Transform _child in parentContainer)
        {
            Destroy(_child.gameObject);
        }

        foreach(Lobby _lobby in _lobbiesResponse.Results)
        {
            LobbyItem _lobbyItem = Instantiate(lobbyItemPrefab, parentContainer);
            _lobbyItem.InizilizeLobby(_lobby);
        }
    }
}
