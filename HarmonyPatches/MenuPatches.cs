using HarmonyLib;
using System.Reflection;

namespace BSNightcore.HarmonyPatches
{
    [HarmonyPatch(typeof(GameplayModifiersPanelController), "RefreshTotalMultiplierAndRankUI")]
    public class RefreshMultipliersPatch
    {
        static void Postfix(GameplayModifiersPanelController __instance)
        {
            MenuAudioHelper.SetPitchSpeed(__instance);
        }
    }

    [HarmonyPatch(typeof(GameplaySetupViewController), "DidActivate")]
    public class GameplaySetupViewControllerPatch
    {
        private static FieldInfo _gameplayModifiersPanelControllerField;

        static GameplaySetupViewControllerPatch()
        {
            _gameplayModifiersPanelControllerField = typeof(GameplaySetupViewController).GetField("_gameplayModifiersPanelController", BindingFlags.Instance | BindingFlags.NonPublic);
        }

        static void Postfix(GameplaySetupViewController __instance)
        {
            var controller = _gameplayModifiersPanelControllerField.GetValue(__instance) as GameplayModifiersPanelController;
            if (controller != null)
            {
                MenuAudioHelper.SetPitchSpeed(controller);
            }
        }
    }

    [HarmonyPatch(typeof(GameplaySetupViewController), "OnDisable")]
    public class DisableGameplaySetupViewControllerPatch
    {
        static void Postfix()
        {
            if (AudioManagerHolder.Instance != null)
            {
                AudioManagerHolder.Instance.musicSpeed = 1f;
            }
        }
    }

    public static class MenuAudioHelper
    {
        private static FieldInfo _gameplayModifiersField;

        static MenuAudioHelper()
        {
            _gameplayModifiersField = typeof(GameplayModifiersPanelController).GetField("_gameplayModifiers", BindingFlags.Instance | BindingFlags.NonPublic);
        }

        public static void SetPitchSpeed(GameplayModifiersPanelController controller)
        {
            if (!PluginConfig.Instance.EnableInMenu || AudioManagerHolder.Instance == null)
                return;

            var gameplayModifiers = _gameplayModifiersField.GetValue(controller) as GameplayModifiers;
            if (gameplayModifiers == null) return;

            float multiplier = gameplayModifiers.songSpeedMul;
            
            // If speed is slower and slower mode is disabled, don't apply the effect
            if (multiplier < 1f && !PluginConfig.Instance.EnableInSlowerMode)
            {
                multiplier = 1f;
            }

            // Set the speed and disable pitch compensation
            AudioManagerHolder.Instance.musicSpeed = multiplier;
            AudioManagerHolder.Instance.musicPitch = 1f;
        }
    }
}
