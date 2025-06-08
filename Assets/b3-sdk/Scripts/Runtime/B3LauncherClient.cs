using System;
using System.Threading.Tasks;
using UnityEngine;
using BasementSDK.Utils;
using Newtonsoft.Json;
namespace BasementSDK {
    public static class B3LauncherClient {
        #region API Calls
        public static async Task<ChannelStatus> GetChannelStatus(GetChannelStatusBody data, Action<ChannelStatus> callback)
        {
            string res = await B3Utils.PostJSON("https://api.basement.fun/launcher", "channelStatus", data.ToJSON(), null);
            if (res == null)
            {
                callback?.Invoke(null);
                return null;
            }
            ChannelStatus result = JsonConvert.DeserializeObject<ChannelStatus>(res);
            callback?.Invoke(result);
            return result;
        }
        public static async Task<SendNotificationResult> SendNotification(SendNotificationBody data, Action<SendNotificationResult> callback)
        {
            string res = await B3Utils.PostJSON("https://api.basement.fun/launcher", "sendNotification", data.ToJSON(), null);
            if (res == null)
            {
                callback?.Invoke(null);
                return null;
            }
            SendNotificationResult result = JsonConvert.DeserializeObject<SendNotificationResult>(res);
            callback?.Invoke(result);
            return result;
        }
        public static async Task<GetUserActivitiesResult> GetUserActivities(GetUserActivitiesBody data, Action<GetUserActivitiesResult> callback)
        {
            string res = await B3Utils.GetJSON($"https://api.basement.fun/activities?pageSize={data.pageSize}&pageNumber={data.pageNumber}&walletAddress={data.walletAddress}&type={data.type}", null, null);
            if (res == null)
            {
                callback?.Invoke(null);
                return null;
            }
            GetUserActivitiesResult result = JsonConvert.DeserializeObject<GetUserActivitiesResult>(res);
            callback?.Invoke(result);
            return result;
        }
        public static async Task<SendCustomActivityResult> SendCustomActivity(SendCustomActivityBody data, Action<SendCustomActivityResult> callback)
        {
            string res = await B3Utils.PostJSON("https://api.basement.fun/launcher", "sendCustomActivity", data.ToJSON(), null);
            if (res == null)
            {
                callback?.Invoke(null);
                return null;
            }
            SendCustomActivityResult result = JsonConvert.DeserializeObject<SendCustomActivityResult>(res);
            callback?.Invoke(result);
            return result;
        }
        public static async Task<GetStateResult<T>> GetState<T>(GetStateBody data, Action<GetStateResult<T>> callback)
        {
            string res = await B3Utils.PostJSON("https://api.basement.fun/launcher", "getState", data.ToJSON(), null);
            if (res == null)
            {
                callback?.Invoke(null);
                return null;
            }
            GetStateResult<T> result = JsonConvert.DeserializeObject<GetStateResult<T>>(res);
            callback?.Invoke(result);
            return result;
        }
        /// <summary>
        /// Set the saved state for the currently authed user.
        /// </summary>
        /// <typeparam name="T">Serializable type of the state object</typeparam>
        /// <param name="data"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public static async Task<SetStateResult> SetState<T>(SetStateBody<T> data, Action<SetStateResult> callback)
        {
            string res = await B3Utils.PostJSON("https://api.basement.fun/launcher", "setState", data.ToJSON(), null);
            if (res == null)
            {
                callback?.Invoke(null);
                return null;
            }
            SetStateResult result = JsonConvert.DeserializeObject<SetStateResult>(res);
            callback?.Invoke(result);
            return result;
        }
        public static async Task<GetLeaderboardResult> GetLeaderboard(GetLeaderboardBody data, Action<GetLeaderboardResult> callback)
        {
            string res = await B3Utils.PostJSON("https://api.basement.fun/launcher", "getGameScoresLeaderboard", data.ToJSON(), null);
            if (res == null)
            {
                callback?.Invoke(null);
                return null;
            }
            GetLeaderboardResult result = JsonConvert.DeserializeObject<GetLeaderboardResult>(res);
            callback?.Invoke(result);
            return result;
        }
        public static async Task<GetUserLeaderboardPositionResult> GetUserLeaderboardPosition(GetUserLeaderboardPositionBody data, Action<GetUserLeaderboardPositionResult> callback)
        {
            string res = await B3Utils.PostJSON("https://api.basement.fun/launcher", "getUsersPositionInGameScoreLeaderboard", data.ToJSON(), null);
            if (res == null)
            {
                callback?.Invoke(null);
                return null;
            }
            GetUserLeaderboardPositionResult result = JsonConvert.DeserializeObject<GetUserLeaderboardPositionResult>(res);
            callback?.Invoke(result);
            return result;
        }
        public static async Task<GetUserScoresResult> GetUserScores(GetUserScoresBody data, Action<GetUserScoresResult> callback)
        {
            string res = await B3Utils.PostJSON("https://api.basement.fun/launcher", "getUserScores", data.ToJSON(), null);
            if (res == null)
            {
                callback?.Invoke(null);
                return null;
            }
            GetUserScoresResult result = JsonConvert.DeserializeObject<GetUserScoresResult>(res);
            callback?.Invoke(result);
            return result;
        }
        public static async Task<SetUserScoreResult> SetUserScore(SetUserScoreBody data, Action<SetUserScoreResult> callback)
        {
            string res = await B3Utils.PostJSON("https://api.basement.fun/launcher", "setUserScore", data.ToJSON(), null);
            if (res == null)
            {
                callback?.Invoke(null);
                return null;
            }
            SetUserScoreResult result = JsonConvert.DeserializeObject<SetUserScoreResult>(res);
            callback?.Invoke(result);
            return result;
        }
        public static async Task<CreateUnverifiedChannelResult> CreateUnverifiedChannel(CreateUnverifiedChannelBody data, Action<CreateUnverifiedChannelResult> callback)
        {
            string res = await B3Utils.PostJSON("https://api.basement.fun/launcher", "createUnverifiedChannel", data.ToJSON(), null);
            if (res == null)
            {
                callback?.Invoke(null);
                return null;
            }
            CreateUnverifiedChannelResult result = JsonConvert.DeserializeObject<CreateUnverifiedChannelResult>(res);
            callback?.Invoke(result);
            return result;
        }
        public static async Task<VerifyUnverifiedChannelResult> VerifyUnverifiedChannel(VerifyUnverifiedChannelBody data, Action<VerifyUnverifiedChannelResult> callback)
        {
            string res = await B3Utils.PostJSON("https://api.basement.fun/launcher", "verifyUnverifiedChannel", data.ToJSON(), null);
            if (res == null)
            {
                callback?.Invoke(null);
                return null;
            }
            VerifyUnverifiedChannelResult result = JsonConvert.DeserializeObject<VerifyUnverifiedChannelResult>(res);
            callback?.Invoke(result);
            return result;
        }
        public static async Task<GenericResult> ChannelHeartbeat(ChannelHeartbeatBody data, Action<GenericResult> callback)
        {
            string res = await B3Utils.PostJSON("https://api.basement.fun/launcher", "channelHeartbeat", data.ToJSON(), null);
            if (res == null)
            {
                callback?.Invoke(null);
                return null;
            }
            GenericResult result = JsonConvert.DeserializeObject<GenericResult>(res);
            callback?.Invoke(result);
            return result;
        }
        public static async Task<TriggerRulesEngineResult> TriggerRulesEngine(TriggerRulesEngineBody data, Action<TriggerRulesEngineResult> callback)
        {
            string res = await B3Utils.PostJSON("https://api.basement.fun/launcher", "triggerRulesEngine", data.ToJSON(), null);
            if (res == null)
            {
                callback?.Invoke(null);
                return null;
            }
            TriggerRulesEngineResult result = JsonConvert.DeserializeObject<TriggerRulesEngineResult>(res);
            callback?.Invoke(result);
            return result;
        }
        public static async Task<CreateMessageChannelResult> CreateMessageChannel(CreateMessageChannelBody data, Action<CreateMessageChannelResult> callback)
        {
            string res = await B3Utils.PostJSON("https://api.basement.fun/launcher", "createMessageChannel", data.ToJSON(), null);
            if (res == null)
            {
                callback?.Invoke(null);
                return null;
            }
            CreateMessageChannelResult result = JsonConvert.DeserializeObject<CreateMessageChannelResult>(res);
            callback?.Invoke(result);
            return result;
        }
        public static async Task<GetMessageChannelsResult> GetMessageChannels(GetMessageChannelsBody data, Action<GetMessageChannelsResult> callback)
        {
            string res = await B3Utils.PostJSON("https://api.basement.fun/launcher", "getMessageChannels", data.ToJSON(), null);
            if (res == null)
            {
                callback?.Invoke(null);
                return null;
            }
            GetMessageChannelsResult result = JsonConvert.DeserializeObject<GetMessageChannelsResult>(res);
            callback?.Invoke(result);
            return result;
        }
        public static async Task<GetChannelMessagesResult> GetChannelMessages(GetChannelMessagesBody data, Action<GetChannelMessagesResult> callback)
        {
            string res = await B3Utils.PostJSON("https://api.basement.fun/launcher", "getChannelMessages", data.ToJSON(), null);
            if (res == null)
            {
                callback?.Invoke(null);
                return null;
            }
            GetChannelMessagesResult result = JsonConvert.DeserializeObject<GetChannelMessagesResult>(res);
            callback?.Invoke(result);
            return result;
        }
        public static async Task<SendChannelMessageResult> SendChannelMessage(SendChannelMessageBody data, Action<SendChannelMessageResult> callback)
        {
            string res = await B3Utils.PostJSON("https://api.basement.fun/launcher", "sendChannelMessage", data.ToJSON(), null);
            if (res == null)
            {
                callback?.Invoke(null);
                return null;
            }
            SendChannelMessageResult result = JsonConvert.DeserializeObject<SendChannelMessageResult>(res);
            callback?.Invoke(result);
            return result;
        }
        public static async Task<EditChannelMessageResult> EditChannelMessage(EditChannelMessageBody data, Action<EditChannelMessageResult> callback)
        {
            string res = await B3Utils.PostJSON("https://api.basement.fun/launcher", "editChannelMessage", data.ToJSON(), null);
            if (res == null)
            {
                callback?.Invoke(null);
                return null;
            }
            EditChannelMessageResult result = JsonConvert.DeserializeObject<EditChannelMessageResult>(res);
            callback?.Invoke(result);
            return result;
        }
        public static async Task<UnsendChannelMessageResult> UnsendChannelMessage(UnsendChannelMessageBody data, Action<UnsendChannelMessageResult> callback)
        {
            string res = await B3Utils.PostJSON("https://api.basement.fun/launcher", "unsendChannelMessage", data.ToJSON(), null);
            if (res == null)
            {
                callback?.Invoke(null);
                return null;
            }
            UnsendChannelMessageResult result = JsonConvert.DeserializeObject<UnsendChannelMessageResult>(res);
            callback?.Invoke(result);
            return result;
        }
        #endregion

