using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using RouletteApp.Models;

namespace RouletteApp.Services
{
    public class TcpListenerService
    {
        private TcpListener? _listener;
        private readonly int _port = 4948;
        private bool _isRunning;
        
        public event EventHandler<Statistics>? StatisticsReceived;

        public void Start()
        {
            _isRunning = true;
            _ = Task.Run(ListenForMessages);
        }

        public void Stop()
        {
            _isRunning = false;
            _listener?.Stop();
        }

        private async Task ListenForMessages()
        {
            try
            {
                _listener = new TcpListener(IPAddress.Any, _port);
                _listener.Start();
                
                while (_isRunning)
                {
                    TcpClient client = await _listener.AcceptTcpClientAsync();
                    Task.Run(() => HandleClientAsync(client));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in TCP listener: {ex.Message}");
            }
            finally
            {
                _listener?.Stop();
            }
        }

        private async Task HandleClientAsync(TcpClient client)
        {
            using (client)
            using (NetworkStream stream = client.GetStream())
            {
                byte[] buffer = new byte[1024];
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                
                if (bytesRead > 0)
                {
                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    ProcessMessage(message);
                }
            }
        }

        private void ProcessMessage(string message)
        {
            try
            {
                var statistics = JsonSerializer.Deserialize<Statistics>(message);
                StatisticsReceived?.Invoke(this, statistics);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing message: {ex.Message}");
            }
        }
    }
}