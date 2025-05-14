//using UnityEngine;
//using Thirdweb;
//using System.Threading.Tasks;
//using Thirdweb.Unity;
//using System.Numerics;
//using TMPro; // For TextMeshProUGUI
//using System.Text; // For StringBuilder
//using System.Collections.Generic; // For List<T>
//using UnityEngine.Networking; // For UnityWebRequest
//using UnityEngine.UI; // For RawImage
//using Newtonsoft.Json.Linq; // For JSON parsing

//public class WalletConnectManager : MonoBehaviour
//{
//    private ThirdwebManager thirdwebManager;
//    private IThirdwebWallet wallet;
//    private string walletAddress;
//    [field: SerializeField, Header("Wallet Options")]
//    private ulong ActiveChainId = 84532;

//    [field: SerializeField, Header("Send ETH amount")]
//    public string Amount { get; set; } // Amount of ETH to send (in ETH, e.g., "0.01")
//    [field: SerializeField, Header("Send ETH address")]
//    public string ToAddress { get; set; } // Recipient address

//    [field: SerializeField, Header("Send Custom Token Options")]
//    public string TokenName { get; set; } // Name of the custom token (e.g., "USDC")
//    [field: SerializeField]
//    public string TokenContractAddress { get; set; } // Contract address of the custom token
//    [field: SerializeField]
//    public string TokenAmount { get; set; } // Amount of tokens to send (e.g., "100")
//    [field: SerializeField]
//    public string TokenRecipientAddress { get; set; } // Recipient address for the token transfer

//    [field: SerializeField, Header("Claim Token Options")]
//    public string ClaimTokenContractAddress { get; set; } // Contract address of the claim token (Drop contract)
//    [field: SerializeField]
//    public string ClaimTokenAmount { get; set; } // Amount of tokens to claim (e.g., "100")

//    [field: SerializeField, Header("Claim Nft Options")]
//    public string ClaimNftContractAddress { get; set; } // Contract address of the claim token (Drop contract)
//    [field: SerializeField]
//    public string ClaimNftAmount { get; set; } // Amount of tokens to claim (e.g., "100")

//    [field: SerializeField, Header("UI Elements")]
//    public GameObject ConnectButton { get; set; } // The connect button GameObject
//    [field: SerializeField]
//    public GameObject DisconnectButton { get; set; } // The disconnect button GameObject
//    [field: SerializeField]
//    public TextMeshProUGUI ConnectedText { get; set; } // The "Connected" text (TextMeshProUGUI)
//    [field: SerializeField]
//    public TextMeshProUGUI ClaimedNFTText { get; set; } // The "Claimed NFT" text (TextMeshProUGUI)
//    [field: SerializeField]
//    public TextMeshProUGUI AddressText { get; set; } // The address text (TextMeshProUGUI) to show first/last 3 chars
//    [field: SerializeField]
//    public TextMeshProUGUI EthBalanceText { get; set; } // The ETH balance text (TextMeshProUGUI)
//    [field: SerializeField]
//    public TextMeshProUGUI CustomTokenBalanceText { get; set; } // The custom token balance text (TextMeshProUGUI)
//    [field: SerializeField]
//    public TextMeshProUGUI ClaimedTokenBalanceText { get; set; } // The claimed token balance text (TextMeshProUGUI)

//    [field: SerializeField, Header("NFT Display Canvas")]
//    public Canvas NftDisplayCanvas { get; set; } // The canvas for NFT display
//    [field: SerializeField]
//    public GameObject NftDisplayPrefab { get; set; } // Prefab with UI elements for NFT (name, description, token ID, image)
//    [field: SerializeField]
//    public Transform NftDisplayParent { get; set; } // Parent transform for instantiated NFT UI elements

//    private List<GameObject> instantiatedNfts = new List<GameObject>(); // Track instantiated NFT UI elements

//    void Awake()
//    {
//        thirdwebManager = FindObjectOfType<ThirdwebManager>();
//        if (thirdwebManager == null)
//        {
//            Debug.LogError("ThirdwebManager not found in the scene! Please add the ThirdwebManager prefab.");
//        }

//        // Initialize UI elements on start
//        if (ConnectButton != null) ConnectButton.SetActive(true);
//        if (DisconnectButton != null)
//        {
//            DisconnectButton.SetActive(false);
//            var buttonComponent = DisconnectButton.GetComponent<UnityEngine.UI.Button>();
//            if (buttonComponent != null)
//            {
//                buttonComponent.interactable = true;
//                Debug.Log("DisconnectButton is interactable: " + buttonComponent.interactable);
//            }
//            else
//            {
//                Debug.LogError("DisconnectButton does not have a Button component!");
//            }
//        }
//        if (ConnectedText != null) ConnectedText.gameObject.SetActive(false);
//        if (AddressText != null) AddressText.gameObject.SetActive(false);
//        if (EthBalanceText != null) EthBalanceText.gameObject.SetActive(false);
//        if (CustomTokenBalanceText != null) CustomTokenBalanceText.gameObject.SetActive(false);
//        if (ClaimedTokenBalanceText != null) ClaimedTokenBalanceText.gameObject.SetActive(false);
//        if (ClaimedNFTText != null) ClaimedNFTText.gameObject.SetActive(false);
//        if (NftDisplayCanvas != null) NftDisplayCanvas.gameObject.SetActive(false);
//    }

//    public async void Connect()
//    {
//        if (thirdwebManager == null)
//        {
//            Debug.LogError("Cannot connect: ThirdwebManager is not initialized.");
//            return;
//        }

//        try
//        {
//            var options = new WalletOptions(
//                provider: WalletProvider.WalletConnectWallet,
//                chainId: 84532
//            );

//            Debug.Log("Initiating wallet connection...");
//            wallet = await ThirdwebManager.Instance.ConnectWallet(options);
//            walletAddress = await wallet.GetAddress();
//            Debug.Log($"Wallet connected successfully! Address: {walletAddress}");

//            var balance = await wallet.GetBalance(chainId: ActiveChainId);
//            var balanceEth = Utils.ToEth(wei: balance.ToString(), decimalsToDisplay: 4, addCommas: true);
//            Debug.Log($"Wallet balance: {balanceEth}");

//            if (ConnectButton != null) ConnectButton.SetActive(false);
//            if (DisconnectButton != null)
//            {
//                DisconnectButton.SetActive(true);
//                var buttonComponent = DisconnectButton.GetComponent<UnityEngine.UI.Button>();
//                if (buttonComponent != null)
//                {
//                    buttonComponent.interactable = true;
//                    Debug.Log("DisconnectButton is interactable after Connect: " + buttonComponent.interactable);
//                }
//            }
//            if (ConnectedText != null)
//            {
//                ConnectedText.gameObject.SetActive(true);
//                ConnectedText.text = "Connected";
//            }
//            if (AddressText != null && !string.IsNullOrEmpty(walletAddress))
//            {
//                AddressText.gameObject.SetActive(true);
//                string shortAddress = $"{walletAddress.Substring(0, 3)}...{walletAddress.Substring(walletAddress.Length - 3)}";
//                AddressText.text = shortAddress;
//            }
//        }
//        catch (System.Exception ex)
//        {
//            Debug.LogError($"Failed to connect wallet: {ex.Message}");
//            wallet = null;
//            walletAddress = null;

//            if (ConnectButton != null) ConnectButton.SetActive(true);
//            if (DisconnectButton != null) DisconnectButton.SetActive(false);
//            if (ConnectedText != null) ConnectedText.gameObject.SetActive(false);
//            if (AddressText != null) AddressText.gameObject.SetActive(false);
//            if (EthBalanceText != null) EthBalanceText.gameObject.SetActive(false);
//            if (CustomTokenBalanceText != null) CustomTokenBalanceText.gameObject.SetActive(false);
//            if (ClaimedTokenBalanceText != null) ClaimedTokenBalanceText.gameObject.SetActive(false);
//            if (ClaimedNFTText != null) ClaimedNFTText.gameObject.SetActive(false);
//            if (NftDisplayCanvas != null) NftDisplayCanvas.gameObject.SetActive(false);
//        }
//    }

//    public async void Disconnect()
//    {
//        if (wallet == null)
//        {
//            Debug.LogWarning("No wallet to disconnect.");
//            return;
//        }

//        try
//        {
//            Debug.Log("Disconnect function called! Disconnecting wallet...");
//            await wallet.Disconnect();
//            wallet = null;
//            walletAddress = null;
//            Debug.Log("Wallet disconnected successfully.");

//            if (ConnectButton != null) ConnectButton.SetActive(true);
//            if (DisconnectButton != null) DisconnectButton.SetActive(false);
//            if (ConnectedText != null) ConnectedText.gameObject.SetActive(false);
//            if (AddressText != null) AddressText.gameObject.SetActive(false);
//            if (EthBalanceText != null) EthBalanceText.gameObject.SetActive(false);
//            if (CustomTokenBalanceText != null) CustomTokenBalanceText.gameObject.SetActive(false);
//            if (ClaimedTokenBalanceText != null) ClaimedTokenBalanceText.gameObject.SetActive(false);
//            if (ClaimedNFTText != null) ClaimedNFTText.gameObject.SetActive(false);
//            if (NftDisplayCanvas != null) NftDisplayCanvas.gameObject.SetActive(false);
//            ClearNftDisplay();
//        }
//        catch (System.Exception ex)
//        {
//            Debug.LogError($"Failed to disconnect wallet: {ex.Message}");
//        }
//    }

//    public async void SendEth()
//    {
//        if (thirdwebManager == null)
//        {
//            Debug.LogError("Cannot send ETH: ThirdwebManager is not initialized.");
//            return;
//        }

//        if (wallet == null)
//        {
//            Debug.LogError("Cannot send ETH: Wallet is not connected. Please connect the wallet first.");
//            return;
//        }

//        if (string.IsNullOrEmpty(ToAddress) || !ToAddress.StartsWith("0x") || ToAddress.Length != 42)
//        {
//            Debug.LogError("Invalid recipient address. Please provide a valid Ethereum address (e.g., 0x...)");
//            return;
//        }

//        if (string.IsNullOrEmpty(Amount) || !float.TryParse(Amount, out float ethAmount) || ethAmount <= 0)
//        {
//            Debug.LogError("Invalid amount. Please provide a valid ETH amount (e.g., 0.01)");
//            return;
//        }

//        try
//        {
//            Debug.Log($"Sending {Amount} ETH to {ToAddress} from wallet {walletAddress}...");

//            Debug.Log($"Ensuring wallet is on chain {ActiveChainId}...");
//            if (wallet is WalletConnectWallet walletConnect)
//            {
//                await walletConnect.EnsureCorrectNetwork(ActiveChainId);
//            }
//            else
//            {
//                Debug.LogWarning("Wallet is not a WalletConnectWallet; cannot ensure correct network.");
//            }

//            Debug.Log("Waiting for chain switch to take effect...");
//            await Task.Delay(10000);

//            string weiAmountString = Utils.ToWei(Amount);
//            Debug.Log($"Converted amount to Wei (string): {weiAmountString}");

//            BigInteger weiAmount = BigInteger.Parse(weiAmountString);
//            Debug.Log($"Converted Wei to BigInteger: {weiAmount}");

//            var transactionResult = await wallet.Transfer(ActiveChainId, ToAddress, weiAmount);

//            Debug.Log($"ETH sent successfully! Transaction Hash: {transactionResult.TransactionHash}");

//            var balance = await wallet.GetBalance(chainId: ActiveChainId);
//            var balanceEth = Utils.ToEth(wei: balance.ToString(), decimalsToDisplay: 4, addCommas: true);
//            Debug.Log($"Updated wallet balance: {balanceEth}");
//            if (EthBalanceText != null)
//            {
//                EthBalanceText.gameObject.SetActive(true);
//                EthBalanceText.text = $"ETH Balance: {balanceEth}";
//            }
//        }
//        catch (System.Exception ex)
//        {
//            Debug.LogError($"Failed to send ETH: {ex.Message}");
//        }
//    }

//    public async void SendCustomToken()
//    {
//        if (thirdwebManager == null)
//        {
//            Debug.LogError("Cannot send token: ThirdwebManager is not initialized.");
//            return;
//        }

//        if (wallet == null)
//        {
//            Debug.LogError("Cannot send token: Wallet is not connected. Please connect the wallet first.");
//            return;
//        }

//        if (string.IsNullOrEmpty(TokenContractAddress) || !TokenContractAddress.StartsWith("0x") || TokenContractAddress.Length != 42)
//        {
//            Debug.LogError("Invalid token contract address. Please provide a valid Ethereum address (e.g., 0x...)");
//            return;
//        }

//        if (string.IsNullOrEmpty(TokenRecipientAddress) || !TokenRecipientAddress.StartsWith("0x") || TokenRecipientAddress.Length != 42)
//        {
//            Debug.LogError("Invalid recipient address. Please provide a valid Ethereum address (e.g., 0x...)");
//            return;
//        }

//        if (string.IsNullOrEmpty(TokenAmount) || !float.TryParse(TokenAmount, out float tokenAmount) || tokenAmount <= 0)
//        {
//            Debug.LogError("Invalid token amount. Please provide a valid amount (e.g., 100)");
//            return;
//        }

//        try
//        {
//            Debug.Log($"Sending {TokenAmount} {TokenName} to {TokenRecipientAddress} from wallet {walletAddress}...");

//            Debug.Log($"Ensuring wallet is on chain {ActiveChainId}...");
//            if (wallet is WalletConnectWallet walletConnect)
//            {
//                await walletConnect.EnsureCorrectNetwork(ActiveChainId);
//            }
//            else
//            {
//                Debug.LogWarning("Wallet is not a WalletConnectWallet; cannot ensure correct network.");
//            }

//            Debug.Log("Waiting for chain switch to take effect...");
//            await Task.Delay(10000);

//            var contract = await ThirdwebManager.Instance.GetContract(TokenContractAddress, ActiveChainId);

//            var decimals = await contract.ERC20_Decimals();
//            Debug.Log($"{TokenName} decimals: {decimals}");

//            string tokenAmountInWei = Utils.ToWei(TokenAmount);
//            Debug.Log($"Converted token amount to Wei (string): {tokenAmountInWei}");

//            BigInteger tokenAmountBigInt = BigInteger.Parse(tokenAmountInWei);
//            Debug.Log($"Converted token amount to BigInteger: {tokenAmountBigInt}");

//            var transactionResult = await contract.ERC20_Transfer(wallet, TokenRecipientAddress, tokenAmountBigInt);

//            Debug.Log($"Token sent successfully! Transaction Hash: {transactionResult.TransactionHash}");

//            var tokenBalance = await contract.ERC20_BalanceOf(walletAddress);
//            var tokenBalanceFormatted = Utils.ToEth(tokenBalance.ToString(), (int)decimals, addCommas: true);
//            Debug.Log($"Updated {TokenName} balance for {walletAddress}: {tokenBalanceFormatted}");
//            if (CustomTokenBalanceText != null)
//            {
//                CustomTokenBalanceText.gameObject.SetActive(true);
//                CustomTokenBalanceText.text = $"{TokenName} Balance: {tokenBalanceFormatted}";
//            }
//        }
//        catch (System.Exception ex)
//        {
//            Debug.LogError($"Failed to send {TokenName}: {ex.Message}");
//        }
//    }

//    public async void ClaimToken()
//    {
//        if (thirdwebManager == null)
//        {
//            Debug.LogError("Cannot claim token: ThirdwebManager is not initialized.");
//            return;
//        }

//        if (wallet == null)
//        {
//            Debug.LogError("Cannot claim token: Wallet is not connected. Please connect the wallet first.");
//            return;
//        }

//        if (string.IsNullOrEmpty(ClaimTokenContractAddress) || !ClaimTokenContractAddress.StartsWith("0x") || ClaimTokenContractAddress.Length != 42)
//        {
//            Debug.LogError("Invalid claim token contract address. Please provide a valid Ethereum address (e.g., 0x...)");
//            return;
//        }

//        if (string.IsNullOrEmpty(ClaimTokenAmount) || !float.TryParse(ClaimTokenAmount, out float claimAmount) || claimAmount <= 0)
//        {
//            Debug.LogError("Invalid claim amount. Please provide a valid amount (e.g., 100)");
//            return;
//        }

//        try
//        {
//            Debug.Log($"Claiming {ClaimTokenAmount} tokens to {walletAddress} from contract {ClaimTokenContractAddress}...");

//            Debug.Log($"Ensuring wallet is on chain {ActiveChainId}...");
//            if (wallet is WalletConnectWallet walletConnect)
//            {
//                await walletConnect.EnsureCorrectNetwork(ActiveChainId);
//            }
//            else
//            {
//                Debug.LogWarning("Wallet is not a WalletConnectWallet; cannot ensure correct network.");
//            }

//            Debug.Log("Waiting for chain switch to take effect...");
//            await Task.Delay(10000);

//            var contract = await ThirdwebManager.Instance.GetContract(ClaimTokenContractAddress, ActiveChainId);

//            var decimals = await contract.ERC20_Decimals();
//            Debug.Log($"Claim token decimals: {decimals}");

//            string claimAmountInWei = Utils.ToWei(ClaimTokenAmount);
//            Debug.Log($"Converted claim amount to Wei (string): {claimAmountInWei}");

//            var transactionResult = await contract.DropERC20_Claim(wallet, walletAddress, claimAmountInWei);
//            Debug.Log($"Tokens claimed successfully! Transaction Hash: {transactionResult.TransactionHash}");
//            await Task.Delay(10000);

//            var tokenBalance = await contract.ERC20_BalanceOf(walletAddress);
//            var tokenBalanceFormatted = Utils.ToEth(tokenBalance.ToString(), (int)decimals, addCommas: true);
//            Debug.Log($"Updated token balance for {walletAddress}: {tokenBalanceFormatted}");
//            if (ClaimedTokenBalanceText != null)
//            {
//                ClaimedTokenBalanceText.gameObject.SetActive(true);
//                ClaimedTokenBalanceText.text = $"CYS: {tokenBalanceFormatted}";
//            }
//        }
//        catch (System.Exception ex)
//        {
//            Debug.LogError($"Failed to claim tokens: {ex.Message}");
//        }
//    }

//    public async void ConnectWithEcosystem()
//    {
//        if (thirdwebManager == null)
//        {
//            Debug.LogError("Cannot connect: ThirdwebManager is not initialized.");
//            return;
//        }

//        try
//        {
//            var ecosystemWalletOptions = new EcosystemWalletOptions(ecosystemId: "ecosystem.your-ecosystem", email: "myepicemail@domain.id");
//            var options = new WalletOptions(
//                provider: WalletProvider.EcosystemWallet,
//                chainId: 84532,
//                ecosystemWalletOptions: ecosystemWalletOptions
//            );
//            wallet = await ThirdwebManager.Instance.ConnectWallet(options);
//            walletAddress = await wallet.GetAddress();
//            Debug.Log($"Wallet connected successfully! Address: {walletAddress}");

//            var balance = await wallet.GetBalance(chainId: ActiveChainId);
//            var balanceEth = Utils.ToEth(wei: balance.ToString(), decimalsToDisplay: 4, addCommas: true);
//            Debug.Log($"Wallet balance: {balanceEth}");

//            if (ConnectButton != null) ConnectButton.SetActive(false);
//            if (DisconnectButton != null)
//            {
//                DisconnectButton.SetActive(true);
//                var buttonComponent = DisconnectButton.GetComponent<UnityEngine.UI.Button>();
//                if (buttonComponent != null)
//                {
//                    buttonComponent.interactable = true;
//                    Debug.Log("DisconnectButton is interactable after Connect: " + buttonComponent.interactable);
//                }
//            }
//            if (ConnectedText != null)
//            {
//                ConnectedText.gameObject.SetActive(true);
//                ConnectedText.text = "Connected";
//            }
//            if (AddressText != null && !string.IsNullOrEmpty(walletAddress))
//            {
//                AddressText.gameObject.SetActive(true);
//                string shortAddress = $"{walletAddress.Substring(0, 3)}...{walletAddress.Substring(walletAddress.Length - 3)}";
//                AddressText.text = shortAddress;
//            }
//        }
//        catch (System.Exception ex)
//        {
//            Debug.LogError($"Failed to connect wallet: {ex.Message}");
//            wallet = null;
//            walletAddress = null;

//            if (ConnectButton != null) ConnectButton.SetActive(true);
//            if (DisconnectButton != null) DisconnectButton.SetActive(false);
//            if (ConnectedText != null) ConnectedText.gameObject.SetActive(false);
//            if (AddressText != null) AddressText.gameObject.SetActive(false);
//            if (EthBalanceText != null) EthBalanceText.gameObject.SetActive(false);
//            if (CustomTokenBalanceText != null) CustomTokenBalanceText.gameObject.SetActive(false);
//            if (ClaimedTokenBalanceText != null) ClaimedTokenBalanceText.gameObject.SetActive(false);
//            if (ClaimedNFTText != null) ClaimedNFTText.gameObject.SetActive(false);
//            if (NftDisplayCanvas != null) NftDisplayCanvas.gameObject.SetActive(false);
//        }
//    }



//    //public async void ClaimNft()
//    //{
//    //    if (thirdwebManager == null)
//    //    {
//    //        Debug.LogError("Cannot claim NFT: ThirdwebManager is not initialized.");
//    //        return;
//    //    }

//    //    if (wallet == null)
//    //    {
//    //        Debug.LogError("Cannot claim NFT: Wallet is not connected. Please connect the wallet first.");
//    //        return;
//    //    }

//    //    if (string.IsNullOrEmpty(ClaimNftContractAddress) || !ClaimNftContractAddress.StartsWith("0x") || ClaimNftContractAddress.Length != 42)
//    //    {
//    //        Debug.LogError("Invalid claim NFT contract address. Please provide a valid Ethereum address (e.g., 0x...)");
//    //        return;
//    //    }

//    //    if (string.IsNullOrEmpty(ClaimNftAmount) || !int.TryParse(ClaimNftAmount, out int claimAmount) || claimAmount <= 0)
//    //    {
//    //        Debug.LogError("Invalid claim amount. Please provide a valid integer amount (e.g., 1)");
//    //        return;
//    //    }

//    //    if (NftDisplayCanvas == null || NftDisplayPrefab == null)
//    //    {
//    //        Debug.LogError("NFT display canvas or prefab not assigned in Inspector.");
//    //        return;
//    //    }

//    //    Transform parentTransform = NftDisplayParent != null ? NftDisplayParent : NftDisplayCanvas.transform;
//    //    Debug.Log($"Using parent transform: {parentTransform.name}");

//    //    try
//    //    {
//    //        if (ClaimedNFTText != null)
//    //        {
//    //            ClaimedNFTText.gameObject.SetActive(true);
//    //            ClaimedNFTText.text = "Claiming...";
//    //        }

//    //        Debug.Log($"Claiming {claimAmount} NFTs to {walletAddress} from contract {ClaimNftContractAddress}...");

//    //        Debug.Log($"Ensuring wallet is on chain {ActiveChainId}...");
//    //        if (wallet is WalletConnectWallet walletConnect)
//    //        {
//    //            await walletConnect.EnsureCorrectNetwork(ActiveChainId);
//    //        }
//    //        else
//    //        {
//    //            Debug.LogWarning("Wallet is not a WalletConnectWallet; cannot ensure correct network.");
//    //        }

//    //        Debug.Log("Waiting for chain switch to take effect...");
//    //        await Task.Delay(20000); // 20 seconds for Base Sepolia

//    //        var contract = await ThirdwebManager.Instance.GetContract(ClaimNftContractAddress, ActiveChainId);

//    //        var transactionResult = await contract.DropERC721_Claim(wallet, walletAddress, claimAmount);
//    //        Debug.Log($"NFTs claimed successfully! Transaction Hash: {transactionResult.TransactionHash}");

//    //        // Wait for blockchain state to update
//    //        BigInteger tokenBalance = 0;
//    //        int maxAttempts = 5;
//    //        for (int attempt = 0; attempt < maxAttempts; attempt++)
//    //        {
//    //            tokenBalance = await contract.ERC721_BalanceOf(walletAddress);
//    //            Debug.Log($"Balance check attempt {attempt + 1}/{maxAttempts}: {tokenBalance} NFTs");
//    //            if (tokenBalance >= claimAmount) break;
//    //            await Task.Delay(5000); // 5 seconds per attempt
//    //        }

//    //        if (tokenBalance == 0)
//    //        {
//    //            Debug.LogError($"No NFTs owned by {walletAddress} after claim. Check contract logic or transaction.");
//    //            if (ClaimedNFTText != null)
//    //            {
//    //                ClaimedNFTText.text = $"No NFTs owned. Tx Hash: {transactionResult.TransactionHash}";
//    //            }
//    //            return;
//    //        }

