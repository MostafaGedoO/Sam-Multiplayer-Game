using TMPro;
using UnityEngine;

public class ScreenHudHandler : MonoBehaviour
{
    public static ScreenHudHandler Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI playerName;
    [SerializeField] private TextMeshProUGUI playerTailLength;

    private void Awake()
    {
        Instance = this;
    }

    public void SetPlayerName(string _playerName)
    {
        playerName.text = _playerName;
    }

}
