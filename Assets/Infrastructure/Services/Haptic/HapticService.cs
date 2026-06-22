using Infrastructure.Player;
using UnityEngine;

namespace Infrastructure.Services.Haptic
{
    public class HapticService
    {
        private readonly PlayerDataManager _playerDataManager;

#if UNITY_ANDROID && !UNITY_EDITOR
    private static AndroidJavaObject vibrator;
    private static AndroidJavaClass vibrationEffectClass;
    private static bool initialized = false;
#endif

        private static HapticService _instance;
        public static HapticService Instance => _instance;

        
        public static void Initialize(PlayerDataManager playerDataManager)
        {
            _instance = new HapticService(playerDataManager);
        }
        
        private HapticService(PlayerDataManager playerDataManager)
        {
            _playerDataManager = playerDataManager;
            Init();
        }

        public void Play(HapticType type)
        {
            if (_playerDataManager.IsVibrationsMuted)
            {
                return;
            }

            Vibrate(type);
        }

        private static void Vibrate(HapticType type)
        {
            Debug.Log($"Vibrating with type: {type}");
            //  Medium   Vibrate(100, 100);
            //  Long   Vibrate(500, 200);

            switch (type)
            {
                case HapticType.Button:
                    Vibrate(40, 40);
                    break;
                case HapticType.ThermometerInteraction:
                    Vibrate(60, 100);
                    break;
                default:
                    Debug.LogError($"Unknown vibration type: {type}");
                    Vibrate(40, 40);
                    break;
            }
        }


        private void Init()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (initialized) return;

        try
        {
            AndroidJavaClass unityPlayer =
                new AndroidJavaClass("com.unity3d.player.UnityPlayer");

            AndroidJavaObject activity =
                unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

            vibrator = activity.Call<AndroidJavaObject>(
                "getSystemService",
                "vibrator"
            );

            vibrationEffectClass =
                new AndroidJavaClass("android.os.VibrationEffect");

            initialized = true;
        }
        catch
        {
            initialized = false;
        }
#endif
        }

        private static void Vibrate(long milliseconds, int amplitude = 255)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
        Init();

        if (!initialized)
        {
            Handheld.Vibrate();
            return;
        }

        try
        {
            AndroidJavaObject effect =
                vibrationEffectClass.CallStatic<AndroidJavaObject>(
                    "createOneShot",
                    milliseconds,
                    amplitude
                );

            vibrator.Call("vibrate", effect);
        }
        catch
        {
            // fallback
            Handheld.Vibrate();
        }
#else
            Handheld.Vibrate();
#endif
        }
    }

    public enum HapticType
    {
        Button = 1,
        ThermometerInteraction = 2
    }
}