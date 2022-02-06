using HearthstoneParody.Configs;
using HearthstoneParody.Core;
using HearthstoneParody.Data;
using UnityEngine;
using Zenject;

namespace HearthstoneParody.Installers
{
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

            Container.BindFactory<CardTemplate, Player, Card, Card.Factory>();
        }
    }
}