using HarmonyLib;
using System.Reflection;

namespace BSNightcore.HarmonyPatches
{
    [HarmonyPatch(typeof(GameplayCoreInstaller), "InstallBindings")]
    public class GameplayCoreInstallerPatch
    {
        private static FieldInfo _sceneSetupDataField;
        private static FieldInfo _audioManagerField;

        static GameplayCoreInstallerPatch()
        {
            _sceneSetupDataField = typeof(GameplayCoreInstaller).GetField("_sceneSetupData", BindingFlags.Instance | BindingFlags.NonPublic);
            _audioManagerField = typeof(GameplayCoreInstaller).GetField("_audioManager", BindingFlags.Instance | BindingFlags.NonPublic);
        }

        static void Postfix(GameplayCoreInstaller __instance)
        {
            var sceneSetupData = _sceneSetupDataField.GetValue(__instance) as GameplayCoreSceneSetupData;
            if (sceneSetupData == null) return;

            // Check if we're in practice mode
            bool inPracticeMode = sceneSetupData.practiceSettings != null;
            
            // If in practice mode and practice is disabled, don't apply
            if (inPracticeMode && !PluginConfig.Instance.EnableInPractice)
                return;

            // Get the speed multiplier
            float speedMul = inPracticeMode ? 
                sceneSetupData.practiceSettings.songSpeedMul : 
                sceneSetupData.gameplayModifiers.songSpeedMul;

            // If speed is slower and slower mode is disabled, don't apply
            if (speedMul < 1f && !PluginConfig.Instance.EnableInSlowerMode)
                return;

            // Remove pitch compensation by setting musicPitch to 1
            var audioManager = _audioManagerField.GetValue(__instance) as AudioManagerSO;
            if (audioManager != null)
            {
                audioManager.musicPitch = 1f;
            }
        }
    }

    [HarmonyPatch(typeof(SettingsApplicatorSO), "ApplyGameSettings")]
    public class SettingsApplicatorPatch
    {
        private static FieldInfo _audioManagerField;

        static SettingsApplicatorPatch()
        {
            _audioManagerField = typeof(SettingsApplicatorSO).GetField("_audioManager", BindingFlags.Instance | BindingFlags.NonPublic);
        }

        static void Postfix(SettingsApplicatorSO __instance)
        {
            var audioManager = _audioManagerField.GetValue(__instance) as AudioManagerSO;
            if (audioManager != null)
            {
                AudioManagerHolder.Instance = audioManager;
            }
        }
    }
}
