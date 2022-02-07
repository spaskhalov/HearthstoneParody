using HearthstoneParody.Configs;
using UnityEngine;
using Zenject;

namespace HearthstoneParody.Installers
{
    [CreateAssetMenu(fileName = "GameSettings", menuName = "Installers/GameSettingsInstaller")]
    public class GameSettingsInstaller : ScriptableObjectInstaller<GameSettingsInstaller>
    {
        [SerializeField] private CardsDatabaseConfig cardsDatabaseConfig;
        [SerializeField] private GameBalanceConfig gameBalanceConfig;
        public override void InstallBindings()
        {
            Container.BindInstance(cardsDatabaseConfig).AsSingle();
            Container.BindInstance(gameBalanceConfig).AsSingle();
        }
    }
}