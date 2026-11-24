using UnityEngine;

[CreateAssetMenu(fileName = "PatronEspiral", menuName = "Patrones/PatronEspiral")]
public class PatronEspiral : PatronDisparo
{
    [SerializeField] private int _numeroBalas = 8;
    [SerializeField] private float _giroPorRafaga = 15f;
    [SerializeField] private bool _apuntarAlJugador = false;
    
    public override void Disparar(EnemigoTirador tirador, float anguloObjetivo, int rafaga)
    {
        float paso = 360f;
        float anguloBase = _apuntarAlJugador ? anguloObjetivo : 0f;
        float desfase = rafaga * _giroPorRafaga;

        for(int i = 0; i < _numeroBalas; i++)
        {
            float angulo = anguloBase + desfase + (i * paso / _numeroBalas);
            tirador.CrearBala(angulo);
        }
    }
}
