using UnityEngine;

[CreateAssetMenu(fileName = "PatronAnillo", menuName = "Patrones/PatronAnillo")]
public class PatronAnillo : PatronDisparo
{
    [SerializeField] private int _numeroBalas = 8;

    public override void Disparar(EnemigoTirador tirador, float anguloObjetivo, int rafaga)
    {
        float paso = 360f / _numeroBalas;
        for(int i = 0; i < _numeroBalas; i++)
        {
            float angulo = anguloObjetivo + paso * i;
            tirador.CrearBala(angulo);
        }
    }
}
