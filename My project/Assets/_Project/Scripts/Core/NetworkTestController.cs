using UnityEngine;
using VContainer;

public class NetworkTestController : MonoBehaviour
{
    private INetworkService _networkService;

    [Inject]
    public void Construct(INetworkService networkService)
    {
        _networkService = networkService;
    }

    public void OnSubscribeButtonClicked()
    {
        _networkService.Subscribe<HelloMessage>(OnHelloMessageReceived);
    }

    public void OnSendHelloButtonClicked()
    {
        _networkService.SendToSubscribers(new HelloMessage
        {
            text = "Hello Client!"
        });
    }

    private void OnHelloMessageReceived(HelloMessage message)
    {
        Debug.Log($"[Client] Received: {message.text}");
    }
}