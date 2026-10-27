using UnityEngine;

namespace CementerioSimple
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class EnemigoCuerpoACuerpo : MonoBehaviour
    {
        [Header("Movimiento")]
        [SerializeField] float velocidad = 1.2f;
        [SerializeField] float distanciaPatrulla = 1.6f;
        [SerializeField] float distanciaDeteccion = 5;
        [SerializeField] LayerMask capaSuelo = 1 << 6;
        [Header("Combate")]
        [SerializeField] int vidaMaxima = 3;
        [SerializeField] int dano = 1;
        [SerializeField] float alcanceAtaque = 1.25f;
        [SerializeField] float preparacionAtaque = .45f;
        [SerializeField] float intervaloAtaque = 1.3f;
        public int Vida { get; private set; }
        public bool Atacando => preparando;
        public bool Herido => Time.time < finHerida;
        Rigidbody2D cuerpo;
        SpriteRenderer imagen;
        float origen, direccion = -1, siguienteAtaque, momentoGolpe, finHerida;
        bool preparando;

        void Awake()
        {
            Vida = vidaMaxima;
            cuerpo = GetComponent<Rigidbody2D>();
            imagen = GetComponent<SpriteRenderer>();
            origen = transform.position.x;
        }

        void Update()
        {
            var juego = ControlJuego.Instancia;
            if (!juego.Jugando) { cuerpo.linearVelocity = Vector2.zero; return; }
            Vector2 diferencia = juego.Jugador.transform.position - transform.position;
            bool cerca = Mathf.Abs(diferencia.x) < distanciaDeteccion && Mathf.Abs(diferencia.y) < 1.5f;
            if (cerca) direccion = diferencia.x >= 0 ? 1 : -1;
            else if (transform.position.x > origen + distanciaPatrulla) direccion = -1;
            else if (transform.position.x < origen - distanciaPatrulla) direccion = 1;
            imagen.flipX = direccion < 0;
            if (preparando)
            {
                cuerpo.linearVelocity = new Vector2(0, cuerpo.linearVelocity.y);
                if (Time.time >= momentoGolpe)
                {
                    preparando = false;
                    if (Mathf.Abs(diferencia.x) < alcanceAtaque && Mathf.Abs(diferencia.y) < 1.2f && !Physics2D.Linecast(transform.position, juego.Jugador.transform.position, capaSuelo)) juego.Jugador.RecibirDano(dano);
                }
                return;
            }
            if (cerca && Mathf.Abs(diferencia.x) < alcanceAtaque && Time.time >= siguienteAtaque)
            {
                preparando = true;
                momentoGolpe = Time.time + preparacionAtaque;
                siguienteAtaque = Time.time + intervaloAtaque;
                return;
            }
            float avance = cerca && Mathf.Abs(diferencia.x) < alcanceAtaque * .8f ? 0 : direccion * velocidad;
            // Evita caminar hacia un borde sin suelo o atravesar un obstáculo.
            Vector2 delante = (Vector2)transform.position + new Vector2(direccion * .55f, -.45f);
            if (!Physics2D.Raycast(delante, Vector2.down, .8f, capaSuelo) || Physics2D.Raycast(transform.position, Vector2.right * direccion, .5f, capaSuelo)) avance = 0;
            cuerpo.linearVelocity = new Vector2(avance, cuerpo.linearVelocity.y);
        }

        public void RecibirDano(int cantidad)
        {
            if (Vida <= 0 || !ControlJuego.Instancia.Jugando) return;
            Vida = Mathf.Max(0, Vida - cantidad);
            finHerida = Time.time + .15f;
            if (Vida > 0) return;
            Detener();
            gameObject.SetActive(false);
            ControlJuego.Instancia.RegistrarEnemigo();
        }

        public void Detener()
        {
            cuerpo.linearVelocity = Vector2.zero;
            cuerpo.simulated = false;
        }
    }
}
