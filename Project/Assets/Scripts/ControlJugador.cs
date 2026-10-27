using UnityEngine;
using UnityEngine.InputSystem;

namespace CementerioSimple
{
    [RequireComponent(typeof(Rigidbody2D), typeof(CapsuleCollider2D))]
    public class ControlJugador : MonoBehaviour
    {
        [Header("Movimiento")]
        [SerializeField] float velocidad = 4.5f;
        [SerializeField] float fuerzaSalto = 10.5f;
        [SerializeField] LayerMask capaSuelo = 1 << 6;
        [SerializeField] Vector2 tamanoDetectorSuelo = new Vector2(.45f, .14f);
        [SerializeField] float distanciaPies = .79f;
        [SerializeField] float alturaDerrota = -6;
        [Header("Combate")]
        [SerializeField] int vidaMaxima = 5;
        [SerializeField] int danoGolpe = 1;
        [SerializeField] float alcanceGolpe = .9f;
        [SerializeField] float desplazamientoGolpe = .8f;
        [SerializeField] float intervaloGolpe = .45f;
        [SerializeField] float proteccionTrasDano = .8f;
        [SerializeField] LayerMask capaEnemigos = 1 << 7;
        [Header("Curación")]
        [SerializeField] float alcanceRecoger = 1.35f;
        public int Vida { get; private set; }
        public int VidaMaxima => vidaMaxima;
        public float AlcanceRecoger => alcanceRecoger;
        public bool Muerto => Vida <= 0;
        public bool Atacando => Time.time < finAtaque;
        public bool Herido => Time.time < finProteccion;
        public bool EnSuelo => Physics2D.OverlapBox((Vector2)transform.position + Vector2.down * distanciaPies, tamanoDetectorSuelo, 0, capaSuelo);
        Rigidbody2D cuerpo;
        SpriteRenderer imagen;
        float movimiento, direccion = 1, siguienteGolpe, finAtaque, finProteccion;
        bool saltoSolicitado;

        void Awake()
        {
            cuerpo = GetComponent<Rigidbody2D>();
            imagen = GetComponent<SpriteRenderer>();
            Vida = vidaMaxima;
        }

        void Update()
        {
            if (!ControlJuego.Instancia.Jugando || Muerto) { movimiento = 0; return; }
            // Si el personaje sale por un extremo del suelo, termina la partida en lugar de caer sin fin.
            if (transform.position.y < alturaDerrota) { RecibirDano(vidaMaxima); return; }
            var teclado = Keyboard.current;
            if (teclado == null) return;
            movimiento = (teclado.dKey.isPressed || teclado.rightArrowKey.isPressed ? 1 : 0) - (teclado.aKey.isPressed || teclado.leftArrowKey.isPressed ? 1 : 0);
            if (movimiento != 0) direccion = Mathf.Sign(movimiento);
            imagen.flipX = direccion < 0;
            if (teclado.spaceKey.wasPressedThisFrame || teclado.wKey.wasPressedThisFrame) saltoSolicitado = true;
            if (teclado.jKey.isPressed || (Mouse.current != null && Mouse.current.leftButton.isPressed)) Pegar();
            if (teclado.eKey.wasPressedThisFrame) Recoger();
        }

        void FixedUpdate()
        {
            if (!ControlJuego.Instancia.Jugando || Muerto) { cuerpo.linearVelocity = Vector2.zero; return; }
            cuerpo.linearVelocity = new Vector2(movimiento * velocidad, cuerpo.linearVelocity.y);
            if (saltoSolicitado)
            {
                // El salto solo se admite apoyado en el suelo: no hay salto doble.
                if (EnSuelo) Saltar();
                saltoSolicitado = false;
            }
        }

        void Saltar()
        {
            cuerpo.linearVelocity = new Vector2(cuerpo.linearVelocity.x, fuerzaSalto);
            SonidoJuego.Instancia.ReproducirSalto();
        }

        void Pegar()
        {
            if (Time.time < siguienteGolpe) return;
            siguienteGolpe = Time.time + intervaloGolpe;
            finAtaque = siguienteGolpe;
            Vector2 centro = (Vector2)transform.position + Vector2.right * direccion * desplazamientoGolpe;
            foreach (var colision in Physics2D.OverlapCircleAll(centro, alcanceGolpe, capaEnemigos))
            {
                var enemigo = colision.GetComponent<EnemigoCuerpoACuerpo>();
                if (enemigo && !Physics2D.Linecast(transform.position, enemigo.transform.position, capaSuelo)) enemigo.RecibirDano(danoGolpe);
            }
            SonidoJuego.Instancia.ReproducirGolpe();
        }

        void Recoger()
        {
            var corazon = CorazonCuracion.BuscarCercano(transform.position, alcanceRecoger);
            if (corazon) corazon.Recoger(this);
        }

        public void RecibirDano(int cantidad)
        {
            if (!ControlJuego.Instancia.Jugando || Muerto || Time.time < finProteccion) return;
            Vida = Mathf.Max(0, Vida - cantidad);
            finProteccion = Time.time + proteccionTrasDano;
            SonidoJuego.Instancia.ReproducirDano();
            if (Muerto) ControlJuego.Instancia.Perder();
        }

        public bool Curar(int cantidad)
        {
            if (Muerto || Vida >= vidaMaxima || !ControlJuego.Instancia.Jugando) return false;
            Vida = Mathf.Min(vidaMaxima, Vida + cantidad);
            SonidoJuego.Instancia.ReproducirCuracion();
            return true;
        }

        public void Detener()
        {
            movimiento = 0;
            cuerpo.linearVelocity = Vector2.zero;
            cuerpo.simulated = false;
        }
    }
}
