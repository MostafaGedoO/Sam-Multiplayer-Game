using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager instance;

    [SerializeField] private GameObject foodPrefabe;
    [SerializeField] private int foodToSpawn = 50;

    private Vector2 minMaxXY = new(15, 10);

    public override async void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        instance = this;

        if(IsServer)
        {
            while(NetworkManager.ConnectedClients.Count < 2)
            {
                await Task.Delay(500);
            }

            for(int i = 0; i < foodToSpawn; i++)
            {
               GameObject _food = Instantiate(foodPrefabe,new Vector3(Random.Range(-minMaxXY.x,minMaxXY.x),Random.Range(-minMaxXY.y + 1,minMaxXY.y),0),Quaternion.identity);
                _food.GetComponent<NetworkObject>().Spawn();
            }
        }
    }

    [ServerRpc]
    public void UpdateGameStateServerRpc()
    {
        UpdateGameStateClientRpc();
        Debug.Log("Server is Checking The Game state");
    }

    [ClientRpc]
    private void UpdateGameStateClientRpc()
    {
        Debug.Log("Client is Checking The Game state");
        Food[] _foodObjects = FindObjectsByType<Food>(FindObjectsSortMode.None);
        Debug.Log("Foods Remaining: " + _foodObjects.Length);
        if (_foodObjects.Length == 0)
        {
            PlayerController[] players = FindObjectsByType<PlayerController>(FindObjectsSortMode.None);
            int _maxPlayerIndex = 0;

            for (int i = 0; i < players.Length - 1; i++)
            {
                if (players[i].GetTailValue() < players[i + 1].GetTailValue())
                {
                    _maxPlayerIndex = i + 1;
                }
            }

            for (int i = 0; i < players.Length; i++)
            {
                if (i != _maxPlayerIndex)
                {
                    players[i].SetPlayerEndState(false);
                }
            }

            players[_maxPlayerIndex].SetPlayerEndState(true);
        }
    }
}
