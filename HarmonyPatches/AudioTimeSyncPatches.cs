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
            if (_audioSourceField == null) return;

            var audioSource = _audioSourceField.GetValue(controller) as UnityEngine.AudioSource;
            if (audioSource != null)
            {
                // Set pitch equal to timeScale to remove compensation
                // If timeScale is 1.5x, pitch will be 1.5x (nightcore)
                // If timeScale is 0.5x, pitch will be 0.5x (slowcore)
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
