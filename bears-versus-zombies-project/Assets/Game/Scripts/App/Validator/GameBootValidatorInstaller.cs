using SampleGame.Gameplay;
using UnityEngine;
using Zenject;

namespace SampleGame.App
{
    [CreateAssetMenu(fileName = "GameBootValidatorInstaller", 
        menuName = "SampleGame/Installers/GameBootValidatorInstaller")]
    public sealed class GameBootValidatorInstaller : ScriptableObjectInstaller
    {
        public override void InstallBindings()
        {
            // Container.BindInterfacesAndSelfTo<GameBootValidator>().AsSingle().NonLazy();
        }
    }
}