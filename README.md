# Servidor UDP

Este proyecto implementa un servidor UDP que proporciona la fecha y hora actuales a los clientes que se conectan. El servidor escucha en un puerto específico y responde a las solicitudes de forma asíncrona.

## Descripción del Servidor

El servidor UDP está diseñado para recibir solicitudes de clientes y enviar de vuelta la fecha y la hora actuales en formato de texto. Utiliza el protocolo UDP para la comunicación, lo que permite un envío de datos rápido y eficiente, aunque sin garantizar la entrega.

### Características

- Escucha en el puerto **12345**.
- Maneja múltiples solicitudes de clientes simultáneamente.
- Establece un tiempo de espera de **5000 ms** para recibir solicitudes.
- Utiliza la biblioteca **Spectre.Console** para un estilo visual atractivo en la consola.

### Ejecución del Servidor

1. Asegúrate de tener el SDK de .NET instalado en tu máquina.
2. Compila y ejecuta la clase `Servidor`:

```bash
dotnet run --project Servidor.csproj
