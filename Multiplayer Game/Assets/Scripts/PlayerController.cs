using UnityEngine;
using Unity.Netcode;
using TMPro;
using System;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] private float moveSpeed = 10;
    [SerializeField] private TextMeshPro playerNumber;
    [SerializeField] private string[] playerNames;
    [SerializeField] private Color[] playersColors;
    
    [Header("Tail")]
    [SerializeField] private TrailRenderer trailRenderer;
    private NetworkVariable<int> tailLength = new NetworkVariable<int>(1);

    private Vector2 mousePosition;
    private Vector3 mouseWorldPosition;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        GetComponentInChildren<SpriteRenderer>().color = playersColors[OwnerClientId];
        playerNumber.text = OwnerClientId.ToString();
        trailRenderer.startColor = playersColors[OwnerClientId];
        trailRenderer.endColor = playersColors[OwnerClientId];

        if (IsOwner)
        {
            if(ScreenHudHandler.Instance == null)
            {
                Invoke("HandlePlayerName", 0.25f);
            }
            else
            {
                ScreenHudHandler.Instance.SetPlayerName(playerNames[OwnerClientId]);
            }
        }

        tailLength.OnValueChanged += OnTailLengthChanged;
        Debug.Log("Player: " + OwnerClientId + " Initial Tail Is: " + tailLength.Value);

        UpdateTrailRenderer();
    }

    private void HandlePlayerName()
    {
        if (ScreenHudHandler.Instance == null)
        {
            Invoke("HandlePlayerName", 0.25f);
        }
        else
        {
            ScreenHudHandler.Instance.SetPlayerName(playerNames[OwnerClientId]);
        }
    }

    private void OnTailLengthChanged(int previousValue, int newValue)
    {
        Debug.Log($"Player {OwnerClientId}: Tail length changed from {previousValue} to {newValue}");
        UpdateTrailRenderer();

        if (IsOwner)
        {
            ScreenHudHandler.Instance.SetPlayerLength(tailLength.Value.ToString());
        }
    }

    private void UpdateTrailRenderer()
    {
        trailRenderer.time = tailLength.Value / 4f;
    }

    public void IncreaseSnakeTail()
    {
        IncreaseSnakeTailServerRpc();
    }

    [Rpc(SendTo.Server)]
    private void IncreaseSnakeTailServerRpc()
    {
        tailLength.Value++;
    }

    private void Update()
    {
        if (!Application.isFocused) return;

        if(!IsOwner) return;

        MovePlayer();
    }

    private void MovePlayer()
    {
        mousePosition = Input.mousePosition;
        mouseWorldPosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x,mousePosition.y,0));
        mouseWorldPosition.z = 0;

        transform.position = Vector3.MoveTowards(transform.position, mouseWorldPosition , moveSpeed * Time.deltaTime);

        if(transform.position != mouseWorldPosition)
        {
            Vector3 rotateDirection = mouseWorldPosition - transform.position;
            transform.up = rotateDirection;
        }
    }

    public int GetTailValue()
    {
        return tailLength.Value;
    }

    public void SetPlayerEndState(bool _state)
    {
        if(!IsOwner) return;
        ScreenHudHandler.Instance.SetPlayerEndState(_state);
        enabled = false;
    }
}
