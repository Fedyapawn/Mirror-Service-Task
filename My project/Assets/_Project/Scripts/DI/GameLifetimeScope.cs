using UnityEngine;
using VContainer;
using VContainer.Unity;

public class GameLifetimeScope : LifetimeScope
{
    [SerializeField] private NetworkTestController networkTestController;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.Register<NetworkService>(Lifetime.Singleton)
            .AsImplementedInterfaces()
            .AsSelf();

        builder.RegisterEntryPoint<NetworkInitializationEntryPoint>();

        builder.RegisterComponent(networkTestController);
    }
}

public class NetworkInitializationEntryPoint : IStartable
{
    private readonly NetworkService _networkService;

    public NetworkInitializationEntryPoint(NetworkService networkService)
    {
        _networkService = networkService;
    }

    public void Start()
    {
        _networkService.Initialize();
    }
}