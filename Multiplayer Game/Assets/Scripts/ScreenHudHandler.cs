using TMPro;
using UnityEngine;

public class ScreenHudHandler : MonoBehaviour
{
    public static ScreenHudHandler Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI playerName;
    [SerializeField] private TextMeshProUGUI playerTailLength;
    [SerializeField] private GameObject winText;
    [SerializeField] private GameObject loseText;

    private void Awake()
    {
        Instance = this;
    }

    public void SetPlayerName(string _playerName)
    {
        playerName.text = _playerName;
    } 
    
    public void SetPlayerLength(string _playerName)
    {
        playerTailLength.text = _playerName;
    }

    public void SetPlayerEndState(bool _win)
    {
        if(_win)
        {
            winText.SetActive(true);
        }
        else
        {
            loseText.SetActive(true);
        }
    }

}
