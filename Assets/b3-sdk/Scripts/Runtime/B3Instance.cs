using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using System.Web;
using System;
using Unity.VisualScripting;
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

namespace BasementSDK {
    public class B3Instance : MonoBehaviour
    {
        public B3ConfigSO b3Config;
        public static B3Instance Instance {
            get;
            private set;
        }

        public string SessionJWT {get; private set;}
        private B3LauncherClient.ChannelStatus channelStatus;
        public static string DeeplinkURL  {get; private set;}
        private static Coroutine heartbeatCoroutine;

        public static event Action<B3LauncherClient.ChannelStatus> OnSessionReady;
        
        void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            if (Application.platform == RuntimePlatform.WebGLPlayer){
                TryInitChannelWebGL();
                InitWebhooks();
            }
            SetupDeeplinks();
        }
        private void SetupDeeplinks(){
            Application.deepLinkActivated += OnDeepLinkActivated;
            if (!string.IsNullOrEmpty(Application.absoluteURL))
            {
                OnDeepLinkActivated(Application.absoluteURL);
            }
        }
                
        public void OnDeepLinkActivated(string url)
        {
            DeeplinkURL = url;
            Uri uri = new(url);
            var queryParams = HttpUtility.ParseQueryString(uri.Query);

            string token = queryParams.Get("token");
            if (!string.IsNullOrEmpty(token))
            {
                SessionJWT = token;
                SetupSessionFromJWT();
            }
        }

        private bool InitWebhooks(){
            if(SessionJWT == null) return false;
            _ = new GameObject("B3WebhooksHandler").AddComponent<B3WebhooksHandler>();
            return true;
        }

        private async Task TryInitChannelWebGL()
        {
            if (await SetupSessionFromJWT()) return;

            try
            {
                SessionJWT = B3LauncherJSLibUtil.GetQueryParam("token");
                if (SessionJWT != null){
                    if (await SetupSessionFromJWT(b3Config.isWebGLGameEmbedded)) return;
                }
            }
            catch {}
        }
        private async Task<bool> SetupSessionFromJWT(bool runHeartbeat = false)
        {
            if(SessionJWT == null) return false;
            channelStatus = await B3LauncherClient.GetChannelStatus(new B3LauncherClient.GetChannelStatusBody { launcherJwt = SessionJWT }, null);
            OnSessionReady?.Invoke(channelStatus);
            if (runHeartbeat){
                if (heartbeatCoroutine != null) StopCoroutine(heartbeatCoroutine);
                heartbeatCoroutine = StartCoroutine(Heartbeat());
            }
            return channelStatus != null && channelStatus.exists && channelStatus.present;
        }

        IEnumerator Heartbeat()
        {
            while (true)
            {
                yield return new WaitForSeconds(60f);
                B3LauncherClient.ChannelHeartbeat(new B3LauncherClient.ChannelHeartbeatBody { launcherJwt = SessionJWT }, null);
            }
        }

        public void LoginViaSSO(Action<B3LauncherClient.ChannelStatus> callback){
            Application.OpenURL($"https://basement.fun/games/{b3Config.gameSlug}/sso");
            Action<B3LauncherClient.ChannelStatus> cb = null;
            cb = (status) => {
                callback?.Invoke(status);
                OnSessionReady -= cb;
            };
            OnSessionReady+=cb;
        }
    }
}

