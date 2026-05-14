using System;
using System.Collections.Generic;
using Mirror;

public class NetworkService : INetworkService
{
    private readonly Dictionary<string, Action<byte[]>> _clientHandlers = new();
    private readonly Dictionary<string, HashSet<NetworkConnectionToClient>> _serverSubscribers = new();

    public void Initialize()
    {
        NetworkClient.RegisterHandler<GenericNetworkMessage>(OnGenericMessageReceived);
        NetworkServer.RegisterHandler<SubscribeRequestMessage>(OnSubscribeRequestReceived);
    }

    public void Subscribe<T>(Action<T> handler) where T : struct, NetworkMessage
    {
        string typeName = typeof(T).FullName;

        if (!_clientHandlers.ContainsKey(typeName))
        {
            _clientHandlers[typeName] = (bytes) =>
            {
                NetworkReader reader = new NetworkReader(bytes);
                T message = reader.Read<T>();
                handler.Invoke(message);
            };

            if (NetworkClient.isConnected)
            {
                NetworkClient.Send(new SubscribeRequestMessage { messageTypeName = typeName });
            }
        }
    }

    public void Unsubscribe<T>(Action<T> handler) where T : struct, NetworkMessage
    {
        _clientHandlers.Remove(typeof(T).FullName);
    }

    public void SendToSubscribers<T>(T message) where T : struct, NetworkMessage
    {
        string typeName = typeof(T).FullName;

        if (_serverSubscribers.TryGetValue(typeName, out var connections))
        {
            NetworkWriter writer = new NetworkWriter();
            writer.Write(message);
            byte[] payload = writer.ToArray();

            GenericNetworkMessage wrapper = new GenericNetworkMessage
            {
                messageTypeName = typeName,
                payload = payload
            };

            foreach (var conn in connections)
            {
                conn.Send(wrapper);
            }
        }
    }

    private void OnGenericMessageReceived(GenericNetworkMessage msg)
    {
        if (_clientHandlers.TryGetValue(msg.messageTypeName, out var handler))
        {
            handler.Invoke(msg.payload);
        }
    }

    private void OnSubscribeRequestReceived(NetworkConnectionToClient conn, SubscribeRequestMessage msg)
    {
        if (!_serverSubscribers.ContainsKey(msg.messageTypeName))
        {
            _serverSubscribers[msg.messageTypeName] = new HashSet<NetworkConnectionToClient>();
        }

        _serverSubscribers[msg.messageTypeName].Add(conn);
    }
}