# El Cementerio

## Objetivo y plataforma
Juego 2D para **Windows de 64 bits**. El objetivo es derrotar a cuatro enemigos sin perder toda la vida. Se eligió PC para utilizar teclado/mouse y ejecutar el juego localmente, sin servidor.

## Configuración
Unity **6000.6.7f1**, pipeline **URP con Renderer 2D**, calidad Medium, sprites con filtro Point, VSync y sin HDR ni antialiasing. Dependencias: URP 17.6.0, Input System 1.19.0, Sprite/Tilemap 2D e IMGUI, declaradas en `Project/Packages/manifest.json`.

## Cómo ejecutar
- **Windows:** abrir `Build/ElCementerioSimple.exe` y conservar los demás archivos de Build.
- **Unity:** añadir la carpeta `Project` desde Unity Hub, abrir `Assets/Scenes/Cementerio.unity` y pulsar Play.

## Controles
A/D o flechas: mover. W/Espacio: saltar. J o clic izquierdo: pegar. E: recoger un corazón y recuperar 2 puntos de vida. Esc: pausa. R: reintentar al terminar. F11: pantalla completa en el ejecutable.

## Funcionamiento
El ciclo de juego incluye inicio, partida, pausa, victoria, derrota y reintento. La cuarta baja permite ganar; quedarse sin vida provoca la derrota.

Los prefabs reutilizan jugador, enemigo y corazón. El suelo usa Tilemap; Rigidbody2D, colliders, gravedad y un material sin rebote permiten saltar y detectar colisiones. La vida, velocidades y tiempos son configurables en el Inspector.

La música en bucle y los efectos de salto, golpe, daño, curación y menús usan dos fuentes de audio, con volúmenes separados ajustables en pausa.

## Riesgos y mitigaciones
- Importación inicial lenta: esperar a que Unity termine de instalar los paquetes.
- Diferencias entre editor y ejecutable: revisar ambos.
- Pérdida de avances: utilizar Git y respaldos.
- Curación accidental: requiere E y no consume corazones con vida completa.

## Integrantes y roles
- **Francisco Ortiz:** programación y ajuste de las acciones del personaje.
- **Agustín Feijoo:** recursos visuales, incorporación de imágenes y pruebas para detectar errores.
- **Ambos:** selección, incorporación y ajustes del sonido.

## Repositorio
Pendiente de añadir el enlace de GitHub. La entrega debe incluir `Project/`, `Build/` y este README, con los commits reales de ambos integrantes.
