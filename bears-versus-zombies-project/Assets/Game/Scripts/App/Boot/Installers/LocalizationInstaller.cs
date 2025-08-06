using System;
using Modules.Localization.Core.Detectors;
using Modules.Localization.Core.Types;
using Modules.Localization.I2System;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace SampleGame.App
{
    [Serializable]
    public sealed class LocalizationInstaller : Installer<LocalizationInstaller.Settings, LocalizationInstaller>
    {
        private readonly Settings _settings;

        public LocalizationInstaller(Settings settings)
        {
            _settings = settings;
        }
        
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<ConstantLanguageDetector>()
                .AsSingle()
                .WithArguments(_settings.DefaultLanguage)
                .WhenInjectedInto<I2LocalizationSystem>();

            Container.BindInterfacesTo<I2LocalizationSystem>()
                .AsSingle()
                .WithArguments(_settings.LanguagesMapping)
                .WhenInjectedInto<GameLocalizationSystem>();
            
            Container.BindInterfacesAndSelfTo<GameLocalizationSystem>().AsSingle();
        }

        [Serializable]
        public class Settings
        {
            [SerializeField] private Language _defaultLanguage;
            [SerializeField, Required] private I2LanguagesMapping _languagesMapping;
            
            public Language DefaultLanguage => _defaultLanguage;
            
            public I2LanguagesMapping LanguagesMapping => _languagesMapping;
        }
    }
}