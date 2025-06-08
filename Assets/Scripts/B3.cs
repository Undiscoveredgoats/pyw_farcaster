using UnityEngine;
using BasementSDK;
using System;
using UnityEngine.UI;
using TMPro;

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

public class B3 : MonoBehaviour
{

    public TMP_InputField jwtInput;
    public TMP_InputField otherWalletInput;
    public TextMeshProUGUI sessionText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        B3Instance.OnSessionReady += (session) => {
            Debug.Log("Session ready: " + JsonUtility.ToJson(session));
            sessionText.text = JsonUtility.ToJson(session);
        };
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetScore(int score)
    {
        Debug.Log("Setting Score...");
        B3LauncherClient.SetUserScore(new B3LauncherClient.SetUserScoreBody
        {
            launcherJwt = B3Instance.Instance.SessionJWT ?? jwtInput.text,
            score = score,
            nonce = "test123"
        }, (score) => {
            Debug.Log("Score result: " + JsonUtility.ToJson(score));
        });
    }

    public void SignInWithBSMNT()
    {
        Debug.Log("Opening browser... Complete sign in");
        B3Instance.Instance.LoginViaSSO((channel) => {
            Debug.Log("Logged in: " + JsonUtility.ToJson(channel));
        });
    }

    public void OpenMintModal()
    {
        Debug.Log("Triggering rule...");
        B3LauncherClient.TriggerRulesEngine(new B3LauncherClient.TriggerRulesEngineBody
        {
            launcherJwt = B3Instance.Instance.SessionJWT ?? jwtInput.text,
            trigger = "open-mint-modal",
            contractAddress = "0x80f0E6644723aBb03AA8867d21e32bD854B2A2d9",
            mintNftlayout = "compact",
            chainId = 8453
        }, (result) => {
            Debug.Log("Trigger result: " + JsonUtility.ToJson(result));
        });
    }

    public void TriggerRule()
    {
        Debug.Log("Triggering rule...");
        B3LauncherClient.TriggerRulesEngine(new B3LauncherClient.TriggerRulesEngineBody
        {
            launcherJwt = B3Instance.Instance.SessionJWT ?? jwtInput.text,
            trigger = "test",
            value = UnityEngine.Random.Range(0, 100)
        }, (score) => {
            Debug.Log("Trigger result: " + JsonUtility.ToJson(score));
        });
    }
}
