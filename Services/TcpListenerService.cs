using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using Avalonia.Threading;
using Models;
using Serilog;

namespace Services
{
    public class TcpListenerService
    {
        private TcpListener? _listener;
        private readonly int _port = 4948;
        private bool _isRunning;
        private readonly ILogger _logger;
        
        public event EventHandler<Statistics>? StatisticsReceived;

        
        public TcpListenerService(ILogger? logger = null)
        {
            _logger = logger ?? Log.Logger;
        }
        
        public void Start()
        {
            _isRunning = true;
            _logger.Information("TcpListenerService is running on port {Port}", _port);
            _ = Task.Run(ListenForMessages);
        }

        public void Stop()
        {
            _isRunning = false;
            _listener?.Stop();
            _logger.Information("TcpListenerService stopped");
        }

        private async Task ListenForMessages()
        {
            try
            {
                _listener = new TcpListener(IPAddress.Any, _port);
                _listener.Start();
                _logger.Information("TCP listener ir running and waiting for a connection...");
                
                while (_isRunning)
                {
                    TcpClient client = await _listener.AcceptTcpClientAsync();
                    var endpoint = client.Client.RemoteEndPoint as IPEndPoint;
                    _logger.Information("Client is connected: {RemoteAddress}:{RemotePort}", 
                        endpoint?.Address, endpoint?.Port);
                    _ = HandleClientAsync(client);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error TCP listener");
            }
            finally
            {
                _listener?.Stop();
                _logger.Information("TCP listener stopped");
            }
        }

        private async Task HandleClientAsync(TcpClient client)
        {
            try
            {
                using (client)
                using (NetworkStream stream = client.GetStream())
                {
                    byte[] buffer = new byte[4096];
                    _logger.Debug("Reading message from client");
                    int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
            
                    if (bytesRead > 0)
                    {
                        string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        _logger.Debug("Received message: {Message}", message);
                        ProcessMessage(message);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error TCP client: Client handling error");
            }
        }

        private void ProcessMessage(string message)
        {
            try
            {
                _logger.Information("Processing received message: {Message}", message);
                
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
        
                var statistics = JsonSerializer.Deserialize<Statistics>(message, options);
                _logger.Information("Recived statistics: ActivePlayers={ActivePlayers}, BiggestMultiplier={BiggestMultiplier}",
                    statistics?.ActivePlayers, statistics?.BiggestMultiplier);
                if (statistics != null)
                {
                    Dispatcher.UIThread.Post(() =>
                    {
                        StatisticsReceived?.Invoke(this, statistics);
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error TCP client: Message processing failed");
            }
        }
    }
}