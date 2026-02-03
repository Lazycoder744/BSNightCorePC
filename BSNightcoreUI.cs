using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.GameplaySetup;
using System;
using Zenject;

namespace BSNightcore
{
    internal class BSNightcoreUI : IInitializable, IDisposable
    {
        private readonly GameplaySetup _gameplaySetup;

        public BSNightcoreUI(GameplaySetup gameplaySetup)
        {
            _gameplaySetup = gameplaySetup;
        }

        [UIValue("modEnabled")]
        private bool ModEnabled
        {
            get => PluginConfig.Instance.ModEnabled;
            set
            {
                PluginConfig.Instance.ModEnabled = value;
                
                if (!value && AudioManagerHolder.Instance != null)
                {
                    AudioManagerHolder.Instance.musicSpeed = 1f;
                    AudioManagerHolder.Instance.musicPitch = 1f;
                }
            }
        }

        public void Initialize()
        {
            _gameplaySetup.AddTab("BSNightCore", "BSNightcore.BSNightcoreView.bsml", this);
        }

        public void Dispose()
        {
            _gameplaySetup?.RemoveTab("BSNightCore");
        }
    }
}