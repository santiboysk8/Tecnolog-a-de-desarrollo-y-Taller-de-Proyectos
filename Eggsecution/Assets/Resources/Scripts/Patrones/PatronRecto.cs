using UnityEngine;

// [CreateAssetMenu] añade una opción al menú de Unity: clic derecho → Create → Patrones → Recto
// fileName = nombre por defecto del archivo; menuName = dónde aparece en el menú
[CreateAssetMenu(fileName = "Patron_Recto", menuName = "Patrones/Recto")]
public class PatronRecto : PatronDisparo
{
    // Una sola bala, directa al jugador
    public override void Disparar(EnemigoTirador tirador, float anguloObjetivo, int rafaga)
    {
        tirador.CrearBala(anguloObjetivo);
    }
}
