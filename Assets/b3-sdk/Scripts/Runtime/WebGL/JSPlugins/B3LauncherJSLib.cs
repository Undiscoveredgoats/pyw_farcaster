using System;
using System.Runtime.InteropServices;
namespace BasementSDK {
    public static class B3LauncherJSLibUtil{
        [DllImport("__Internal")]
        public static extern string GetQueryParam(string paramId);

        [DllImport("__Internal")]
        public static extern void InjectWebhooks();

        [DllImport("__Internal")]
        public static extern void PostLauncherMessage(string message);
    }
}