using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
using System;
using System.Globalization;

public class PlayerController : MonoBehaviour {

    public GameObject Bomb;
    public GameObject Particles2D;
    public GameObject Particles2DHorizon;
    public GameObject PlayerImage;
    private GameObject Popup;

    [SerializeField]
    public int _playerId;
    [SerializeField]
    public int _bombAmount;
    [SerializeField]
    public Color _playerColor = Color.white;
    [SerializeField]
    private float _gravityScale = 0.92f;
    [SerializeField]
    private float _vForce = 0.5f;
    [SerializeField]
    private float _hForce = 0.5f;
    [SerializeField]
    private float _turningScale = 5f;
    [SerializeField]
    private DrawPlayerPath _playerPath;
    [SerializeField]
    private float _minYVel = 0.05f;
    [SerializeField]
    private float _hDeadZone = 0.1f;
    [SerializeField]
    private float _InputDeadZone = 0.01f;
    [SerializeField]
    private float MAX_HORIZONTAL_VELOCITY = 10;
    [SerializeField]
    private float MAX_VERTICAL_VELOCITY = 10;
    [SerializeField]
    private float MIN_VERTICAL_VELOCITY = 0;
    [SerializeField]
    public Animator _cameraAnimator;

    private bool _horizontalParticlesDisplayed = true;
    

    private const float MINIMUM_VERTICAL_SPEED = 2.0f;

    private const float IDLE_TIME_DEATH = 3.0f;

    public Rigidbody2D _rigidbody;
    private int _started = 0;
    private float _tmp;
    private Vector3 _playerViewportPos;
    public Camera cam;
    public Player _player;

    private float _timingDeath;

    private float _xAxisValue;
    private float _yAxisValue;
    private bool atk;
    private float _angle;
    private string _name;
    private int _consecutiveKills = 0;
    networkSocket nt;
    public string Name
    {
        get
        {
            return _name;
        }
        set
        {
            _name = value;
        }
    }

    private bool _dead = false;

    private int _score = 0;
    [SerializeField]
    private ParticleSystem _particleSystem;
    private ParticleSystem.EmissionModule em;
    private bool dataSended;
    private float next_move;
    private float move_freq = 0.1f;

    public bool Dead
    {
        get
        {
            return _dead;
        }
    }

    public Color PlayerColor
    {
        get
        {
            return _playerColor;
        }
    }

    public void IncreaseScore()
    {
        _score++;
    }

    public void ResetScore()
    {
        _score = 0;
    }

    public int GetScore()
    {
        return _score;
    }
    
    void Awake() {
        
        //_playerImage = transform.Find("PlayerImage").gameObject;
        _player = ReInput.players.GetPlayer(_playerId);
        _rigidbody = GetComponent<Rigidbody2D>();
        cam = Camera.main;
        this.transform.position = new Vector3(transform.position.x, 0f, transform.position.z);
        _rigidbody.gravityScale = 0;
        PlayerImage.GetComponent<SpriteRenderer>().color = _playerColor;
        _particleSystem.GetComponent<ParticleSystemRenderer>().material.SetColor("_TintColor", _playerColor);
        _consecutiveKills = 0;
        Popup = GameObject.Find("Popup");
    }
	
    void Start()
    {
        dataSended = false;
        nt = FindObjectOfType<networkSocket>();
        _consecutiveKills = 0;
        em = _particleSystem.emission;
        _playerPath.SetColor(_playerColor);
        _timingDeath = 0f;
        StopRumble();
        next_move = Time.time + move_freq;
    }
	
