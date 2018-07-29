using System;
using System.Configuration;
using StackExchange.Redis;

namespace DomainDrivenDesign.Core.Redis
{
    public static class RedisServices
    {
        static IServer _server;
        static SocketManager _socketManager;
        static IConnectionMultiplexer _connectionMultiplexer;

        public static bool IsEnable { get; private set; }

        static IConnectionMultiplexer RedisConnectionMultiplexer
        {
            get
            {
                if (_connectionMultiplexer != null && _connectionMultiplexer.IsConnected)
                    return _connectionMultiplexer;

                if (_connectionMultiplexer != null && !_connectionMultiplexer.IsConnected)
                {
                    _connectionMultiplexer.Dispose();
                }

                _connectionMultiplexer = GetConnection();
                if (!_connectionMultiplexer.IsConnected)
                {
                    var exception = new Exception("Can not connect to redis");
                    Console.WriteLine(exception);
                    throw exception;
                }
                return _connectionMultiplexer;
            }
        }

        public static IDatabase RedisDatabase
        {
            get
            {
                var redisDatabase = RedisConnectionMultiplexer.GetDatabase();

                return redisDatabase;
            }
        }

        public static ISubscriber RedisSubscriber
        {
            get
            {
                var redisSubscriber = RedisConnectionMultiplexer.GetSubscriber();

                return redisSubscriber;
            }
        }

        static RedisServices()
        {
            var endpoint = ConfigurationManager.AppSettings["RedisEndpoint"];

            IsEnable = !string.IsNullOrEmpty(endpoint);

            var soketName = endpoint ?? "127.0.0.1";
            _socketManager = new SocketManager(soketName);
        }

        static ConnectionMultiplexer GetConnection()
        {
            var endpoint = ConfigurationManager.AppSettings["RedisEndpoint"];

            IsEnable = !string.IsNullOrEmpty(endpoint);

            endpoint = endpoint ?? "127.0.0.1";
            var port = int.Parse(ConfigurationManager.AppSettings["RedisPort"] ?? "6379");
            var pwd = ConfigurationManager.AppSettings["RedisPassword"] ?? "badpaybad.info";

            var options = new ConfigurationOptions
            {
                EndPoints =
                {
                    {endpoint, port}
                },
                Password = pwd,
                AllowAdmin = false,
                SyncTimeout = 5 * 1000,
                SocketManager = _socketManager,
                AbortOnConnectFail = false,
                ConnectTimeout = 5 * 1000,
            };

            return ConnectionMultiplexer.Connect(options);
        }
    }
}