        #region LauncherModals
        public static void OpenTipModal(string walletToTip)
        {
            B3LauncherJSLibUtil.PostLauncherMessage(
                JsonConvert.SerializeObject(new
                {
                    type = "TIP_REQUEST",
                    walletToTip,
                })
            );
        }

        public static void OpenTradeModal(string otherWallet){
            B3LauncherJSLibUtil.PostLauncherMessage(
                JsonConvert.SerializeObject(new
                {
                    type = "TRADE_REQUEST",
                    otherWallet,
                })
            );
        }       
        #endregion

        #region Object Types
        public static class B3ObjectTypes {
            [Serializable]
            public class CustomActivity {
                public string _id;
                public string label;
                public string normalizedAddress;
                public long timestamp;
                public string eventId;
                public string gameId;
            }
            [Serializable]
            public class StateInfo {
                public string _id;
                public string gameId;
                public string ipfsHash;
                public string normalizedAddress;
                public string label;
                public long updatedAt;
            }
            [Serializable]
            public class State<T> : StateInfo{
                public T state;
            }
            [Serializable]
            public class Score {
                public string _id;
                public string nonce;
                public string gameId;
                public string normalizedAddress;
                public long score;
                public long updatedAt;
            }
            [Serializable]
            public class MessageChannel {
                [Serializable]
                public class Participant {
                    public string wallet;
                    public int userGroup;
                    public string[] permissions;
                }
                public string _id;
                public string name;
                public string chatPicture;
                public long createdAt;
                public long updatedAt;
                public Participant[] participants;
                public string gameId;
            }
            [Serializable]
            public class Message {
                public string _id;
                public string channelId;
                public string senderId;
                public string content;
                public long createdAt;
                public long? updatedAt;
            }
        }
        #endregion

