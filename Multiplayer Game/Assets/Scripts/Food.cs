using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

public class Food : NetworkBehaviour
{
    private async void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && IsServer)
        {
            if(collision.TryGetComponent<PlayerController>(out var controller))
            {
                controller.IncreaseSnakeTail();
                NetworkObject.Despawn();
                Destroy(gameObject);
                await Task.Delay(100);
                GameManager.instance.UpdateGameStateServerRpc();
            }
        }
    }
}