//    //        if (ClaimedNFTText != null)
//    //        {
//    //            ClaimedNFTText.text = $"Claimed! Tx Hash: {transactionResult.TransactionHash}\nBalance: {tokenBalance}";
//    //        }

//    //        ClearNftDisplay();
//    //        Debug.Log("Cleared previous NFT display.");

//    //        List<string> tokenIds = new List<string>();
//    //        // Try Thirdweb's GetOwnedNFTs if available
//    //        try
//    //        {
//    //            var ownedNfts = await contract.ERC1155_GetOwnedNFTs(walletAddress);
//    //            foreach (var nft in ownedNfts)
//    //            {
//    //                tokenIds.Add(nft.Metadata.Id.ToString());
//    //                Debug.Log($"Fetched token ID via GetOwnedNFTs: {nft.Metadata.Id}");
//    //            }
//    //        }
//    //        catch (System.Exception ex)
//    //        {
//    //            Debug.LogWarning($"GetOwnedNFTs not supported or failed: {ex.Message}. Falling back to tokenOfOwnerByIndex.");
//    //            // Try tokenOfOwnerByIndex
//    //            for (int i = 0; i < tokenBalance; i++)
//    //            {
//    //                try
//    //                {
//    //                    BigInteger index = new BigInteger(i);
//    //                    var tokenId = await contract.Read<BigInteger>("tokenOfOwnerByIndex", walletAddress, index);
//    //                    tokenIds.Add(tokenId.ToString());
//    //                    Debug.Log($"Fetched token ID at index {i}: {tokenId}");
//    //                }
//    //                catch (System.Exception ex2)
//    //                {
//    //                    Debug.LogWarning($"Failed to fetch token ID at index {i}: {ex2.Message}");
//    //                }
//    //            }
//    //        }

//    //        // Fallback to sequential token IDs
//    //        if (tokenIds.Count == 0 && tokenBalance > 0)
//    //        {
//    //            Debug.LogWarning("No token IDs fetched. Assuming sequential IDs based on totalSupply.");
//    //            BigInteger totalSupply;
//    //            try
//    //            {
//    //                totalSupply = await contract.Read<BigInteger>("totalSupply");
//    //                Debug.Log($"Contract totalSupply: {totalSupply}");
//    //            }
//    //            catch
//    //            {
//    //                Debug.LogWarning("Could not fetch totalSupply; assuming totalSupply = tokenBalance.");
//    //                totalSupply = tokenBalance;
//    //            }

//    //            // Assume the claimed NFTs are the last 'claimAmount' tokens
//    //            BigInteger startId = totalSupply - claimAmount;
//    //            if (startId < 0) startId = 0;
//    //            for (int i = 0; i < claimAmount; i++)
//    //            {
//    //                BigInteger assumedId = startId + i;
//    //                tokenIds.Add(assumedId.ToString());
//    //                Debug.Log($"Assumed token ID: {assumedId}");
//    //            }
//    //        }

//    //        Debug.Log($"Total token IDs fetched: {tokenIds.Count}");
//    //        if (tokenIds.Count == 0)
//    //        {
//    //            Debug.LogError("No token IDs available. Verify contract or ownership on Basescan.");
//    //            if (ClaimedNFTText != null)
//    //            {
//    //                ClaimedNFTText.text = $"No NFTs found for {walletAddress}. Balance: {tokenBalance}";
//    //            }
//    //            return;
//    //        }

//    //        if (NftDisplayCanvas != null)
//    //        {
//    //            NftDisplayCanvas.gameObject.SetActive(true);
//    //            NftDisplayCanvas.enabled = true;
//    //            Debug.Log($"Activated NftDisplayCanvas: {NftDisplayCanvas.name}, IsActive: {NftDisplayCanvas.gameObject.activeInHierarchy}");
//    //        }
//    //        else
//    //        {
//    //            Debug.LogError("NftDisplayCanvas is null!");
//    //            return;
//    //        }

//    //        foreach (var tokenId in tokenIds)
//    //        {
//    //            try
//    //            {
//    //                Debug.Log($"Processing token ID: {tokenId}");
//    //                BigInteger tokenIdBigInt = BigInteger.Parse(tokenId);
//    //                var nft = await contract.ERC721_GetNFT(tokenIdBigInt);
//    //                string name = nft.Metadata.Name ?? $"NFT #{tokenId}";
//    //                string description = nft.Metadata.Description ?? "No description";
//    //                string tokenUri = nft.Metadata.Uri ?? "";
//    //                Debug.Log($"NFT metadata - TokenID: {tokenId}, Name: {name}, Description: {description}, TokenUri: {tokenUri}");

//    //                string imageUrl = await GetImageUrlFromTokenUri(tokenUri);
//    //                Debug.Log($"Image URL for token ID {tokenId}: {imageUrl}");
//    //                Texture2D imageTexture = null;
//    //                if (!string.IsNullOrEmpty(imageUrl))
//    //                {
//    //                    imageTexture = await LoadImage(imageUrl);
//    //                    Debug.Log($"Image texture loaded for token ID {tokenId}: {(imageTexture != null ? "Success" : "Failed")}");
//    //                }

//    //                GameObject nftDisplay = Instantiate(NftDisplayPrefab, parentTransform);
//    //                nftDisplay.SetActive(true);
//    //                instantiatedNfts.Add(nftDisplay);
//    //                Debug.Log($"Instantiated prefab for token ID {tokenId}: {nftDisplay.name}");

//    //                TextMeshProUGUI[] textComponents = nftDisplay.GetComponentsInChildren<TextMeshProUGUI>(true);
//    //                RawImage imageRaw = nftDisplay.GetComponentInChildren<RawImage>(true);

//    //                TextMeshProUGUI nameText = null;
//    //                TextMeshProUGUI descText = null;
//    //                TextMeshProUGUI tokenIdText = null;

//    //                foreach (var text in textComponents)
//    //                {
//    //                    string textName = text.gameObject.name.ToLower();
//    //                    if (textName.Contains("name")) nameText = text;
//    //                    else if (textName.Contains("description") || textName.Contains("desc")) descText = text;
//    //                    else if (textName.Contains("tokenid") || textName.Contains("token")) tokenIdText = text;
//    //                }

//    //                if (nameText != null) nameText.text = $"Name: {name}";
//    //                else Debug.LogWarning($"NameText not found in prefab for token ID {tokenId}");
//    //                if (descText != null) descText.text = $"Description: {description}";
//    //                else Debug.LogWarning($"DescriptionText not found in prefab for token ID {tokenId}");
//    //                if (tokenIdText != null) tokenIdText.text = $"Token ID: {tokenId}";
//    //                else Debug.LogWarning($"TokenIdText not found in prefab for token ID {tokenId}");
//    //                if (imageRaw != null)
//    //                {
//    //                    imageRaw.texture = imageTexture ?? Texture2D.grayTexture;
//    //                    Debug.Log($"Set RawImage for token ID {tokenId}: {(imageTexture != null ? "Image" : "Placeholder")}");
//    //                }
//    //                else Debug.LogWarning($"RawImage not found in prefab for token ID {tokenId}");

//    //                Debug.Log($"Displayed NFT: TokenID={tokenId}, Name={name}, Description={description}, ImageURL={imageUrl}");
//    //            }
//    //            catch (System.Exception ex)
//    //            {
//    //                Debug.LogError($"Failed to process NFT with TokenID {tokenId}: {ex.Message}");
//    //            }
//    //        }

//    //        if (ClaimedNFTText != null)
//    //        {
//    //            ClaimedNFTText.gameObject.SetActive(true);
//    //            ClaimedNFTText.text = $"NFT Balance: {tokenBalance}\nTx Hash: {transactionResult.TransactionHash}";
//    //        }
//    //    }
//    //    catch (System.Exception ex)
//    //    {
//    //        Debug.LogError($"Failed to claim NFTs: {ex.Message}");
//    //        if (ClaimedNFTText != null)
//    //        {
//    //            ClaimedNFTText.text = $"Failed to claim: {ex.Message}";
//    //        }
//    //        if (NftDisplayCanvas != null) NftDisplayCanvas.gameObject.SetActive(false);
//    //    }
//    //}

//    //private async Task<string> GetImageUrlFromTokenUri(string tokenUri)
//    //{
//    //    if (string.IsNullOrEmpty(tokenUri))
//    //    {
//    //        Debug.LogWarning("Token URI is empty.");
//    //        return null;
//    //    }

//    //    if (tokenUri.StartsWith("ipfs://"))
//    //    {
//    //        tokenUri = tokenUri.Replace("ipfs://", "https://cloudflare-ipfs.com/ipfs/"); // Use Cloudflare for reliability
//    //        Debug.Log($"Converted IPFS URI to: {tokenUri}");
//    //    }

//    //    try
//    //    {
//    //        UnityWebRequest request = UnityWebRequest.Get(tokenUri);
//    //        var operation = request.SendWebRequest();
//    //        while (!operation.isDone) await Task.Yield();

//    //        if (request.result != UnityWebRequest.Result.Success)
//    //        {
//    //            Debug.LogError($"Failed to fetch metadata from {tokenUri}: {request.error}");
//    //            return null;
//    //        }

//    //        string json = request.downloadHandler.text;
//    //        JObject metadata = JObject.Parse(json);
//    //        string imageUrl = metadata["image"]?.ToString() ?? metadata["image_url"]?.ToString();
//    //        Debug.Log($"Parsed metadata JSON, image URL: {imageUrl}");

//    //        if (!string.IsNullOrEmpty(imageUrl) && imageUrl.StartsWith("ipfs://"))
//    //        {
//    //            imageUrl = imageUrl.Replace("ipfs://", "https://cloudflare-ipfs.com/ipfs/");
//    //            Debug.Log($"Converted IPFS image URL to: {imageUrl}");
//    //        }

//    //        return imageUrl;
//    //    }
//    //    catch (System.Exception ex)
//    //    {
//    //        Debug.LogError($"Failed to parse metadata from {tokenUri}: {ex.Message}");
//    //        return null;
//    //    }
//    //}

//    //private async Task<Texture2D> LoadImage(string imageUrl)
//    //{
//    //    if (string.IsNullOrEmpty(imageUrl))
//    //    {
//    //        Debug.LogWarning("Image URL is empty.");
//    //        return null;
//    //    }

//    //    try
//    //    {
//    //        UnityWebRequest request = UnityWebRequestTexture.GetTexture(imageUrl);
//    //        var operation = request.SendWebRequest();
//    //        while (!operation.isDone) await Task.Yield();

//    //        if (request.result != UnityWebRequest.Result.Success)
//    //        {
//    //            Debug.LogError($"Failed to load image from {imageUrl}: {request.error}");
//    //            return null;
//    //        }

//    //        return DownloadHandlerTexture.GetContent(request);
//    //    }
//    //    catch (System.Exception ex)
//    //    {
//    //        Debug.LogError($"Failed to load image from {imageUrl}: {ex.Message}");
//    //        return null;
//    //    }
//    //}

//    public async void ClaimNft()
//    {
//        if (thirdwebManager == null)
//        {
//            Debug.LogError("Cannot claim NFT: ThirdwebManager is not initialized.");
//            return;
//        }

//        if (wallet == null)
//        {
//            Debug.LogError("Cannot claim NFT: Wallet is not connected. Please connect the wallet first.");
//            return;
//        }

//        if (string.IsNullOrEmpty(ClaimNftContractAddress) || !ClaimNftContractAddress.StartsWith("0x") || ClaimNftContractAddress.Length != 42)
//        {
//            Debug.LogError("Invalid claim NFT contract address. Please provide a valid Ethereum address (e.g., 0x...)");
//            return;
//        }

//        if (string.IsNullOrEmpty(ClaimNftAmount) || !int.TryParse(ClaimNftAmount, out int claimAmount) || claimAmount <= 0)
//        {
//            Debug.LogError("Invalid claim amount. Please provide a valid integer amount (e.g., 1)");
//            return;
//        }

//        if (NftDisplayCanvas == null || NftDisplayPrefab == null)
//        {
//            Debug.LogError("NFT display canvas or prefab not assigned in Inspector.");
//            return;
//        }

//        Transform parentTransform = NftDisplayParent != null ? NftDisplayParent : NftDisplayCanvas.transform;
//        Debug.Log($"Using parent transform: {parentTransform.name}");

//        try
//        {
//            if (ClaimedNFTText != null)
//            {
//                ClaimedNFTText.gameObject.SetActive(true);
//                ClaimedNFTText.text = "Claiming...";
//            }

//            Debug.Log($"Claiming {claimAmount} NFTs to {walletAddress} from contract {ClaimNftContractAddress}...");

//            Debug.Log($"Ensuring wallet is on chain {ActiveChainId}...");
//            if (wallet is WalletConnectWallet walletConnect)
//            {
//                await walletConnect.EnsureCorrectNetwork(ActiveChainId);
//            }
//            else
//            {
//                Debug.LogWarning("Wallet is not a WalletConnectWallet; cannot ensure correct network.");
//            }

//            Debug.Log("Waiting for chain switch to take effect...");
//            await Task.Delay(20000); // 20 seconds for Base Sepolia

//            var contract = await ThirdwebManager.Instance.GetContract(ClaimNftContractAddress, ActiveChainId);

//            var transactionResult = await contract.DropERC721_Claim(wallet, walletAddress, claimAmount);
//            Debug.Log($"NFTs claimed successfully! Transaction Hash: {transactionResult.TransactionHash}");

//            BigInteger tokenBalance = 0;
//            int maxAttempts = 5;
//            for (int attempt = 0; attempt < maxAttempts; attempt++)
//            {
//                tokenBalance = await contract.ERC721_BalanceOf(walletAddress);
//                Debug.Log($"Balance check attempt {attempt + 1}/{maxAttempts}: {tokenBalance} NFTs");
//                if (tokenBalance >= claimAmount) break;
//                await Task.Delay(5000);
//            }

//            if (tokenBalance == 0)
//            {
//                Debug.LogError($"No NFTs owned by {walletAddress} after claim. Check contract logic or transaction.");
//                if (ClaimedNFTText != null)
//                {
//                    ClaimedNFTText.text = $"No NFTs owned. Tx Hash: {transactionResult.TransactionHash}";
//                }
//                return;
//            }

//            if (ClaimedNFTText != null)
//            {
//                ClaimedNFTText.text = $"Claimed! Tx Hash: {transactionResult.TransactionHash}\nBalance: {tokenBalance}";
//            }

//            ClearNftDisplay();
//            Debug.Log("Cleared previous NFT display.");

//            List<string> tokenIds = new List<string>();
//            try
//            {
//                var ownedNfts = await contract.ERC1155_GetOwnedNFTs(walletAddress);
//                foreach (var nft in ownedNfts)
//                {
//                    tokenIds.Add(nft.Metadata.Id.ToString());
//                    Debug.Log($"Fetched token ID via GetOwnedNFTs: {nft.Metadata.Id}");
//                }
//            }
//            catch (System.Exception ex)
//            {
//                Debug.LogWarning($"GetOwnedNFTs not supported or failed: {ex.Message}. Falling back to tokenOfOwnerByIndex.");
//                for (int i = 0; i < tokenBalance; i++)
//                {
//                    try
//                    {
//                        BigInteger index = new BigInteger(i);
//                        var tokenId = await contract.Read<BigInteger>("tokenOfOwnerByIndex", walletAddress, index);
//                        tokenIds.Add(tokenId.ToString());
//                        Debug.Log($"Fetched token ID at index {i}: {tokenId}");
//                    }
//                    catch (System.Exception ex2)
//                    {
//                        Debug.LogWarning($"Failed to fetch token ID at index {i}: {ex2.Message}");
//                    }
//                }
//            }

//            if (tokenIds.Count == 0 && tokenBalance > 0)
//            {
//                Debug.LogWarning("No token IDs fetched. Assuming sequential IDs based on totalSupply.");
//                BigInteger totalSupply;
//                try
//                {
//                    totalSupply = await contract.Read<BigInteger>("totalSupply");
//                    Debug.Log($"Contract totalSupply: {totalSupply}");
//                }
//                catch
//                {
//                    Debug.LogWarning("Could not fetch totalSupply; assuming totalSupply = tokenBalance.");
//                    totalSupply = tokenBalance;
//                }

//                BigInteger startId = totalSupply - claimAmount;
//                if (startId < 0) startId = 0;
//                for (int i = 0; i < claimAmount; i++)
//                {
//                    BigInteger assumedId = startId + i;
//                    tokenIds.Add(assumedId.ToString());
//                    Debug.Log($"Assumed token ID: {assumedId}");
//                }
//            }

//            Debug.Log($"Total token IDs fetched: {tokenIds.Count}");
//            if (tokenIds.Count == 0)
//            {
//                Debug.LogError("No token IDs available. Verify contract or ownership on Basescan.");
//                if (ClaimedNFTText != null)
//                {
//                    ClaimedNFTText.text = $"No NFTs found for {walletAddress}. Balance: {tokenBalance}";
//                }
//                return;
//            }

//            if (NftDisplayCanvas != null)
//            {
//                NftDisplayCanvas.gameObject.SetActive(true);
//                NftDisplayCanvas.enabled = true;
//                Debug.Log($"Activated NftDisplayCanvas: {NftDisplayCanvas.name}, IsActive: {NftDisplayCanvas.gameObject.activeInHierarchy}");
//            }
//            else
//            {
//                Debug.LogError("NftDisplayCanvas is null!");
//                return;
//            }

//            foreach (var tokenId in tokenIds)
//            {
//                try
//                {
//                    Debug.Log($"Processing token ID: {tokenId}");
//                    BigInteger tokenIdBigInt = BigInteger.Parse(tokenId);
//                    var nft = await contract.ERC721_GetNFT(tokenIdBigInt);
//                    string name = nft.Metadata.Name ?? $"NFT #{tokenId}";
//                    string description = nft.Metadata.Description ?? "No description";
//                    string tokenUri = nft.Metadata.Uri ?? "";
//                    Debug.Log($"NFT metadata - TokenID: {tokenId}, Name: {name}, Description: {description}, TokenUri: {tokenUri}");

//                    string imageUrl = await GetImageUrlFromTokenUri(tokenUri);
//                    Debug.Log($"Image URL for token ID {tokenId}: {imageUrl ?? "null"}");
//                    Texture2D imageTexture = null;
//                    if (!string.IsNullOrEmpty(imageUrl))
//                    {
//                        imageTexture = await LoadImage(imageUrl);
//                        Debug.Log($"Image texture loaded for token ID {tokenId}: {(imageTexture != null ? "Success" : "Failed")}");
//                    }
//                    else
//                    {
//                        Debug.LogWarning($"No image URL for token ID {tokenId}. Using placeholder.");
//                        imageUrl = "https://via.placeholder.com/150"; // Fallback placeholder
//                        imageTexture = await LoadImage(imageUrl);
//                    }

//                    GameObject nftDisplay = Instantiate(NftDisplayPrefab, parentTransform);
//                    nftDisplay.SetActive(true);
//                    instantiatedNfts.Add(nftDisplay);
//                    Debug.Log($"Instantiated prefab for token ID {tokenId}: {nftDisplay.name}");

//                    TextMeshProUGUI[] textComponents = nftDisplay.GetComponentsInChildren<TextMeshProUGUI>(true);
//                    RawImage imageRaw = nftDisplay.GetComponentInChildren<RawImage>(true);

//                    TextMeshProUGUI nameText = null;
//                    TextMeshProUGUI descText = null;
//                    TextMeshProUGUI tokenIdText = null;

//                    foreach (var text in textComponents)
//                    {
//                        string textName = text.gameObject.name.ToLower();
//                        if (textName.Contains("name")) nameText = text;
//                        else if (textName.Contains("description") || textName.Contains("desc")) descText = text;
//                        else if (textName.Contains("tokenid") || textName.Contains("token")) tokenIdText = text;
//                    }

//                    if (nameText != null)
//                    {
//                        nameText.text = $"Name: {name}";
//                        nameText.color = new Color(1, 1, 1, 1); // Ensure full opacity
//                        Debug.Log($"Set NameText for token ID {tokenId}: {nameText.text}");
//                    }
//                    else Debug.LogWarning($"NameText not found in prefab for token ID {tokenId}");
//                    if (descText != null)
//                    {
//                        descText.text = $"Description: {description}";
//                        descText.color = new Color(1, 1, 1, 1);
//                        Debug.Log($"Set DescriptionText for token ID {tokenId}: {descText.text}");
//                    }
//                    else Debug.LogWarning($"DescriptionText not found in prefab for token ID {tokenId}");
//                    if (tokenIdText != null)
//                    {
//                        tokenIdText.text = $"Token ID: {tokenId}";
//                        tokenIdText.color = new Color(1, 1, 1, 1);
//                        Debug.Log($"Set TokenIdText for token ID {tokenId}: {tokenIdText.text}");
//                    }
//                    else Debug.LogWarning($"TokenIdText not found in prefab for token ID {tokenId}");
//                    if (imageRaw != null)
//                    {
//                        imageRaw.color = Color.white; // Reset tint
//                        imageRaw.texture = imageTexture ?? Texture2D.grayTexture;
//                        Debug.Log($"Set RawImage for token ID {tokenId}: {(imageTexture != null ? "Image" : "Placeholder")}");
//                    }
//                    else Debug.LogWarning($"RawImage not found in prefab for token ID {tokenId}");

//                    Debug.Log($"Displayed NFT: TokenID={tokenId}, Name={name}, Description={description}, ImageURL={imageUrl ?? "null"}");
//                }
//                catch (System.Exception ex)
//                {
//                    Debug.LogError($"Failed to process NFT with TokenID {tokenId}: {ex.Message}");
//                }
//            }

//            if (ClaimedNFTText != null)
//            {
//                ClaimedNFTText.gameObject.SetActive(true);
//                ClaimedNFTText.color = new Color(1, 1, 1, 1); // Ensure full opacity
//                ClaimedNFTText.text = $"NFT Balance: {tokenBalance}\nTx Hash: {transactionResult.TransactionHash}";
//            }
//        }
//        catch (System.Exception ex)
//        {
//            Debug.LogError($"Failed to claim NFTs: {ex.Message}");
//            if (ClaimedNFTText != null)
//            {
//                ClaimedNFTText.text = $"Failed to claim: {ex.Message}";
//            }
//            if (NftDisplayCanvas != null) NftDisplayCanvas.gameObject.SetActive(false);
//        }
//    }

//    private async Task<string> GetImageUrlFromTokenUri(string tokenUri)
//    {
//        if (string.IsNullOrEmpty(tokenUri))
//        {
//            Debug.LogWarning("Token URI is empty or null.");
//            return null;
//        }

//        Debug.Log($"Fetching metadata from token URI: {tokenUri}");
//        if (tokenUri.StartsWith("ipfs://"))
//        {
//            tokenUri = tokenUri.Replace("ipfs://", "https://cloudflare-ipfs.com/ipfs/");
//            Debug.Log($"Converted IPFS URI to: {tokenUri}");
//        }

//        try
//        {
//            UnityWebRequest request = UnityWebRequest.Get(tokenUri);
//            request.timeout = 10; // Set timeout to avoid hanging
//            var operation = request.SendWebRequest();
//            while (!operation.isDone) await Task.Yield();

//            if (request.result != UnityWebRequest.Result.Success)
//            {
//                Debug.LogError($"Failed to fetch metadata from {tokenUri}: {request.error} (Response Code: {request.responseCode})");
//                return null;
//            }

//            string json = request.downloadHandler.text;
//            Debug.Log($"Metadata JSON: {json}");
//            JObject metadata = JObject.Parse(json);
//            string imageUrl = metadata["image"]?.ToString() ?? metadata["image_url"]?.ToString();
//            if (string.IsNullOrEmpty(imageUrl))
//            {
//                Debug.LogWarning($"No 'image' or 'image_url' field found in metadata: {json}");
//                return null;
//            }

//            Debug.Log($"Parsed image URL: {imageUrl}");
//            if (imageUrl.StartsWith("ipfs://"))
//            {
//                imageUrl = imageUrl.Replace("ipfs://", "https://cloudflare-ipfs.com/ipfs/");
//                Debug.Log($"Converted IPFS image URL to: {imageUrl}");
//            }

//            return imageUrl;
//        }
//        catch (System.Exception ex)
//        {
//            Debug.LogError($"Failed to parse metadata from {tokenUri}: {ex.Message}");
//            return null;
//        }
//    }

//    private async Task<Texture2D> LoadImage(string imageUrl)
//    {
//        if (string.IsNullOrEmpty(imageUrl))
//        {
//            Debug.LogWarning("Image URL is empty or null.");
//            return null;
//        }

//        Debug.Log($"Loading image from: {imageUrl}");
//        try
//        {
//            UnityWebRequest request = UnityWebRequestTexture.GetTexture(imageUrl);
//            request.timeout = 10; // Set timeout
//            var operation = request.SendWebRequest();
//            while (!operation.isDone) await Task.Yield();

