using UnityEngine;

public static class Vibrator
{
    #if UNITY_ANDROID && !UNITY_EDITOR
        public static AndroidJavaClass UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        public static AndroidJavaObject CurrentActivity = UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        public static AndroidJavaObject vibrator = CurrentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");
    #else
        public static AndroidJavaClass UnityPlayer;
        public static AndroidJavaObject CurrentActivity;
        public static AndroidJavaObject vibrator;
    #endif

    public static void Vibrate(long milliseconds = 250)
    {
        if(IsAndroid())
            vibrator.Call("vibrate", milliseconds);
        else
            Handheld.Vibrate();
    }

    public static void Cancel()
    {
        if (IsAndroid())
        {
            vibrator.Call("cancel");
        }
    }

    public static bool IsAndroid()
    {
        #if UNITY_ANDROID && !UNITY_EDITOR
            return true;
        #else
            return false;
        #endif
    }
}
