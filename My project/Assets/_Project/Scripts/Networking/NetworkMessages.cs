using Mirror;

public struct GenericNetworkMessage : NetworkMessage
{
    public string messageTypeName; 
    public byte[] payload;         
}

public struct SubscribeRequestMessage : NetworkMessage
{
    public string messageTypeName;
}

public struct HelloMessage : NetworkMessage
{
    public string text;
}