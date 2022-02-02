using HearthstoneParody;
using HearthstoneParody.Configs;
using HearthstoneParody.Core;
using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "CoreInstaller", menuName = "Installers/CoreInstaller")]
public class CoreInstaller : ScriptableObjectInstaller<CoreInstaller>
{
    [SerializeField]
    private CardsDatabaseConfig cardsDatabaseConfig;
    public override void InstallBindings()
    {
        Container.BindInstance(cardsDatabaseConfig).AsSingle();
        Container.BindInterfacesAndSelfTo<CardsDatabaseLoader>().AsSingle();
        Container.BindInterfacesAndSelfTo<ProgressLogger>().AsSingle();
    }
}