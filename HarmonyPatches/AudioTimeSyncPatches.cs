using HarmonyLib;
using System.Reflection;

namespace BSNightcore.HarmonyPatches
{
    public static class AudioTimeSyncHelper
    {
        private static FieldInfo _audioSourceField;

        static AudioTimeSyncHelper()
        {
            _audioSourceField = typeof(AudioTimeSyncController).GetField("_audioSource", BindingFlags.Instance | BindingFlags.NonPublic);
        }

        public static void RemovePitchCompensation(AudioTimeSyncController controller)
        {
            if (!PluginConfig.Instance.ModEnabled) return;
            if (_audioSourceField == null) return;

            var audioSource = _audioSourceField.GetValue(controller) as UnityEngine.AudioSource;
            if (audioSource != null)
            {
                audioSource.pitch = controller.timeScale;
            }
        }
    }

    [HarmonyPatch(typeof(AudioTimeSyncController), "StartSong")]
    public class AudioTimeSyncControllerStartPatch
    {
        static void Postfix(AudioTimeSyncController __instance)
        {
            AudioTimeSyncHelper.RemovePitchCompensation(__instance);
        }
    }

    [HarmonyPatch(typeof(AudioTimeSyncController), "set_timeScale")]
    public class AudioTimeSyncControllerTimeScalePatch
    {
        static void Postfix(AudioTimeSyncController __instance)
        {
            AudioTimeSyncHelper.RemovePitchCompensation(__instance);
        }
    }
}