//            if (request.result != UnityWebRequest.Result.Success)
//            {
//                Debug.LogError($"Failed to load image from {imageUrl}: {request.error} (Response Code: {request.responseCode})");
//                return null;
//            }

//            Texture2D texture = DownloadHandlerTexture.GetContent(request);
//            Debug.Log($"Image loaded successfully: {imageUrl} (Size: {texture.width}x{texture.height})");
//            return texture;
//        }
//        catch (System.Exception ex)
//        {
//            Debug.LogError($"Failed to load image from {imageUrl}: {ex.Message}");
//            return null;
//        }
//    }

//    private void ClearNftDisplay()
//    {
//        foreach (var nftDisplay in instantiatedNfts)
//        {
//            if (nftDisplay != null) Destroy(nftDisplay);
//        }
//        instantiatedNfts.Clear();
//        Debug.Log("Cleared NFT display.");
//    }
//}




//using UnityEngine;
//using Thirdweb;
//using System.Threading.Tasks;
//using Thirdweb.Unity;
//using System.Numerics;
//using TMPro;
//using System.Text;
//using System.Collections.Generic;
//using UnityEngine.Networking;
//using UnityEngine.UI;
//using Newtonsoft.Json.Linq;

//public class WalletConnectManager : MonoBehaviour
//{
//    private ThirdwebManager thirdwebManager;
//    private IThirdwebWallet wallet;
//    private string walletAddress;
//    [field: SerializeField, Header("Wallet Options")]
//    private ulong ActiveChainId = 84532;

//    [field: SerializeField, Header("Send ETH amount")]
//    public string Amount { get; set; }
//    [field: SerializeField, Header("Send ETH address")]
//    public string ToAddress { get; set; }

//    [field: SerializeField, Header("Send Custom Token Options")]
//    public string TokenName { get; set; }
//    [field: SerializeField]
//    public string TokenContractAddress { get; set; }
//    [field: SerializeField]
//    public string TokenAmount { get; set; }
//    [field: SerializeField]
//    public string TokenRecipientAddress { get; set; }

//    [field: SerializeField, Header("Claim Token Options")]
//    public string ClaimTokenContractAddress { get; set; }
//    [field: SerializeField]
//    public string ClaimTokenAmount { get; set; }

//    [field: SerializeField, Header("Claim Nft Options")]
//    public string ClaimNftContractAddress { get; set; }
//    [field: SerializeField]
//    public string ClaimNftAmount { get; set; }

//    [field: SerializeField, Header("UI Elements")]
//    public GameManager GameManager { get; set; }
//    [field: SerializeField]
//    public Button ClaimButton { get; set; }
//    [field: SerializeField]
//    public GameObject ConnectButton { get; set; }
//    [field: SerializeField]
//    public GameObject DisconnectButton { get; set; }
//    [field: SerializeField]
//    public TextMeshProUGUI ConnectedText { get; set; }
//    [field: SerializeField]
//    public TextMeshProUGUI ClaimedNFTText { get; set; }
//    [field: SerializeField]
//    public TextMeshProUGUI AddressText { get; set; }
//    [field: SerializeField]
//    public TextMeshProUGUI EthBalanceText { get; set; }
//    [field: SerializeField]
//    public TextMeshProUGUI CustomTokenBalanceText { get; set; }
//    [field: SerializeField]
//    public TextMeshProUGUI ClaimedTokenBalanceText { get; set; }

//    [field: SerializeField, Header("NFT Display Canvas")]
//    public Canvas NftDisplayCanvas { get; set; }
//    [field: SerializeField]
//    public GameObject NftDisplayPrefab { get; set; }
//    [field: SerializeField]
//    public Transform NftDisplayParent { get; set; }

//    private List<GameObject> instantiatedNfts = new List<GameObject>();

//    void Awake()
//    {
//        thirdwebManager = FindObjectOfType<ThirdwebManager>();
//        if (thirdwebManager == null)
//        {
//            Debug.LogError("ThirdwebManager not found in the scene! Please add the ThirdwebManager prefab.");
//        }

//        if (GameManager == null)
//        {
//            GameManager = FindObjectOfType<GameManager>();
//            if (GameManager == null)
//            {
//                Debug.LogError("GameManager not found in the scene! Please add the GameManager component.");
//            }
//        }

//        if (ConnectButton != null) ConnectButton.SetActive(true);
//        if (DisconnectButton != null)
//        {
//            DisconnectButton.SetActive(false);
//            var buttonComponent = DisconnectButton.GetComponent<UnityEngine.UI.Button>();
//            if (buttonComponent != null)
//            {
//                buttonComponent.interactable = true;
//            }
//            else
//            {
//                Debug.LogError("DisconnectButton does not have a Button component!");
//            }
//        }
//        if (ClaimButton != null)
//        {
//            ClaimButton.interactable = true;
//        }
//        if (ConnectedText != null) ConnectedText.gameObject.SetActive(false);
//        if (AddressText != null) AddressText.gameObject.SetActive(false);
//        if (EthBalanceText != null) EthBalanceText.gameObject.SetActive(false);
//        if (CustomTokenBalanceText != null) CustomTokenBalanceText.gameObject.SetActive(false);
//        if (ClaimedTokenBalanceText != null) ClaimedTokenBalanceText.gameObject.SetActive(false);
//        if (ClaimedNFTText != null) ClaimedNFTText.gameObject.SetActive(false);
//        if (NftDisplayCanvas != null) NftDisplayCanvas.gameObject.SetActive(false);
//    }

//    public async void Connect()
//    {
//        if (thirdwebManager == null)
//        {
//            Debug.LogError("Cannot connect: ThirdwebManager is not initialized.");
//            return;
//        }

//        try
//        {
//            if (ConnectedText != null)
//            {
//                ConnectedText.gameObject.SetActive(true);
//                ConnectedText.text = "Connecting...";
//            }
//            if (DisconnectButton != null)
//            {
//                var disconnectButton = DisconnectButton.GetComponent<Button>();
//                if (disconnectButton != null)
//                {
//                    disconnectButton.interactable = false;
//                }
//            }

//            // Disconnect existing wallet if connected
//            if (wallet != null)
//            {
//                await wallet.Disconnect();
//                wallet = null;
//                walletAddress = null;
//                Debug.Log("Disconnected existing wallet to start new connection.");
//            }

//            var options = new WalletOptions(
//                provider: WalletProvider.WalletConnectWallet,
//                chainId: 84532
//            );

//            Debug.Log("Initiating wallet connection...");
//            wallet = await ThirdwebManager.Instance.ConnectWallet(options);
//            walletAddress = await wallet.GetAddress();
//            Debug.Log($"Wallet connected successfully! Address: {walletAddress}");

//            var balance = await wallet.GetBalance(chainId: ActiveChainId);
//            var balanceEth = Utils.ToEth(wei: balance.ToString(), decimalsToDisplay: 4, addCommas: true);
//            Debug.Log($"Wallet balance: {balanceEth}");
//            if (EthBalanceText != null)
//            {
//                EthBalanceText.gameObject.SetActive(true);
//                EthBalanceText.text = $"ETH Balance: {balanceEth}";
//            }

//            if (!string.IsNullOrEmpty(TokenContractAddress))
//            {
//                var contract = await ThirdwebManager.Instance.GetContract(TokenContractAddress, ActiveChainId);
//                var decimals = await contract.ERC20_Decimals();
//                var tokenBalance = await contract.ERC20_BalanceOf(walletAddress);
//                var tokenBalanceFormatted = Utils.ToEth(tokenBalance.ToString(), (int)decimals, addCommas: true);
//                Debug.Log($"Custom token balance for {walletAddress}: {tokenBalanceFormatted}");
//                if (CustomTokenBalanceText != null)
//                {
//                    CustomTokenBalanceText.gameObject.SetActive(true);
//                    CustomTokenBalanceText.text = $"{TokenName} Balance: {tokenBalanceFormatted}";
//                }
//            }

//            if (ConnectButton != null)
//            {
//                ConnectButton.SetActive(true);
//                var connectButton = ConnectButton.GetComponent<Button>();
//                if (connectButton != null)
//                {
//                    connectButton.interactable = true;
//                }
//            }
//            if (DisconnectButton != null)
//            {
//                DisconnectButton.SetActive(true);
//                var buttonComponent = DisconnectButton.GetComponent<Button>();
//                if (buttonComponent != null)
//                {
//                    buttonComponent.interactable = true;
//                }
//            }
//            if (ConnectedText != null)
//            {
//                ConnectedText.text = "Connected";
//            }
//            if (AddressText != null && !string.IsNullOrEmpty(walletAddress))
//            {
//                AddressText.gameObject.SetActive(true);
//                string shortAddress = $"{walletAddress.Substring(0, 3)}...{walletAddress.Substring(walletAddress.Length - 3)}";
//                AddressText.text = shortAddress;
//            }
//        }
//        catch (System.Exception ex)
//        {
//            Debug.LogWarning($"Wallet connection failed or canceled: {ex.Message}");
//            wallet = null;
//            walletAddress = null;

//            if (ConnectButton != null)
//            {
//                ConnectButton.SetActive(true);
//                var connectButton = ConnectButton.GetComponent<Button>();
//                if (connectButton != null)
//                {
//                    connectButton.interactable = true;
//                }
//            }
//            if (DisconnectButton != null)
//            {
//                DisconnectButton.SetActive(false);
//                var disconnectButton = DisconnectButton.GetComponent<Button>();
//                if (disconnectButton != null)
//                {
//                    disconnectButton.interactable = false;
//                }
//            }
//            if (ConnectedText != null)
//            {
//                ConnectedText.gameObject.SetActive(false);
//            }
//            if (AddressText != null) AddressText.gameObject.SetActive(false);
//            if (EthBalanceText != null) EthBalanceText.gameObject.SetActive(false);
//            if (CustomTokenBalanceText != null) CustomTokenBalanceText.gameObject.SetActive(false);
//            if (ClaimedTokenBalanceText != null) ClaimedTokenBalanceText.gameObject.SetActive(false);
//            if (ClaimedNFTText != null) ClaimedNFTText.gameObject.SetActive(false);
//            if (NftDisplayCanvas != null) NftDisplayCanvas.gameObject.SetActive(false);
//        }
//    }

//    public async void Disconnect()
//    {
//        if (wallet == null)
//        {
//            Debug.LogWarning("No wallet to disconnect.");
//            return;
//        }

//        try
//        {
//            Debug.Log("Disconnecting wallet...");
//            await wallet.Disconnect();
//            wallet = null;
//            walletAddress = null;

//            if (ConnectButton != null)
//            {
//                ConnectButton.SetActive(true);
//                var connectButton = ConnectButton.GetComponent<Button>();
//                if (connectButton != null) connectButton.interactable = true;
//            }
//            if (DisconnectButton != null) DisconnectButton.SetActive(false);
//            if (ClaimButton != null) ClaimButton.interactable = true;
//            if (ConnectedText != null) ConnectedText.gameObject.SetActive(false);
//            if (AddressText != null) AddressText.gameObject.SetActive(false);
//            if (EthBalanceText != null) EthBalanceText.gameObject.SetActive(false);
//            if (CustomTokenBalanceText != null) CustomTokenBalanceText.gameObject.SetActive(false);
//            if (ClaimedTokenBalanceText != null) ClaimedTokenBalanceText.gameObject.SetActive(false);
//            if (ClaimedNFTText != null) ClaimedNFTText.gameObject.SetActive(false);
//            if (NftDisplayCanvas != null) NftDisplayCanvas.gameObject.SetActive(false);
//            ClearNftDisplay();
//        }
//        catch (System.Exception ex)
//        {
//            Debug.LogError($"Failed to disconnect wallet: {ex.Message}");
//        }
//    }

//    public async void SendEth()
//    {
//        if (thirdwebManager == null || wallet == null)
//        {
//            Debug.LogError("Cannot send ETH: ThirdwebManager or wallet not initialized.");
//            return;
//        }

//        if (string.IsNullOrEmpty(ToAddress) || !ToAddress.StartsWith("0x") || ToAddress.Length != 42)
//        {
//            Debug.LogError("Invalid recipient address.");
//            return;
//        }

//        if (string.IsNullOrEmpty(Amount) || !float.TryParse(Amount, out float ethAmount) || ethAmount <= 0)
//        {
//            Debug.LogError("Invalid ETH amount.");
//            return;
//        }

//        try
//        {
//            Debug.Log($"Sending {Amount} ETH to {ToAddress}...");
//            if (wallet is WalletConnectWallet walletConnect)
//            {
//                await walletConnect.EnsureCorrectNetwork(ActiveChainId);
//            }
//            await Task.Delay(10000);
//            string weiAmountString = Utils.ToWei(Amount);
//            BigInteger weiAmount = BigInteger.Parse(weiAmountString);
//            var transactionResult = await wallet.Transfer(ActiveChainId, ToAddress, weiAmount);
//            Debug.Log($"ETH sent! Transaction Hash: {transactionResult.TransactionHash}");

//            var balance = await wallet.GetBalance(chainId: ActiveChainId);
//            var balanceEth = Utils.ToEth(wei: balance.ToString(), decimalsToDisplay: 4, addCommas: true);
//            if (EthBalanceText != null)
//            {
//                EthBalanceText.gameObject.SetActive(true);
//                EthBalanceText.text = $"ETH Balance: {balanceEth}";
//            }
//        }
//        catch (System.Exception ex)
//        {
//            Debug.LogError($"Failed to send ETH: {ex.Message}");
//        }
//    }

//    public async void SendCustomToken()
//    {
//        if (thirdwebManager == null || wallet == null)
//        {
//            Debug.LogError("Cannot send token: ThirdwebManager or wallet not initialized.");
//            return;
//        }

//        if (string.IsNullOrEmpty(TokenContractAddress) || string.IsNullOrEmpty(TokenRecipientAddress))
//        {
//            Debug.LogError("Invalid token contract or recipient address.");
//            return;
//        }

//        if (string.IsNullOrEmpty(TokenAmount) || !float.TryParse(TokenAmount, out float tokenAmount) || tokenAmount <= 0)
//        {
//            Debug.LogError("Invalid token amount.");
//            return;
//        }

//        try
//        {
//            Debug.Log($"Sending {TokenAmount} {TokenName} to {TokenRecipientAddress}...");
//            if (wallet is WalletConnectWallet walletConnect)
//            {
//                await walletConnect.EnsureCorrectNetwork(ActiveChainId);
//            }
//            await Task.Delay(10000);
//            var contract = await ThirdwebManager.Instance.GetContract(TokenContractAddress, ActiveChainId);
//            var decimals = await contract.ERC20_Decimals();
//            string tokenAmountInWei = Utils.ToWei(TokenAmount);
//            BigInteger tokenAmountBigInt = BigInteger.Parse(tokenAmountInWei);
//            var transactionResult = await contract.ERC20_Transfer(wallet, TokenRecipientAddress, tokenAmountBigInt);
//            Debug.Log($"Token sent! Transaction Hash: {transactionResult.TransactionHash}");

//            var tokenBalance = await contract.ERC20_BalanceOf(walletAddress);
//            var tokenBalanceFormatted = Utils.ToEth(tokenBalance.ToString(), (int)decimals, addCommas: true);
//            if (CustomTokenBalanceText != null)
//            {
//                CustomTokenBalanceText.gameObject.SetActive(true);
//                CustomTokenBalanceText.text = $"{TokenName} Balance: {tokenBalanceFormatted}";
//            }
//        }
//        catch (System.Exception ex)
//        {
//            Debug.LogError($"Failed to send {TokenName}: {ex.Message}");
//        }
//    }

//    public async void ClaimToken()
//    {
//        if (thirdwebManager == null || wallet == null)
//        {
//            Debug.LogError("Cannot claim token: ThirdwebManager or wallet not initialized.");
//            return;
//        }

//        if (string.IsNullOrEmpty(ClaimTokenContractAddress) || !ClaimTokenContractAddress.StartsWith("0x") || ClaimTokenContractAddress.Length != 42)
//        {
//            Debug.LogError("Invalid claim token contract address.");
//            return;
//        }

//        if (GameManager == null)
//        {
//            Debug.LogError("GameManager not assigned. Cannot get total XP.");
//            return;
//        }

//        try
//        {
//            if (ClaimButton != null) ClaimButton.interactable = false;
//            if (ClaimedTokenBalanceText != null)
//            {
//                ClaimedTokenBalanceText.gameObject.SetActive(true);
//                ClaimedTokenBalanceText.text = "Claiming...";
//            }

//            float totalXP = GameManager.GetTotalXP();
//            decimal tokenAmount = (decimal)totalXP * 100;
//            ClaimTokenAmount = tokenAmount.ToString();
//            string claimAmountInWei = Utils.ToWei(ClaimTokenAmount);
//            Debug.Log($"Claiming {tokenAmount} tokens ({claimAmountInWei} wei) based on {totalXP} XP");

//            if (wallet is WalletConnectWallet walletConnect)
//            {
//                await walletConnect.EnsureCorrectNetwork(ActiveChainId);
//            }
//            await Task.Delay(10000);

//            var contract = await ThirdwebManager.Instance.GetContract(ClaimTokenContractAddress, ActiveChainId);
//            var decimals = await contract.ERC20_Decimals();
//            var transactionResult = await contract.DropERC20_Claim(wallet, walletAddress, claimAmountInWei);
//            Debug.Log($"Tokens claimed successfully! Transaction Hash: {transactionResult.TransactionHash}");
//            await Task.Delay(10000);

//            var tokenBalance = await contract.ERC20_BalanceOf(walletAddress);
//            var tokenBalanceFormatted = Utils.ToEth(tokenBalance.ToString(), (int)decimals, addCommas: true);
//            Debug.Log($"Updated token balance for {walletAddress}: {tokenBalanceFormatted}");
//            if (ClaimedTokenBalanceText != null)
//            {
//                ClaimedTokenBalanceText.text = $"Claimed: {tokenBalanceFormatted} CYS";
//            }
//        }
//        catch (System.Exception ex)
//        {
//            Debug.LogError($"Failed to claim tokens: {ex.Message}");
//            if (ClaimedTokenBalanceText != null)
//            {
//                ClaimedTokenBalanceText.text = $"Claim Failed: {ex.Message}";
//            }
//            if (ClaimButton != null) ClaimButton.interactable = true;
//        }
//    }

//    public async void ConnectWithEcosystem()
//    {
//        if (thirdwebManager == null)
//        {
//            Debug.LogError("Cannot connect: ThirdwebManager is not initialized.");
//            return;
//        }

//        try
//        {
//            if (ConnectedText != null)
//            {
//                ConnectedText.gameObject.SetActive(true);
//                ConnectedText.text = "Connecting...";
//            }
//            if (DisconnectButton != null)
//            {
//                var disconnectButton = DisconnectButton.GetComponent<Button>();
//                if (disconnectButton != null)
//                {
//                    disconnectButton.interactable = false;
//                }
//            }

//            // Disconnect existing wallet if connected
//            if (wallet != null)
//            {
//                await wallet.Disconnect();
//                wallet = null;
//                walletAddress = null;
//                Debug.Log("Disconnected existing wallet to start new connection.");
//            }

//            var ecosystemWalletOptions = new EcosystemWalletOptions(ecosystemId: "ecosystem.your-ecosystem", email: "myepicemail@domain.id");
//            var options = new WalletOptions(
//                provider: WalletProvider.EcosystemWallet,
//                chainId: 84532,
//                ecosystemWalletOptions: ecosystemWalletOptions
//            );
//            wallet = await ThirdwebManager.Instance.ConnectWallet(options);
//            walletAddress = await wallet.GetAddress();
//            Debug.Log($"Wallet connected successfully! Address: {walletAddress}");

//            var balance = await wallet.GetBalance(chainId: ActiveChainId);
//            var balanceEth = Utils.ToEth(wei: balance.ToString(), decimalsToDisplay: 4, addCommas: true);
//            Debug.Log($"Wallet balance: {balanceEth}");
//            if (EthBalanceText != null)
//            {
//                EthBalanceText.gameObject.SetActive(true);
//                EthBalanceText.text = $"ETH Balance: {balanceEth}";
//            }

//            if (!string.IsNullOrEmpty(TokenContractAddress))
//            {
//                var contract = await ThirdwebManager.Instance.GetContract(TokenContractAddress, ActiveChainId);
//                var decimals = await contract.ERC20_Decimals();
//                var tokenBalance = await contract.ERC20_BalanceOf(walletAddress);
//                var tokenBalanceFormatted = Utils.ToEth(tokenBalance.ToString(), (int)decimals, addCommas: true);
//                Debug.Log($"Custom token balance for {walletAddress}: {tokenBalanceFormatted}");
//                if (CustomTokenBalanceText != null)
//                {
//                    CustomTokenBalanceText.gameObject.SetActive(true);
//                    CustomTokenBalanceText.text = $"{TokenName} Balance: {tokenBalanceFormatted}";
//                }
//            }

//            if (ConnectButton != null)
//            {
//                ConnectButton.SetActive(true);
//                var connectButton = ConnectButton.GetComponent<Button>();
//                if (connectButton != null)
//                {
//                    connectButton.interactable = true;
//                }
//            }
//            if (DisconnectButton != null)
//            {
//                DisconnectButton.SetActive(true);
//                var buttonComponent = DisconnectButton.GetComponent<Button>();
//                if (buttonComponent != null)
//                {
//                    buttonComponent.interactable = true;
//                }
//            }
//            if (ConnectedText != null)
//            {
//                ConnectedText.text = "Connected";
//            }
//            if (AddressText != null && !string.IsNullOrEmpty(walletAddress))
//            {
//                AddressText.gameObject.SetActive(true);
//                string shortAddress = $"{walletAddress.Substring(0, 3)}...{walletAddress.Substring(walletAddress.Length - 3)}";
//                AddressText.text = shortAddress;
//            }
//        }
//        catch (System.Exception ex)
//        {
//            Debug.LogWarning($"Wallet connection failed or canceled: {ex.Message}");
//            wallet = null;
//            walletAddress = null;

//            if (ConnectButton != null)
//            {
//                ConnectButton.SetActive(true);
//                var connectButton = ConnectButton.GetComponent<Button>();
//                if (connectButton != null)
//                {
//                    connectButton.interactable = true;
//                }
//            }
//            if (DisconnectButton != null)
//            {
//                DisconnectButton.SetActive(false);
//                var disconnectButton = DisconnectButton.GetComponent<Button>();
//                if (disconnectButton != null)
//                {
//                    disconnectButton.interactable = false;
//                }
//            }
//            if (ConnectedText != null)
//            {
//                ConnectedText.gameObject.SetActive(false);
//            }
//            if (AddressText != null) AddressText.gameObject.SetActive(false);
//            if (EthBalanceText != null) EthBalanceText.gameObject.SetActive(false);
//            if (CustomTokenBalanceText != null) CustomTokenBalanceText.gameObject.SetActive(false);
//            if (ClaimedTokenBalanceText != null) ClaimedTokenBalanceText.gameObject.SetActive(false);
//            if (ClaimedNFTText != null) ClaimedNFTText.gameObject.SetActive(false);
//            if (NftDisplayCanvas != null) NftDisplayCanvas.gameObject.SetActive(false);
//        }
//    }

//    public async void ClaimNft()
//    {
//        if (thirdwebManager == null || wallet == null)
//        {
//            Debug.LogError("Cannot claim NFT: ThirdwebManager or wallet not initialized.");
//            return;
//        }

//        if (string.IsNullOrEmpty(ClaimNftContractAddress) || !ClaimNftContractAddress.StartsWith("0x") || ClaimNftContractAddress.Length != 42)
//        {
//            Debug.LogError("Invalid claim NFT contract address.");
//            return;
//        }

//        if (string.IsNullOrEmpty(ClaimNftAmount) || !int.TryParse(ClaimNftAmount, out int claimAmount) || claimAmount <= 0)
//        {
//            Debug.LogError("Invalid claim amount.");
//            return;
//        }

//        if (NftDisplayCanvas == null || NftDisplayPrefab == null)
//        {
//            Debug.LogError("NFT display canvas or prefab not assigned.");
//            return;
//        }

//        Transform parentTransform = NftDisplayParent != null ? NftDisplayParent : NftDisplayCanvas.transform;

//        try
//        {
//            if (ClaimedNFTText != null)
//            {
//                ClaimedNFTText.gameObject.SetActive(true);
//                ClaimedNFTText.text = "Claiming...";
//            }

//            if (wallet is WalletConnectWallet walletConnect)
//            {
//                await walletConnect.EnsureCorrectNetwork(ActiveChainId);
//            }
//            await Task.Delay(20000);

//            var contract = await ThirdwebManager.Instance.GetContract(ClaimNftContractAddress, ActiveChainId);
//            var transactionResult = await contract.DropERC721_Claim(wallet, walletAddress, claimAmount);
//            Debug.Log($"NFTs claimed! Transaction Hash: {transactionResult.TransactionHash}");

