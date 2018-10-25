using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour {

    private static GameManager _instance;

    public static GameManager Instance
    {
        get
        {
            return _instance;
        }
    }

    private System.IO.StreamWriter fileDatosPosicion;
    private System.IO.StreamWriter fileDatosDistancia;
    //private System.IO.StreamWriter fileDatosImagen;
    private GameObject[] _players;
    [SerializeField]
    private List<Sprite> _shipSprites;
    [SerializeField]
    private List<Color> _colors;
    [SerializeField]
    private List<string> _names;
    public GameObject Player;
    public Animator _cameraAnimator;
    public List<LimitsController> _limitsControllers;

    private int _numPlayers;

    private bool _gameInProgress = true;
    private bool fileOpen;
    private const float WORLD_CENTER = 7f;

    // Use this for initialization
    void Awake() {
        

        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        _numPlayers = GameSettingsManager.Instance.GetNumberOfPlayers();
        _players = new GameObject[_numPlayers];

        _cameraAnimator = GameObject.FindGameObjectWithTag("Camera").GetComponent<Animator>();

        //Randomize Color
        for (int i = 0; i < _colors.Count; i++)
        {
            int tmpIndex = Random.Range(0, _colors.Count - 1);
            Color tmpColor = _colors[tmpIndex];
            _colors[tmpIndex] = _colors[i];
            _colors[i] = tmpColor;
        }
        //Randomize Sprite
        for (int i = 0; i < _shipSprites.Count; i++)
        {
            int tmpIndex = Random.Range(0, _shipSprites.Count - 1);
            Sprite tmpSprite = _shipSprites[tmpIndex];
            _shipSprites[tmpIndex] = _shipSprites[i];
            _shipSprites[i] = tmpSprite;

        }
        //Randomize Name
        for (int i = 0; i < _names.Count; i++)
        {
            int tmpIndex = Random.Range(0, _names.Count - 1);
            string tmpSprite = _names[tmpIndex];
            _names[tmpIndex] = _names[i];
            _names[i] = tmpSprite;

        }

        //Initialice players
        for (int i = 0; i < _numPlayers; i++)
        {
            _players[i] = (GameObject)Instantiate(Player, null, true) as GameObject;
            _players[i].GetComponent<PlayerController>()._playerId = i;
            _players[i].GetComponent<PlayerController>()._playerColor = _colors[i];
            _players[i].GetComponent<PlayerController>().Name = _names[i];
            _players[i].GetComponent<PlayerController>()._cameraAnimator = _cameraAnimator;
            _players[i].GetComponent<PlayerController>().PlayerImage.GetComponent<SpriteRenderer>().sprite = _shipSprites[i];
        }

        PositionPlayers();

        AudioManager.Instance.StartIngameMusic();
    }

    public List<PlayerController> GetPlayerControllerList()
    {
        List<PlayerController> playerList = new List<PlayerController>();

        for (int i = 0; i < _players.Length; i++)
        {
            playerList.Add(_players[i].GetComponent<PlayerController>());
        }

        return playerList;
    }

    private void PositionPlayers()
    {
        fileDatosPosicion = new System.IO.StreamWriter(@"DatosPosicion.txt",true);
        fileDatosDistancia = new System.IO.StreamWriter(@"DatosDistancia.txt", true);
        //fileDatosImagen = new System.IO.StreamWriter(@"DatosImagen.txt", true);
        fileOpen = true;
        Debug.Log("openfile");
        float offset = WORLD_CENTER * 2 / (_numPlayers - 1);
        float currentXPosition = -WORLD_CENTER;

        List<GameObject> tempList = new List<GameObject>(_players);

        //Randomize player order
        for (int i = 0; i < tempList.Count; i++)
        {
            int tmpIndex = Random.Range(0, tempList.Count - 1);
            GameObject tmpGameObject = tempList[tmpIndex];
            tempList[tmpIndex] = tempList[i];
            tempList[i] = tmpGameObject;

        }

        for (int i = 0; i < _numPlayers; i++)
        {
            tempList[i].transform.position = new Vector3(currentXPosition, 0, 0);
            tempList[i].GetComponent<PlayerController>().ResetPlayer();
            currentXPosition += offset;
        }
    }

    public void CheckWinCondition()
    {
        StopCoroutine("CheckWinConditionLogic");
        StartCoroutine("CheckWinConditionLogic");
    }

    public IEnumerator CheckWinConditionLogic()
    {
        yield return new WaitForSeconds(1f);
        if (_gameInProgress)
        {
            List<PlayerController> playerList = GetPlayerControllerList();
            PlayerController alive = null;
            int numberOfAlive = 0;
            for (int i = 0; i < playerList.Count; i++) {
                if (!playerList[i].Dead)
                {
                    alive = playerList[i];
                    numberOfAlive++;
                }
            }

            if (numberOfAlive < 2)
            {
                if (alive == null)
                {
                    //draw
                    _gameInProgress = false;
                    UIManager.Instance.ShowDrawScreen();

                }
                else
                {
                    _gameInProgress = false;
                    alive.IncreaseScore();
                    UIManager.Instance.ShowWinScreen(alive);
                }
                fileOpen = false;
                fileDatosPosicion.Close();
                fileDatosDistancia.Close();
                //fileDatosImagen.Close();

            }
        }
    }

    public void ResetPlayers()
    {
        _gameInProgress = true;
        PositionPlayers();
        DeleteBombs();
        AudioManager.Instance.StartIngameMusic();
    }

    // Update is called once per frame
    void Update() {

        
        if (fileOpen)
        {
            List<PlayerController> playerList = GetPlayerControllerList();
            for (int i = 0; i < playerList.Count; i++)
            {

                

                if (!playerList[i].Dead)
                {

                    GameObject[] bombs = GameObject.FindGameObjectsWithTag("Bomb");
                    Vector2 bombpos;
                    bombpos.x = 10000;
                    bombpos.y = 10000;
                    float dist = 1000000;
                    for (int z = 0; z < bombs.Length; z++)
                    {

                        BombController bomb = bombs[z].GetComponent<BombController>();
                        if (bomb._bombId != playerList[i]._playerId)
                        {
                            float aux = Vector2.Distance(playerList[i].transform.position, bombs[z].transform.position);
                            if (aux < dist)
                            {
                                dist = aux;
                                bombpos = bombs[z].transform.position;
                            }
                        }

                        
                    }

                    int j = (i + 1) % playerList.Count;
                    dist = 1000000;
                    for (int z = 0; z < playerList.Count; z++)
                    {
                        if (z != i)
                        {
                            float aux = Vector2.Distance(playerList[i].transform.position, playerList[z].transform.position);
                            if (aux < dist)
                            {
                                dist = aux;
                                j = z;
                            }
                        }
                    }

                    
                    fileDatosPosicion.WriteLine(playerList[i].transform.position.x.ToString() + " " + playerList[i].transform.position.y.ToString() + " " + playerList[i]._rigidbody.velocity.x.ToString() + " " + playerList[i]._rigidbody.velocity.y.ToString() + " " + playerList[j].transform.position.x.ToString() + " " + playerList[j].transform.position.y.ToString() + " " + playerList[j]._rigidbody.velocity.x.ToString() + " " + playerList[j]._rigidbody.velocity.y.ToString() + " " + bombpos.x.ToString() + " " + bombpos.y.ToString());
                    fileDatosDistancia.WriteLine(Mathf.Sign(playerList[i].transform.position.y).ToString() + " " + playerList[i]._rigidbody.velocity.x.ToString() + " " + playerList[i]._rigidbody.velocity.y.ToString() + " " + (playerList[i].transform.position.x - playerList[j].transform.position.x).ToString() + " " + (playerList[i].transform.position.y - playerList[j].transform.position.y).ToString() + " " + Mathf.Sign(playerList[j].transform.position.y).ToString()  + " " + playerList[j]._rigidbody.velocity.x.ToString() + " " + playerList[j]._rigidbody.velocity.y.ToString() + " " + (playerList[i].transform.position.x - bombpos.x).ToString() + " " + (playerList[i].transform.position.y - bombpos.y).ToString());

                    float _yAxisValue = playerList[i]._player.GetAxis("Propulse");
                    float _xAxisValue = playerList[i]._player.GetAxis("HorizontalAxis");
                    bool _attack = playerList[i]._player.GetButtonDown("Attack");

                    string input = _yAxisValue.ToString() + " " + _xAxisValue.ToString() + " " + _attack.ToString();
                    fileDatosPosicion.WriteLine(input);
                    fileDatosDistancia.WriteLine(input);
                   // fileDatosImagen.WriteLine(input);
                }
            }
        }
        
    }

    public string getPlayerData(int i)
    {
        string str = "";
        List<PlayerController> playerList = GetPlayerControllerList();
        GameObject[] bombs = GameObject.FindGameObjectsWithTag("Bomb");
    
        Vector2 bombpos;
        bombpos.x = 0;
        bombpos.y = 0;
        float dist = 1000000;
        for (int z = 0; z < bombs.Length; z++)
        {

            BombController bomb = bombs[z].GetComponent<BombController>();
            if (bomb._bombId != playerList[i]._playerId)
            {
                float aux = Vector2.Distance(playerList[i].transform.position, bombs[z].transform.position);
                if (aux < dist)
                {
                    dist = aux;
                    bombpos = bombs[z].transform.position;
                }
            }


        }

        int j = (i + 1) % playerList.Count;
        dist = 1000000;
        for (int z = 0; z < playerList.Count; z++)
        {
            if (z != i)
            {
                float aux = Vector2.Distance(playerList[i].transform.position, playerList[z].transform.position);
                if (aux < dist)
                {
                    dist = aux;
                    j = z;
                }
            }
        }


        str = playerList[i].transform.position.x.ToString() + " " + playerList[i].transform.position.y.ToString() + " " + playerList[i]._rigidbody.velocity.x.ToString() + " " + playerList[i]._rigidbody.velocity.y.ToString() + " " + playerList[j].transform.position.x.ToString() + " " + playerList[j].transform.position.y.ToString() + " " + playerList[j]._rigidbody.velocity.x.ToString() + " " + playerList[j]._rigidbody.velocity.y.ToString() + " " + bombpos.x.ToString() + " " + bombpos.y.ToString();
             
        return str;
    }

    private void DeleteBombs()
    {
        GameObject[] bombs = GameObject.FindGameObjectsWithTag("Bomb");

        for (int i = 0; i < bombs.Length; i++)
        {
            Destroy(bombs[i]);
        }
    }

    public void StopWalls()
    {
        foreach (LimitsController limitController in _limitsControllers)
        {
            limitController.StopMovement();
        }
    }

    public void ResetWalls()
    {
        foreach (LimitsController limitController in _limitsControllers)
        {
            limitController.ResetLimit();
        }
    }

    public bool ReachedRoundScore()
    {
        List<PlayerController> playerList = GetPlayerControllerList();
        foreach (PlayerController player in playerList)
        {
            if(player.GetScore() == GameSettingsManager.Instance.GetNumberOfRounds())
            {
                return true;
            }
        }

        return false;
    }

    public PlayerController GetRoundWinnerPlayer()
    {
        List<PlayerController> playerList = GetPlayerControllerList();
        PlayerController winningPlayer = null;
        foreach (PlayerController player in playerList)
        {
            if(player.GetScore() == GameSettingsManager.Instance.GetNumberOfRounds())
            {
                winningPlayer = player;
            }
        }

        return winningPlayer;
    }
}

