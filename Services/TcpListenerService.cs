using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using Models;
using Serilog;

namespace Services
{
    /// <summary>
    /// Service for receiving statistics data over TCP connections.
    /// </summary>
    public class TcpListenerService
    {
        private TcpListener? _listener;
        private readonly int _port = 4948;
        private bool _isRunning;
        private readonly ILogger _logger;
        private readonly IDispatcher _dispatcher;
        
        // Event raised when statistics data is received and validated.
        public event EventHandler<Statistics>? StatisticsReceived;

        
        // <summary>
        /// Initializes a new TCP listener service with optional logger and dispatcher.
        /// </summary>
        /// <param name="logger">Logger for service activity</param>
        /// <param name="dispatcher">UI dispatcher for event invocation (uses Avalonia if null)</param>
        public TcpListenerService(ILogger? logger = null, IDispatcher? dispatcher = null)
        {
            _logger = logger ?? Log.Logger;
            _dispatcher = dispatcher ?? new AvaloniaDispatcher();
        }
        
        // Starts the TCP listener service on the configured port.
        public void Start()
        {
            _isRunning = true;
            _logger.Information("TcpListenerService is running on port {Port}", _port);
            _ = Task.Run(ListenForMessages);
        }

        // Stops the TCP listener service.
        public void Stop()
        {
            _isRunning = false;
            _listener?.Stop();
            _logger.Information("TcpListenerService stopped");
        }

        // Listens for incoming connections and manages client connection acceptance.
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

        /// <summary>
        /// Processes a client connection to receive statistics data.
        /// </summary>
        /// <param name="client">Connected TCP client</param>
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

        /// <summary>
        /// Deserializes and validates received JSON message as Statistics object.
        /// </summary>
        /// <param name="message">JSON message to process</param>
        private void ProcessMessage(string message)
        {
            try
            {
                _logger.Information("Processing received message: {Message}", message);
                
                if (string.IsNullOrWhiteSpace(message)) 
                {
                    _logger.Warning("Received empty message, ignoring");
                    return;
                }
                
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
        
                var statistics = JsonSerializer.Deserialize<Statistics>(message, options);
                
                if (statistics == null)
                {
                    _logger.Warning("Failed to deserialize message");
                    return;
                }
                
                _logger.Information("Recived statistics: ActivePlayers={ActivePlayers}, BiggestMultiplier={BiggestMultiplier}",
                    statistics?.ActivePlayers, statistics?.BiggestMultiplier);
                
                bool hasValidData = statistics.ActivePlayers.HasValue &&   statistics.ActivePlayers >=0
                                    || statistics.BiggestMultiplier.HasValue && statistics.BiggestMultiplier > 0;
                
                if (hasValidData)
                {
                    _dispatcher.Post(() =>
                    {
                        StatisticsReceived?.Invoke(this, statistics);
                    });
                }
                else
                {
                    _logger.Warning("Received statistics with no useful values, ignoring");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error processing message: {Message}", ex.Message);
            }
        }
        
    }
    
    
}