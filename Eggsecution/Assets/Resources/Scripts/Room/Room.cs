using System;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Room : MonoBehaviour
{
    [Header("Puertas")]
    [SerializeField] private GameObject[] _doors;

    [Header("Recompensas")]
    [SerializeField] private GameObject _keyPrefab;
    [SerializeField] private Transform _keySpawnPoint;
    private Enemigo[] _enemies;
    private int _aliveEnemies;
    private bool _started = false;
    private bool _cleared = false;

    public event Action OnRoomCleared;

    private void Awake()
    {
        _enemies = GetComponentsInChildren<Enemigo>(true);
        foreach (Enemigo enemy in _enemies)
        {
            enemy.gameObject.SetActive(false);
        }
        SetDoors(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_started) return;
        if (other.GetComponentInParent<PlayerMovement>() == null) return;

        StartRoom();
    }

    private void StartRoom()
    {
        _started = true;
        if (_enemies.Length == 0)
        {
            ClearRoom();
            return;
        }
        SetDoors(true);
        _aliveEnemies = _enemies.Length;

        foreach ( Enemigo enemy in _enemies)
        {
            enemy.gameObject.SetActive(true);
            enemy.OnMuerte += OnEnemyDied;
        }
        Debug.Log(name + " sala cerrada , " + _aliveEnemies + " enemigos");
    }

    private void OnEnemyDied()
    {
        _aliveEnemies--;
        if (_aliveEnemies <= 0)
        {
            ClearRoom();
        }
    }

    private void ClearRoom()
    {
        if(_cleared) return;
        _cleared = true;

        SetDoors(false);

        if(_keyPrefab != null)
        {
            Vector3 position = _keySpawnPoint != null? _keySpawnPoint.position : transform.position;
            Instantiate(_keyPrefab, position, Quaternion.identity);
        }

        OnRoomCleared?.Invoke();
        Debug.Log(name + " sala completada");
    }
    private void SetDoors(bool closed)
    {
        foreach(GameObject door in _doors)
        {
            if(door != null) door.SetActive(closed);
        }
    }
}
