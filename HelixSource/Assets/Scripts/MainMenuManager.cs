using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Rewired;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour {

    private Player _player;

    [SerializeField]
    private UnityEngine.UI.Text _numberOfPlayersLabel;

    [SerializeField]
    private List<UnityEngine.UI.Text> _roundLabels;

	// Use this for initialization
	void Awake () {
        _player = ReInput.players.GetPlayer(0);
        AudioManager.Instance.StartMenuMusic();
	}
	
	// Update is called once per frame
	void Update () {
        UpdateUI();
        if(_player.GetButtonDown("IncreasePlayerNumber"))
        {
            GameSettingsManager.Instance.IncreasePlayerNumberIndex();
        }
        if(_player.GetButtonDown("DecreasePlayerNumber"))
        {
            GameSettingsManager.Instance.DecreasePlayerNumberIndex();
        }
        if(_player.GetButtonDown("PlayRounds1"))
        {
            GameSettingsManager.Instance.SetNumberOfRoundsIndex(0);
            SceneManager.LoadScene("IngameScene");
        }
        if(_player.GetButtonDown("PlayRounds2"))
        {
            GameSettingsManager.Instance.SetNumberOfRoundsIndex(1);
            SceneManager.LoadScene("IngameScene");
        }
        if(_player.GetButtonDown("PlayRounds3"))
        {
            GameSettingsManager.Instance.SetNumberOfRoundsIndex(2);
            SceneManager.LoadScene("IngameScene");
        }
        if(_player.GetButtonDown("Quit"))
        {
            Application.Quit();
        }

	}

    void UpdateUI()
    {
        _numberOfPlayersLabel.text = GameSettingsManager.Instance.GetNumberOfPlayers().ToString();
        _roundLabels[0].text = GameSettingsManager.Instance.GetNumberOfRoundsIndex(0).ToString();
        _roundLabels[1].text= GameSettingsManager.Instance.GetNumberOfRoundsIndex(1).ToString();
        _roundLabels[2].text = GameSettingsManager.Instance.GetNumberOfRoundsIndex(2).ToString();
    }
}