//            BigInteger tokenBalance = 0;
//            int maxAttempts = 5;
//            for (int attempt = 0; attempt < maxAttempts; attempt++)
//            {
//                tokenBalance = await contract.ERC721_BalanceOf(walletAddress);
//                Debug.Log($"Balance check attempt {attempt + 1}/{maxAttempts}: {tokenBalance} NFTs");
//                if (tokenBalance >= claimAmount) break;
//                await Task.Delay(5000);
//            }

//            if (tokenBalance == 0)
//            {
//                Debug.LogError($"No NFTs owned by {walletAddress}.");
//                if (ClaimedNFTText != null)
//                {
//                    ClaimedNFTText.text = $"No NFTs owned. Tx Hash: {transactionResult.TransactionHash}";
//                }
//                return;
//            }

//            if (ClaimedNFTText != null)
//            {
//                ClaimedNFTText.text = $"Claimed! Tx Hash: {transactionResult.TransactionHash}\nBalance: {tokenBalance}";
//            }

//            ClearNftDisplay();

//            List<string> tokenIds = new List<string>();
//            try
//            {
//                var ownedNfts = await contract.ERC1155_GetOwnedNFTs(walletAddress);
//                foreach (var nft in ownedNfts)
//                {
//                    tokenIds.Add(nft.Metadata.Id.ToString());
//                    Debug.Log($"Fetched token ID: {nft.Metadata.Id}");
//                }
//            }
//            catch (System.Exception ex)
//            {
//                Debug.LogWarning($"GetOwnedNFTs failed: {ex.Message}. Using tokenOfOwnerByIndex.");
//                for (int i = 0; i < tokenBalance; i++)
//                {
//                    try
//                    {
//                        BigInteger index = new BigInteger(i);
//                        var tokenId = await contract.Read<BigInteger>("tokenOfOwnerByIndex", walletAddress, index);
//                        tokenIds.Add(tokenId.ToString());
//                        Debug.Log($"Fetched token ID at index {i}: {tokenId}");
//                    }
//                    catch (System.Exception ex2)
//                    {
//                        Debug.LogWarning($"Failed to fetch token ID at index {i}: {ex2.Message}");
//                    }
//                }
//            }

//            if (tokenIds.Count == 0 && tokenBalance > 0)
//            {
//                Debug.LogWarning("No token IDs fetched. Assuming sequential IDs.");
//                BigInteger totalSupply;
//                try
//                {
//                    totalSupply = await contract.Read<BigInteger>("totalSupply");
//                }
//                catch
//                {
//                    totalSupply = tokenBalance;
//                }
//                BigInteger startId = totalSupply - claimAmount;
//                if (startId < 0) startId = 0;
//                for (int i = 0; i < claimAmount; i++)
//                {
//                    BigInteger assumedId = startId + i;
//                    tokenIds.Add(assumedId.ToString());
//                    Debug.Log($"Assumed token ID: {assumedId}");
//                }
//            }

//            if (tokenIds.Count == 0)
//            {
//                Debug.LogError("No token IDs available.");
//                if (ClaimedNFTText != null)
//                {
//                    ClaimedNFTText.text = $"No NFTs found for {walletAddress}. Balance: {tokenBalance}";
//                }
//                return;
//            }

//            if (NftDisplayCanvas != null)
//            {
//                NftDisplayCanvas.gameObject.SetActive(true);
//                NftDisplayCanvas.enabled = true;
//            }

//            foreach (var tokenId in tokenIds)
//            {
//                try
//                {
//                    BigInteger tokenIdBigInt = BigInteger.Parse(tokenId);
//                    var nft = await contract.ERC721_GetNFT(tokenIdBigInt);
//                    string name = nft.Metadata.Name ?? $"NFT #{tokenId}";
//                    string description = nft.Metadata.Description ?? "No description";
//                    string tokenUri = nft.Metadata.Uri ?? "";

//                    string imageUrl = await GetImageUrlFromTokenUri(tokenUri);
//                    Texture2D imageTexture = null;
//                    if (!string.IsNullOrEmpty(imageUrl))
//                    {
//                        imageTexture = await LoadImage(imageUrl);
//                    }
//                    else
//                    {
//                        imageUrl = "https://via.placeholder.com/150";
//                        imageTexture = await LoadImage(imageUrl);
//                    }

//                    GameObject nftDisplay = Instantiate(NftDisplayPrefab, parentTransform);
//                    nftDisplay.SetActive(true);
//                    instantiatedNfts.Add(nftDisplay);

//                    TextMeshProUGUI[] textComponents = nftDisplay.GetComponentsInChildren<TextMeshProUGUI>(true);
//                    RawImage imageRaw = nftDisplay.GetComponentInChildren<RawImage>(true);

//                    TextMeshProUGUI nameText = null;
//                    TextMeshProUGUI descText = null;
//                    TextMeshProUGUI tokenIdText = null;

//                    foreach (var text in textComponents)
//                    {
//                        string textName = text.gameObject.name.ToLower();
//                        if (textName.Contains("name")) nameText = text;
//                        else if (textName.Contains("description") || textName.Contains("desc")) descText = text;
//                        else if (textName.Contains("tokenid") || textName.Contains("token")) tokenIdText = text;
//                    }

//                    if (nameText != null)
//                    {
//                        nameText.text = $"Name: {name}";
//                        nameText.color = new Color(1, 1, 1, 1);
//                    }
//                    if (descText != null)
//                    {
//                        descText.text = $"Description: {description}";
//                        descText.color = new Color(1, 1, 1, 1);
//                    }
//                    if (tokenIdText != null)
//                    {
//                        tokenIdText.text = $"Token ID: {tokenId}";
//                        tokenIdText.color = new Color(1, 1, 1, 1);
//                    }
//                    if (imageRaw != null)
//                    {
//                        imageRaw.color = Color.white;
//                        imageRaw.texture = imageTexture ?? Texture2D.grayTexture;
//                    }

//                    Debug.Log($"Displayed NFT: TokenID={tokenId}, Name={name}, Description={description}, ImageURL={imageUrl ?? "null"}");
//                }
//                catch (System.Exception ex)
//                {
//                    Debug.LogError($"Failed to process NFT with TokenID {tokenId}: {ex.Message}");
//                }
//            }

//            if (ClaimedNFTText != null)
//            {
//                ClaimedNFTText.gameObject.SetActive(true);
//                ClaimedNFTText.color = new Color(1, 1, 1, 1);
//                ClaimedNFTText.text = $"NFT Balance: {tokenBalance}\nTx Hash: {transactionResult.TransactionHash}";
//            }
//        }
//        catch (System.Exception ex)
//        {
//            Debug.LogError($"Failed to claim NFTs: {ex.Message}");
//            if (ClaimedNFTText != null)
//            {
//                ClaimedNFTText.text = $"Failed to claim: {ex.Message}";
//            }
//            if (NftDisplayCanvas != null) NftDisplayCanvas.gameObject.SetActive(false);
//        }
//    }

//    private async Task<string> GetImageUrlFromTokenUri(string tokenUri)
//    {
//        if (string.IsNullOrEmpty(tokenUri))
//        {
//            Debug.LogWarning("Token URI is empty or null.");
//            return null;
//        }

//        Debug.Log($"Fetching metadata from token URI: {tokenUri}");
//        if (tokenUri.StartsWith("ipfs://"))
//        {
//            tokenUri = tokenUri.Replace("ipfs://", "https://cloudflare-ipfs.com/ipfs/");
//            Debug.Log($"Converted IPFS URI to: {tokenUri}");
//        }

//        try
//        {
//            UnityWebRequest request = UnityWebRequest.Get(tokenUri);
//            request.timeout = 10;
//            var operation = request.SendWebRequest();
//            while (!operation.isDone) await Task.Yield();

//            if (request.result != UnityWebRequest.Result.Success)
//            {
//                Debug.LogError($"Failed to fetch metadata from {tokenUri}: {request.error} (Response Code: {request.responseCode})");
//                return null;
//            }

//            string json = request.downloadHandler.text;
//            Debug.Log($"Metadata JSON: {json}");
//            JObject metadata = JObject.Parse(json);
//            string imageUrl = metadata["image"]?.ToString() ?? metadata["image_url"]?.ToString();
//            if (string.IsNullOrEmpty(imageUrl))
//            {
//                Debug.LogWarning($"No 'image' or 'image_url' field found in metadata: {json}");
//                return null;
//            }

//            Debug.Log($"Parsed image URL: {imageUrl}");
//            if (imageUrl.StartsWith("ipfs://"))
//            {
//                imageUrl = imageUrl.Replace("ipfs://", "https://cloudflare-ipfs.com/ipfs/");
//                Debug.Log($"Converted IPFS image URL to: {imageUrl}");
//            }

//            return imageUrl;
//        }
//        catch (System.Exception ex)
//        {
//            Debug.LogError($"Failed to parse metadata from {tokenUri}: {ex.Message}");
//            return null;
//        }
//    }

//    private async Task<Texture2D> LoadImage(string imageUrl)
//    {
//        if (string.IsNullOrEmpty(imageUrl))
//        {
//            Debug.LogWarning("Image URL is empty or null.");
//            return null;
//        }

//        Debug.Log($"Loading image from: {imageUrl}");
//        try
//        {
//            UnityWebRequest request = UnityWebRequestTexture.GetTexture(imageUrl);
//            request.timeout = 10;
//            var operation = request.SendWebRequest();
//            while (!operation.isDone) await Task.Yield();

//            if (request.result != UnityWebRequest.Result.Success)
//            {
//                Debug.LogError($"Failed to load image from {imageUrl}: {request.error} (Response Code: {request.responseCode})");
//                return null;
//            }

//            Texture2D texture = DownloadHandlerTexture.GetContent(request);
//            Debug.Log($"Image loaded successfully: {imageUrl} (Size: {texture.width}x{texture.height})");
//            return texture;
//        }
//        catch (System.Exception ex)
//        {
//            Debug.LogError($"Failed to load image from {imageUrl}: {ex.Message}");
//            return null;
//        }
//    }

//    private void ClearNftDisplay()
//    {
//        foreach (var nftDisplay in instantiatedNfts)
//        {
//            if (nftDisplay != null) Destroy(nftDisplay);
//        }
//        instantiatedNfts.Clear();
//        Debug.Log("Cleared NFT display.");
//    }
//}


//using UnityEngine;
//using Thirdweb;
//using System.Threading.Tasks;
//using Thirdweb.Unity;
//using System.Numerics;
//using TMPro;
//using System.Text;
//using System.Collections.Generic;
//using UnityEngine.Networking;
//using UnityEngine.UI;
//using Newtonsoft.Json.Linq;

//public class WalletConnectManager : MonoBehaviour
//{
//    private ThirdwebManager thirdwebManager;
//    private IThirdwebWallet wallet;
//    private string walletAddress;
//    [field: SerializeField, Header("Wallet Options")]
//    private ulong ActiveChainId = 84532;

//    [field: SerializeField, Header("Send ETH amount")]
//    public string Amount { get; set; }
//    [field: SerializeField, Header("Send ETH address")]
//    public string ToAddress { get; set; }

//    [field: SerializeField, Header("Send Custom Token Options")]
//    public string TokenName { get; set; }
//    [field: SerializeField]
//    public string TokenContractAddress { get; set; }
//    [field: SerializeField]
//    public string TokenAmount { get; set; }
//    [field: SerializeField]
//    public string TokenRecipientAddress { get; set; }

//    [field: SerializeField, Header("Claim Token Options")]
//    public string ClaimTokenContractAddress { get; set; }
//    [field: SerializeField]
//    public string ClaimTokenAmount { get; set; }

//    [field: SerializeField, Header("Claim Nft Options")]
//    public string ClaimNftContractAddress { get; set; }
//    [field: SerializeField]
//    public string ClaimNftAmount { get; set; }

//    [field: SerializeField, Header("UI Elements")]
//    public GameManager GameManager { get; set; }
//    [field: SerializeField]
//    public Button ClaimButton { get; set; }
//    [field: SerializeField]
//    public GameObject ConnectButton { get; set; }
//    [field: SerializeField]
//    public GameObject DisconnectButton { get; set; }
//    [field: SerializeField]
//    public TextMeshProUGUI ConnectedText { get; set; }
//    [field: SerializeField]
//    public TextMeshProUGUI ClaimedNFTText { get; set; }
//    [field: SerializeField]
//    public TextMeshProUGUI AddressText { get; set; }
//    [field: SerializeField]
//    public TextMeshProUGUI EthBalanceText { get; set; }
//    [field: SerializeField]
//    public TextMeshProUGUI CustomTokenBalanceText { get; set; }
//    [field: SerializeField]
//    public TextMeshProUGUI ClaimedTokenBalanceText { get; set; }

//    [field: SerializeField, Header("NFT Display Canvas")]
//    public Canvas NftDisplayCanvas { get; set; }
//    [field: SerializeField]
//    public GameObject NftDisplayPrefab { get; set; }
//    [field: SerializeField]
//    public Transform NftDisplayParent { get; set; }

//    private List<GameObject> instantiatedNfts = new List<GameObject>();

//    void Awake()
//    {
//        thirdwebManager = FindObjectOfType<ThirdwebManager>();
//        if (thirdwebManager == null)
//        {
//            Debug.LogError("ThirdwebManager not found in the scene! Please add the ThirdwebManager prefab.");
//        }

//        if (GameManager == null)
//        {
//            GameManager = FindObjectOfType<GameManager>();
//            if (GameManager == null)
//            {
//                Debug.LogError("GameManager not found in the scene! Please add the GameManager component.");
//            }
//        }

//        if (ConnectButton != null) ConnectButton.SetActive(true);
//        if (DisconnectButton != null)
//        {
//            DisconnectButton.SetActive(false);
//            var buttonComponent = DisconnectButton.GetComponent<UnityEngine.UI.Button>();
//            if (buttonComponent != null)
//            {
//                buttonComponent.interactable = true;
//            }
//            else
//            {
//                Debug.LogError("DisconnectButton does not have a Button component!");
//            }
//        }
//        if (ClaimButton != null)
//        {
//            ClaimButton.interactable = true;
//        }
//        if (ConnectedText != null) ConnectedText.gameObject.SetActive(false);
//        if (AddressText != null) AddressText.gameObject.SetActive(false);
//        if (EthBalanceText != null) EthBalanceText.gameObject.SetActive(false);
//        if (CustomTokenBalanceText != null) CustomTokenBalanceText.gameObject.SetActive(false);
//        if (ClaimedTokenBalanceText != null) ClaimedTokenBalanceText.gameObject.SetActive(false);
//        if (ClaimedNFTText != null) ClaimedNFTText.gameObject.SetActive(false);
//        if (NftDisplayCanvas != null) NftDisplayCanvas.gameObject.SetActive(false);
//    }

//    public async void Connect()
//    {
//        if (thirdwebManager == null)
//        {
//            Debug.LogError("Cannot connect: ThirdwebManager is not initialized.");
//            return;
//        }

//        try
//        {
//            if (ConnectedText != null)
//            {
//                ConnectedText.gameObject.SetActive(true);
//                ConnectedText.text = "Connecting...";
//            }
//            if (DisconnectButton != null)
//            {
//                var disconnectButton = DisconnectButton.GetComponent<Button>();
//                if (disconnectButton != null)
//                {
//                    disconnectButton.interactable = false;
//                }
//            }

//            // Disconnect existing wallet if connected
//            if (wallet != null)
//            {
//                await wallet.Disconnect();
//                wallet = null;
//                walletAddress = null;
//                Debug.Log("Disconnected existing wallet to start new connection.");
//            }

//            var options = new WalletOptions(
//                provider: WalletProvider.WalletConnectWallet,
//                chainId: 84532
//            );

//            Debug.Log("Initiating wallet connection...");
//            wallet = await ThirdwebManager.Instance.ConnectWallet(options);
//            walletAddress = await wallet.GetAddress();
//            Debug.Log($"Wallet connected successfully! Address: {walletAddress}");

//            var balance = await wallet.GetBalance(chainId: ActiveChainId);
//            var balanceEth = Utils.ToEth(wei: balance.ToString(), decimalsToDisplay: 2, addCommas: true);
//            Debug.Log($"Wallet balance: {balanceEth}");
//            if (EthBalanceText != null)
//            {
//                EthBalanceText.gameObject.SetActive(true);
//                EthBalanceText.text = $"ETH: {balanceEth}";
//            }

//            if (!string.IsNullOrEmpty(TokenContractAddress))
//            {
//                var contract = await ThirdwebManager.Instance.GetContract(TokenContractAddress, ActiveChainId);
//                var decimals = 2;
//                var tokenBalance = await contract.ERC20_BalanceOf(walletAddress);
//                var tokenBalanceFormatted = Utils.ToEth(tokenBalance.ToString(), decimals, addCommas: true);
//                Debug.Log($"Custom token balance for {walletAddress}: {tokenBalanceFormatted}");
//                if (CustomTokenBalanceText != null)
//                {
//                    CustomTokenBalanceText.gameObject.SetActive(true);
//                    CustomTokenBalanceText.text = $"{TokenName}: {tokenBalanceFormatted}";
//                }
//            }

//            if (ConnectButton != null)
//            {
//                ConnectButton.SetActive(true);
//                var connectButton = ConnectButton.GetComponent<Button>();
//                if (connectButton != null)
//                {
//                    connectButton.interactable = true;
//                }
//            }
//            if (DisconnectButton != null)
//            {
//                DisconnectButton.SetActive(true);
//                var buttonComponent = DisconnectButton.GetComponent<Button>();
//                if (buttonComponent != null)
//                {
//                    buttonComponent.interactable = true;
//                }
//            }
//            if (ConnectedText != null)
//            {
//                ConnectedText.text = "Connected";
//            }
//            if (AddressText != null && !string.IsNullOrEmpty(walletAddress))
//            {
//                AddressText.gameObject.SetActive(true);
//                string shortAddress = $"{walletAddress.Substring(0, 3)}...{walletAddress.Substring(walletAddress.Length - 3)}";
//                AddressText.text = shortAddress;
//            }
//        }
//        catch (System.Exception ex)
//        {
//            Debug.LogWarning($"Wallet connection failed or canceled: {ex.Message}");
//            wallet = null;
//            walletAddress = null;

//            if (ConnectButton != null)
//            {
//                ConnectButton.SetActive(true);
//                var connectButton = ConnectButton.GetComponent<Button>();
//                if (connectButton != null)
//                {
//                    connectButton.interactable = true;
//                }
//            }
//            if (DisconnectButton != null)
//            {
//                DisconnectButton.SetActive(false);
//                var disconnectButton = DisconnectButton.GetComponent<Button>();
//                if (disconnectButton != null)
//                {
//                    disconnectButton.interactable = false;
//                }
//            }
//            if (ConnectedText != null)
//            {
//                ConnectedText.gameObject.SetActive(false);
//            }
//            if (AddressText != null) AddressText.gameObject.SetActive(false);
//            if (EthBalanceText != null) EthBalanceText.gameObject.SetActive(false);
//            if (CustomTokenBalanceText != null) CustomTokenBalanceText.gameObject.SetActive(false);
//            if (ClaimedTokenBalanceText != null) ClaimedTokenBalanceText.gameObject.SetActive(false);
//            if (ClaimedNFTText != null) ClaimedNFTText.gameObject.SetActive(false);
//            if (NftDisplayCanvas != null) NftDisplayCanvas.gameObject.SetActive(false);
//        }
//    }

//    public async void Disconnect()
//    {
//        if (wallet == null)
//        {
//            Debug.LogWarning("No wallet to disconnect.");
//            return;
//        }

//        try
//        {
//            Debug.Log("Disconnecting wallet...");
//            await wallet.Disconnect();
//            wallet = null;
//            walletAddress = null;

//            if (ConnectButton != null)
//            {
//                ConnectButton.SetActive(true);
//                var connectButton = ConnectButton.GetComponent<Button>();
//                if (connectButton != null) connectButton.interactable = true;
//            }
//            if (DisconnectButton != null) DisconnectButton.SetActive(false);
//            if (ClaimButton != null) ClaimButton.interactable = true;
//            if (ConnectedText != null) ConnectedText.gameObject.SetActive(false);
//            if (AddressText != null) AddressText.gameObject.SetActive(false);
//            if (EthBalanceText != null) EthBalanceText.gameObject.SetActive(false);
//            if (CustomTokenBalanceText != null) CustomTokenBalanceText.gameObject.SetActive(false);
//            if (ClaimedTokenBalanceText != null) ClaimedTokenBalanceText.gameObject.SetActive(false);
//            if (ClaimedNFTText != null) ClaimedNFTText.gameObject.SetActive(false);
//            if (NftDisplayCanvas != null) NftDisplayCanvas.gameObject.SetActive(false);
//            ClearNftDisplay();
//        }
//        catch (System.Exception ex)
//        {
//            Debug.LogError($"Failed to disconnect wallet: {ex.Message}");
//        }
//    }

//    public async void SendEth()
//    {
//        if (thirdwebManager == null || wallet == null)
//        {
//            Debug.LogError("Cannot send ETH: ThirdwebManager or wallet not initialized.");
//            return;
//        }

//        if (string.IsNullOrEmpty(ToAddress) || !ToAddress.StartsWith("0x") || ToAddress.Length != 42)
//        {
//            Debug.LogError("Invalid recipient address.");
//            return;
//        }

//        if (string.IsNullOrEmpty(Amount) || !float.TryParse(Amount, out float ethAmount) || ethAmount <= 0)
//        {
//            Debug.LogError("Invalid ETH amount.");
//            return;
//        }

//        try
//        {
//            Debug.Log($"Sending {Amount} ETH to {ToAddress}...");
//            if (wallet is WalletConnectWallet walletConnect)
//            {
//                await walletConnect.EnsureCorrectNetwork(ActiveChainId);
//            }
//            await Task.Delay(10000);
//            string weiAmountString = Utils.ToWei(Amount);
//            BigInteger weiAmount = BigInteger.Parse(weiAmountString);
//            var transactionResult = await wallet.Transfer(ActiveChainId, ToAddress, weiAmount);
//            Debug.Log($"ETH sent! Transaction Hash: {transactionResult.TransactionHash}");

//            var balance = await wallet.GetBalance(chainId: ActiveChainId);
//            var balanceEth = Utils.ToEth(wei: balance.ToString(), decimalsToDisplay: 2, addCommas: true);
//            if (EthBalanceText != null)
//            {
//                EthBalanceText.gameObject.SetActive(true);
//                EthBalanceText.text = $"ETH: {balanceEth}";
//            }
//        }
//        catch (System.Exception ex)
//        {
//            Debug.LogError($"Failed to send ETH: {ex.Message}");
//        }
//    }

//    public async void SendCustomToken()
//    {
//        if (thirdwebManager == null || wallet == null)
//        {
//            Debug.LogError("Cannot send token: ThirdwebManager or wallet not initialized.");
//            return;
//        }

//        if (string.IsNullOrEmpty(TokenContractAddress) || string.IsNullOrEmpty(TokenRecipientAddress))
//        {
//            Debug.LogError("Invalid token contract or recipient address.");
//            return;
//        }

//        if (string.IsNullOrEmpty(TokenAmount) || !float.TryParse(TokenAmount, out float tokenAmount) || tokenAmount <= 0)
//        {
//            Debug.LogError("Invalid token amount.");
//            return;
//        }

//        try
//        {
//            Debug.Log($"Sending {TokenAmount} {TokenName} to {TokenRecipientAddress}...");
//            if (wallet is WalletConnectWallet walletConnect)
//            {
//                await walletConnect.EnsureCorrectNetwork(ActiveChainId);
//            }
//            await Task.Delay(10000);
//            var contract = await ThirdwebManager.Instance.GetContract(TokenContractAddress, ActiveChainId);
//            var decimals = 2;
//            string tokenAmountInWei = Utils.ToWei(TokenAmount);
//            BigInteger tokenAmountBigInt = BigInteger.Parse(tokenAmountInWei);
//            var transactionResult = await contract.ERC20_Transfer(wallet, TokenRecipientAddress, tokenAmountBigInt);
//            Debug.Log($"Token sent! Transaction Hash: {transactionResult.TransactionHash}");

//            var tokenBalance = await contract.ERC20_BalanceOf(walletAddress);
//            var tokenBalanceFormatted = Utils.ToEth(tokenBalance.ToString(), decimals, addCommas: true);
//            if (CustomTokenBalanceText != null)
//            {
//                CustomTokenBalanceText.gameObject.SetActive(true);
//                CustomTokenBalanceText.text = $"{TokenName} : {tokenBalanceFormatted}";
//            }
//        }
//        catch (System.Exception ex)
//        {
//            Debug.LogError($"Failed to send {TokenName}: {ex.Message}");
//        }
//    }

