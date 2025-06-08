using System;
using System.Collections;
using System.Collections.Generic;
using BasementSDK;
using TMPro;
using UnityEngine;
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

public class UIManagerTest : MonoBehaviour
{
    public TMP_InputField jwtInput;
    public TMP_InputField otherWalletInput;
    public TextMeshProUGUI sessionText;

    void Start()
    {
        B3Instance.OnSessionReady += (session) => {
            Debug.Log("Session ready: " + JsonUtility.ToJson(session));
            sessionText.text = JsonUtility.ToJson(session);
        };
    }

    public void DebugButton(){
        Debug.Log("Debug received...");
        B3LauncherClient.GetChannelStatus(new B3LauncherClient.GetChannelStatusBody {
            launcherJwt = B3Instance.Instance.SessionJWT ?? jwtInput.text
        }, (channelStatus) => {
            Debug.Log("Session JWT: " + B3Instance.Instance.SessionJWT);
            Debug.Log("Channel Status: " + JsonUtility.ToJson(channelStatus));
        });
    }

    public void SetScoreButton(){
        Debug.Log("Setting Score...");
        B3LauncherClient.SetUserScore(new B3LauncherClient.SetUserScoreBody {
            launcherJwt = B3Instance.Instance.SessionJWT ?? jwtInput.text,
            score = UnityEngine.Random.Range(0, 100),
            nonce = "test123"
        }, (score) => {
            Debug.Log("Score result: " + JsonUtility.ToJson(score));
        });
    }

    public void TriggerRuleButton(){
        Debug.Log("Triggering rule...");
        B3LauncherClient.TriggerRulesEngine(new B3LauncherClient.TriggerRulesEngineBody {
            launcherJwt = B3Instance.Instance.SessionJWT ?? jwtInput.text,
            trigger = "test",
            value = UnityEngine.Random.Range(0, 100)
        }, (score) => {
            Debug.Log("Trigger result: " + JsonUtility.ToJson(score));
        });
    }

    public void OpenTipModal(){
        Debug.Log("Sending window message...");
        B3LauncherClient.OpenTipModal(String.IsNullOrEmpty(otherWalletInput.text) ? "0xC5Ace087f703398F64FF9efdf9101edeF6390c9a" : otherWalletInput.text);
    }

    public void OpenMintModal(){
        Debug.Log("Triggering rule...");
        B3LauncherClient.TriggerRulesEngine(new B3LauncherClient.TriggerRulesEngineBody {
            launcherJwt = B3Instance.Instance.SessionJWT ?? jwtInput.text,
            trigger = "open-mint-modal",
            contractAddress = "0x80f0E6644723aBb03AA8867d21e32bD854B2A2d9",
            mintNftlayout = "compact",
            chainId = 8453
        }, (result) => {
            Debug.Log("Trigger result: " + JsonUtility.ToJson(result));
        });
    }

    public void StartTrade(){
        Debug.Log("Triggering rule...");
        B3LauncherClient.TriggerRulesEngine(new B3LauncherClient.TriggerRulesEngineBody {
            launcherJwt = B3Instance.Instance.SessionJWT ?? jwtInput.text,
            trigger = "start-swap-modal",
            otherWallet = String.IsNullOrEmpty(otherWalletInput.text) ? "0x5D2DCA2C8a327290870c2343707852290Dce312f" : otherWalletInput.text,
        }, (result) => {
            Debug.Log("Trigger result: " + JsonUtility.ToJson(result));
        });
    }

    public void SignInWithBSMNT(){
        Debug.Log("Opening browser... Complete sign in");
        B3Instance.Instance.LoginViaSSO((channel) => {
            Debug.Log("Logged in: " + JsonUtility.ToJson(channel));
        });
    }
    public void test(){
        Debug.Log("test");
    }
}
