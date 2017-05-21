using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script is for all player actions which includes walking horizontal, wakling vertical, and deploying bombs.
/// </summary>
public class PlayerActions : MonoBehaviour {

    // Variables to prevent players from moving through each other
    [SerializeField]
    private LayerMask _whatToHit;
    private Vector2 _offset;
    private RaycastHit2D _hitUp;
    private RaycastHit2D _hitRight;
    private RaycastHit2D _hitDown;
    private RaycastHit2D _hitLeft;
    private Vector2 _direction;
    [SerializeField]
    private float _offSetValue = 0.0f;
    [SerializeField]
    private float _magnitude = 1.0f;
    private Vector2 _movingTowards;

    //Power up
    public int range = 2;

    //Checks player moves
    private Vector2 _pos;
    private bool validMove = false;

    //Player's identifier, used for keeping track of which player is which
    [SerializeField]
    private int _playerNum;
    public int PlayerNumber {
        get { return _playerNum; }
        set { _playerNum = value; }
    }

    //For coloring the players
    private SpriteRenderer _sr;

    // Username
    public string playerName;
    public string PlayerName {
        get { return playerName; }
        set { playerName = value; }
    }

    private void OnEnable() {
        _movingTowards = new Vector2(transform.position.x + _offSetValue, transform.position.y + 1.0f) + Vector2.up;
        _direction = (_movingTowards - new Vector2(transform.position.x + _offSetValue, transform.position.y + 1.0f)).normalized;
    }

    // Use this for initialization
    void Start() {
        _sr = gameObject.GetComponent<SpriteRenderer>();
        setPlayerColor(_sr);
    }
    
    private void Update() {
        Debug.DrawLine(new Vector2(transform.position.x + _offSetValue, transform.position.y + 1.0f),
            new Vector2(transform.position.x + _offSetValue, transform.position.y + 1.0f) + (Vector2.up * _magnitude * _direction.y), Color.yellow);
    }

    void setPlayerColor(SpriteRenderer sr) {
        switch (_playerNum) {
            case 0:
                sr.material.SetColor("_Color", Color.red);
                break;
            case 1:
                sr.material.SetColor("_Color", Color.blue);
                break;
            case 2:
                sr.material.SetColor("_Color", Color.green);
                break;
            case 3:
                sr.material.SetColor("_Color", Color.yellow);
                break;
        }
    }

    void ValidPos(Vector3 pos) {

        Tile[,] tileMap = MapGenerator.Instance.tileMap;

        if (tileMap != null) {
            Tile tile = tileMap[(int)pos.x, (int)pos.y].GetComponent<Tile>();
            if (tile.type == TileType.Wall || tile.type == TileType.Destructable || tile.type == TileType.WallPowerUp) {
                validMove = false;
            }
            else {
                validMove = true;
            }
        }

    }

    public void RequestAction(PlayerIO command) {
        validMove = false;
        _pos = gameObject.transform.position;

        _offset = new Vector2(transform.position.x + _offSetValue, transform.position.y + 1.0f);
        _movingTowards = _offset + Vector2.up;
        _direction = (_movingTowards - _offset).normalized;
        _hitUp = Physics2D.Raycast(_offset, _direction, _magnitude, _whatToHit);

        _offset = new Vector2(transform.position.x + 1.0f, transform.position.y + _offSetValue);
        _movingTowards = _offset + Vector2.right;
        _direction = (_movingTowards - _offset).normalized;
        _hitRight = Physics2D.Raycast(_offset, _direction, _magnitude, _whatToHit);

        _offset = new Vector2(transform.position.x + _offSetValue, transform.position.y + 0.0f);
        _movingTowards = _offset + Vector2.down;
        _direction = (_movingTowards - _offset).normalized;
        _hitDown = Physics2D.Raycast(_offset, _direction, _magnitude, _whatToHit);

        _offset = new Vector2(transform.position.x + 0.0f, transform.position.y + _offSetValue);
        _movingTowards = _offset + Vector2.left;
        _direction = (_movingTowards - _offset).normalized;
        _hitLeft = Physics2D.Raycast(_offset, _direction, _magnitude, _whatToHit);


        switch (command.button) {

            case ButtonEnum.DeployBomb:
                Spawner.Instance.GetComponent<Spawner>().SpawnObject(Prefab.Bomb, new Vector3(Mathf.Round(gameObject.transform.position.x), Mathf.Round(gameObject.transform.position.y), 0));
                break;
            case ButtonEnum.MoveDown:
                if (_hitDown.collider == null) { _pos.y -= 1; }
                break;
            case ButtonEnum.MoveUp:
                if (_hitUp.collider == null) { _pos.y += 1; }
                break;
            case ButtonEnum.MoveLeft:
                if (_hitLeft.collider == null) { _pos.x -= 1; }
                break;
            case ButtonEnum.MoveRight:
                if (_hitRight.collider == null) { _pos.x += 1; }
                break;
        }

        ValidPos(_pos);
        if (validMove) { gameObject.transform.position = _pos; }
    }

    public void LeaveGame() {
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("PowerUp")) {
            Destroy(other.gameObject);
            ReplaceWithBasicTile();
            range++;
        }

    }

    //private void OnCollisionEnter2D(Collision2D collision) {
    //    if (collision.gameObject.CompareTag("Player")) {
    //        validMove = false;
    //    }
    //}

    //On Power Up Pick Up, replaces the tile with a basic tile.
    void ReplaceWithBasicTile() {
        Tile temp = Instantiate(MapGenerator.Instance.tileTypes[(int)TileType.Basic]).GetComponent<Tile>();
        temp.x = (int)gameObject.transform.position.x;
        temp.y = (int)gameObject.transform.position.y;
        temp.SetLocation();
        MapGenerator.Instance.tileMap[temp.x, temp.y] = temp;
    }
}
