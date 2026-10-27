using UnityEngine;

namespace CementerioSimple
{
    public class CorazonCuracion : MonoBehaviour
    {
        [SerializeField, Min(1)] int vidaRecuperada = 2;
        public bool Recoger(ControlJugador jugador)
        {
            // Es curación inmediata al pulsar E: sin consumo automático ni animación de comer.
            if (!gameObject.activeInHierarchy || Vector2.Distance(jugador.transform.position, transform.position) > jugador.AlcanceRecoger) return false;
            if (!jugador.Curar(vidaRecuperada)) return false;
            gameObject.SetActive(false);
            return true;
        }
        public static CorazonCuracion BuscarCercano(Vector2 posicion, float alcance)
        {
            CorazonCuracion cercano = null;
            foreach (var corazon in FindObjectsByType<CorazonCuracion>())
            {
                float distancia = Vector2.Distance(posicion, corazon.transform.position);
                if (distancia < alcance) { alcance = distancia; cercano = corazon; }
            }
            return cercano;
        }
    }
}
