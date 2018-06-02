using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettingsManager : MonoBehaviour {

    private static GameSettingsManager _instance;

    private const int MAX_NUMBER_PLAYERS = 8;

    [SerializeField]
    private List<int> _playerNumbers;
    [SerializeField]
    private List<int> _numberRoundsPossibilities;

    private int _playerNumberIndex = 0;
    private int _roundNumberIndex = 0;

    public static GameSettingsManager Instance
    {
        get
        {
            return _instance;
        }
    }

	// Use this for initialization
	void Start () {
        if(_instance == null)
        {
            DontDestroyOnLoad(this);
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
	}

    public int GetMaxNumberOfPlayers()
    {
        return MAX_NUMBER_PLAYERS;
    }

    public int GetNumberOfPlayers()
    {
        return _playerNumbers[_playerNumberIndex];
    }

    public int GetNumberOfRounds()
    {
        return _numberRoundsPossibilities[_roundNumberIndex];
    }

    public int GetNumberOfRoundsIndex(int index)
    {
        return _numberRoundsPossibilities[index];
    }

    public void SetNumberOfPlayersIndex(int index)
    {
        _playerNumberIndex = index;
    }

    public void SetNumberOfRoundsIndex(int index)
    {
        _roundNumberIndex = index;
    }
	
    public void IncreasePlayerNumberIndex()
    {
        _playerNumberIndex++;
        _playerNumberIndex = Mathf.Clamp(_playerNumberIndex, 0, _playerNumbers.Count - 1);

//        if(_playerNumberIndex >= _playerNumbers.Count)
//        {
//            _playerNumberIndex = 0;
//        }
    }

    public void DecreasePlayerNumberIndex()
    {
        _playerNumberIndex--;
        _playerNumberIndex = Mathf.Clamp(_playerNumberIndex, 0, _playerNumbers.Count - 1);

//        if(_playerNumberIndex <= 0f)
//        {
//            _playerNumberIndex = _playerNumbers.Count - 1;
//        }
    }
}