//    public async void ClaimToken()
//    {
//        if (thirdwebManager == null || wallet == null)
//        {
//            Debug.LogError("Cannot claim token: ThirdwebManager or wallet not initialized.");
//            return;
//        }

//        if (string.IsNullOrEmpty(ClaimTokenContractAddress) || !ClaimTokenContractAddress.StartsWith("0x") || ClaimTokenContractAddress.Length != 42)
//        {
//            Debug.LogError("Invalid claim token contract address.");
//            return;
//        }

//        if (GameManager == null)
//        {
//            Debug.LogError("GameManager not assigned. Cannot get total XP.");
//            return;
//        }

//        try
//        {
//            if (ClaimButton != null) ClaimButton.interactable = false;
//            if (ClaimedTokenBalanceText != null)
//            {
//                ClaimedTokenBalanceText.gameObject.SetActive(true);
//                ClaimedTokenBalanceText.text = "Claiming...";
//            }

//            float totalXP = GameManager.GetTotalXP();
//            decimal tokenAmount = (decimal)totalXP * 100;
//            ClaimTokenAmount = tokenAmount.ToString();
//            string claimAmountInWei = Utils.ToWei(ClaimTokenAmount);
//            Debug.Log($"Claiming {tokenAmount} tokens ({claimAmountInWei} wei) based on {totalXP} XP");

//            if (wallet is WalletConnectWallet walletConnect)
//            {
//                await walletConnect.EnsureCorrectNetwork(ActiveChainId);
//            }
//            await Task.Delay(10000);

//            var contract = await ThirdwebManager.Instance.GetContract(ClaimTokenContractAddress, ActiveChainId);
//            var decimals = 2;
//            var transactionResult = await contract.DropERC20_Claim(wallet, walletAddress, claimAmountInWei);
//            Debug.Log($"Tokens claimed successfully! Transaction Hash: {transactionResult.TransactionHash}");
//            await Task.Delay(10000);

//            var tokenBalance = await contract.ERC20_BalanceOf(walletAddress);
//            var tokenBalanceFormatted = Utils.ToEth(tokenBalance.ToString(), decimals, addCommas: true);
//            Debug.Log($"Updated token balance for {walletAddress}: {tokenBalanceFormatted}");
//            if (ClaimedTokenBalanceText != null)
//            {
//                ClaimedTokenBalanceText.text = $"Claimed: {tokenBalanceFormatted} CYS";
//            }
//        }
//        catch (System.Exception ex)
//        {
//            Debug.LogError($"Failed to claim tokens: {ex.Message}");
//            if (ClaimedTokenBalanceText != null)
//            {
//                ClaimedTokenBalanceText.text = $"Claim Failed: {ex.Message}";
//            }
//            if (ClaimButton != null) ClaimButton.interactable = true;
//        }
//    }

//    public async void ConnectWithEcosystem()
//    {
//        if (thirdwebManager == null)
//        {
//            Debug.LogError("Cannot connect: ThirdwebManager is not initialized.");
//            return;
//        }

//        try
//        {
//            if (ConnectedText != null)
//            {
//                ConnectedText.gameObject.SetActive(true);
//                ConnectedText.text = "Connecting...";
//            }
//            if (DisconnectButton != null)
//            {
//                var disconnectButton = DisconnectButton.GetComponent<Button>();
//                if (disconnectButton != null)
//                {
//                    disconnectButton.interactable = false;
//                }
//            }

//            // Disconnect existing wallet if connected
//            if (wallet != null)
//            {
//                await wallet.Disconnect();
//                wallet = null;
//                walletAddress = null;
//                Debug.Log("Disconnected existing wallet to start new connection.");
//            }

//            var ecosystemWalletOptions = new EcosystemWalletOptions(ecosystemId: "ecosystem.your-ecosystem", email: "myepicemail@domain.id");
//            var options = new WalletOptions(
//                provider: WalletProvider.EcosystemWallet,
//                chainId: 84532,
//                ecosystemWalletOptions: ecosystemWalletOptions
//            );
//            wallet = await ThirdwebManager.Instance.ConnectWallet(options);
//            walletAddress = await wallet.GetAddress();
//            Debug.Log($"Wallet connected successfully! Address: {walletAddress}");

//            var balance = await wallet.GetBalance(chainId: ActiveChainId);
//            var balanceEth = Utils.ToEth(wei: balance.ToString(), decimalsToDisplay: 2, addCommas: true);
//            Debug.Log($"Wallet balance: {balanceEth}");
//            if (EthBalanceText != null)
//            {
//                EthBalanceText.gameObject.SetActive(true);
//                EthBalanceText.text = $"ETH: {balanceEth}";
//            }

//            if (!string.IsNullOrEmpty(TokenContractAddress))
//            {
//                var contract = await ThirdwebManager.Instance.GetContract(TokenContractAddress, ActiveChainId);
//                var decimals = 2;
//                var tokenBalance = await contract.ERC20_BalanceOf(walletAddress);
//                var tokenBalanceFormatted = Utils.ToEth(tokenBalance.ToString(), decimals, addCommas: true);
//                Debug.Log($"Custom token balance for {walletAddress}: {tokenBalanceFormatted}");
//                if (CustomTokenBalanceText != null)
//                {
//                    CustomTokenBalanceText.gameObject.SetActive(true);
//                    CustomTokenBalanceText.text = $"{TokenName}: {tokenBalanceFormatted}";
//                }
//            }

//            if (ConnectButton != null)
//            {
//                ConnectButton.SetActive(true);
//                var connectButton = ConnectButton.GetComponent<Button>();
//                if (connectButton != null)
//                {
//                    connectButton.interactable = true;
//                }
//            }
//            if (DisconnectButton != null)
//            {
//                DisconnectButton.SetActive(true);
//                var buttonComponent = DisconnectButton.GetComponent<Button>();
//                if (buttonComponent != null)
//                {
//                    buttonComponent.interactable = true;
//                }
//            }
//            if (ConnectedText != null)
//            {
//                ConnectedText.text = "Connected";
//            }
//            if (AddressText != null && !string.IsNullOrEmpty(walletAddress))
//            {
//                AddressText.gameObject.SetActive(true);
//                string shortAddress = $"{walletAddress.Substring(0, 3)}...{walletAddress.Substring(walletAddress.Length - 3)}";
//                AddressText.text = shortAddress;
//            }
//        }
//        catch (System.Exception ex)
//        {
//            Debug.LogWarning($"Wallet connection failed or canceled: {ex.Message}");
//            wallet = null;
//            walletAddress = null;

//            if (ConnectButton != null)
//            {
//                ConnectButton.SetActive(true);
//                var connectButton = ConnectButton.GetComponent<Button>();
//                if (connectButton != null)
//                {
//                    connectButton.interactable = true;
//                }
//            }
//            if (DisconnectButton != null)
//            {
//                DisconnectButton.SetActive(false);
//                var disconnectButton = DisconnectButton.GetComponent<Button>();
//                if (disconnectButton != null)
//                {
//                    disconnectButton.interactable = false;
//                }
//            }
//            if (ConnectedText != null)
//            {
//                ConnectedText.gameObject.SetActive(false);
//            }
//            if (AddressText != null) AddressText.gameObject.SetActive(false);
//            if (EthBalanceText != null) EthBalanceText.gameObject.SetActive(false);
//            if (CustomTokenBalanceText != null) CustomTokenBalanceText.gameObject.SetActive(false);
//            if (ClaimedTokenBalanceText != null) ClaimedTokenBalanceText.gameObject.SetActive(false);
//            if (ClaimedNFTText != null) ClaimedNFTText.gameObject.SetActive(false);
//            if (NftDisplayCanvas != null) NftDisplayCanvas.gameObject.SetActive(false);
//        }
//    }

//    public async void ClaimNft()
//    {
//        if (thirdwebManager == null || wallet == null)
//        {
//            Debug.LogError("Cannot claim NFT: ThirdwebManager or wallet not initialized.");
//            return;
//        }

//        if (string.IsNullOrEmpty(ClaimNftContractAddress) || !ClaimNftContractAddress.StartsWith("0x") || ClaimNftContractAddress.Length != 42)
//        {
//            Debug.LogError("Invalid claim NFT contract address.");
//            return;
//        }

//        if (string.IsNullOrEmpty(ClaimNftAmount) || !int.TryParse(ClaimNftAmount, out int claimAmount) || claimAmount <= 0)
//        {
//            Debug.LogError("Invalid claim amount.");
//            return;
//        }

//        if (NftDisplayCanvas == null || NftDisplayPrefab == null)
//        {
//            Debug.LogError("NFT display canvas or prefab not assigned.");
//            return;
//        }

//        Transform parentTransform = NftDisplayParent != null ? NftDisplayParent : NftDisplayCanvas.transform;

//        try
//        {
//            if (ClaimedNFTText != null)
//            {
//                ClaimedNFTText.gameObject.SetActive(true);
//                ClaimedNFTText.text = "Claiming...";
//            }

//            if (wallet is WalletConnectWallet walletConnect)
//            {
//                await walletConnect.EnsureCorrectNetwork(ActiveChainId);
//            }
//            await Task.Delay(20000);

//            var contract = await ThirdwebManager.Instance.GetContract(ClaimNftContractAddress, ActiveChainId);
//            var transactionResult = await contract.DropERC721_Claim(wallet, walletAddress, claimAmount);
//            Debug.Log($"NFTs claimed! Transaction Hash: {transactionResult.TransactionHash}");

//            BigInteger tokenBalance = 0;
//            int maxAttempts = 5;
//            for (int attempt = 0; attempt < maxAttempts; attempt++)
//            {
//                tokenBalance = await contract.ERC721_BalanceOf(walletAddress);
//                Debug.Log($"Balance check attempt {attempt + 1}/{maxAttempts}: {tokenBalance} NFTs");
//                if (tokenBalance >= claimAmount) break;
//                await Task.Delay(5000);
//            }

//            if (tokenBalance == 0)
//            {
//                Debug.LogError($"No NFTs owned by {walletAddress}.");
//                if (ClaimedNFTText != null)
//                {
//                    ClaimedNFTText.text = $"No NFTs owned. Tx Hash: {transactionResult.TransactionHash}";
//                }
//                return;
//            }

//            if (ClaimedNFTText != null)
//            {
//                ClaimedNFTText.text = $"Claimed! Tx Hash: {transactionResult.TransactionHash}\nBalance: {tokenBalance}";
//            }

//            ClearNftDisplay();

//            List<string> tokenIds = new List<string>();
//            try
//            {
//                var ownedNfts = await contract.ERC1155_GetOwnedNFTs(walletAddress);
//                foreach (var nft in ownedNfts)
//                {
//                    tokenIds.Add(nft.Metadata.Id.ToString());
//                    Debug.Log($"Fetched token ID: {nft.Metadata.Id}");
//                }
//            }
//            catch (System.Exception ex)
//            {
//                Debug.LogWarning($"GetOwnedNFTs failed: {ex.Message}. Using tokenOfOwnerByIndex.");
//                for (int i = 0; i < tokenBalance; i++)
//                {
//                    try
//                    {
//                        BigInteger index = new BigInteger(i);
//                        var tokenId = await contract.Read<BigInteger>("tokenOfOwnerByIndex", walletAddress, index);
//                        tokenIds.Add(tokenId.ToString());
//                        Debug.Log($"Fetched token ID at index {i}: {tokenId}");
//                    }
//                    catch (System.Exception ex2)
//                    {
//                        Debug.LogWarning($"Failed to fetch token ID at index {i}: {ex2.Message}");
//                    }
//                }
//            }

//            if (tokenIds.Count == 0 && tokenBalance > 0)
//            {
//                Debug.LogWarning("No token IDs fetched. Assuming sequential IDs.");
//                BigInteger totalSupply;
//                try
//                {
//                    totalSupply = await contract.Read<BigInteger>("totalSupply");
//                }
//                catch
//                {
//                    totalSupply = tokenBalance;
//                }
//                BigInteger startId = totalSupply - claimAmount;
//                if (startId < 0) startId = 0;
//                for (int i = 0; i < claimAmount; i++)
//                {
//                    BigInteger assumedId = startId + i;
//                    tokenIds.Add(assumedId.ToString());
//                    Debug.Log($"Assumed token ID: {assumedId}");
//                }
//            }

//            if (tokenIds.Count == 0)
//            {
//                Debug.LogError("No token IDs available.");
//                if (ClaimedNFTText != null)
//                {
//                    ClaimedNFTText.text = $"No NFTs found for {walletAddress}. Balance: {tokenBalance}";
//                }
//                return;
//            }

//            if (NftDisplayCanvas != null)
//            {
//                NftDisplayCanvas.gameObject.SetActive(true);
//                NftDisplayCanvas.enabled = true;
//            }

//            foreach (var tokenId in tokenIds)
//            {
//                try
//                {
//                    BigInteger tokenIdBigInt = BigInteger.Parse(tokenId);
//                    var nft = await contract.ERC721_GetNFT(tokenIdBigInt);
//                    string name = nft.Metadata.Name ?? $"NFT #{tokenId}";
//                    string description = nft.Metadata.Description ?? "No description";
//                    string tokenUri = nft.Metadata.Uri ?? "";

//                    string imageUrl = await GetImageUrlFromTokenUri(tokenUri);
//                    Texture2D imageTexture = null;
//                    if (!string.IsNullOrEmpty(imageUrl))
//                    {
//                        imageTexture = await LoadImage(imageUrl);
//                    }
//                    else
//                    {
//                        imageUrl = "https://via.placeholder.com/150";
//                        imageTexture = await LoadImage(imageUrl);
//                    }

//                    GameObject nftDisplay = Instantiate(NftDisplayPrefab, parentTransform);
//                    nftDisplay.SetActive(true);
//                    instantiatedNfts.Add(nftDisplay);

//                    TextMeshProUGUI[] textComponents = nftDisplay.GetComponentsInChildren<TextMeshProUGUI>(true);
//                    RawImage imageRaw = nftDisplay.GetComponentInChildren<RawImage>(true);

//                    TextMeshProUGUI nameText = null;
//                    TextMeshProUGUI descText = null;
//                    TextMeshProUGUI tokenIdText = null;

//                    foreach (var text in textComponents)
//                    {
//                        string textName = text.gameObject.name.ToLower();
//                        if (textName.Contains("name")) nameText = text;
//                        else if (textName.Contains("description") || textName.Contains("desc")) descText = text;
//                        else if (textName.Contains("tokenid") || textName.Contains("token")) tokenIdText = text;
//                    }

//                    if (nameText != null)
//                    {
//                        nameText.text = $"Name: {name}";
//                        nameText.color = new Color(1, 1, 1, 1);
//                    }
//                    if (descText != null)
//                    {
//                        descText.text = $"Description: {description}";
//                        descText.color = new Color(1, 1, 1, 1);
//                    }
//                    if (tokenIdText != null)
//                    {
//                        tokenIdText.text = $"Token ID: {tokenId}";
//                        tokenIdText.color = new Color(1, 1, 1, 1);
//                    }
//                    if (imageRaw != null)
//                    {
//                        imageRaw.color = Color.white;
//                        imageRaw.texture = imageTexture ?? Texture2D.grayTexture;
//                    }

//                    Debug.Log($"Displayed NFT: TokenID={tokenId}, Name={name}, Description={description}, ImageURL={imageUrl ?? "null"}");
//                }
//                catch (System.Exception ex)
//                {
//                    Debug.LogError($"Failed to process NFT with TokenID {tokenId}: {ex.Message}");
//                }
//            }

//            if (ClaimedNFTText != null)
//            {
//                ClaimedNFTText.gameObject.SetActive(true);
//                ClaimedNFTText.color = new Color(1, 1, 1, 1);
//                ClaimedNFTText.text = $"NFT Balance: {tokenBalance}\nTx Hash: {transactionResult.TransactionHash}";
//            }
//        }
//        catch (System.Exception ex)
//        {
//            Debug.LogError($"Failed to claim NFTs: {ex.Message}");
//            if (ClaimedNFTText != null)
//            {
//                ClaimedNFTText.text = $"Failed to claim: {ex.Message}";
//            }
//            if (NftDisplayCanvas != null) NftDisplayCanvas.gameObject.SetActive(false);
//        }
//    }

//    private async Task<string> GetImageUrlFromTokenUri(string tokenUri)
//    {
//        if (string.IsNullOrEmpty(tokenUri))
//        {
//            Debug.LogWarning("Token URI is empty or null.");
//            return null;
//        }

//        Debug.Log($"Fetching metadata from token URI: {tokenUri}");
//        if (tokenUri.StartsWith("ipfs://"))
//        {
//            tokenUri = tokenUri.Replace("ipfs://", "https://cloudflare-ipfs.com/ipfs/");
//            Debug.Log($"Converted IPFS URI to: {tokenUri}");
//        }

//        try
//        {
//            UnityWebRequest request = UnityWebRequest.Get(tokenUri);
//            request.timeout = 10;
//            var operation = request.SendWebRequest();
//            while (!operation.isDone) await Task.Yield();

//            if (request.result != UnityWebRequest.Result.Success)
//            {
//                Debug.LogError($"Failed to fetch metadata from {tokenUri}: {request.error} (Response Code: {request.responseCode})");
//                return null;
//            }

//            string json = request.downloadHandler.text;
//            Debug.Log($"Metadata JSON: {json}");
//            JObject metadata = JObject.Parse(json);
//            string imageUrl = metadata["image"]?.ToString() ?? metadata["image_url"]?.ToString();
//            if (string.IsNullOrEmpty(imageUrl))
//            {
//                Debug.LogWarning($"No 'image' or 'image_url' field found in metadata: {json}");
//                return null;
//            }

//            Debug.Log($"Parsed image URL: {imageUrl}");
//            if (imageUrl.StartsWith("ipfs://"))
//            {
//                imageUrl = imageUrl.Replace("ipfs://", "https://cloudflare-ipfs.com/ipfs/");
//                Debug.Log($"Converted IPFS image URL to: {imageUrl}");
//            }

//            return imageUrl;
//        }
//        catch (System.Exception ex)
//        {
//            Debug.LogError($"Failed to parse metadata from {tokenUri}: {ex.Message}");
//            return null;
//        }
//    }

//    private async Task<Texture2D> LoadImage(string imageUrl)
//    {
//        if (string.IsNullOrEmpty(imageUrl))
//        {
//            Debug.LogWarning("Image URL is empty or null.");
//            return null;
//        }

//        Debug.Log($"Loading image from: {imageUrl}");
//        try
//        {
//            UnityWebRequest request = UnityWebRequestTexture.GetTexture(imageUrl);
//            request.timeout = 10;
//            var operation = request.SendWebRequest();
//            while (!operation.isDone) await Task.Yield();

//            if (request.result != UnityWebRequest.Result.Success)
//            {
//                Debug.LogError($"Failed to load image from {imageUrl}: {request.error} (Response Code: {request.responseCode})");
//                return null;
//            }

//            Texture2D texture = DownloadHandlerTexture.GetContent(request);
//            Debug.Log($"Image loaded successfully: {imageUrl} (Size: {texture.width}x{texture.height})");
//            return texture;
//        }
//        catch (System.Exception ex)
//        {
//            Debug.LogError($"Failed to load image from {imageUrl}: {ex.Message}");
//            return null;
//        }
//    }

//    private void ClearNftDisplay()
//    {
//        foreach (var nftDisplay in instantiatedNfts)
//        {
//            if (nftDisplay != null) Destroy(nftDisplay);
//        }
//        instantiatedNfts.Clear();
//        Debug.Log("Cleared NFT display.");
//    }
//}





//using UnityEngine;
//using Thirdweb;
//using System.Threading.Tasks;
//using Thirdweb.Unity;
//using System.Numerics;
//using TMPro;
//using System.Text;
//using System.Collections.Generic;
//using UnityEngine.Networking;
//using UnityEngine.UI;
//using Newtonsoft.Json.Linq;

//public class WalletConnectManager : MonoBehaviour
//{
//    private ThirdwebManager thirdwebManager;
//    private IThirdwebWallet wallet;
//    private string walletAddress;
//    [field: SerializeField, Header("Wallet Options")]
//    private ulong ActiveChainId = 84532;

//    [field: SerializeField, Header("Send ETH amount")]
//    public string Amount { get; set; }
//    [field: SerializeField, Header("Send ETH address")]
//    public string ToAddress { get; set; }

//    [field: SerializeField, Header("Send Custom Token Options")]
//    public string TokenName { get; set; }
//    [field: SerializeField]
//    public string TokenContractAddress { get; set; }
//    [field: SerializeField]
//    public string TokenAmount { get; set; }
//    [field: SerializeField]
//    public string TokenRecipientAddress { get; set; }

//    [field: SerializeField, Header("Claim Token Options")]
//    public string ClaimTokenContractAddress { get; set; }
//    [field: SerializeField]
//    public string ClaimTokenAmount { get; set; }

//    [field: SerializeField, Header("Claim Nft Options")]
//    public string ClaimNftContractAddress { get; set; }
//    [field: SerializeField]
//    public string ClaimNftAmount { get; set; }

//    [field: SerializeField, Header("UI Elements")]
//    public GameManager GameManager { get; set; }
//    [field: SerializeField]
//    public Button ClaimButton { get; set; }
//    [field: SerializeField]
//    public GameObject ConnectButton { get; set; }
//    [field: SerializeField]
//    public GameObject DisconnectButton { get; set; }
//    [field: SerializeField]
//    public TextMeshProUGUI ConnectedText { get; set; }
//    [field: SerializeField]
//    public TextMeshProUGUI ClaimedNFTText { get; set; }
//    [field: SerializeField]
//    public TextMeshProUGUI AddressText { get; set; }
//    [field: SerializeField]
//    public TextMeshProUGUI EthBalanceText { get; set; }
//    [field: SerializeField]
//    public TextMeshProUGUI CustomTokenBalanceText { get; set; }
//    [field: SerializeField]
//    public TextMeshProUGUI ClaimedTokenBalanceText { get; set; }

//    [field: SerializeField, Header("NFT Display Canvas")]
//    public Canvas NftDisplayCanvas { get; set; }
//    [field: SerializeField]
//    public GameObject NftDisplayPrefab { get; set; }
//    [field: SerializeField]
//    public Transform NftDisplayParent { get; set; }

//    private List<GameObject> instantiatedNfts = new List<GameObject>();

//    void Awake()
//    {
//        thirdwebManager = FindObjectOfType<ThirdwebManager>();
//        if (thirdwebManager == null)
//        {
//            Debug.LogError("ThirdwebManager not found in the scene! Please add the ThirdwebManager prefab.");
//        }

//        if (GameManager == null)
//        {
//            GameManager = FindObjectOfType<GameManager>();
//            if (GameManager == null)
//            {
//                Debug.LogError("GameManager not found in the scene! Please add the GameManager component.");
//            }
//        }

//        if (ConnectButton != null) ConnectButton.SetActive(true);
//        if (DisconnectButton != null)
//        {
//            DisconnectButton.SetActive(false);
//            var buttonComponent = DisconnectButton.GetComponent<UnityEngine.UI.Button>();
//            if (buttonComponent != null)
//            {
//                buttonComponent.interactable = true;
//            }
//            else
//            {
//                Debug.LogError("DisconnectButton does not have a Button component!");
//            }
//        }
//        if (ClaimButton != null)
//        {
//            ClaimButton.interactable = true;
//        }
//        if (ConnectedText != null) ConnectedText.gameObject.SetActive(false);
//        if (AddressText != null) AddressText.gameObject.SetActive(false);
//        if (EthBalanceText != null) EthBalanceText.gameObject.SetActive(false);
//        if (CustomTokenBalanceText != null) CustomTokenBalanceText.gameObject.SetActive(false);
//        if (ClaimedTokenBalanceText != null) ClaimedTokenBalanceText.gameObject.SetActive(false);
//        if (ClaimedNFTText != null) ClaimedNFTText.gameObject.SetActive(false);
//        if (NftDisplayCanvas != null) NftDisplayCanvas.gameObject.SetActive(false);
//    }

//    public async void Connect()
//    {
//        if (thirdwebManager == null)
//        {
//            Debug.LogError("Cannot connect: ThirdwebManager is not initialized.");
//            return;
//        }

//        try
//        {
//            if (ConnectedText != null)
//            {
//                ConnectedText.gameObject.SetActive(true);
//                ConnectedText.text = "Connecting...";
//            }
//            if (DisconnectButton != null)
//            {
//                var disconnectButton = DisconnectButton.GetComponent<Button>();
//                if (disconnectButton != null)
//                {
//                    disconnectButton.interactable = false;
//                }
//            }

//            // Disconnect existing wallet if connected
//            if (wallet != null)
//            {
//                await wallet.Disconnect();
//                wallet = null;
//                walletAddress = null;
//                Debug.Log("Disconnected existing wallet to start new connection.");
//            }

