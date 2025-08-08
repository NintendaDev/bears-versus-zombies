using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace SampleGame.App
{
    [CreateAssetMenu(fileName = "GameFacadeInstaller", 
        menuName = "SampleGame/Installers/GameFacadeInstaller")]
    public sealed class GameFacadeInstaller : ScriptableObjectInstaller<GameFacadeInstaller>
    {
        [SerializeField, MinValue(0)]
        private float _serverDelaySeconds;
        
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<ConnectionTokenServiceMock>()
                .AsSingle()
                .WithArguments(_serverDelaySeconds);
            
            Container.BindInterfacesAndSelfTo<GameFacade>().AsSingle();
        }
    }
}