        #region API Types
        [Serializable]
        public class GetChannelStatusBody : B3Utils.JSONAble
        {
            public string launcherJwt;
        }
        [Serializable]
        public class ChannelStatus {
            public bool exists;
            public bool present;
            public string wallet;
            public long openedAt;
        }
        [Serializable]
        public class SendNotificationBody : B3Utils.JSONAble 
        {
            public string launcherJwt;
            public string message;
            public string type;
        }
        [Serializable]
        public class SendNotificationResult {
            public bool success;
        }
        [Serializable]
        public class GetUserActivitiesBody : B3Utils.JSONAble {
            public int pageSize;
            public int pageNumber;
            public string walletAddress;
            public string type;
        }
       
        [Serializable]
        public class GetUserActivitiesResult {
             [Serializable]
            public class Activity {
                [Serializable]
                public class User {
                    public string id;
                    public string address;
                    public string username;
                }
                public string type;
                public string eventId;
                public string gameId;
                public string gameName;
                public string gameSlug;
                public User user;
                public string displayText;
                public long timestamp;
            }
            public Activity[] activities;
            public int total;
        }
        [Serializable]
        public class SendCustomActivityBody : B3Utils.JSONAble {
            public string launcherJwt;
            public string label;
            public string eventId;
        }
        [Serializable]
        public class SendCustomActivityResult {
            public bool success;
            public B3ObjectTypes.CustomActivity activity;
        }
        [Serializable]
        public class GetStateBody : B3Utils.JSONAble {
            public string launcherJwt;
            public int limit;
            public int skip;
            public string label;
        }
        [Serializable]
        public class GetStateResult<T> {
            public bool success;
            public B3ObjectTypes.State<T>[] states;
        }
        [Serializable]
        public class SetStateBody<T> : B3Utils.JSONAble {
            public string launcherJwt;
            public string label;
            public T state;
        }
        [Serializable]
        public class SetStateResult {
            public bool success;
            public B3ObjectTypes.StateInfo newState;
        }
        [Serializable]
        public class GetLeaderboardBody : B3Utils.JSONAble{
            public string gameId;
            public int limit;
            public int skip;
        }
        [Serializable]
        public class GetLeaderboardResult {
            [Serializable]
            public class LeaderboardEntry {
                public string _id;
                public string nonce;
                public string gameId;
                public string normalizedAddress;
                public int score;
                public long updatedAt;
                public string username;
                public string avatar;
            }
            public bool success;
            public LeaderboardEntry[] leaderboard;
        }
        [Serializable]
        public class GetUserLeaderboardPositionBody : B3Utils.JSONAble {
            public string gameId;
            public string wallet;
        }
        [Serializable]
        public class GetUserLeaderboardPositionResult {
            public bool success;
            public int position;
        }
        [Serializable]
        public class GetUserScoresBody : B3Utils.JSONAble{
            public string launcherJwt;
            public int limit;
            public int skip;
            /// <summary>
            /// Optional. If provided, will fetch score with nonce.
            /// </summary>
            public string nonce;
        }
        [Serializable]
        public class GetUserScoresResult {
            public bool success;
            [Serializable]
            public class Score {
                public int score;
                public string nonce;
                public long updatedAt;
            }
        }
        [Serializable]
        public class SetUserScoreBody : B3Utils.JSONAble {
            public string launcherJwt;
            public int score;
            public string nonce;
        }
        [Serializable]
        public class SetUserScoreResult {
            public bool success;
            public B3ObjectTypes.Score newScore;
        }
        [Serializable]
        public class CreateUnverifiedChannelBody : B3Utils.JSONAble {
            public string wallet;
        }
        [Serializable]
        public class CreateUnverifiedChannelResult {
            public bool success;
            public string signRequest;
            public string channelId;
        }
        [Serializable]
        public class VerifyUnverifiedChannelBody : B3Utils.JSONAble {
            public string channelId;
            public string signature;
            public int? chainId;
        }
        [Serializable]
        public class VerifyUnverifiedChannelResult {
            public bool success;
            public string jwt;
        }
        [Serializable]
        public class ChannelHeartbeatBody : B3Utils.JSONAble {
            public string launcherJwt;
        }
        [Serializable]
        public class GenericResult {
            public bool success;
        }
        [Serializable]
        public class TriggerRulesEngineBody : B3Utils.JSONAble {
            public string launcherJwt;
            public string trigger;
            public string nonce;
            public object value;
            public string otherWallet;
            public string walletToTip;
            public string amountEth;
            public int? chainId;
            public string profileAddress;
            public string interactionId;
            public string contractAddress;
            public string mintNftlayout;
        }
        [Serializable]
        public class TriggerRulesEngineResult {
            public bool success;
            public string triggerUuid;
            public string[] actions;
        }
        [Serializable]
        public class CreateMessageChannelBody : B3Utils.JSONAble {
            public string launcherJwt;
            public string[] otherParticipants;
        }
        [Serializable]
        public class CreateMessageChannelResult : B3ObjectTypes.MessageChannel{
        }
        [Serializable]
        public class GetMessageChannelsBody : B3Utils.JSONAble {
            public string launcherJwt;
            public int limit;
            public int skip;
        }
        [Serializable]
        public class GetMessageChannelsResult {
            public int total;
            public int limit;
            public int skip;
            public B3ObjectTypes.MessageChannel[] data;
        }
        [Serializable]
        public class GetChannelMessagesBody : B3Utils.JSONAble {
            public string launcherJwt;
            public string channelId;
            public int limit;
            public int skip;
        }
        [Serializable]
        public class GetChannelMessagesResult {
            public int total;
            public int limit;
            public int skip;
            public B3ObjectTypes.Message[] data;
        }
        [Serializable]
        public class SendChannelMessageBody : B3Utils.JSONAble {
            public string launcherJwt;
            public string channelId;
            public string content;
        }
        [Serializable]
        public class SendChannelMessageResult : B3ObjectTypes.Message {
            
        }

        [Serializable]
        public class EditChannelMessageBody : B3Utils.JSONAble {
            public string launcherJwt;
            public string messageId;
            public string newContent;
        }
        [Serializable]
        public class EditChannelMessageResult : B3ObjectTypes.Message {

        }
        [Serializable]
        public class UnsendChannelMessageBody : B3Utils.JSONAble {
            public string launcherJwt;
            public string messageId;
        }
        [Serializable]
        public class UnsendChannelMessageResult : B3ObjectTypes.Message{

        }
        #endregion
    }
}