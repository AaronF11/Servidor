using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Spectre.Console;

class Servidor
{
    private const int Port = 12345; // Puerto en el que el servidor escuchará las conexiones

    /// <summary>
    /// Método principal que inicia el servidor UDP.
    /// </summary>
    public static async Task Main()
    {
        Console.Title = "Servidor"; // Establece el título de la ventana de la consola

        // Muestra el título "Servidor" en la consola con estilo
        AnsiConsole.Write(new FigletText("Servidor")
            .Centered()
            .Color(Color.Blue));

        // Muestra una barra de progreso mientras se inicia el servidor
        await AnsiConsole.Progress()
            .StartAsync(async ctx =>
            {
                var task1 = ctx.AddTask($"[blue]Iniciando servidor...[/]");

                while (!ctx.IsFinished)
                {
                    await Task.Delay(25); // Simula trabajo en progreso

                    task1.Increment(1.5); // Incrementa el progreso de la tarea
                }
            });

        // Muestra un mensaje de conexión al puerto
        AnsiConsole.Write(new Markup($"[bold blue]Conectado al puerto[/] [bold blue1]{Port}[/]"));
        AnsiConsole.WriteLine();
        AnsiConsole.WriteLine();

        UdpClient server = new UdpClient(Port); // Crea un nuevo cliente UDP en el puerto especificado

        while (true) // Bucle infinito para esperar solicitudes de clientes
        {
            try
            {
                await AnsiConsole.Status()
                    .StartAsync("Esperando conexiones de clientes...", async ctx =>
                    {
                        ctx.Status("[bold rapidblink yellow]Esperando solicitud del cliente[/]"); // Estado de espera
                        ctx.Spinner(Spinner.Known.Runner);
                        ctx.SpinnerStyle(Style.Parse("green"));

                        IPEndPoint clientEndpoint = new IPEndPoint(IPAddress.Any, 0); // Crea un punto de conexión para el cliente
                        server.Client.ReceiveTimeout = 5000; // Establece el tiempo de espera para la recepción

                        // Escucha la solicitud del cliente
                        byte[] clientRequest = await Task.Run(() =>
                        {
                            try
                            {
                                return server.Receive(ref clientEndpoint); // Espera y recibe la solicitud
                            }
                            catch (SocketException ex)
                            {
                                if (ex.SocketErrorCode == SocketError.TimedOut)
                                {
                                    AnsiConsole.MarkupLine("[red]No se recibió solicitud en el tiempo de espera de 5000 ms.[/]"); // Manejo del tiempo de espera
                                }
                                else
                                {
                                    AnsiConsole.MarkupLine("[red]Error de socket: {0}[/]", ex.Message); // Manejo de otros errores de socket
                                }
                                return null;
                            }
                        });

                        if (clientRequest == null)
                        {
                            return; // Salir del bucle si hubo un error en la recepción
                        }

                        // Confirmación de conexión recibida
                        AnsiConsole.MarkupLine("[yellow]Solicitud recibida de {0}:{1}[/]", clientEndpoint.Address, clientEndpoint.Port);

                        ctx.Status("[green]Procesando solicitud...[/]"); // Mensaje de procesamiento
                        Thread.Sleep(1000); // Simulación de tiempo de procesamiento

                        // Obtener fecha y hora actual
                        string dateTime = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
                        byte[] responseBytes = Encoding.UTF8.GetBytes(dateTime); // Convierte la fecha y hora a bytes

                        // Enviar respuesta al cliente
                        server.Send(responseBytes, responseBytes.Length, clientEndpoint);

                        // Mostrar confirmación de envío
                        AnsiConsole.MarkupLine("[green bold]Fecha y hora enviadas:[/] [bold cyan]{0}[/] a [yellow]{1}:{2}[/]", dateTime, clientEndpoint.Address, clientEndpoint.Port);

                        // Mostrar una tabla resumen
                        var table = new Table();
                        table.AddColumn("Dirección IP");
                        table.AddColumn("Puerto");
                        table.AddColumn("Fecha y Hora Enviada");

                        table.AddRow(
                            clientEndpoint.Address.ToString(),
                            clientEndpoint.Port.ToString(),
                            $"[cyan]{dateTime}[/]");

                        AnsiConsole.Write(table); // Muestra la tabla en la consola
                    });
            }
            catch (Exception ex)
            {
                // Mostrar panel de error para otras excepciones
                var panel = new Panel($"[red bold]Error inesperado:[/] {ex.Message}")
                    .BorderColor(Color.Red);
                AnsiConsole.Write(panel);
            }
        }
    }
}
