using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;

public class TextDisplayManager : MonoBehaviour
{
    public static TextDisplayManager Instance { get; private set; }



    [SerializeField] private List<TextMeshProUGUI> scoreFields = new List<TextMeshProUGUI>(); // List of TextMeshProUGUI fields
    [SerializeField] private List<TextMeshProUGUI> nameFields = new List<TextMeshProUGUI>(); // List of TextMeshProUGUI fields
    [SerializeField] private List<string> nameList = new List<string>(); // List of strings
    [SerializeField] private List<uint> scoreList = new List<uint>(); // List of string


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    public async void UpdateTextFields()
     
    {
        await WalletConnectManager.Instance.GetScoreList();
        UpdateNameFields();
        UpdateScoreFields();
    }

    void UpdateNameFields()
    {
        nameList = WalletConnectManager.Instance.NameList();

        for (int i = 0; i < nameList.Count; i++)
        {

            nameFields[i].text = nameList[i];
        }      
    }

    void UpdateScoreFields()
    {
        scoreList = WalletConnectManager.Instance.ScoreList();

        for (int i = 0; i < scoreList.Count; i++)
        {
            scoreFields[i].text = scoreList[i].ToString();
        }
    }
}