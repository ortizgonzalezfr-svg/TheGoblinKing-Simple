using UnityEngine;

namespace CementerioSimple
{
    [DefaultExecutionOrder(100)]
    public class AnimacionPersonaje : MonoBehaviour
    {
        [SerializeField] Texture2D hoja;
        [SerializeField] int filas = 4;
        [SerializeField] float alturaImagen = 1.9f;
        [SerializeField] float velocidadAnimacion = 8;
        Sprite[] fotogramas;
        ControlJugador jugador;
        EnemigoCuerpoACuerpo enemigo;
        SpriteRenderer imagen;
        Rigidbody2D cuerpo;

        void Awake()
        {
            jugador = GetComponent<ControlJugador>();
            enemigo = GetComponent<EnemigoCuerpoACuerpo>();
            imagen = GetComponent<SpriteRenderer>();
            cuerpo = GetComponent<Rigidbody2D>();
            int ancho = hoja.width / 4, alto = hoja.height / filas;
            fotogramas = new Sprite[filas * 4];
            for (int fila = 0; fila < filas; fila++)
                for (int columna = 0; columna < 4; columna++)
                    fotogramas[fila * 4 + columna] = Sprite.Create(hoja, new Rect(columna * ancho, hoja.height - (fila + 1) * alto, ancho, alto), new Vector2(.5f, .07f + .75f / alturaImagen), alto / alturaImagen, 0, SpriteMeshType.FullRect);
        }

        void LateUpdate()
        {
            if (fotogramas == null) return;
            int fotograma = 0;
            bool herido = jugador ? jugador.Herido : enemigo && enemigo.Herido;
            if (jugador)
            {
                if (jugador.Muerto) fotograma = 15;
                else if (jugador.Atacando) fotograma = 8 + Mathf.FloorToInt(Time.time * velocidadAnimacion) % 4;
                else if (!jugador.EnSuelo && Mathf.Abs(cuerpo.linearVelocity.y) > .5f) fotograma = cuerpo.linearVelocity.y > 0 ? 12 : 13;
                else if (Mathf.Abs(cuerpo.linearVelocity.x) > .2f) fotograma = 4 + Mathf.FloorToInt(Time.time * velocidadAnimacion) % 4;
            }
            else if (enemigo)
            {
                if (herido) fotograma = 6;
                else if (enemigo.Atacando) fotograma = 4;
                else if (Mathf.Abs(cuerpo.linearVelocity.x) > .2f) fotograma = 2 + Mathf.FloorToInt(Time.time * velocidadAnimacion) % 2;
            }
            imagen.sprite = fotogramas[Mathf.Clamp(fotograma, 0, fotogramas.Length - 1)];
            imagen.color = herido && Mathf.FloorToInt(Time.time * 12) % 2 == 0 ? new Color(1, .45f, .45f) : Color.white;
        }

        void OnDestroy()
        {
            if (fotogramas == null) return;
            foreach (var fotograma in fotogramas) Destroy(fotograma);
        }
    }
}
