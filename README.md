# El Cementerio

## Objetivo
Derrotar a cuatro enemigos en un nivel 2D. Se gana al completar las cuatro bajas y se pierde al quedarse sin vida.

## Plataforma
PC Windows de 64 bits, elegida para utilizar teclado/mouse y ejecutar el juego localmente.

## Configuración clave
Unity 6000.6.7f1, pipeline URP con Renderer 2D y calidad Medium. Los recursos se organizan en `Assets/`: Scripts, Scenes, Prefabs, Art, Audio y Settings.

El juego gestiona inicio, partida, fin y reintento mediante estados. Utiliza prefabs reutilizables y parámetros configurables en el Inspector. Rigidbody2D, colliders y gravedad resuelven el salto y las colisiones. La música y los efectos de salto y curación usan fuentes de audio con volúmenes separados.

## Instrucciones de ejecución
- Windows: abrir `Build/ElCementerioSimple.exe`, conservando sus archivos acompañantes.
- Unity: abrir `Project`, cargar `Assets/Scenes/Cementerio.unity` y pulsar Play.

## Controles
A/D o flechas: mover. W/Espacio: saltar. J o clic izquierdo: pegar. E: recoger un corazón y recuperar vida. Esc: pausa. R: reintentar al terminar.

## Dependencias
Unity, Universal Render Pipeline, Input System y herramientas 2D de Sprite/Tilemap. Los paquetes están declarados en `Project/Packages/manifest.json`.

## Riesgos y mitigaciones
- Importación inicial lenta: esperar a que Unity termine de instalar los paquetes.
- Diferencias entre editor y ejecutable: comprobar ambos antes de entregar.

## Roles de la dupla
- Francisco Ortiz: programación y ajuste de las acciones del personaje.
- Agustín Feijoo: recursos visuales, incorporación de imágenes y pruebas para detectar errores.
- Ambos: selección, incorporación y ajustes del sonido.
