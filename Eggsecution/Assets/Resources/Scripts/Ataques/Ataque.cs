using System;
using UnityEngine;

public abstract class Ataque : MonoBehaviour
{
    [SerializeField] protected float _danio = 10f;

    protected Bando _bando;
    private bool _configurado = false;

    public float Danio
    {
        get => _danio;
        set => _danio = value;
    }
    public event Action<Personaje> OnImpacto;

    public void Configurar(Bando bando)
    {
        _bando = bando;
        _configurado = true;
    }

    protected bool IntentarDanar(Personaje personaje)
    {
        if (personaje == null) return false;           
        if (!_configurado) return false;              
        if (personaje.Bando == _bando) return false;   
        if (personaje.EstaMuerto) return false;        

        personaje.RecibirDanio(_danio);
        OnImpacto?.Invoke(personaje);
        return true;
    }
}
