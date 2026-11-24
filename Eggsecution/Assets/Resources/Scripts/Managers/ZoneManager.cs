using UnityEngine;
using System;
using UnityEngine.SceneManagement;


public class ZoneManager : MonoBehaviour
{
    public static ZoneManager Instance;

    [Header("Zonas")]
    [SerializeField] private Transform[] _zoneSpawns;
    [SerializeField] private string _endScene = "Menu";

    [Header("Reinicio")]
    [SerializeField] private float _restartDelay = 2f;

    private int _keys = 0;
    private bool _restarting = false;
    private bool _changingZone = false;

    public int Keys => _keys;
    public event Action<int> OnKeysChanged;
    public event Action<int> OnZoneChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

        }
        else
        {
            Destroy(gameObject);
        }

    }

    private void OnDestroy()
    {
        if ( Instance == this)
        {
            Instance = null;
        }
    }

    public void AddKey()
    {
        _keys++;
        OnKeysChanged?.Invoke(_keys);
    }

    public bool UseKey()
    {
        if (_keys > 0)
        {
            _keys--;
            OnKeysChanged?.Invoke(_keys);
            return true;
        }
        return false;
    }

    public Vector3 GetSpawnPosition(int playerIndex)
    {
        if (_zoneSpawns == null || _zoneSpawns.Length == 0) return Vector3.zero;

        int zone = Mathf.Clamp(RunData.CurrentZone, 0, _zoneSpawns.Length - 1);
        Transform spawn = _zoneSpawns[zone];

        if (playerIndex >= spawn.childCount) return spawn.position;
        return spawn.GetChild(playerIndex).position;
    }

    public void GoToNextZone()
    {
        if (_changingZone) return;
        _changingZone = true;

        RunData.CurrentZone++;

        if (RunData.CurrentZone >= _zoneSpawns.Length)
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(_endScene);
            return;
        }

        _keys = 0;
        OnKeysChanged?.Invoke(_keys);

        TeleportPlayers();
        OnZoneChanged?.Invoke(RunData.CurrentZone);
        _changingZone = false;
    }

    public void TeleportPlayers()
    {
        for(int i = 0; i < PlayerMovement.Jugadores.Count; i++)
        {
            Transform player = PlayerMovement.Jugadores[i];
            Vector3 position = GetSpawnPosition(i);

            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            if(rb != null)
            {
                rb.position = position;
            }
            player.position = position;
        }
        Physics2D.SyncTransforms();
        if (CameraFollow.Instance != null)
        {
            CameraFollow.Instance.SnapToLeader();
        }
    }

    public void PlayerDied()
    {
        if (_restarting) return;

        foreach(Transform player in PlayerMovement.Jugadores)
        {
            Personaje personaje = player.GetComponent<Personaje>();
            if (personaje != null && !personaje.EstaMuerto)
            {
                return;
            }
        }
        _restarting = true;
        Invoke(nameof(RestartZone), _restartDelay);
    }

    private void RestartZone()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


}

  