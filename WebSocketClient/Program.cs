using System;
using System.Threading;
using WebSocketSharp;

namespace WebSocketClient
{
//         try
//         {
//             // Iniciar la conexión
//             await connection.StartAsync();
//             Console.WriteLine("Conectado al servidor SignalR. Escuchando mensajes...");
//         }
//         catch (Exception ex)
//         {
//             Console.WriteLine($"Error al conectar: {ex.Message}");
//             return;
//         }
//
//         // Mantener la aplicación corriendo
//         Console.WriteLine("Presiona Ctrl+C para salir.");
//         await Task.Delay(-1); // Bloquea indefinidamente
    internal class Program
    {
        
        public static void Main(string[] args)
        {
            using (var ws = new WebSocket("ws://localhost:8080/chat"))
            {
                // Evento cuando se establece la conexión
                ws.OnOpen += (sender, e) =>
                {
                    Console.WriteLine("Conexión establecida");
                    // Enviar un mensaje al servidor una vez que se haya conectado
                    ws.Send("Hola, servidor!");
                };

                // Evento cuando se recibe un mensaje del servidor
                ws.OnMessage += (sender, e) =>
                {
                    Console.WriteLine("Mensaje recibido: " + e.Data);
                };

                // Evento cuando ocurre un error en la conexión
                ws.OnError += (sender, e) =>
                {
                    Console.WriteLine("Error: " + e.Message);
                };

                // Evento cuando la conexión se cierra
                ws.OnClose += (sender, e) =>
                {
                    Console.WriteLine("Conexión cerrada: " + e.Reason);
                };

                try
                {
                    // Conectar al servidor WebSocket
                    ws.Connect();

                    // Mantener la conexión abierta (esperando eventos)
                    // Console.ReadKey();
                    Console.WriteLine("Conexted");
                    Console.WriteLine("Presiona Ctrl+C para salir."); 
                    Thread.Sleep(Timeout.Infinite); // Bloquea indefinidamente
                }
                catch (Exception c)
                {
                    Console.WriteLine(c);
                    throw;
                }
            }
        }
    }
}

// using System;
// using System.Threading.Tasks;
// using Microsoft.AspNetCore.SignalR.Client;
//
// class Program
// {
//     static async Task Main(string[] args)
//     {
//         // URL del servidor SignalR
//         string serverUrl = "https://localhost:5001/hub"; // Cambia la URL según tu configuración
//
//         var connection = new HubConnectionBuilder()
//             .WithUrl(serverUrl) // Dirección del servidor SignalR
//             .WithAutomaticReconnect() // Reconexión automática
//             .Build();
//
//         // Manejar eventos de reconexión
//         connection.Reconnecting += error =>
//         {
//             Console.WriteLine($"Conexión perdida. Intentando reconectar: {error?.Message}");
//             return Task.CompletedTask;
//         };
//
//         connection.Reconnected += connectionId =>
//         {
//             Console.WriteLine($"Reconectado al servidor. ConnectionId: {connectionId}");
//             return Task.CompletedTask;
//         };
//
//         connection.Closed += async error =>
//         {
//             Console.WriteLine($"Conexión cerrada. Intentando reconectar: {error?.Message}");
//             await Task.Delay(new Random().Next(0, 5) * 1000);
//             await connection.StartAsync();
//         };
//
//         // Suscribirse a eventos del servidor
//         connection.On<string, string>("ReceiveMessage", (user, message) =>
//         {
//             Console.WriteLine($"Mensaje recibido de {user}: {message}");
//         });
//
//         try
//         {
//             // Iniciar la conexión
//             await connection.StartAsync();
//             Console.WriteLine("Conectado al servidor SignalR. Escuchando mensajes...");
//         }
//         catch (Exception ex)
//         {
//             Console.WriteLine($"Error al conectar: {ex.Message}");
//             return;
//         }
//
//         // Mantener la aplicación corriendo
//         Console.WriteLine("Presiona Ctrl+C para salir.");
//         await Task.Delay(-1); // Bloquea indefinidamente
//     }
// }



// using System;
// using System.Threading;
// using System.Threading.Tasks;
// using Microsoft.AspNetCore.SignalR.Client;
//
// public class Program
// {
//     static async Task Main(string[] args)
//     {
//         // URL del servidor SignalR
//         string serverUrl = "https://localhost:5001/hub"; // Cambia la URL según tu configuración
//
//         // Configurar la conexión a SignalR
//         var connection = new HubConnectionBuilder()
//             .WithUrl(serverUrl)
//             .WithAutomaticReconnect()
//             .Build();
//
//         // Suscribirse a los eventos con tipado explícito
//         connection.On<string, string>("ReceiveMessage", (user, message) =>
//         {
//             Console.WriteLine($"Mensaje recibido de {user}: {message}");
//         });
//
//         connection.On<string>("Notify", (notification) =>
//         {
//             Console.WriteLine($"Notificación: {notification}");
//         });
//
//         // Implementar reintentos personalizados en caso de error al conectar
//         const int maxRetryAttempts = 5;
//         const int delayBetweenRetriesInMilliseconds = 3000;
//         int attempt = 0;
//         bool connected = false;
//
//         while (attempt < maxRetryAttempts && !connected)
//         {
//             try
//             {
//                 attempt++;
//                 Console.WriteLine($"Intentando conectar al servidor SignalR. Intento {attempt}/{maxRetryAttempts}...");
//                 await connection.StartAsync();
//                 connected = true;
//                 Console.WriteLine("Conectado al servidor SignalR. Escuchando mensajes...");
//             }
//             catch (Exception ex)
//             {
//                 Console.WriteLine($"Error al conectar: {ex.Message}");
//                 if (attempt < maxRetryAttempts)
//                 {
//                     Console.WriteLine($"Reintentando en {delayBetweenRetriesInMilliseconds / 1000} segundos...");
//                     await Task.Delay(delayBetweenRetriesInMilliseconds);
//                 }
//                 else
//                 {
//                     Console.WriteLine("No se pudo establecer la conexión después de varios intentos. Saliendo...");
//                 }
//             }
//         }
//
//         if (!connected)
//         {
//             return; // Salir si no se pudo conectar
//         }
//
//         // Mantener la aplicación en ejecución
//         Console.WriteLine("Presiona Ctrl+C para salir.");
//         await Task.Delay(-1);
//     }
// }
