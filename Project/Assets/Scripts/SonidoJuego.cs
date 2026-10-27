using UnityEngine;

namespace CementerioSimple
{
    public class SonidoJuego : MonoBehaviour
    {
        static SonidoJuego instancia;
        public static SonidoJuego Instancia
        {
            get { if (!instancia) instancia = FindAnyObjectByType<SonidoJuego>(); return instancia; }
        }
        [SerializeField] AudioSource musica;
        [SerializeField] AudioSource efectos;
        [SerializeField] AudioClip salto, golpe, dano, curacion, boton, final;
        public int SaltosReproducidos { get; private set; }
        public int CuracionesReproducidas { get; private set; }
        public float VolumenMusica { get => musica.volume; set => musica.volume = Mathf.Clamp01(value); }
        public float VolumenEfectos { get => efectos.volume; set => efectos.volume = Mathf.Clamp01(value); }
        void Awake() { instancia = this; musica.Play(); }
        // Las dos fuentes permiten mezclar ambiente y efectos por separado.
        public void ReproducirSalto() { SaltosReproducidos++; efectos.PlayOneShot(salto); }
        public void ReproducirGolpe() => efectos.PlayOneShot(golpe);
        public void ReproducirDano() => efectos.PlayOneShot(dano);
        public void ReproducirCuracion() { CuracionesReproducidas++; efectos.PlayOneShot(curacion); }
        public void ReproducirBoton() => efectos.PlayOneShot(boton);
        public void ReproducirFinal() => efectos.PlayOneShot(final);
    }
}
