using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace BasementSDK
{
    public class B3WebhooksHandler : MonoBehaviour
    {
        public static event Action<BaseWebhookData> OnWebhookReceived;
        void Awake()
        {
            B3LauncherJSLibUtil.InjectWebhooks();
        }

        void onWebhookReceived(string data){
            BaseWebhookData webhookData = JsonConvert.DeserializeObject<BaseWebhookData>(data);
            OnWebhookReceived?.Invoke(webhookData);
        }

        [Serializable]
        public class BaseWebhookData{
            public string callbackType;
            public string callbackUuid;
            public string ruleActionType;
            public object data;
        }
    }
}

