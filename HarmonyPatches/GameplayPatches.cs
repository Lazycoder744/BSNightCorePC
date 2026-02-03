using HarmonyLib;
using System.Reflection;

namespace BSNightcore.HarmonyPatches
{
    [HarmonyPatch(typeof(GameplayCoreInstaller), "InstallBindings")]
    public class GameplayCoreInstallerPatch
    {
        private static FieldInfo _audioManagerField;

        static GameplayCoreInstallerPatch()
        {
            _audioManagerField = typeof(GameplayCoreInstaller).GetField("_audioManager", BindingFlags.Instance | BindingFlags.NonPublic);
        }

        static void Postfix(GameplayCoreInstaller __instance)
        {
            if (!PluginConfig.Instance.ModEnabled) return;

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