//            var options = new WalletOptions(
//                provider: WalletProvider.WalletConnectWallet,
//                chainId: 84532
//            );

//            Debug.Log("Initiating wallet connection...");
//            wallet = await ThirdwebManager.Instance.ConnectWallet(options);
//            walletAddress = await wallet.GetAddress();
//            Debug.Log($"Wallet connected successfully! Address: {walletAddress}");

//            var balance = await wallet.GetBalance(chainId: ActiveChainId);
//            var balanceEth = Utils.ToEth(wei: balance.ToString(), decimalsToDisplay: 2, addCommas: true);
//            Debug.Log($"Wallet balance: {balanceEth}");
//            if (EthBalanceText != null)
//            {
//                EthBalanceText.gameObject.SetActive(true);
//                EthBalanceText.text = $"ETH: {balanceEth}";
//            }

//            if (!string.IsNullOrEmpty(TokenContractAddress))
//            {
//                var contract = await ThirdwebManager.Instance.GetContract(TokenContractAddress, ActiveChainId);
//                var decimals = 2;
//                var tokenBalance = await contract.ERC20_BalanceOf(walletAddress);
//                var tokenBalanceFormatted = Utils.ToEth(tokenBalance.ToString(), decimals, addCommas: true);
//                Debug.Log($"Custom token balance for {walletAddress}: {tokenBalanceFormatted}");
//                if (CustomTokenBalanceText != null)
//                {
//                    CustomTokenBalanceText.gameObject.SetActive(true);
//                    CustomTokenBalanceText.text = $"{TokenName}: {tokenBalanceFormatted}";
//                }
//            }

//            if (ConnectButton != null)
//            {
//                ConnectButton.SetActive(false);
//                var connectButton = ConnectButton.GetComponent<Button>();
//                if (connectButton != null)
//                {
//                    connectButton.interactable = true;
//                }
//            }
//            if (DisconnectButton != null)
//            {
//                DisconnectButton.SetActive(true);
//                var buttonComponent = DisconnectButton.GetComponent<Button>();
//                if (buttonComponent != null)
//                {
//                    buttonComponent.interactable = true;
//                }
//            }
//            if (ConnectedText != null)
//            {
//                ConnectedText.text = "Connected";
//            }
//            if (AddressText != null && !string.IsNullOrEmpty(walletAddress))
//            {
//                AddressText.gameObject.SetActive(true);
//                string shortAddress = $"{walletAddress.Substring(0, 3)}...{walletAddress.Substring(walletAddress.Length - 3)}";
//                AddressText.text = shortAddress;
//            }
//        }
//        catch (System.Exception ex)
//        {
//            Debug.LogWarning($"Wallet connection failed or canceled: {ex.Message}");
//            wallet = null;
//            walletAddress = null;

//            if (ConnectButton != null)
//            {
//                ConnectButton.SetActive(true);
//                var connectButton = ConnectButton.GetComponent<Button>();
//                if (connectButton != null)
//                {
//                    connectButton.interactable = true;
//                }
//            }
//            if (DisconnectButton != null)
//            {
//                DisconnectButton.SetActive(false);
//                var disconnectButton = DisconnectButton.GetComponent<Button>();
//                if (disconnectButton != null)
//                {
//                    disconnectButton.interactable = false;
//                }
//            }
//            if (ConnectedText != null)
//            {
//                ConnectedText.gameObject.SetActive(false);
//            }
//            if (AddressText != null) AddressText.gameObject.SetActive(false);
//            if (EthBalanceText != null) EthBalanceText.gameObject.SetActive(false);
//            if (CustomTokenBalanceText != null) CustomTokenBalanceText.gameObject.SetActive(false);
//            if (ClaimedTokenBalanceText != null) ClaimedTokenBalanceText.gameObject.SetActive(false);
//            if (ClaimedNFTText != null) ClaimedNFTText.gameObject.SetActive(false);
//            if (NftDisplayCanvas != null) NftDisplayCanvas.gameObject.SetActive(false);
//        }
//    }

//    public async void Disconnect()
//    {
//        if (wallet == null)
//        {
//            Debug.LogWarning("No wallet to disconnect.");
//            return;
//        }

//        try
//        {
//            Debug.Log("Disconnecting wallet...");
//            await wallet.Disconnect();
//            wallet = null;
//            walletAddress = null;

//            if (ConnectButton != null)
//            {
//                ConnectButton.SetActive(true);
//                var connectButton = ConnectButton.GetComponent<Button>();
//                if (connectButton != null) connectButton.interactable = true;
//            }
//            if (DisconnectButton != null) DisconnectButton.SetActive(false);
//            if (ClaimButton != null) ClaimButton.interactable = true;
//            if (ConnectedText != null) ConnectedText.gameObject.SetActive(false);
//            if (AddressText != null) AddressText.gameObject.SetActive(false);
//            if (EthBalanceText != null) EthBalanceText.gameObject.SetActive(false);
//            if (CustomTokenBalanceText != null) CustomTokenBalanceText.gameObject.SetActive(false);
//            if (ClaimedTokenBalanceText != null) ClaimedTokenBalanceText.gameObject.SetActive(false);
//            if (ClaimedNFTText != null) ClaimedNFTText.gameObject.SetActive(false);
//            if (NftDisplayCanvas != null) NftDisplayCanvas.gameObject.SetActive(false);
//            ClearNftDisplay();
//        }
//        catch (System.Exception ex)
//        {
//            Debug.LogError($"Failed to disconnect wallet: {ex.Message}");
//        }
//    }

//    public async void SendEth()
//    {
//        if (thirdwebManager == null || wallet == null)
//        {
//            Debug.LogError("Cannot send ETH: ThirdwebManager or wallet not initialized.");
//            return;
//        }

//        if (string.IsNullOrEmpty(ToAddress) || !ToAddress.StartsWith("0x") || ToAddress.Length != 42)
//        {
//            Debug.LogError("Invalid recipient address.");
//            return;
//        }

//        if (string.IsNullOrEmpty(Amount) || !float.TryParse(Amount, out float ethAmount) || ethAmount <= 0)
//        {
//            Debug.LogError("Invalid ETH amount.");
//            return;
//        }

//        try
//        {
//            Debug.Log($"Sending {Amount} ETH to {ToAddress}...");
//            if (wallet is WalletConnectWallet walletConnect)
//            {
//                await walletConnect.EnsureCorrectNetwork(ActiveChainId);
//            }
//            await Task.Delay(10000);
//            string weiAmountString = Utils.ToWei(Amount);
//            BigInteger weiAmount = BigInteger.Parse(weiAmountString);
//            var transactionResult = await wallet.Transfer(ActiveChainId, ToAddress, weiAmount);
//            Debug.Log($"ETH sent! Transaction Hash: {transactionResult.TransactionHash}");

//            var balance = await wallet.GetBalance(chainId: ActiveChainId);
//            var balanceEth = Utils.ToEth(wei: balance.ToString(), decimalsToDisplay: 2, addCommas: true);
//            if (EthBalanceText != null)
//            {
//                EthBalanceText.gameObject.SetActive(true);
//                EthBalanceText.text = $"ETH: {balanceEth}";
//            }
//        }
//        catch (System.Exception ex)
//        {
//            Debug.LogError($"Failed to send ETH: {ex.Message}");
//        }
//    }

//    public async void SendCustomToken()
//    {
//        if (thirdwebManager == null || wallet == null)
//        {
//            Debug.LogError("Cannot send token: ThirdwebManager or wallet not initialized.");
//            return;
//        }

//        if (string.IsNullOrEmpty(TokenContractAddress) || string.IsNullOrEmpty(TokenRecipientAddress))
//        {
//            Debug.LogError("Invalid token contract or recipient address.");
//            return;
//        }

//        if (string.IsNullOrEmpty(TokenAmount) || !float.TryParse(TokenAmount, out float tokenAmount) || tokenAmount <= 0)
//        {
//            Debug.LogError("Invalid token amount.");
//            return;
//        }

//        try
//        {
//            Debug.Log($"Sending {TokenAmount} {TokenName} to {TokenRecipientAddress}...");
//            if (wallet is WalletConnectWallet walletConnect)
//            {
//                await walletConnect.EnsureCorrectNetwork(ActiveChainId);
//            }
//            await Task.Delay(10000);
//            var contract = await ThirdwebManager.Instance.GetContract(TokenContractAddress, ActiveChainId);
//            var decimals = 2;
//            string tokenAmountInWei = Utils.ToWei(TokenAmount);
//            BigInteger tokenAmountBigInt = BigInteger.Parse(tokenAmountInWei);
//            var transactionResult = await contract.ERC20_Transfer(wallet, TokenRecipientAddress, tokenAmountBigInt);
//            Debug.Log($"Token sent! Transaction Hash: {transactionResult.TransactionHash}");

//            var tokenBalance = await contract.ERC20_BalanceOf(walletAddress);
//            var tokenBalanceFormatted = Utils.ToEth(tokenBalance.ToString(), decimals, addCommas: true);
//            if (CustomTokenBalanceText != null)
//            {
//                CustomTokenBalanceText.gameObject.SetActive(true);
//                CustomTokenBalanceText.text = $"{TokenName}: {tokenBalanceFormatted}";
//            }
//        }
//        catch (System.Exception ex)
//        {
//            Debug.LogError($"Failed to send {TokenName}: {ex.Message}");
//        }
//    }

//    public async void ClaimToken()
//    {
//        if (thirdwebManager == null || wallet == null)
//        {
//            Debug.LogError("Cannot claim token: ThirdwebManager or wallet not initialized.");
//            return;
//        }

//        if (string.IsNullOrEmpty(ClaimTokenContractAddress) || !ClaimTokenContractAddress.StartsWith("0x") || ClaimTokenContractAddress.Length != 42)
//        {
//            Debug.LogError("Invalid claim token contract address.");
//            return;
//        }

//        if (GameManager == null)
//        {
//            Debug.LogError("GameManager not assigned. Cannot get total XP.");
//            return;
//        }

//        try
//        {
//            if (ClaimButton != null) ClaimButton.interactable = false;
//            if (ClaimedTokenBalanceText != null)
//            {
//                ClaimedTokenBalanceText.gameObject.SetActive(true);
//                ClaimedTokenBalanceText.text = "Claiming...";
//            }

//            float totalXP = GameManager.GetTotalXP();
//            decimal tokenAmount = (decimal)totalXP;
//            ClaimTokenAmount = tokenAmount.ToString();
//            var contract = await ThirdwebManager.Instance.GetContract(ClaimTokenContractAddress, ActiveChainId);
//            var decimals = 2;
//            string claimAmountInWei = Utils.ToWei(tokenAmount.ToString());
//            Debug.Log($"Claiming {tokenAmount} tokens ({claimAmountInWei} wei) based on {totalXP} XP");

//            if (wallet is WalletConnectWallet walletConnect)
//            {
//                await walletConnect.EnsureCorrectNetwork(ActiveChainId);
//            }
//            await Task.Delay(10000);

//            var transactionResult = await contract.DropERC20_Claim(wallet, walletAddress, ClaimTokenAmount);
//            Debug.Log($"Tokens claimed successfully! Transaction Hash: {transactionResult.TransactionHash}");
//            await Task.Delay(10000);

//            var tokenBalance = await contract.ERC20_BalanceOf(walletAddress);
//            var tokenBalanceFormatted = Utils.ToEth(tokenBalance.ToString(), decimals,  addCommas: true);
//            Debug.Log($"Updated token balance for {walletAddress}: {tokenBalanceFormatted}");
//            if (ClaimedTokenBalanceText != null)
//            {
//                ClaimedTokenBalanceText.text = $"Claimed: {tokenBalanceFormatted} B3";
//            }
//        }
//        catch (System.Exception ex)
//        {
//            Debug.LogError($"Failed to claim tokens: {ex.Message}");
//            if (ClaimedTokenBalanceText != null)
//            {
//                ClaimedTokenBalanceText.text = $"Claim Failed: {ex.Message}";
//            }
//            if (ClaimButton != null) ClaimButton.interactable = true;
//        }
//    }

//    public async void ConnectWithEcosystem()
//    {
//        if (thirdwebManager == null)
//        {
//            Debug.LogError("Cannot connect: ThirdwebManager is not initialized.");
//            return;
//        }

//        try
//        {
//            if (ConnectedText != null)
//            {
//                ConnectedText.gameObject.SetActive(true);
//                ConnectedText.text = "Connecting...";
//            }
//            if (DisconnectButton != null)
//            {
//                var disconnectButton = DisconnectButton.GetComponent<Button>();
//                if (disconnectButton != null)
//                {
//                    disconnectButton.interactable = false;
//                }
//            }

//            // Disconnect existing wallet if connected
//            if (wallet != null)
//            {
//                await wallet.Disconnect();
//                wallet = null;
//                walletAddress = null;
//                Debug.Log("Disconnected existing wallet to start new connection.");
//            }

//            var ecosystemWalletOptions = new EcosystemWalletOptions(ecosystemId: "ecosystem.your-ecosystem", email: "myepicemail@domain.id");
//            var options = new WalletOptions(
//                provider: WalletProvider.EcosystemWallet,
//                chainId: 84532,
//                ecosystemWalletOptions: ecosystemWalletOptions
//            );
//            wallet = await ThirdwebManager.Instance.ConnectWallet(options);
//            walletAddress = await wallet.GetAddress();
//            Debug.Log($"Wallet connected successfully! Address: {walletAddress}");

//            var balance = await wallet.GetBalance(chainId: ActiveChainId);
//            var balanceEth = Utils.ToEth(wei: balance.ToString(), decimalsToDisplay: 2, addCommas: true);
//            Debug.Log($"Wallet balance: {balanceEth}");
//            if (EthBalanceText != null)
//            {
//                EthBalanceText.gameObject.SetActive(true);
//                EthBalanceText.text = $"ETH: {balanceEth}";
//            }

//            if (!string.IsNullOrEmpty(TokenContractAddress))
//            {
//                var contract = await ThirdwebManager.Instance.GetContract(TokenContractAddress, ActiveChainId);
//                var decimals = 2;
//                var tokenBalance = await contract.ERC20_BalanceOf(walletAddress);
//                var tokenBalanceFormatted = Utils.ToEth(tokenBalance.ToString(), decimals, addCommas: true);
//                Debug.Log($"Custom token balance for {walletAddress}: {tokenBalanceFormatted}");
//                if (CustomTokenBalanceText != null)
//                {
//                    CustomTokenBalanceText.gameObject.SetActive(true);
//                    CustomTokenBalanceText.text = $"{TokenName}: {tokenBalanceFormatted}";
//                }
//            }

//            if (ConnectButton != null)
//            {
//                ConnectButton.SetActive(true);
//                var connectButton = ConnectButton.GetComponent<Button>();
//                if (connectButton != null)
//                {
//                    connectButton.interactable = true;
//                }
//            }
//            if (DisconnectButton != null)
//            {
//                DisconnectButton.SetActive(true);
//                var buttonComponent = DisconnectButton.GetComponent<Button>();
//                if (buttonComponent != null)
//                {
//                    buttonComponent.interactable = true;
//                }
//            }
//            if (ConnectedText != null)
//            {
//                ConnectedText.text = "Connected";
//            }
//            if (AddressText != null && !string.IsNullOrEmpty(walletAddress))
//            {
//                AddressText.gameObject.SetActive(true);
//                string shortAddress = $"{walletAddress.Substring(0, 3)}...{walletAddress.Substring(walletAddress.Length - 3)}";
//                AddressText.text = shortAddress;
//            }
//        }
//        catch (System.Exception ex)
//        {
//            Debug.LogWarning($"Wallet connection failed or canceled: {ex.Message}");
//            wallet = null;
//            walletAddress = null;

//            if (ConnectButton != null)
//            {
//                ConnectButton.SetActive(true);
//                var connectButton = ConnectButton.GetComponent<Button>();
//                if (connectButton != null)
//                {
//                    connectButton.interactable = true;
//                }
//            }
//            if (DisconnectButton != null)
//            {
//                DisconnectButton.SetActive(false);
//                var disconnectButton = DisconnectButton.GetComponent<Button>();
//                if (disconnectButton != null)
//                {
//                    disconnectButton.interactable = false;
//                }
//            }
//            if (ConnectedText != null)
//            {
//                ConnectedText.gameObject.SetActive(false);
//            }
//            if (AddressText != null) AddressText.gameObject.SetActive(false);
//            if (EthBalanceText != null) EthBalanceText.gameObject.SetActive(false);
//            if (CustomTokenBalanceText != null) CustomTokenBalanceText.gameObject.SetActive(false);
//            if (ClaimedTokenBalanceText != null) ClaimedTokenBalanceText.gameObject.SetActive(false);
//            if (ClaimedNFTText != null) ClaimedNFTText.gameObject.SetActive(false);
//            if (NftDisplayCanvas != null) NftDisplayCanvas.gameObject.SetActive(false);
//        }
//    }

//    public async void ClaimNft()
//    {
//        if (thirdwebManager == null || wallet == null)
//        {
//            Debug.LogError("Cannot claim NFT: ThirdwebManager or wallet not initialized.");
//            return;
//        }

//        if (string.IsNullOrEmpty(ClaimNftContractAddress) || !ClaimNftContractAddress.StartsWith("0x") || ClaimNftContractAddress.Length != 42)
//        {
//            Debug.LogError("Invalid claim NFT contract address.");
//            return;
//        }

//        if (string.IsNullOrEmpty(ClaimNftAmount) || !int.TryParse(ClaimNftAmount, out int claimAmount) || claimAmount <= 0)
//        {
//            Debug.LogError("Invalid claim amount.");
//            return;
//        }

//        if (NftDisplayCanvas == null || NftDisplayPrefab == null)
//        {
//            Debug.LogError("NFT display canvas or prefab not assigned.");
//            return;
//        }

//        Transform parentTransform = NftDisplayParent != null ? NftDisplayParent : NftDisplayCanvas.transform;

//        try
//        {
//            if (ClaimedNFTText != null)
//            {
//                ClaimedNFTText.gameObject.SetActive(true);
//                ClaimedNFTText.text = "Claiming...";
//            }

//            if (wallet is WalletConnectWallet walletConnect)
//            {
//                await walletConnect.EnsureCorrectNetwork(ActiveChainId);
//            }
//            await Task.Delay(20000);

//            var contract = await ThirdwebManager.Instance.GetContract(ClaimNftContractAddress, ActiveChainId);
//            var transactionResult = await contract.DropERC721_Claim(wallet, walletAddress, claimAmount);
//            Debug.Log($"NFTs claimed! Transaction Hash: {transactionResult.TransactionHash}");

//            BigInteger tokenBalance = 0;
//            int maxAttempts = 5;
//            for (int attempt = 0; attempt < maxAttempts; attempt++)
//            {
//                tokenBalance = await contract.ERC721_BalanceOf(walletAddress);
//                Debug.Log($"Balance check attempt {attempt + 1}/{maxAttempts}: {tokenBalance} NFTs");
//                if (tokenBalance >= claimAmount) break;
//                await Task.Delay(5000);
//            }

//            if (tokenBalance == 0)
//            {
//                Debug.LogError($"No NFTs owned by {walletAddress}.");
//                if (ClaimedNFTText != null)
//                {
//                    ClaimedNFTText.text = $"No NFTs owned. Tx Hash: {transactionResult.TransactionHash}";
//                }
//                return;
//            }

//            if (ClaimedNFTText != null)
//            {
//                ClaimedNFTText.text = $"Claimed! Tx Hash: {transactionResult.TransactionHash}\nBalance: {tokenBalance}";
//            }

//            ClearNftDisplay();

//            List<string> tokenIds = new List<string>();
//            try
//            {
//                var ownedNfts = await contract.ERC1155_GetOwnedNFTs(walletAddress);
//                foreach (var nft in ownedNfts)
//                {
//                    tokenIds.Add(nft.Metadata.Id.ToString());
//                    Debug.Log($"Fetched token ID: {nft.Metadata.Id}");
//                }
//            }
//            catch (System.Exception ex)
//            {
//                Debug.LogWarning($"GetOwnedNFTs failed: {ex.Message}. Using tokenOfOwnerByIndex.");
//                for (int i = 0; i < tokenBalance; i++)
//                {
//                    try
//                    {
//                        BigInteger index = new BigInteger(i);
//                        var tokenId = await contract.Read<BigInteger>("tokenOfOwnerByIndex", walletAddress, index);
//                        tokenIds.Add(tokenId.ToString());
//                        Debug.Log($"Fetched token ID at index {i}: {tokenId}");
//                    }
//                    catch (System.Exception ex2)
//                    {
//                        Debug.LogWarning($"Failed to fetch token ID at index {i}: {ex2.Message}");
//                    }
//                }
//            }

//            if (tokenIds.Count == 0 && tokenBalance > 0)
//            {
//                Debug.LogWarning("No token IDs fetched. Assuming sequential IDs.");
//                BigInteger totalSupply;
//                try
//                {
//                    totalSupply = await contract.Read<BigInteger>("totalSupply");
//                }
//                catch
//                {
//                    totalSupply = tokenBalance;
//                }
//                BigInteger startId = totalSupply - claimAmount;
//                if (startId < 0) startId = 0;
//                for (int i = 0; i < claimAmount; i++)
//                {
//                    BigInteger assumedId = startId + i;
//                    tokenIds.Add(assumedId.ToString());
//                    Debug.Log($"Assumed token ID: {assumedId}");
//                }
//            }

//            if (tokenIds.Count == 0)
//            {
//                Debug.LogError("No token IDs available.");
//                if (ClaimedNFTText != null)
//                {
//                    ClaimedNFTText.text = $"No NFTs found for {walletAddress}. Balance: {tokenBalance}";
//                }
//                return;
//            }

//            if (NftDisplayCanvas != null)
//            {
//                NftDisplayCanvas.gameObject.SetActive(true);
//                NftDisplayCanvas.enabled = true;
//            }

//            foreach (var tokenId in tokenIds)
//            {
//                try
//                {
//                    BigInteger tokenIdBigInt = BigInteger.Parse(tokenId);
//                    var nft = await contract.ERC721_GetNFT(tokenIdBigInt);
//                    string name = nft.Metadata.Name ?? $"NFT #{tokenId}";
//                    string description = nft.Metadata.Description ?? "No description";
//                    string tokenUri = nft.Metadata.Uri ?? "";

//                    string imageUrl = await GetImageUrlFromTokenUri(tokenUri);
//                    Texture2D imageTexture = null;
//                    if (!string.IsNullOrEmpty(imageUrl))
//                    {
//                        imageTexture = await LoadImage(imageUrl);
//                    }
//                    else
//                    {
//                        imageUrl = "https://via.placeholder.com/150";
//                        imageTexture = await LoadImage(imageUrl);
//                    }

//                    GameObject nftDisplay = Instantiate(NftDisplayPrefab, parentTransform);
//                    nftDisplay.SetActive(true);
//                    instantiatedNfts.Add(nftDisplay);

//                    TextMeshProUGUI[] textComponents = nftDisplay.GetComponentsInChildren<TextMeshProUGUI>(true);
//                    RawImage imageRaw = nftDisplay.GetComponentInChildren<RawImage>(true);

//                    TextMeshProUGUI nameText = null;
//                    TextMeshProUGUI descText = null;
//                    TextMeshProUGUI tokenIdText = null;

//                    foreach (var text in textComponents)
//                    {
//                        string textName = text.gameObject.name.ToLower();
//                        if (textName.Contains("name")) nameText = text;
//                        else if (textName.Contains("description") || textName.Contains("desc")) descText = text;
//                        else if (textName.Contains("tokenid") || textName.Contains("token")) tokenIdText = text;
//                    }

//                    if (nameText != null)
//                    {
//                        nameText.text = $"Name: {name}";
//                        nameText.color = new Color(1, 1, 1, 1);
//                    }
//                    if (descText != null)
//                    {
//                        descText.text = $"Description: {description}";
//                        descText.color = new Color(1, 1, 1, 1);
//                    }
//                    if (tokenIdText != null)
//                    {
//                        tokenIdText.text = $"Token ID: {tokenId}";
//                        tokenIdText.color = new Color(1, 1, 1, 1);
//                    }
//                    if (imageRaw != null)
//                    {
//                        imageRaw.color = Color.white;
//                        imageRaw.texture = imageTexture ?? Texture2D.grayTexture;
//                    }

//                    Debug.Log($"Displayed NFT: TokenID={tokenId}, Name={name}, Description={description}, ImageURL={imageUrl ?? "null"}");
//                }
//                catch (System.Exception ex)
//                {
//                    Debug.LogError($"Failed to process NFT with TokenID {tokenId}: {ex.Message}");
//                }
//            }

