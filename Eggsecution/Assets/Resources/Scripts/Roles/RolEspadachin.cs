using UnityEngine;

// ESPADACHÍN: igual que el mago pero con estamina y espadazos
public class RolEspadachin : Rol
{
    [SerializeField] private GameObject _swordCollider;          // C_Sword
    [SerializeField] private float _costeEstamina = 10f;
    [SerializeField] private float _regeneracionPorSegundo = 15f; // Se recupera más rápido que el maná
   

    public override string NombreRecurso => "Estamina";

    private void Update()
    {
        if (_recurso < _recursoMaximo)
        {
            Recuperar(_regeneracionPorSegundo * Time.deltaTime);
        }
    }

    public override void Atacar()
    {
        if (Gastar(_costeEstamina))
        {
           
             _jugador.SpawnHaciaApuntado(_swordCollider);

           

            Debug.Log("Estamina: " + _recurso);
        }
    }
}
