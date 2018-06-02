using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Rewired;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour {

    public GameObject _roundWinnerScreenPanel;
    public GameObject _drawScreenPanel;
    public GameObject _winningScreenPanel;
    public GameObject _scoresScreenPanel;
    public UnityEngine.UI.Text _winningPlayerText;
    public UnityEngine.UI.Text _roundWinningPlayerText;
    public GameObject _returnToMainMenuText;

    private bool _reachedRoundEnd = false;
    private bool _screenShown = false;
    private Player _player;

    public static UIManager _instance;

    public List<UnityEngine.UI.Image> _playerSprites;
    public List<UnityEngine.UI.Text> _scoreLabels;
    public List<UnityEngine.UI.Text> _playerNames;

    public static UIManager Instance
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
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        HideScreens();
        _player = ReInput.players.GetPlayer(0);
	}
	
	// Update is called once per frame
	void Update () {
        if(_screenShown && _player.GetButtonDown("Attack"))
        {
            if(!GameManager.Instance.ReachedRoundScore())
            {
                HideScreens();
                GameManager.Instance.ResetPlayers();
                GameManager.Instance.ResetWalls();
            }
            else
            {
                ShowRoundWinScreen(GameManager.Instance.GetRoundWinnerPlayer());
            }
        }

        if(_reachedRoundEnd && _player.GetButtonDown("Attack"))
        {
            SceneManager.LoadScene("MenuScene");
        }

        if(_player.GetButtonDown("MainMenu"))
        {
            SceneManager.LoadScene("MenuScene");
        }
	}

//    public void CheckPlayerHasWon()
//    {
//        StartCoroutine(CheckPlayerHasWonCoroutine());
//    }
//
//    private IEnumerator CheckPlayerHasWonCoroutine()
//    {
//        yield return new WaitForSeconds(2f);
//        PlayerController[] players = FindObjectsOfType<PlayerController>();
//
//        if(players.Length == 1) // todo
//        {
//            ShowGameWinningScreen(players[0]);
//        }
//    }
//
    public void HideScreens()
    {
        _screenShown = false;
        _drawScreenPanel.SetActive(false);
        _scoresScreenPanel.SetActive(false);
        _winningScreenPanel.SetActive(false);
        _roundWinnerScreenPanel.SetActive(false);
    }


    public void ShowDrawScreen()
    {
        _drawScreenPanel.SetActive(true);
        _scoresScreenPanel.SetActive(true);
        _winningScreenPanel.SetActive(false);
        _roundWinnerScreenPanel.SetActive(false);
        GameManager.Instance.StopWalls();

        _screenShown = true;


        RefreshScores();
    }

    public void ShowWinScreen(PlayerController winningPlayer)
    {
        _winningScreenPanel.SetActive(true);
        _scoresScreenPanel.SetActive(true);
        _drawScreenPanel.SetActive(false);
        _roundWinnerScreenPanel.SetActive(false);
        GameManager.Instance.StopWalls();

        _screenShown = true;

        _winningPlayerText.text = winningPlayer.Name + "!";
        _winningPlayerText.color = winningPlayer.PlayerColor;

        RefreshScores();
    }

    public void ShowRoundWinScreen(PlayerController winningPlayer)
    {
        _winningScreenPanel.SetActive(false);
        _scoresScreenPanel.SetActive(false);
        _drawScreenPanel.SetActive(false);
        _roundWinnerScreenPanel.SetActive(true);

        _returnToMainMenuText.SetActive(false);

        GameManager.Instance.StopWalls();

        _screenShown = true;
        StartCoroutine("EnableReturnToMain");

        _roundWinningPlayerText.text = winningPlayer.Name + "!";
        _roundWinningPlayerText.color = winningPlayer.PlayerColor;

        //RefreshScores();
    }

    public IEnumerator EnableReturnToMain()
    {
        yield return new WaitForSeconds(3f);
        _reachedRoundEnd = true;
        _returnToMainMenuText.SetActive(true);
    }

    private void RefreshScores()
    {
        List<PlayerController> playerList = GameManager.Instance.GetPlayerControllerList();
        for (int i = 0; i < _scoreLabels.Count; i++)
        {
            if(i < playerList.Count)
            {
                _playerSprites[i].sprite = playerList[i].PlayerImage.GetComponent<SpriteRenderer>().sprite;
                _playerSprites[i].color = playerList[i].PlayerColor;

                _playerNames[i].text = playerList[i].Name;
                _playerNames[i].color = playerList[i].PlayerColor;

                _scoreLabels[i].text = playerList[i].GetScore().ToString();
                _scoreLabels[i].color = playerList[i].PlayerColor;

            }
            else
            {
                _playerSprites[i].gameObject.SetActive(false);
                _scoreLabels[i].gameObject.SetActive(false);
                _playerNames[i].gameObject.SetActive(false);
            }
        }
    }
}