//            if (ClaimedNFTText != null)
//            {
//                ClaimedNFTText.gameObject.SetActive(true);
//                ClaimedNFTText.color = new Color(1, 1, 1, 1);
//                ClaimedNFTText.text = $"NFT Balance: {tokenBalance}\nTx Hash: {transactionResult.TransactionHash}";
//            }
//        }
//        catch (System.Exception ex)
//        {
//            Debug.LogError($"Failed to claim NFTs: {ex.Message}");
//            if (ClaimedNFTText != null)
//            {
//                ClaimedNFTText.text = $"Failed to claim: {ex.Message}";
//            }
//            if (NftDisplayCanvas != null) NftDisplayCanvas.gameObject.SetActive(false);
//        }
//    }

//    private async Task<string> GetImageUrlFromTokenUri(string tokenUri)
//    {
//        if (string.IsNullOrEmpty(tokenUri))
//        {
//            Debug.LogWarning("Token URI is empty or null.");
//            return null;
//        }

//        Debug.Log($"Fetching metadata from token URI: {tokenUri}");
//        if (tokenUri.StartsWith("ipfs://"))
//        {
//            tokenUri = tokenUri.Replace("ipfs://", "https://cloudflare-ipfs.com/ipfs/");
//            Debug.Log($"Converted IPFS URI to: {tokenUri}");
//        }

//        try
//        {
//            UnityWebRequest request = UnityWebRequest.Get(tokenUri);
//            request.timeout = 10;
//            var operation = request.SendWebRequest();
//            while (!operation.isDone) await Task.Yield();

//            if (request.result != UnityWebRequest.Result.Success)
//            {
//                Debug.LogError($"Failed to fetch metadata from {tokenUri}: {request.error} (Response Code: {request.responseCode})");
//                return null;
//            }

//            string json = request.downloadHandler.text;
//            Debug.Log($"Metadata JSON: {json}");
//            JObject metadata = JObject.Parse(json);
//            string imageUrl = metadata["image"]?.ToString() ?? metadata["image_url"]?.ToString();
//            if (string.IsNullOrEmpty(imageUrl))
//            {
//                Debug.LogWarning($"No 'image' or 'image_url' field found in metadata: {json}");
//                return null;
//            }

//            Debug.Log($"Parsed image URL: {imageUrl}");
//            if (imageUrl.StartsWith("ipfs://"))
//            {
//                imageUrl = imageUrl.Replace("ipfs://", "https://cloudflare-ipfs.com/ipfs/");
//                Debug.Log($"Converted IPFS image URL to: {imageUrl}");
//            }

//            return imageUrl;
//        }
//        catch (System.Exception ex)
//        {
//            Debug.LogError($"Failed to parse metadata from {tokenUri}: {ex.Message}");
//            return null;
//        }
//    }

//    private async Task<Texture2D> LoadImage(string imageUrl)
//    {
//        if (string.IsNullOrEmpty(imageUrl))
//        {
//            Debug.LogWarning("Image URL is empty or null.");
//            return null;
//        }

//        Debug.Log($"Loading image from: {imageUrl}");
//        try
//        {
//            UnityWebRequest request = UnityWebRequestTexture.GetTexture(imageUrl);
//            request.timeout = 10;
//            var operation = request.SendWebRequest();
//            while (!operation.isDone) await Task.Yield();

//            if (request.result != UnityWebRequest.Result.Success)
//            {
//                Debug.LogError($"Failed to load image from {imageUrl}: {request.error} (Response Code: {request.responseCode})");
//                return null;
//            }

//            Texture2D texture = DownloadHandlerTexture.GetContent(request);
//            Debug.Log($"Image loaded successfully: {imageUrl} (Size: {texture.width}x{texture.height})");
//            return texture;
//        }
//        catch (System.Exception ex)
//        {
//            Debug.LogError($"Failed to load image from {imageUrl}: {ex.Message}");
//            return null;
//        }
//    }

//    private void ClearNftDisplay()
//    {
//        foreach (var nftDisplay in instantiatedNfts)
//        {
//            if (nftDisplay != null) Destroy(nftDisplay);
//        }
//        instantiatedNfts.Clear();
//        Debug.Log("Cleared NFT display.");
//    }
//}








using UnityEngine;
using Thirdweb;
using System.Threading.Tasks;
using Thirdweb.Unity;
using System.Numerics;
using TMPro;
using System.Text;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;
using System; // Added for TimeoutException
using UnityEngine.Events;

public class WalletConnectManager : MonoBehaviour
{
    public UnityEvent<string> OnLoggedIn;
    private ThirdwebManager thirdwebManager;
    private IThirdwebWallet wallet;
    private string walletAddress;
    [field: SerializeField, Header("Wallet Options")]
    private ulong ActiveChainId = 84532;

    [field: SerializeField, Header("Send ETH amount")]
    public string Amount { get; set; }
    [field: SerializeField, Header("Send ETH address")]
    public string ToAddress { get; set; }

    [field: SerializeField, Header("Send Custom Token Options")]
    public string TokenName { get; set; }
    [field: SerializeField]
    public string TokenContractAddress { get; set; }
    [field: SerializeField]
    public string TokenAmount { get; set; }
    [field: SerializeField]
    public string TokenRecipientAddress { get; set; }

    [field: SerializeField, Header("Claim Token Options")]
    public string ClaimTokenContractAddress { get; set; }
    [field: SerializeField]
    public string ClaimTokenAmount { get; set; }

    [field: SerializeField, Header("Claim Nft Options")]
    public string ClaimNftContractAddress { get; set; }
    [field: SerializeField]
    public string ClaimNftAmount { get; set; }

    [field: SerializeField, Header("UI Elements")]
    public GameManager GameManager { get; set; }
    [field: SerializeField]
    public Button ClaimButton { get; set; }
    [field: SerializeField]
    public GameObject ConnectButton { get; set; }
    [field: SerializeField]
    public GameObject DisconnectButton { get; set; }
    [field: SerializeField]
    public TextMeshProUGUI ConnectedText { get; set; }
    [field: SerializeField]
    public TextMeshProUGUI ClaimedNFTText { get; set; }
    [field: SerializeField]
    public TextMeshProUGUI AddressText { get; set; }
    [field: SerializeField]
    public TextMeshProUGUI EthBalanceText { get; set; }
    [field: SerializeField]
    public TextMeshProUGUI CustomTokenBalanceText { get; set; }
    [field: SerializeField]
    public TextMeshProUGUI ClaimedTokenBalanceText { get; set; }

    [field: SerializeField, Header("NFT Display Canvas")]
    public Canvas NftDisplayCanvas { get; set; }
    [field: SerializeField]
    public GameObject NftDisplayPrefab { get; set; }
    [field: SerializeField]
    public Transform NftDisplayParent { get; set; }

    private List<GameObject> instantiatedNfts = new List<GameObject>();
    private float lastFeedbackUpdateTime;
    private int feedbackDotCount;

    void Awake()
    {
        thirdwebManager = FindObjectOfType<ThirdwebManager>();
        if (thirdwebManager == null)
        {
            Debug.LogError("ThirdwebManager not found in the scene! Please add the ThirdwebManager prefab.");
        }

        if (GameManager == null)
        {
            GameManager = FindObjectOfType<GameManager>();
            if (GameManager == null)
            {
                Debug.LogError("GameManager not found in the scene! Please add the GameManager component.");
            }
        }

        if (ConnectButton != null) ConnectButton.SetActive(true);
        if (DisconnectButton != null)
        {
            DisconnectButton.SetActive(false);
            var buttonComponent = DisconnectButton.GetComponent<UnityEngine.UI.Button>();
            if (buttonComponent != null)
            {
                buttonComponent.interactable = true;
            }
            else
            {
                Debug.LogError("DisconnectButton does not have a Button component!");
            }
        }
        if (ClaimButton != null)
        {
            ClaimButton.interactable = true;
        }
        if (ConnectedText != null) ConnectedText.gameObject.SetActive(false);
        if (AddressText != null) AddressText.gameObject.SetActive(false);
        if (EthBalanceText != null) EthBalanceText.gameObject.SetActive(false);
        if (CustomTokenBalanceText != null) CustomTokenBalanceText.gameObject.SetActive(false);
        if (ClaimedTokenBalanceText != null) ClaimedTokenBalanceText.gameObject.SetActive(false);
        if (ClaimedNFTText != null) ClaimedNFTText.gameObject.SetActive(false);
        if (NftDisplayCanvas != null) NftDisplayCanvas.gameObject.SetActive(false);

        //// Ensure Canvas scales correctly for WebGL
        //if (NftDisplayCanvas != null)
        //{
        //    var scaler = NftDisplayCanvas.GetComponent<CanvasScaler>();
        //    if (scaler == null)
        //    {
        //        scaler = NftDisplayCanvas.gameObject.AddComponent<CanvasScaler>();
        //        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        //        scaler.referenceResolution = new Vector2(1920, 1080);
        //        scaler.matchWidthOrHeight = 0.5f;
        //    }
        //}
    }

    private void Update()
    {
        // Update UI feedback animation (e.g., "Processing...")
        if (Time.time - lastFeedbackUpdateTime > 0.5f)
        {
            feedbackDotCount = (feedbackDotCount + 1) % 4;
            string dots = new string('.', feedbackDotCount);
            if (ConnectedText != null && ConnectedText.text.StartsWith("Connecting"))
            {
                ConnectedText.text = $"Connecting{dots}";
            }
            if (ClaimedTokenBalanceText != null && ClaimedTokenBalanceText.text.StartsWith("Claiming"))
            {
                ClaimedTokenBalanceText.text = $"Claiming{dots}";
            }
            if (ClaimedNFTText != null && ClaimedNFTText.text.StartsWith("Claiming"))
            {
                ClaimedNFTText.text = $"Claiming{dots}";
            }
            lastFeedbackUpdateTime = Time.time;
        }
    }

    public async void Connect()
    {
        if (thirdwebManager == null)
        {
            Debug.LogError("Cannot connect: ThirdwebManager is not initialized.");
            if (ConnectedText != null)
            {
                ConnectedText.gameObject.SetActive(true);
                ConnectedText.text = "Error: ThirdwebManager missing";
            }
            return;
        }

        try
        {
            if (ConnectedText != null)
            {
                ConnectedText.gameObject.SetActive(true);
                ConnectedText.text = "Connecting...";
            }
            if (DisconnectButton != null)
            {
                var disconnectButton = DisconnectButton.GetComponent<Button>();
                if (disconnectButton != null)
                {
                    disconnectButton.interactable = false;
                }
            }

            // Disconnect existing wallet if connected
            if (wallet != null)
            {
                await wallet.Disconnect();
                wallet = null;
                walletAddress = null;
                Debug.Log("Disconnected existing wallet to start new connection.");
            }

            var options = new WalletOptions(
                provider: WalletProvider.WalletConnectWallet,
                chainId: 84532
            );

            Debug.Log("WebGL: Initiating WalletConnect connection...");
#if UNITY_WEBGL
            Debug.Log("WebGL: Ensure browser supports WebSockets and localhost allows outbound connections.");
#endif

            // Add timeout for WalletConnect connection
            async Task<IThirdwebWallet> ConnectWithTimeout(WalletOptions opts, int timeoutMs)
            {
                var connectTask = ThirdwebManager.Instance.ConnectWallet(opts);
                var timeoutTask = Task.Delay(timeoutMs);
                var completedTask = await Task.WhenAny(connectTask, timeoutTask);
                if (completedTask == timeoutTask)
                {
                    throw new TimeoutException("WalletConnect connection timed out after " + timeoutMs + "ms");
                }
                return await connectTask;
            }

            // Attempt connection with retry logic
            int maxRetries = 2;
            int retryCount = 0;
            bool connected = false;
            while (retryCount <= maxRetries && !connected)
            {
                try
                {
                    wallet = await ConnectWithTimeout(options, 30000); // 30s timeout
                    walletAddress = await wallet.GetAddress();
                    connected = true;
                    Debug.Log($"Wallet connected successfully! Address: {walletAddress}");
                }
                catch (System.Exception ex)
                {
                    retryCount++;
                    string errorMsg = $"Connection attempt {retryCount}/{maxRetries} failed: {ex.Message}";
                    Debug.LogWarning(errorMsg);
                    if (retryCount > maxRetries)
                    {
                        throw new System.Exception(errorMsg);
                    }
                    await Task.Delay(2000); // Wait before retrying
                    Debug.Log("Retrying WalletConnect connection...");
                }
            }

            var balance = await wallet.GetBalance(chainId: ActiveChainId);
            var balanceEth = Utils.ToEth(wei: balance.ToString(), decimalsToDisplay: 2, addCommas: true);
            Debug.Log($"Wallet balance: {balanceEth}");
            if (EthBalanceText != null)
            {
                EthBalanceText.gameObject.SetActive(true);
                EthBalanceText.text = $"ETH: {balanceEth}";
            }

            if (!string.IsNullOrEmpty(TokenContractAddress))
            {
                var contract = await ThirdwebManager.Instance.GetContract(TokenContractAddress, ActiveChainId);
                var decimals = 2;
                var tokenBalance = await contract.ERC20_BalanceOf(walletAddress);
                var tokenBalanceFormatted = Utils.ToEth(tokenBalance.ToString(), decimals, addCommas: true);
                Debug.Log($"Custom token balance for {walletAddress}: {tokenBalanceFormatted}");
                if (CustomTokenBalanceText != null)
                {
                    CustomTokenBalanceText.gameObject.SetActive(true);
                    CustomTokenBalanceText.text = $"{TokenName}: {tokenBalanceFormatted}";
                }
            }

            if (ConnectButton != null)
            {
                ConnectButton.SetActive(false);
                var connectButton = ConnectButton.GetComponent<Button>();
                if (connectButton != null)
                {
                    connectButton.interactable = true;
                }
            }
            if (DisconnectButton != null)
            {
                DisconnectButton.SetActive(true);
                var buttonComponent = DisconnectButton.GetComponent<Button>();
                if (buttonComponent != null)
                {
                    buttonComponent.interactable = true;
                }
            }
            if (ConnectedText != null)
            {
                ConnectedText.text = "Connected";
            }
            if (AddressText != null && !string.IsNullOrEmpty(walletAddress))
            {
                AddressText.gameObject.SetActive(true);
                string shortAddress = $"{walletAddress.Substring(0, 3)}...{walletAddress.Substring(walletAddress.Length - 3)}";
                AddressText.text = shortAddress;
            }
        }
        catch (TimeoutException ex)
        {
            Debug.LogWarning($"Wallet connection timed out: {ex.Message}");
            if (ConnectedText != null)
            {
                ConnectedText.text = "Connection Timeout: Check wallet app or network";
            }
            ResetUIAfterFailure();
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning($"Wallet connection failed: {ex.Message}");
            if (ConnectedText != null)
            {
                ConnectedText.text = $"Connection Failed: {ex.Message}";
            }
            ResetUIAfterFailure();
        }
    }

    private void ResetUIAfterFailure()
    {
        wallet = null;
        walletAddress = null;

        if (ConnectButton != null)
        {
            ConnectButton.SetActive(true);
            var connectButton = ConnectButton.GetComponent<Button>();
            if (connectButton != null)
            {
                connectButton.interactable = true;
            }
        }
        if (DisconnectButton != null)
        {
            DisconnectButton.SetActive(false);
            var disconnectButton = DisconnectButton.GetComponent<Button>();
            if (disconnectButton != null)
            {
                disconnectButton.interactable = false;
            }
        }
        if (AddressText != null) AddressText.gameObject.SetActive(false);
        if (EthBalanceText != null) EthBalanceText.gameObject.SetActive(false);
        if (CustomTokenBalanceText != null) CustomTokenBalanceText.gameObject.SetActive(false);
        if (ClaimedTokenBalanceText != null) ClaimedTokenBalanceText.gameObject.SetActive(false);
        if (ClaimedNFTText != null) ClaimedNFTText.gameObject.SetActive(false);
        if (NftDisplayCanvas != null) NftDisplayCanvas.gameObject.SetActive(false);
    }

    public async void Disconnect()
    {
        if (wallet == null)
        {
            Debug.LogWarning("No wallet to disconnect.");
            return;
        }

        try
        {
            Debug.Log("Disconnecting wallet...");
            await wallet.Disconnect();
            wallet = null;
            walletAddress = null;

            if (ConnectButton != null)
            {
                ConnectButton.SetActive(true);
                var connectButton = ConnectButton.GetComponent<Button>();
                if (connectButton != null) connectButton.interactable = true;
            }
            if (DisconnectButton != null) DisconnectButton.SetActive(false);
            if (ClaimButton != null) ClaimButton.interactable = true;
            if (ConnectedText != null) ConnectedText.gameObject.SetActive(false);
            if (AddressText != null) AddressText.gameObject.SetActive(false);
            if (EthBalanceText != null) EthBalanceText.gameObject.SetActive(false);
            if (CustomTokenBalanceText != null) CustomTokenBalanceText.gameObject.SetActive(false);
            if (ClaimedTokenBalanceText != null) ClaimedTokenBalanceText.gameObject.SetActive(false);
            if (ClaimedNFTText != null) ClaimedNFTText.gameObject.SetActive(false);
            if (NftDisplayCanvas != null) NftDisplayCanvas.gameObject.SetActive(false);
            
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to disconnect wallet: {ex.Message}");
        }
    }

