using Modules.Localization.Core.Detectors;
using Modules.Localization.Core.Types;
using Modules.Localization.I2System;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace SampleGame.App
{
    [CreateAssetMenu(fileName = "LocalizationInstaller", 
        menuName = "SampleGame/Installers/LocalizationInstaller")]
    public sealed class LocalizationInstaller : ScriptableObjectInstaller<LocalizationInstaller>
    {
        [SerializeField] private Language _defaultLanguage;
        [SerializeField, Required] private I2LanguagesMapping _languagesMapping;
        
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<ConstantLanguageDetector>()
                .AsSingle()
                .WithArguments(_defaultLanguage)
                .WhenInjectedInto<I2LocalizationManager>();

            Container.BindInterfacesTo<I2LocalizationManager>()
                .AsSingle()
                .WithArguments(_languagesMapping)
                .WhenInjectedInto<LocalizationManager>();
            
            Container.BindInterfacesAndSelfTo<LocalizationManager>().AsSingle();
            Container.BindInterfacesTo<LocalizationManagerSerializer>().AsSingle();
        }
    }
}