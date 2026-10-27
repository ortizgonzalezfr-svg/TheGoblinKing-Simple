using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace CementerioSimple
{
    public enum EstadoJuego { Inicio, Jugando, Victoria, Derrota, Pausa }

    public class ControlJuego : MonoBehaviour
    {
        static ControlJuego instancia;
        public static ControlJuego Instancia
        {
            get { if (!instancia) instancia = FindAnyObjectByType<ControlJuego>(); return instancia; }
        }
        [SerializeField] ControlJugador jugador;
        [SerializeField, Min(1)] int enemigosParaGanar = 4;
        [Header("Mapa de una pantalla")]
        [SerializeField] Camera camara;
        [SerializeField] SpriteRenderer fondo;
        [SerializeField] float anchoVisible = 24;
        [SerializeField] float alturaMinima = 6.2f;
        public ControlJugador Jugador => jugador;
        public EstadoJuego Estado { get; private set; } = EstadoJuego.Inicio;
        public bool Jugando => Estado == EstadoJuego.Jugando;
        public int EnemigosDerrotados { get; private set; }
        public int Objetivo => enemigosParaGanar;
        GUIStyle texto, titulo, boton;

        void Awake()
        {
            instancia = this;
            Time.timeScale = 1;
            // El contacto causa daño mediante el ataque, no por empujones de los cuerpos.
            Physics2D.IgnoreLayerCollision(0, 7, true);
            Physics2D.IgnoreLayerCollision(7, 7, true);
        }

        void Update()
        {
            var teclado = Keyboard.current;
            if (teclado == null) return;
            if (teclado.enterKey.wasPressedThisFrame && Estado == EstadoJuego.Inicio) IniciarPartida();
            if (teclado.escapeKey.wasPressedThisFrame && (Jugando || Estado == EstadoJuego.Pausa)) CambiarPausa();
            if (teclado.rKey.wasPressedThisFrame && (Estado == EstadoJuego.Derrota || Estado == EstadoJuego.Victoria)) Reintentar();
#if !UNITY_EDITOR
            if (teclado.f11Key.wasPressedThisFrame) Screen.fullScreen = !Screen.fullScreen;
#endif
        }

        void LateUpdate()
        {
            // Cámara fija: adapta el encuadre al monitor sin seguir al personaje.
            if (!camara || !fondo) return;
            camara.orthographicSize = Mathf.Max(alturaMinima, anchoVisible / (2 * camara.aspect));
            float escala = Mathf.Max(camara.orthographicSize * 2.1f / fondo.sprite.bounds.size.y, camara.orthographicSize * 2 * camara.aspect * 1.05f / fondo.sprite.bounds.size.x);
            fondo.transform.localScale = Vector3.one * escala;
        }

        public void IniciarPartida()
        {
            Estado = EstadoJuego.Jugando;
            SonidoJuego.Instancia.ReproducirBoton();
        }

        public void RegistrarEnemigo()
        {
            if (!Jugando) return;
            EnemigosDerrotados++;
            if (EnemigosDerrotados >= enemigosParaGanar) Terminar(EstadoJuego.Victoria);
        }

        public void Perder()
        {
            if (Jugando) Terminar(EstadoJuego.Derrota);
        }

        void Terminar(EstadoJuego resultado)
        {
            Estado = resultado;
            jugador.Detener();
            foreach (var enemigo in FindObjectsByType<EnemigoCuerpoACuerpo>()) enemigo.Detener();
            SonidoJuego.Instancia.ReproducirFinal();
        }

        public void CambiarPausa()
        {
            Estado = Jugando ? EstadoJuego.Pausa : EstadoJuego.Jugando;
            Time.timeScale = Estado == EstadoJuego.Pausa ? 0 : 1;
        }

        public void Reintentar()
        {
            Time.timeScale = 1;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        void OnGUI()
        {
            // Una sola escala mantiene las proporciones de la interfaz en distintas pantallas.
            float escala = Mathf.Min(Screen.width / 1280f, Screen.height / 720f);
            GUI.matrix = Matrix4x4.Scale(new Vector3(escala, escala, 1));
            float ancho = Screen.width / escala, alto = Screen.height / escala;
            if (texto == null)
            {
                var fuente = Font.CreateDynamicFontFromOSFont("Arial", 22);
                texto = new GUIStyle(GUI.skin.label) { font = fuente, fontSize = 22, alignment = TextAnchor.MiddleCenter };
                titulo = new GUIStyle(texto) { fontSize = 36, fontStyle = FontStyle.Bold };
                boton = new GUIStyle(GUI.skin.button) { font = fuente, fontSize = 24 };
            }
            if (Estado != EstadoJuego.Inicio)
            {
                GUI.Box(new Rect(12, 12, 270, 73), "");
                GUI.Label(new Rect(20, 15, 250, 28), "VIDA " + jugador.Vida + " / " + jugador.VidaMaxima, texto);
                GUI.color = new Color(.2f, .03f, .03f);
                GUI.DrawTexture(new Rect(30, 52, 230, 17), Texture2D.whiteTexture);
                GUI.color = new Color(.8f, .12f, .14f);
                GUI.DrawTexture(new Rect(30, 52, 230 * jugador.Vida / (float)jugador.VidaMaxima, 17), Texture2D.whiteTexture);
                GUI.color = Color.white;
                GUI.Box(new Rect(ancho - 230, 12, 215, 45), "");
                GUI.Label(new Rect(ancho - 225, 16, 205, 35), "ENEMIGOS " + EnemigosDerrotados + " / " + enemigosParaGanar, texto);
                if (Jugando && CorazonCuracion.BuscarCercano(jugador.transform.position, jugador.AlcanceRecoger))
                    GUI.Label(new Rect(ancho / 2 - 250, alto - 55, 500, 35), jugador.Vida < jugador.VidaMaxima ? "E: recoger corazón y recuperar vida" : "Vida completa", texto);
            }
            if (Jugando) return;
            float izquierda = ancho / 2 - 390, arriba = alto / 2 - 205;
            GUI.Box(new Rect(izquierda, arriba, 780, 410), "");
            string encabezado = Estado == EstadoJuego.Inicio ? "EL CEMENTERIO — VERSIÓN SIMPLE" : Estado == EstadoJuego.Victoria ? "VICTORIA" : Estado == EstadoJuego.Derrota ? "DERROTA" : "PAUSA";
            GUI.Label(new Rect(izquierda + 15, arriba + 20, 750, 70), encabezado, titulo);
            string mensaje = Estado == EstadoJuego.Inicio ? "Derrota a los cuatro enemigos del cementerio.\nA/D: mover   W/Espacio: saltar   J: pegar\nE: recoger corazón (+2 vida)   Esc: pausa" : Estado == EstadoJuego.Victoria ? "Derrotaste a los cuatro enemigos. Nivel completado." : Estado == EstadoJuego.Derrota ? "Te quedaste sin vida. Puedes volver a intentarlo." : "Ajusta el sonido o continúa la partida.";
            GUI.Label(new Rect(izquierda + 30, arriba + 105, 720, 115), mensaje, texto);
            if (Estado == EstadoJuego.Pausa)
            {
                GUI.Label(new Rect(izquierda + 35, arriba + 215, 140, 25), "Música", texto);
                SonidoJuego.Instancia.VolumenMusica = GUI.HorizontalSlider(new Rect(izquierda + 190, arriba + 225, 170, 20), SonidoJuego.Instancia.VolumenMusica, 0, 1);
                GUI.Label(new Rect(izquierda + 385, arriba + 215, 140, 25), "Efectos", texto);
                SonidoJuego.Instancia.VolumenEfectos = GUI.HorizontalSlider(new Rect(izquierda + 545, arriba + 225, 170, 20), SonidoJuego.Instancia.VolumenEfectos, 0, 1);
            }
            if (GUI.Button(new Rect(izquierda + 230, arriba + 270, 320, 60), Estado == EstadoJuego.Inicio ? "JUGAR" : Estado == EstadoJuego.Pausa ? "CONTINUAR" : "REINTENTAR", boton))
            {
                if (Estado == EstadoJuego.Inicio) IniciarPartida();
                else if (Estado == EstadoJuego.Pausa) CambiarPausa();
                else Reintentar();
            }
#if UNITY_EDITOR
            GUI.Label(new Rect(izquierda + 20, arriba + 350, 740, 32), "Ampliar Game en Unity: Shift + Espacio", texto);
#else
            GUI.Label(new Rect(izquierda + 20, arriba + 350, 740, 32), "F11: pantalla completa   |   R: reintentar al terminar", texto);
#endif
        }
    }
}
