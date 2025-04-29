using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.Logging;

namespace Common.Configuration
{
    public class ServicePorts
    {
        public int ApiGateway { get; private set; } = 5001;
        public int Products { get; private set; } = 5011;
        public int Orders { get; private set; } = 5012;
        public int Payments { get; private set; } = 5013;
        public int Notifications { get; private set; } = 5014;
        public int Users { get; private set; } = 5015;
        public int Inventory { get; private set; } = 5018;
        public int Frontend { get; private set; } = 5019;

        private readonly Dictionary<string, (int DefaultPort, int CurrentPort)> _portMap;
        private readonly ILogger<ServicePorts> _logger;

        public ServicePorts(ILogger<ServicePorts> logger)
        {
            _logger = logger;
            _portMap = new Dictionary<string, (int, int)>
            {
                { "ApiGateway", (5001, ApiGateway) },
                { "Products", (5011, Products) },
                { "Orders", (5012, Orders) },
                { "Payments", (5013, Payments) },
                { "Notifications", (5014, Notifications) },
                { "Users", (5015, Users) },
                { "Inventory", (5018, Inventory) },
                { "Frontend", (5019, Frontend) }
            };
        }

        public int GetAvailablePort(string serviceName)
        {
            if (!_portMap.ContainsKey(serviceName))
            {
                throw new ArgumentException($"Unknown service: {serviceName}");
            }

            var (defaultPort, currentPort) = _portMap[serviceName];
            var port = currentPort;

            while (!IsPortAvailable(port))
            {
                _logger.LogWarning("Port {Port} is in use for {Service}, attempting next available port", port, serviceName);
                port++;
                
                // If we've tried 100 ports after the default, something is wrong
                if (port > defaultPort + 100)
                {
                    _logger.LogError("Unable to find available port for {Service} after 100 attempts", serviceName);
                    throw new InvalidOperationException($"Unable to find available port for {serviceName}");
                }
            }

            // Update the current port if it changed
            if (port != currentPort)
            {
                _logger.LogInformation("Service {Service} will use port {Port} instead of {DefaultPort}", 
                    serviceName, port, defaultPort);
                _portMap[serviceName] = (defaultPort, port);
                
                // Update the property
                var prop = GetType().GetProperty(serviceName);
                prop?.SetValue(this, port);
            }

            return port;
        }

        private bool IsPortAvailable(int port)
        {
            try
            {
                using var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), port));
                return true;
            }
            catch (SocketException)
            {
                return false;
            }
        }
    }
} 