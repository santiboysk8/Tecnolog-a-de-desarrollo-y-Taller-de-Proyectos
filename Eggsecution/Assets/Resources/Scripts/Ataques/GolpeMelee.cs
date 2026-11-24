using System.Collections.Generic;
using UnityEngine;

public class GolpeMelee : Ataque
{
    [SerializeField] private float _duracion = 0.5f;

    private readonly HashSet<Personaje> _golpeados = new HashSet<Personaje>();

    private void Start()
    {
        Destroy(gameObject, _duracion);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Personaje personaje = other.GetComponentInParent<Personaje>();
        if (personaje == null || _golpeados.Contains(personaje))
        {
            return;
        }
        if (IntentarDanar(personaje))
        {
            _golpeados.Add(personaje);
        }
       
    }
}
