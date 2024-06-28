# VSApiGlobal

VSApiGlobal es una solución desarrollada en ASP.NET Core que actúa como un proxy para solicitudes API. Esta solución permite enviar solicitudes a diferentes servidores API configurados y devolver las respuestas en diferentes formatos como JSON, XML, texto plano, HTML, o binario.
Originalmente este proyecto permite centralizar servidores asi como servir de puente para agregar Logs centralizados sin necesidad de estar repitiendo codigo en otros proyectos.

## Características

- **Proxy de API:** Redirige las solicitudes a servidores API configurados.
- **Soporte para múltiples métodos HTTP:** Admite GET, POST, PUT y DELETE.
- **Respuesta en múltiples formatos:** Devuelve respuestas en JSON, XML, texto plano, HTML, o binario según se requiera.
- **Configuración flexible:** Permite configurar múltiples servidores API en el archivo de configuración.
- **Soporte de autenticación:** Permite el uso de encabezados de autenticación para las solicitudes.

## Requisitos

- .NET 6.0 SDK o superior
- Visual Studio 2022 o superior (opcional, pero recomendado)

## Instalación

1. Clona este repositorio en tu máquina local.
2. Navega al directorio del proyecto.
3. Restaura las dependencias del proyecto.

## Configuración

1. Abre el archivo de configuración principal.
2. Agrega y configura los servidores API en la sección correspondiente.

## Ejecución

Para ejecutar el proyecto, utiliza la herramienta de línea de comandos o el entorno de desarrollo integrado (IDE) de tu preferencia.

## Uso

Envía una solicitud POST a la ruta del proxy con un cuerpo JSON que contenga los siguientes campos:

- `servidor`: El nombre del servidor API configurado.
- `metodo`: El método HTTP a utilizar (GET, POST, PUT, DELETE).
- `opcion`: La ruta del endpoint en el servidor API.
- `body`: El formato esperado de la respuesta (`json`, `xml`, `text`, `html`, `binary`).
- `CorregirISO-8859` (opcional): Si se debe corregir el encoding a ISO-8859-1.
- `parametros`: Un diccionario de parámetros que se enviarán con la solicitud.

## Contribución

Si deseas contribuir a este proyecto, por favor realiza un fork del repositorio, crea una rama con tus cambios y envía un pull request para su revisión.

## Licencia

Este proyecto está licenciado bajo los términos de la licencia MIT. Para más detalles, consulta el archivo de licencia en este repositorio.