    public async void SendEth()
    {
        if (thirdwebManager == null || wallet == null)
        {
            Debug.LogError("Cannot send ETH: ThirdwebManager or wallet not initialized.");
            return;
        }

        if (string.IsNullOrEmpty(ToAddress) || !ToAddress.StartsWith("0x") || ToAddress.Length != 42)
        {
            Debug.LogError("Invalid recipient address.");
            return;
        }

        if (string.IsNullOrEmpty(Amount) || !float.TryParse(Amount, out float ethAmount) || ethAmount <= 0)
        {
            Debug.LogError("Invalid ETH amount.");
            return;
        }

        try
        {
            Debug.Log($"Sending {Amount} ETH to {ToAddress}...");
            if (wallet is WalletConnectWallet walletConnect)
            {
                await walletConnect.EnsureCorrectNetwork(ActiveChainId);
            }
            await Task.Delay(5000);
            string weiAmountString = Utils.ToWei(Amount);
            BigInteger weiAmount = BigInteger.Parse(weiAmountString);
            var transactionResult = await wallet.Transfer(ActiveChainId, ToAddress, weiAmount);
            Debug.Log($"ETH sent! Transaction Hash: {transactionResult.TransactionHash}");

            var balance = await wallet.GetBalance(chainId: ActiveChainId);
            var balanceEth = Utils.ToEth(wei: balance.ToString(), decimalsToDisplay: 2, addCommas: true);
            if (EthBalanceText != null)
            {
                EthBalanceText.gameObject.SetActive(true);
                EthBalanceText.text = $"ETH: {balanceEth}";
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to send ETH: {ex.Message}");
        }
    }

    public async void SendCustomToken()
    {
        if (thirdwebManager == null || wallet == null)
        {
            Debug.LogError("Cannot send token: ThirdwebManager or wallet not initialized.");
            return;
        }

        if (string.IsNullOrEmpty(TokenContractAddress) || string.IsNullOrEmpty(TokenRecipientAddress))
        {
            Debug.LogError("Invalid token contract or recipient address.");
            return;
        }

        if (string.IsNullOrEmpty(TokenAmount) || !float.TryParse(TokenAmount, out float tokenAmount) || tokenAmount <= 0)
        {
            Debug.LogError("Invalid token amount.");
            return;
        }

        try
        {
            Debug.Log($"Sending {TokenAmount} {TokenName} to {TokenRecipientAddress}...");
            if (wallet is WalletConnectWallet walletConnect)
            {
                await walletConnect.EnsureCorrectNetwork(ActiveChainId);
            }
            await Task.Delay(5000);
            var contract = await ThirdwebManager.Instance.GetContract(TokenContractAddress, ActiveChainId);
            var decimals = 2;
            string tokenAmountInWei = Utils.ToWei(TokenAmount);
            BigInteger tokenAmountBigInt = BigInteger.Parse(tokenAmountInWei);
            var transactionResult = await contract.ERC20_Transfer(wallet, TokenRecipientAddress, tokenAmountBigInt);
            Debug.Log($"Token sent! Transaction Hash: {transactionResult.TransactionHash}");

            var tokenBalance = await contract.ERC20_BalanceOf(walletAddress);
            var tokenBalanceFormatted = Utils.ToEth(tokenBalance.ToString(), decimals, addCommas: true);
            if (CustomTokenBalanceText != null)
            {
                CustomTokenBalanceText.gameObject.SetActive(true);
                CustomTokenBalanceText.text = $"{TokenName}: {tokenBalanceFormatted}";
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to send {TokenName}: {ex.Message}");
        }
    }
    [Obsolete]
    public async void ClaimToken()
    {
        //if (thirdwebManager == null || wallet == null)
        //{
        //    Debug.LogError("Cannot claim token: ThirdwebManager or wallet not initialized.");
        //    return;
        //}

        //if (string.IsNullOrEmpty(ClaimTokenContractAddress) || !ClaimTokenContractAddress.StartsWith("0x") || ClaimTokenContractAddress.Length != 42)
        //{
        //    Debug.LogError("Invalid claim token contract address.");
        //    return;
        //}

        //if (GameManager == null)
        //{
        //    Debug.LogError("GameManager not assigned. Cannot get total XP.");
        //    return;
        //}

        try
        {
            if (ClaimButton != null) ClaimButton.interactable = false;
            if (ClaimedTokenBalanceText != null)
            {
                ClaimedTokenBalanceText.gameObject.SetActive(true);
                ClaimedTokenBalanceText.text = "Claiming...";
            }

            float totalXP = GameManager.GetTotalXP();
            decimal tokenAmount = (decimal)totalXP;
            ClaimTokenAmount = tokenAmount.ToString();
            var contract = await ThirdwebManager.Instance.GetContract(ClaimTokenContractAddress, ActiveChainId, "[{\"inputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"constructor\"},{\"inputs\":[],\"name\":\"ContractMetadataUnauthorized\",\"type\":\"error\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"recipient\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"value\",\"type\":\"uint256\"}],\"name\":\"CurrencyTransferLibFailedNativeTransfer\",\"type\":\"error\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"expected\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"actual\",\"type\":\"uint256\"}],\"name\":\"DropClaimExceedLimit\",\"type\":\"error\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"expected\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"actual\",\"type\":\"uint256\"}],\"name\":\"DropClaimExceedMaxSupply\",\"type\":\"error\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"expectedCurrency\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"expectedPricePerToken\",\"type\":\"uint256\"},{\"internalType\":\"address\",\"name\":\"actualCurrency\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"actualExpectedPricePerToken\",\"type\":\"uint256\"}],\"name\":\"DropClaimInvalidTokenPrice\",\"type\":\"error\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"expected\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"actual\",\"type\":\"uint256\"}],\"name\":\"DropClaimNotStarted\",\"type\":\"error\"},{\"inputs\":[],\"name\":\"DropExceedMaxSupply\",\"type\":\"error\"},{\"inputs\":[],\"name\":\"DropNoActiveCondition\",\"type\":\"error\"},{\"inputs\":[],\"name\":\"DropUnauthorized\",\"type\":\"error\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"account\",\"type\":\"address\"},{\"internalType\":\"bytes32\",\"name\":\"role\",\"type\":\"bytes32\"}],\"name\":\"PermissionsAlreadyGranted\",\"type\":\"error\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"expected\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"actual\",\"type\":\"address\"}],\"name\":\"PermissionsInvalidPermission\",\"type\":\"error\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"account\",\"type\":\"address\"},{\"internalType\":\"bytes32\",\"name\":\"neededRole\",\"type\":\"bytes32\"}],\"name\":\"PermissionsUnauthorizedAccount\",\"type\":\"error\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"max\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"actual\",\"type\":\"uint256\"}],\"name\":\"PlatformFeeExceededMaxFeeBps\",\"type\":\"error\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"recipient\",\"type\":\"address\"}],\"name\":\"PlatformFeeInvalidRecipient\",\"type\":\"error\"},{\"inputs\":[],\"name\":\"PlatformFeeUnauthorized\",\"type\":\"error\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"recipient\",\"type\":\"address\"}],\"name\":\"PrimarySaleInvalidRecipient\",\"type\":\"error\"},{\"inputs\":[],\"name\":\"PrimarySaleUnauthorized\",\"type\":\"error\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"spender\",\"type\":\"address\"},{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"value\",\"type\":\"uint256\"}],\"name\":\"Approval\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"components\":[{\"internalType\":\"uint256\",\"name\":\"startTimestamp\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"maxClaimableSupply\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"supplyClaimed\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"quantityLimitPerWallet\",\"type\":\"uint256\"},{\"internalType\":\"bytes32\",\"name\":\"merkleRoot\",\"type\":\"bytes32\"},{\"internalType\":\"uint256\",\"name\":\"pricePerToken\",\"type\":\"uint256\"},{\"internalType\":\"address\",\"name\":\"currency\",\"type\":\"address\"},{\"internalType\":\"string\",\"name\":\"metadata\",\"type\":\"string\"}],\"indexed\":false,\"internalType\":\"struct IClaimCondition.ClaimCondition[]\",\"name\":\"claimConditions\",\"type\":\"tuple[]\"},{\"indexed\":false,\"internalType\":\"bool\",\"name\":\"resetEligibility\",\"type\":\"bool\"}],\"name\":\"ClaimConditionsUpdated\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":false,\"internalType\":\"string\",\"name\":\"prevURI\",\"type\":\"string\"},{\"indexed\":false,\"internalType\":\"string\",\"name\":\"newURI\",\"type\":\"string\"}],\"name\":\"ContractURIUpdated\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"delegator\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"fromDelegate\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"toDelegate\",\"type\":\"address\"}],\"name\":\"DelegateChanged\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"delegate\",\"type\":\"address\"},{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"previousBalance\",\"type\":\"uint256\"},{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"newBalance\",\"type\":\"uint256\"}],\"name\":\"DelegateVotesChanged\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[],\"name\":\"EIP712DomainChanged\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":false,\"internalType\":\"address\",\"name\":\"platformFeeRecipient\",\"type\":\"address\"},{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"flatFee\",\"type\":\"uint256\"}],\"name\":\"FlatPlatformFeeUpdated\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":false,\"internalType\":\"uint8\",\"name\":\"version\",\"type\":\"uint8\"}],\"name\":\"Initialized\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"maxTotalSupply\",\"type\":\"uint256\"}],\"name\":\"MaxTotalSupplyUpdated\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"platformFeeRecipient\",\"type\":\"address\"},{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"platformFeeBps\",\"type\":\"uint256\"}],\"name\":\"PlatformFeeInfoUpdated\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":false,\"internalType\":\"enum IPlatformFee.PlatformFeeType\",\"name\":\"feeType\",\"type\":\"uint8\"}],\"name\":\"PlatformFeeTypeUpdated\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"recipient\",\"type\":\"address\"}],\"name\":\"PrimarySaleRecipientUpdated\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"bytes32\",\"name\":\"role\",\"type\":\"bytes32\"},{\"indexed\":true,\"internalType\":\"bytes32\",\"name\":\"previousAdminRole\",\"type\":\"bytes32\"},{\"indexed\":true,\"internalType\":\"bytes32\",\"name\":\"newAdminRole\",\"type\":\"bytes32\"}],\"name\":\"RoleAdminChanged\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"bytes32\",\"name\":\"role\",\"type\":\"bytes32\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"account\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"sender\",\"type\":\"address\"}],\"name\":\"RoleGranted\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"bytes32\",\"name\":\"role\",\"type\":\"bytes32\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"account\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"sender\",\"type\":\"address\"}],\"name\":\"RoleRevoked\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"uint256\",\"name\":\"claimConditionIndex\",\"type\":\"uint256\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"claimer\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"receiver\",\"type\":\"address\"},{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"startTokenId\",\"type\":\"uint256\"},{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"quantityClaimed\",\"type\":\"uint256\"}],\"name\":\"TokensClaimed\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"value\",\"type\":\"uint256\"}],\"name\":\"Transfer\",\"type\":\"event\"},{\"inputs\":[],\"name\":\"CLOCK_MODE\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"DEFAULT_ADMIN_ROLE\",\"outputs\":[{\"internalType\":\"bytes32\",\"name\":\"\",\"type\":\"bytes32\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"DEFAULT_FEE_RECIPIENT\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"DOMAIN_SEPARATOR\",\"outputs\":[{\"internalType\":\"bytes32\",\"name\":\"\",\"type\":\"bytes32\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"spender\",\"type\":\"address\"}],\"name\":\"allowance\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"spender\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"amount\",\"type\":\"uint256\"}],\"name\":\"approve\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"account\",\"type\":\"address\"}],\"name\":\"balanceOf\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"amount\",\"type\":\"uint256\"}],\"name\":\"burn\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"account\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"amount\",\"type\":\"uint256\"}],\"name\":\"burnFrom\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"account\",\"type\":\"address\"},{\"internalType\":\"uint32\",\"name\":\"pos\",\"type\":\"uint32\"}],\"name\":\"checkpoints\",\"outputs\":[{\"components\":[{\"internalType\":\"uint32\",\"name\":\"fromBlock\",\"type\":\"uint32\"},{\"internalType\":\"uint224\",\"name\":\"votes\",\"type\":\"uint224\"}],\"internalType\":\"struct ERC20VotesUpgradeable.Checkpoint\",\"name\":\"\",\"type\":\"tuple\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"_receiver\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"_quantity\",\"type\":\"uint256\"},{\"internalType\":\"address\",\"name\":\"_currency\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"_pricePerToken\",\"type\":\"uint256\"},{\"components\":[{\"internalType\":\"bytes32[]\",\"name\":\"proof\",\"type\":\"bytes32[]\"},{\"internalType\":\"uint256\",\"name\":\"quantityLimitPerWallet\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"pricePerToken\",\"type\":\"uint256\"},{\"internalType\":\"address\",\"name\":\"currency\",\"type\":\"address\"}],\"internalType\":\"struct IDrop.AllowlistProof\",\"name\":\"_allowlistProof\",\"type\":\"tuple\"},{\"internalType\":\"bytes\",\"name\":\"_data\",\"type\":\"bytes\"}],\"name\":\"claim\",\"outputs\":[],\"stateMutability\":\"payable\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"claimCondition\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"currentStartId\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"count\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"clock\",\"outputs\":[{\"internalType\":\"uint48\",\"name\":\"\",\"type\":\"uint48\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"contractType\",\"outputs\":[{\"internalType\":\"bytes32\",\"name\":\"\",\"type\":\"bytes32\"}],\"stateMutability\":\"pure\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"contractURI\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"contractVersion\",\"outputs\":[{\"internalType\":\"uint8\",\"name\":\"\",\"type\":\"uint8\"}],\"stateMutability\":\"pure\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"decimals\",\"outputs\":[{\"internalType\":\"uint8\",\"name\":\"\",\"type\":\"uint8\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"spender\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"subtractedValue\",\"type\":\"uint256\"}],\"name\":\"decreaseAllowance\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"delegatee\",\"type\":\"address\"}],\"name\":\"delegate\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"delegatee\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"nonce\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"expiry\",\"type\":\"uint256\"},{\"internalType\":\"uint8\",\"name\":\"v\",\"type\":\"uint8\"},{\"internalType\":\"bytes32\",\"name\":\"r\",\"type\":\"bytes32\"},{\"internalType\":\"bytes32\",\"name\":\"s\",\"type\":\"bytes32\"}],\"name\":\"delegateBySig\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"account\",\"type\":\"address\"}],\"name\":\"delegates\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"eip712Domain\",\"outputs\":[{\"internalType\":\"bytes1\",\"name\":\"fields\",\"type\":\"bytes1\"},{\"internalType\":\"string\",\"name\":\"name\",\"type\":\"string\"},{\"internalType\":\"string\",\"name\":\"version\",\"type\":\"string\"},{\"internalType\":\"uint256\",\"name\":\"chainId\",\"type\":\"uint256\"},{\"internalType\":\"address\",\"name\":\"verifyingContract\",\"type\":\"address\"},{\"internalType\":\"bytes32\",\"name\":\"salt\",\"type\":\"bytes32\"},{\"internalType\":\"uint256[]\",\"name\":\"extensions\",\"type\":\"uint256[]\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"getActiveClaimConditionId\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_conditionId\",\"type\":\"uint256\"}],\"name\":\"getClaimConditionById\",\"outputs\":[{\"components\":[{\"internalType\":\"uint256\",\"name\":\"startTimestamp\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"maxClaimableSupply\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"supplyClaimed\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"quantityLimitPerWallet\",\"type\":\"uint256\"},{\"internalType\":\"bytes32\",\"name\":\"merkleRoot\",\"type\":\"bytes32\"},{\"internalType\":\"uint256\",\"name\":\"pricePerToken\",\"type\":\"uint256\"},{\"internalType\":\"address\",\"name\":\"currency\",\"type\":\"address\"},{\"internalType\":\"string\",\"name\":\"metadata\",\"type\":\"string\"}],\"internalType\":\"struct IClaimCondition.ClaimCondition\",\"name\":\"condition\",\"type\":\"tuple\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"getFlatPlatformFeeInfo\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"timepoint\",\"type\":\"uint256\"}],\"name\":\"getPastTotalSupply\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"account\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"timepoint\",\"type\":\"uint256\"}],\"name\":\"getPastVotes\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"getPlatformFeeInfo\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"},{\"internalType\":\"uint16\",\"name\":\"\",\"type\":\"uint16\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"getPlatformFeeType\",\"outputs\":[{\"internalType\":\"enum IPlatformFee.PlatformFeeType\",\"name\":\"\",\"type\":\"uint8\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"bytes32\",\"name\":\"role\",\"type\":\"bytes32\"}],\"name\":\"getRoleAdmin\",\"outputs\":[{\"internalType\":\"bytes32\",\"name\":\"\",\"type\":\"bytes32\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"bytes32\",\"name\":\"role\",\"type\":\"bytes32\"},{\"internalType\":\"uint256\",\"name\":\"index\",\"type\":\"uint256\"}],\"name\":\"getRoleMember\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"member\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"bytes32\",\"name\":\"role\",\"type\":\"bytes32\"}],\"name\":\"getRoleMemberCount\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"count\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_conditionId\",\"type\":\"uint256\"},{\"internalType\":\"address\",\"name\":\"_claimer\",\"type\":\"address\"}],\"name\":\"getSupplyClaimedByWallet\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"supplyClaimedByWallet\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"account\",\"type\":\"address\"}],\"name\":\"getVotes\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"bytes32\",\"name\":\"role\",\"type\":\"bytes32\"},{\"internalType\":\"address\",\"name\":\"account\",\"type\":\"address\"}],\"name\":\"grantRole\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"bytes32\",\"name\":\"role\",\"type\":\"bytes32\"},{\"internalType\":\"address\",\"name\":\"account\",\"type\":\"address\"}],\"name\":\"hasRole\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"bytes32\",\"name\":\"role\",\"type\":\"bytes32\"},{\"internalType\":\"address\",\"name\":\"account\",\"type\":\"address\"}],\"name\":\"hasRoleWithSwitch\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"spender\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"addedValue\",\"type\":\"uint256\"}],\"name\":\"increaseAllowance\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"_defaultAdmin\",\"type\":\"address\"},{\"internalType\":\"string\",\"name\":\"_name\",\"type\":\"string\"},{\"internalType\":\"string\",\"name\":\"_symbol\",\"type\":\"string\"},{\"internalType\":\"string\",\"name\":\"_contractURI\",\"type\":\"string\"},{\"internalType\":\"address[]\",\"name\":\"_trustedForwarders\",\"type\":\"address[]\"},{\"internalType\":\"address\",\"name\":\"_saleRecipient\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"_platformFeeRecipient\",\"type\":\"address\"},{\"internalType\":\"uint128\",\"name\":\"_platformFeeBps\",\"type\":\"uint128\"}],\"name\":\"initialize\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"forwarder\",\"type\":\"address\"}],\"name\":\"isTrustedForwarder\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"maxTotalSupply\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"bytes[]\",\"name\":\"data\",\"type\":\"bytes[]\"}],\"name\":\"multicall\",\"outputs\":[{\"internalType\":\"bytes[]\",\"name\":\"results\",\"type\":\"bytes[]\"}],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"name\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"}],\"name\":\"nonces\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"account\",\"type\":\"address\"}],\"name\":\"numCheckpoints\",\"outputs\":[{\"internalType\":\"uint32\",\"name\":\"\",\"type\":\"uint32\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"spender\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"value\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"deadline\",\"type\":\"uint256\"},{\"internalType\":\"uint8\",\"name\":\"v\",\"type\":\"uint8\"},{\"internalType\":\"bytes32\",\"name\":\"r\",\"type\":\"bytes32\"},{\"internalType\":\"bytes32\",\"name\":\"s\",\"type\":\"bytes32\"}],\"name\":\"permit\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"primarySaleRecipient\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"bytes32\",\"name\":\"role\",\"type\":\"bytes32\"},{\"internalType\":\"address\",\"name\":\"account\",\"type\":\"address\"}],\"name\":\"renounceRole\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"bytes32\",\"name\":\"role\",\"type\":\"bytes32\"},{\"internalType\":\"address\",\"name\":\"account\",\"type\":\"address\"}],\"name\":\"revokeRole\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"components\":[{\"internalType\":\"uint256\",\"name\":\"startTimestamp\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"maxClaimableSupply\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"supplyClaimed\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"quantityLimitPerWallet\",\"type\":\"uint256\"},{\"internalType\":\"bytes32\",\"name\":\"merkleRoot\",\"type\":\"bytes32\"},{\"internalType\":\"uint256\",\"name\":\"pricePerToken\",\"type\":\"uint256\"},{\"internalType\":\"address\",\"name\":\"currency\",\"type\":\"address\"},{\"internalType\":\"string\",\"name\":\"metadata\",\"type\":\"string\"}],\"internalType\":\"struct IClaimCondition.ClaimCondition[]\",\"name\":\"_conditions\",\"type\":\"tuple[]\"},{\"internalType\":\"bool\",\"name\":\"_resetClaimEligibility\",\"type\":\"bool\"}],\"name\":\"setClaimConditions\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"string\",\"name\":\"_uri\",\"type\":\"string\"}],\"name\":\"setContractURI\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"_platformFeeRecipient\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"_flatFee\",\"type\":\"uint256\"}],\"name\":\"setFlatPlatformFeeInfo\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_maxTotalSupply\",\"type\":\"uint256\"}],\"name\":\"setMaxTotalSupply\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"_platformFeeRecipient\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"_platformFeeBps\",\"type\":\"uint256\"}],\"name\":\"setPlatformFeeInfo\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"enum IPlatformFee.PlatformFeeType\",\"name\":\"_feeType\",\"type\":\"uint8\"}],\"name\":\"setPlatformFeeType\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"_saleRecipient\",\"type\":\"address\"}],\"name\":\"setPrimarySaleRecipient\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"symbol\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"totalSupply\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"amount\",\"type\":\"uint256\"}],\"name\":\"transfer\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"amount\",\"type\":\"uint256\"}],\"name\":\"transferFrom\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_conditionId\",\"type\":\"uint256\"},{\"internalType\":\"address\",\"name\":\"_claimer\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"_quantity\",\"type\":\"uint256\"},{\"internalType\":\"address\",\"name\":\"_currency\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"_pricePerToken\",\"type\":\"uint256\"},{\"components\":[{\"internalType\":\"bytes32[]\",\"name\":\"proof\",\"type\":\"bytes32[]\"},{\"internalType\":\"uint256\",\"name\":\"quantityLimitPerWallet\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"pricePerToken\",\"type\":\"uint256\"},{\"internalType\":\"address\",\"name\":\"currency\",\"type\":\"address\"}],\"internalType\":\"struct IDrop.AllowlistProof\",\"name\":\"_allowlistProof\",\"type\":\"tuple\"}],\"name\":\"verifyClaim\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"isOverride\",\"type\":\"bool\"}],\"stateMutability\":\"view\",\"type\":\"function\"}]");
            var decimals = 2;
            string claimAmountInWei = Utils.ToWei(tokenAmount.ToString());
            Debug.Log($"Claiming {tokenAmount} tokens ({claimAmountInWei} wei) based on {totalXP} XP");

            if (wallet is WalletConnectWallet walletConnect)
            {
                await walletConnect.EnsureCorrectNetwork(ActiveChainId);
            }
            //await Task.Delay(5000);

            var transactionResult = await contract.DropERC20_Claim(wallet, walletAddress, ClaimTokenAmount);
            //var transactionResult = await contract.TokenERC20_MintTo(wallet, walletAddress, ClaimTokenAmount);
           
            //var transactionResult = await contract.Write(wallet, "claim", 0, ClaimTokenAmount);

            Debug.Log($"Tokens claimed successfully! Transaction Hash: {transactionResult.TransactionHash}");
            //await Task.Delay(5000);

            var tokenBalance = await contract.ERC20_BalanceOf(walletAddress);
            var tokenBalanceFormatted = Utils.ToEth(tokenBalance.ToString(), decimals, addCommas: true);
            Debug.Log($"Updated token balance for {walletAddress}: {tokenBalanceFormatted}");
            if (ClaimedTokenBalanceText != null)
            {
                ClaimedTokenBalanceText.text = $"Claimed: {tokenBalanceFormatted} Color";
            }
            if (ClaimButton != null) ClaimButton.interactable = false;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to claim tokens: {ex.Message}");
            if (ClaimedTokenBalanceText != null)
            {
                ClaimedTokenBalanceText.text = $"Claim Failed: {ex.Message}";
            }
            if (ClaimButton != null) ClaimButton.interactable = true;
        }
    }

    public async void ConnectWithEcosystem()
    {
        if (thirdwebManager == null)
        {
            Debug.LogError("Cannot connect: ThirdwebManager is not initialized.");
            return;
        }

        try
        {
            if (ConnectedText != null)
            {
                ConnectedText.gameObject.SetActive(true);
                ConnectedText.text = "Connecting...";
            }
            if (DisconnectButton != null)
            {
                var disconnectButton = DisconnectButton.GetComponent<Button>();
                if (disconnectButton != null)
                {
                    disconnectButton.interactable = false;
                }
            }

            if (wallet != null)
            {
                await wallet.Disconnect();
                wallet = null;
                walletAddress = null;
                Debug.Log("Disconnected existing wallet to start new connection.");
            }

            var ecosystemWalletOptions = new EcosystemWalletOptions(ecosystemId: "ecosystem.your-ecosystem", email: "myepicemail@domain.id");
            var options = new WalletOptions(
                provider: WalletProvider.EcosystemWallet,
                chainId: 84532,
                ecosystemWalletOptions: ecosystemWalletOptions
            );
            Debug.Log("Initiating ecosystem wallet connection...");
            wallet = await ThirdwebManager.Instance.ConnectWallet(options);
            walletAddress = await wallet.GetAddress();
            Debug.Log($"Wallet connected successfully! Address: {walletAddress}");

            var balance = await wallet.GetBalance(chainId: ActiveChainId);
            var balanceEth = Utils.ToEth(wei: balance.ToString(), decimalsToDisplay: 2, addCommas: true);
            Debug.Log($"Wallet balance: {balanceEth}");
            if (EthBalanceText != null)
            {
                EthBalanceText.gameObject.SetActive(true);
                EthBalanceText.text = $"ETH: {balanceEth}";
            }

            if (!string.IsNullOrEmpty(TokenContractAddress))
            {
                var contract = await ThirdwebManager.Instance.GetContract(TokenContractAddress, ActiveChainId);
                var decimals = 2;
                var tokenBalance = await contract.ERC20_BalanceOf(walletAddress);
                var tokenBalanceFormatted = Utils.ToEth(tokenBalance.ToString(), decimals, addCommas: true);
                Debug.Log($"Custom token balance for {walletAddress}: {tokenBalanceFormatted}");
                if (CustomTokenBalanceText != null)
                {
                    CustomTokenBalanceText.gameObject.SetActive(true);
                    CustomTokenBalanceText.text = $"{TokenName}: {tokenBalanceFormatted}";
                }
            }

            if (ConnectButton != null)
            {
                ConnectButton.SetActive(true);
                var connectButton = ConnectButton.GetComponent<Button>();
                if (connectButton != null)
                {
                    connectButton.interactable = true;
                }
            }
            if (DisconnectButton != null)
            {
                DisconnectButton.SetActive(true);
                var buttonComponent = DisconnectButton.GetComponent<Button>();
                if (buttonComponent != null)
                {
                    buttonComponent.interactable = true;
                }
            }
            if (ConnectedText != null)
            {
                ConnectedText.text = "Connected";
            }
            if (AddressText != null && !string.IsNullOrEmpty(walletAddress))
            {
                AddressText.gameObject.SetActive(true);
                string shortAddress = $"{walletAddress.Substring(0, 3)}...{walletAddress.Substring(walletAddress.Length - 3)}";
                AddressText.text = shortAddress;
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning($"Wallet connection failed or canceled: {ex.Message}");
            if (ConnectedText != null)
            {
                ConnectedText.text = $"Connection Failed: {ex.Message}";
            }
            wallet = null;
            walletAddress = null;

            if (ConnectButton != null)
            {
                ConnectButton.SetActive(true);
                var connectButton = ConnectButton.GetComponent<Button>();
                if (connectButton != null)
                {
                    connectButton.interactable = true;
                }
            }
            if (DisconnectButton != null)
            {
                DisconnectButton.SetActive(false);
                var disconnectButton = DisconnectButton.GetComponent<Button>();
                if (disconnectButton != null)
                {
                    disconnectButton.interactable = false;
                }
            }
            if (ConnectedText != null)
            {
                ConnectedText.gameObject.SetActive(false);
            }
            if (AddressText != null) AddressText.gameObject.SetActive(false);
            if (EthBalanceText != null) EthBalanceText.gameObject.SetActive(false);
            if (CustomTokenBalanceText != null) CustomTokenBalanceText.gameObject.SetActive(false);
            if (ClaimedTokenBalanceText != null) ClaimedTokenBalanceText.gameObject.SetActive(false);
            if (ClaimedNFTText != null) ClaimedNFTText.gameObject.SetActive(false);
            if (NftDisplayCanvas != null) NftDisplayCanvas.gameObject.SetActive(false);
        }
    }


    public async void Login(string authProvider)
    {

        if (ConnectedText != null)
        {
            ConnectedText.gameObject.SetActive(true);
            ConnectedText.text = "Connecting...";
        }
        if (DisconnectButton != null)
        {
            var disconnectButton = DisconnectButton.GetComponent<Button>();
            if (disconnectButton != null)
            {
                disconnectButton.interactable = false;
            }
        }

        AuthProvider provider = AuthProvider.Google;
        switch (authProvider)
        {
            case "google":
                provider = AuthProvider.Google;
                break;
            case "apple":
                provider = AuthProvider.Apple;
                break;
            case "facebook":
                provider = AuthProvider.Facebook;
                break;
        }

        var connection = new WalletOptions(
            provider: WalletProvider.InAppWallet,
            chainId: 84532,
            inAppWalletOptions: new InAppWalletOptions(authprovider: provider),
            smartWalletOptions: new SmartWalletOptions(sponsorGas: true)
        );

        wallet = await ThirdwebManager.Instance.ConnectWallet(connection);
        walletAddress = await wallet.GetAddress();

        OnLoggedIn?.Invoke(walletAddress);

        var balance = await wallet.GetBalance(chainId: ActiveChainId);
        var balanceEth = Utils.ToEth(wei: balance.ToString(), decimalsToDisplay: 2, addCommas: true);
        Debug.Log($"Wallet balance: {balanceEth}");
        if (EthBalanceText != null)
        {
            EthBalanceText.gameObject.SetActive(true);
            EthBalanceText.text = $"ETH: {balanceEth}";
        }

        if (!string.IsNullOrEmpty(TokenContractAddress))
        {
            var contract = await ThirdwebManager.Instance.GetContract(TokenContractAddress, ActiveChainId);
            var decimals = 2;
            var tokenBalance = await contract.ERC20_BalanceOf(walletAddress);
            var tokenBalanceFormatted = Utils.ToEth(tokenBalance.ToString(), decimals, addCommas: true);
            Debug.Log($"Custom token balance for {walletAddress}: {tokenBalanceFormatted}");
            if (CustomTokenBalanceText != null)
            {
                CustomTokenBalanceText.gameObject.SetActive(true);
                CustomTokenBalanceText.text = $"{TokenName}: {tokenBalanceFormatted}";
            }
        }

        //if (ConnectButton != null)
        //{
        //    ConnectButton.SetActive(false);
        //    var connectButton = ConnectButton.GetComponent<Button>();
        //    if (connectButton != null)
        //    {
        //        connectButton.interactable = true;
        //    }
        //}
        if (DisconnectButton != null)
        {
            DisconnectButton.SetActive(true);
            var buttonComponent = DisconnectButton.GetComponent<Button>();
            if (buttonComponent != null)
            {
                buttonComponent.interactable = true;
            }
        }
        if (ConnectedText != null)
        {
            ConnectedText.text = "Connected";
        }
        if (AddressText != null && !string.IsNullOrEmpty(walletAddress))
        {
            AddressText.gameObject.SetActive(true);
            string shortAddress = $"{walletAddress.Substring(0, 3)}...{walletAddress.Substring(walletAddress.Length - 3)}";
            AddressText.text = shortAddress;
        }
    }
}