	// Update is called once per frame
	void Update () {
       
        
        

        if (_playerId == 1)
        {

            if (Time.time > next_move)
            {


               
                next_move = Time.time + move_freq;


                string str = "";
                str = nt.getData();
                Debug.Log(str);
                if (str != "")
                {
                    //Debug.Log(str);
                    string[] arr;
                    arr = str.Split(' ');
                    _xAxisValue = float.Parse(arr[0], CultureInfo.InvariantCulture.NumberFormat);
                    _yAxisValue = float.Parse(arr[1], CultureInfo.InvariantCulture.NumberFormat);
                    if(Math.Abs(float.Parse(arr[2], CultureInfo.InvariantCulture.NumberFormat)) > 0.8 ) atk = true;
                    else atk = false;

                }
                else
                {
                    //dataSended = false;
                    _yAxisValue = 0.0f;
                    atk = false;
                    _xAxisValue = 0.0f;
                }

                //dataSended = true;
                string res = GameManager.Instance.getPlayerData(_playerId);
                //Debug.Log(res);
                byte[] utf8Bytes = System.Text.Encoding.UTF8.GetBytes(res);
                //Debug.Log(System.Text.Encoding.UTF8.GetString(utf8Bytes));
                nt.sendData(res);


                
            }
        }
        else
        {
            _yAxisValue = _player.GetAxis("Propulse");
        atk = _player.GetButtonDown("Attack");
        _xAxisValue = _player.GetAxis("HorizontalAxis");
        }
        if (_started == 1) //When playing
        {
            
            //Vertical Input
            if (Mathf.Sign(transform.position.y) == Mathf.Sign(_rigidbody.velocity.y))
            {
                if (Mathf.Abs(_yAxisValue) > _InputDeadZone )
                {
                    if (_yAxisValue != 0 && Mathf.Abs(_rigidbody.velocity.y + _yAxisValue) > MIN_VERTICAL_VELOCITY && Mathf.Abs(_rigidbody.velocity.y + _yAxisValue) < MAX_VERTICAL_VELOCITY)
                    {
                        em.enabled = true;
                        /*
                        if (_particleSystem.isStopped)
                        {
                            Debug.Log("Play");
                            _particleSystem.Play();
                        }*/
                        
                        if (Mathf.Sign(_rigidbody.velocity.y) != Mathf.Sign(_yAxisValue))
                        {

                            _yAxisValue = Mathf.Sign(_yAxisValue) * _turningScale;
                        }
                        else
                        {
                            _yAxisValue = Mathf.Sign(_yAxisValue) * 1;
                        }
                        _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, _rigidbody.velocity.y + _vForce * _yAxisValue * Time.deltaTime);
                    }
                }
                else
                {
                    
                    em.enabled = false;
                    
                    /*
                    if (_particleSystem.isPlaying)
                    {
                        //Debug.Log("Stop");
                        _particleSystem.Stop();
                    }*/
                }

            }
            //Gravity Center
            _rigidbody.gravityScale = Mathf.Sign(transform.position.y);


            //Attack
            if (atk)
            {
                if (_bombAmount > 0)
                {
                    GameObject go = (GameObject)Instantiate(Bomb, transform.position, Quaternion.identity);
                    go.GetComponent<SpriteRenderer>().color = _playerColor;
                    go.GetComponent<BombController>()._bombId = _playerId;
                    go.GetComponent<BombController>()._player = gameObject;
                    _bombAmount--;
                    AudioManager.Instance.PlayAudio(AudioManager.AudioTracks.PlaceMine);
                }
            }  

            //Horizontal Input
            
            if (Mathf.Abs(_xAxisValue) > _InputDeadZone)
            {
                if (_xAxisValue != 0 && Mathf.Abs(_rigidbody.velocity.x + _xAxisValue) < MAX_HORIZONTAL_VELOCITY)
                {
                    if (Mathf.Sign(_rigidbody.velocity.x) != Mathf.Sign(_xAxisValue))
                    {
                        _xAxisValue = Mathf.Sign(_xAxisValue) * _turningScale;
                    }
                    else
                    {
                        _xAxisValue = Mathf.Sign(_xAxisValue) * 1;
                    }
                    _rigidbody.velocity = new Vector3(_rigidbody.velocity.x + _hForce * _xAxisValue * Time.deltaTime, _rigidbody.velocity.y);
                }
            }
        }
        else//First Input
        {
            
           
            if (_yAxisValue != 0)
            {
                _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, _vForce * 5f * Mathf.Sign(_yAxisValue));
                _started = 1;
            }
            
        }

        //rotate to movement

        _angle = Mathf.Atan2(_rigidbody.velocity.y, _rigidbody.velocity.x) * Mathf.Rad2Deg;
        PlayerImage.transform.rotation = Quaternion.AngleAxis(_angle-90, Vector3.forward);

        //Continuum screen
        _playerViewportPos = cam.WorldToViewportPoint(transform.position);
        if (_playerViewportPos.x > 1)
        {
            _playerPath.SplitBothLines();
            transform.position = cam.ViewportToWorldPoint(_playerViewportPos - Vector3.right);
        }
        else if(_playerViewportPos.x < 0)
        {
            _playerPath.SplitBothLines();
            transform.position = cam.ViewportToWorldPoint(_playerViewportPos + Vector3.right);
        }

        //Kill inactive
        if(_started == 0)
        {
            _timingDeath += Time.deltaTime;

            if(_timingDeath > IDLE_TIME_DEATH)
            {
                KillPlayer();
            }
        }

        //Mid Particle
        
        if(_horizontalParticlesDisplayed && Mathf.Abs(transform.position.y) < 0.1 && Mathf.Abs(_rigidbody.velocity.y) > 4)
        {
            _angle = Mathf.Atan2(_rigidbody.velocity.y, _rigidbody.velocity.x) * Mathf.Rad2Deg;
            _horizontalParticlesDisplayed = false;
            AudioManager.Instance.PlayAudio(AudioManager.AudioTracks.CrossLine);
            GameObject particulas = (GameObject)Instantiate(Particles2DHorizon, transform.position, Quaternion.identity);
            particulas.GetComponent<ParticleSystemRenderer>().material.SetColor("_TintColor", GameObject.FindObjectOfType<DrawCenterLine>()._lineColor);
            particulas.transform.Rotate(_angle +180, -90 ,0);
            StartCoroutine(delayHorizonParticles());
        }


        
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        //here code for +1 to enemy consecutiveKills
        GameObject obj = other.gameObject;
        if (obj.CompareTag("Bomb"))
        {
            BombController bomb = obj.GetComponent<BombController>();
            if (bomb._bombId != _playerId)
            {
                bomb.BombDestroy(true);
                KillPlayer();
            }

        }else if (obj.CompareTag("Wall"))
        {
            KillPlayer();
        }
    }

    void KillPlayer()
    {
        Debug.Log(_consecutiveKills);
        _consecutiveKills = 0;
        _dead = true;
        GameObject particles = (GameObject)Instantiate(Particles2D, transform.position, Quaternion.identity);
        particles.GetComponent<ParticleSystemRenderer>().material.SetColor("_TintColor", _playerColor);
        //Camera.main.gameObject.AddComponent<ScreenShake>();
        //Destroy(gameObject);
        this.gameObject.SetActive(false);
        CheckPlayerHasWon();
        _playerPath.ClearPathLines();
        _cameraAnimator.SetTrigger("SoftShake");
        Rumble(1f);
        AudioManager.Instance.PlayAudio(AudioManager.AudioTracks.Death);
    }

    public void ResetPlayer()
    {
        _dead = false;
        _playerPath.ClearPathLines();
        _playerPath.StartShowingLine();
        _started = 0;
        _timingDeath = 0f;
        _bombAmount = 1;
        this.gameObject.SetActive(true);
        StopRumble();
        _player = ReInput.players.GetPlayer(_playerId);
        //_rigidbody = GetComponent<Rigidbody2D>();
        //cam = Camera.main;
        //this.transform.position = new Vector3(transform.position.x, 0f, transform.position.z);
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.gravityScale = 0;
        PlayerImage.GetComponent<SpriteRenderer>().color = _playerColor;
        _particleSystem.GetComponent<ParticleSystemRenderer>().material.SetColor("_TintColor", _playerColor);
    }

    private void CheckPlayerHasWon()
    {
        GameManager.Instance.CheckWinCondition();
    }


    private void Rumble(float duration)
    {
        // Set vibration for a certain duration
        foreach(Joystick j in _player.controllers.Joysticks) {
            if(!j.supportsVibration) continue;
            if(j.vibrationMotorCount > 0) j.SetVibration(0, 1.0f, duration); // 1 second duration
            if(j.vibrationMotorCount > 0) j.SetVibration(1, 1.0f, duration);
        }
    }

    private void StopRumble()
    {
        // Stop vibration
        foreach(Joystick j in _player.controllers.Joysticks) {
            j.StopVibration();
        }
    }

    private IEnumerator delayHorizonParticles()
    {
        yield return new WaitForSeconds(0.5f);

        _horizontalParticlesDisplayed = true;
    }
    public void NewKill()
    {
        _consecutiveKills++;
        //Popup.GetComponent<PopupController>().showText(gameObject.transform.position.x, gameObject.transform.position.y, _consecutiveKills.ToString());
    }

}
