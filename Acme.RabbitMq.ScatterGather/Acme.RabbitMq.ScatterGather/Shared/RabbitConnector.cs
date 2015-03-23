using System;
using RabbitMQ.Client;
using RabbitMQ.Client.MessagePatterns;

namespace Shared
{
    public class RabbitConnector
    {
        protected RabbitConnectionSettings _connectionSettings;
        protected ConnectionFactory _connectionFactory;
        protected IConnection _connection;
        protected IModel _model;
        protected Subscription _subscription;

        protected void ConfigureConnection()
        {
            _connectionFactory = new ConnectionFactory
            {
                HostName = _connectionSettings.HostName,
                UserName = _connectionSettings.Username,
                Password = _connectionSettings.Password
            };
            if (string.IsNullOrEmpty(_connectionSettings.VirtualHost) == false)
                _connectionFactory.VirtualHost = _connectionSettings.VirtualHost;
            if (_connectionSettings.Port > 0)
                _connectionFactory.Port = _connectionSettings.Port;

            _connection = _connectionFactory.CreateConnection();
            _model = _connection.CreateModel();
        }

        
    }
}
