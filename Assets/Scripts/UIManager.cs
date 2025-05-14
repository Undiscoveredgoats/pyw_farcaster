////using UnityEngine;
////using UnityEngine.SceneManagement;
////using UnityEngine.UI;

////public class GameManager : MonoBehaviour
////{
////    public Canvas mainMenuCanvas;      // Reference to your main menu canvas
////    public Button startButton;         // Reference to your start button
////    public Button pauseButton;         // Reference to your pause button
////    public Button resumeButton;        // Reference to your resume button
////    public Button restartButton;       // Reference to your restart button
////    public Button BMMutton;            // Reference to your back to main menu button
////    public Button RBMMutton;           // Reference to your back to main menu button
////    // Color selection buttons in pause menu
////    public Button purpleButton;
////    public Button blueButton;
////    public Button yellowButton;
////    public Button redButton;
////    public bool isGameStarted = false; // Tracks if game has started
////    public Canvas pauseMenu;
////    public Canvas resumeMenu;
////    public Canvas gameOverMenu;
////    public GameObject player;          // Reference to your player GameObject
////    public Camera mainCamera;          // Reference to your main camera
////    public PaintManager paintManager;  // Reference to your PaintManager
////    private Vector3 initialPlayerPosition; // Store player's starting position
////    private static bool shouldStartImmediately = false; // Flag for immediate start after restart

////    // Camera follow settings
////    public float smoothSpeed = 0.125f; // How smooth the camera follows (lower = smoother but slower)
////    public Vector3 cameraOffset = new Vector3(0f, 0f, -10f); // Offset from player (Z=-10 for 2D)

////    void Start()
////    {
////        // Store initial player position
////        if (player != null)
////        {
////            initialPlayerPosition = player.transform.position;
////        }

////        // Ensure correct UI state at start
////        if (mainMenuCanvas != null && pauseMenu != null && resumeMenu != null && gameOverMenu != null)
////        {
////            if (shouldStartImmediately)
////            {
////                // When restarting, hide all menus except pause menu and start game
////                mainMenuCanvas.gameObject.SetActive(false);
////                pauseMenu.gameObject.SetActive(true);
////                resumeMenu.gameObject.SetActive(false);
////                gameOverMenu.gameObject.SetActive(false);
////                StartGame();
////            }
////            else
////            {
////                AudioManager.Instance.PlayMenuMusic();
////                // Normal start: show main menu only
////                mainMenuCanvas.gameObject.SetActive(true);
////                pauseMenu.gameObject.SetActive(false);
////                resumeMenu.gameObject.SetActive(false);
////                gameOverMenu.gameObject.SetActive(false);
////                Time.timeScale = 0f;
////            }
////        }

////        // Add listeners to buttons
////        if (startButton != null)
////        {
////            startButton.onClick.AddListener(() => OnStartButtonClicked());
////        }

////        if (pauseButton != null)
////        {
////            pauseButton.onClick.AddListener(() => OnPauseButtonClicked());
////        }

////        if (resumeButton != null)
////        {
////            resumeButton.onClick.AddListener(() => OnResumeButtonClicked());
////        }

////        if (restartButton != null)
////        {
////            restartButton.onClick.AddListener(() => OnRestartButtonClicked());
////        }

////        if (BMMutton != null)
////        {
////            BMMutton.onClick.AddListener(() => OnBackToMainMenuButtonClicked());
////        }
////        if (RBMMutton != null)
////        {
////            RBMMutton.onClick.AddListener(() => OnBackToMainMenuButtonClicked());
////        }

////        // Add listeners for color selection buttons
////        if (purpleButton != null)
////        {
////            purpleButton.onClick.AddListener(() => OnColorButtonClicked("purple"));
////        }
////        if (blueButton != null)
////        {
////            blueButton.onClick.AddListener(() => OnColorButtonClicked("blue"));
////        }
////        if (yellowButton != null)
////        {
////            yellowButton.onClick.AddListener(() => OnColorButtonClicked("yellow"));
////        }
////        if (redButton != null)
////        {
////            redButton.onClick.AddListener(() => OnColorButtonClicked("red"));
////        }
////    }

////    void Update()
////    {
////        // Only follow player when game is started
////        if (isGameStarted && player != null && mainCamera != null)
////        {
////            FollowPlayer();
////        }
////    }

////    void FollowPlayer()
////    {
////        // Target position: follow player X, keep camera Y fixed, maintain Z offset
////        Vector3 desiredPosition = new Vector3(
////            player.transform.position.x + cameraOffset.x,
////            mainCamera.transform.position.y, // Keep Y fixed
////            cameraOffset.z
////        );

////        // Smoothly move camera to desired position
////        Vector3 smoothedPosition = Vector3.Lerp(
////            mainCamera.transform.position,
////            desiredPosition,
////            smoothSpeed * Time.deltaTime
////        );

////        mainCamera.transform.position = smoothedPosition;
////    }

////    void OnStartButtonClicked()
////    {
////        AudioManager.Instance.PlayClickSound();
////        StartGame();
////    }

////    void StartGame()
////    {
////        // Hide main menu and reset player position
////        if (mainMenuCanvas != null)
////        {
////            AudioManager.Instance.PlayGameplayMusic();
////            mainMenuCanvas.gameObject.SetActive(false);
////            if (pauseMenu != null)
////            {
////                pauseMenu.gameObject.SetActive(true);
////            }
////        }

////        if (player != null)
////        {
////            player.transform.position = initialPlayerPosition;
////        }

////        // Start the game
////        Time.timeScale = 1f;
////        isGameStarted = true;
////        shouldStartImmediately = false; // Reset the flag
////    }

////    void OnPauseButtonClicked()
////    {
////        AudioManager.Instance.PlayClickSound();
////        PauseGame();
////    }

////    void PauseGame()
////    {
////        AudioManager.Instance.PlayPauseMusic();
////        Time.timeScale = 0f;
////        pauseMenu.gameObject.SetActive(false);
////        resumeMenu.gameObject.SetActive(true);
////    }

////    void OnResumeButtonClicked()
////    {
////        AudioManager.Instance.PlayClickSound();
////        ResumeGame();
////    }

////    void ResumeGame()
////    {
////        AudioManager.Instance.PlayGameplayMusic();
////        Time.timeScale = 1f;
////        resumeMenu.gameObject.SetActive(false);
////        pauseMenu.gameObject.SetActive(true);
////    }

////    public void GameOver()
////    {
////        AudioManager.Instance.PlayGameOverMusic();
////        isGameStarted = false;
////        gameOverMenu.gameObject.SetActive(true);
////        pauseMenu.gameObject.SetActive(false);
////        Time.timeScale = 0f;
////    }

////    void OnRestartButtonClicked()
////    {
////        AudioManager.Instance.PlayClickSound();
////        Restart();
////    }

////    void Restart()
////    {
////        // Set flag to start immediately after reload
////        shouldStartImmediately = true;
////        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
////    }

////    void OnBackToMainMenuButtonClicked()
////    {
////        AudioManager.Instance.PlayClickSound();
////        BackToMainMenu();
////    }

////    public void BackToMainMenu()
////    {
////        // Fully reload the scene to reset everything
////        shouldStartImmediately = false; // Ensure we go to main menu
////        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
////    }

////    void OnColorButtonClicked(string colorName)
////    {
////        AudioManager.Instance.PlayClickSound();
////        SetPaintColor(colorName);
////    }

////    // Method to set paint color via PaintManager
////    void SetPaintColor(string colorName)
////    {
////        if (paintManager != null)
////        {
////            paintManager.SetPaintColor(colorName);
////        }
////        else
////        {
////            Debug.LogError("PaintManager reference is not set in GameManager!");
////        }
////    }
////}

//using UnityEngine;
//using UnityEngine.SceneManagement;
//using UnityEngine.UI;
//using TMPro;

//public class GameManager : MonoBehaviour
//{
//    public Canvas mainMenuCanvas;      // Reference to your main menu canvas
//    public Button startButton;         // Reference to your start button
//    public Button pauseButton;         // Reference to your pause button
//    public Button resumeButton;        // Reference to your resume button
//    public Button restartButton;       // Reference to your restart button
//    public Button BMMutton;            // Reference to your back to main menu button
//    public Button RBMMutton;           // Reference to your back to main menu button
//    // Color selection buttons in pause menu
//    public Button purpleButton;
//    public Button blueButton;
//    public Button yellowButton;
//    public Button redButton;
//    public bool isGameStarted = false; // Tracks if game has started
//    public Canvas pauseMenu;
//    public Canvas resumeMenu;
//    public Canvas gameOverMenu;
//    public GameObject player;          // Reference to your player GameObject
//    public Camera mainCamera;          // Reference to your main camera
//    public PaintManager paintManager;  // Reference to your PaintManager
//    public TextMeshProUGUI xpText;  
//    public TextMeshProUGUI gameOverText;        
//    private Vector3 initialPlayerPosition; // Store player's starting position
//    private static bool shouldStartImmediately = false; // Flag for immediate start after restart
//    private float totalXP = 0f;        // Total XP earned
//    private float obstacleBonus = 10f; // XP bonus per obstacle passed

//    // Camera follow settings
//    public float smoothSpeed = 0.125f; // How smooth the camera follows (lower = smoother but slower)
//    public Vector3 cameraOffset = new Vector3(0f, 0f, -10f); // Offset from player (Z=-10 for 2D)

//    void Start()
//    {
//        // Store initial player position
//        if (player != null)
//        {
//            initialPlayerPosition = player.transform.position;
//        }

//        // Ensure correct UI state at start
//        if (mainMenuCanvas != null && pauseMenu != null && resumeMenu != null && gameOverMenu != null)
//        {
//            if (shouldStartImmediately)
//            {
//                // When restarting, hide all menus except pause menu and start game
//                mainMenuCanvas.gameObject.SetActive(false);
//                pauseMenu.gameObject.SetActive(true);
//                resumeMenu.gameObject.SetActive(false);
//                gameOverMenu.gameObject.SetActive(false);
//                StartGame();
//            }
//            else
//            {
//                AudioManager.Instance.PlayMenuMusic();
//                // Normal start: show main menu only
//                mainMenuCanvas.gameObject.SetActive(true);
//                pauseMenu.gameObject.SetActive(false);
//                resumeMenu.gameObject.SetActive(false);
//                gameOverMenu.gameObject.SetActive(false);
//                Time.timeScale = 0f;
//            }
//        }

//        // Initialize XP text
//        UpdateXPText();

//        // Add listeners to buttons
//        if (startButton != null)
//        {
//            startButton.onClick.AddListener(() => OnStartButtonClicked());
//        }

//        if (pauseButton != null)
//        {
//            pauseButton.onClick.AddListener(() => OnPauseButtonClicked());
//        }

//        if (resumeButton != null)
//        {
//            resumeButton.onClick.AddListener(() => OnResumeButtonClicked());
//        }

//        if (restartButton != null)
//        {
//            restartButton.onClick.AddListener(() => OnRestartButtonClicked());
//        }

//        if (BMMutton != null)
//        {
//            BMMutton.onClick.AddListener(() => OnBackToMainMenuButtonClicked());
//        }
//        if (RBMMutton != null)
//        {
//            RBMMutton.onClick.AddListener(() => OnBackToMainMenuButtonClicked());
//        }

//        // Add listeners for color selection buttons
//        if (purpleButton != null)
//        {
//            purpleButton.onClick.AddListener(() => OnColorButtonClicked("purple"));
//        }
//        if (blueButton != null)
//        {
//            blueButton.onClick.AddListener(() => OnColorButtonClicked("blue"));
//        }
//        if (yellowButton != null)
//        {
//            yellowButton.onClick.AddListener(() => OnColorButtonClicked("yellow"));
//        }
//        if (redButton != null)
//        {
//            redButton.onClick.AddListener(() => OnColorButtonClicked("red"));
//        }
//    }

//    void Update()
//    {
//        // Only follow player when game is started
//        if (isGameStarted && player != null && mainCamera != null)
//        {
//            FollowPlayer();
//        }
//    }

//    void FollowPlayer()
//    {
//        // Target position: follow player X, keep camera Y fixed, maintain Z offset
//        Vector3 desiredPosition = new Vector3(
//            player.transform.position.x + cameraOffset.x,
//            mainCamera.transform.position.y, // Keep Y fixed
//            cameraOffset.z
//        );

//        // Smoothly move camera to desired position
//        Vector3 smoothedPosition = Vector3.Lerp(
//            mainCamera.transform.position,
//            desiredPosition,
//            smoothSpeed * Time.deltaTime
//        );

//        mainCamera.transform.position = smoothedPosition;
//    }

//    void OnStartButtonClicked()
//    {
//        AudioManager.Instance.PlayClickSound();
//        StartGame();
//    }

//    void StartGame()
//    {
//        // Hide main menu and reset player position
//        if (mainMenuCanvas != null)
//        {
//            AudioManager.Instance.PlayGameplayMusic();
//            mainMenuCanvas.gameObject.SetActive(false);
//            if (pauseMenu != null)
//            {
//                pauseMenu.gameObject.SetActive(true);
//            }
//        }

//        if (player != null)
//        {
//            player.transform.position = initialPlayerPosition;
//        }

//        totalXP = 0f; // Reset XP on new game
//        UpdateXPText();

//        // Start the game
//        Time.timeScale = 1f;
//        isGameStarted = true;
//        shouldStartImmediately = false; // Reset the flag
//    }

//    void OnPauseButtonClicked()
//    {
//        AudioManager.Instance.PlayClickSound();
//        PauseGame();
//    }

//    void PauseGame()
//    {
//        AudioManager.Instance.PlayPauseMusic();
//        Time.timeScale = 0f;
//        pauseMenu.gameObject.SetActive(false);
//        resumeMenu.gameObject.SetActive(true);
//        UpdateXPText(); // Update XP display when pausing
//    }

//    void OnResumeButtonClicked()
//    {
//        AudioManager.Instance.PlayClickSound();
//        ResumeGame();
//    }

//    void ResumeGame()
//    {
//        AudioManager.Instance.PlayGameplayMusic();
//        Time.timeScale = 1f;
//        resumeMenu.gameObject.SetActive(false);
//        pauseMenu.gameObject.SetActive(true);
//    }

//    public void GameOver()
//    {
//        AudioManager.Instance.PlayGameOverMusic();
//        isGameStarted = false;
//        gameOverMenu.gameObject.SetActive(true);
//        pauseMenu.gameObject.SetActive(false);
//        Time.timeScale = 0f;
//        UpdateXPText(); // Update XP display on game over
//        UpdateGameOverText();
//    }

//    void OnRestartButtonClicked()
//    {
//        AudioManager.Instance.PlayClickSound();
//        Restart();
//    }

//    void Restart()
//    {
//        // Set flag to start immediately after reload
//        shouldStartImmediately = true;
//        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
//    }

//    void OnBackToMainMenuButtonClicked()
//    {
//        AudioManager.Instance.PlayClickSound();
//        BackToMainMenu();
//    }

//    public void BackToMainMenu()
//    {
//        // Fully reload the scene to reset everything
//        shouldStartImmediately = false; // Ensure we go to main menu
//        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
//    }

//    void OnColorButtonClicked(string colorName)
//    {
//        AudioManager.Instance.PlayClickSound();
//        SetPaintColor(colorName);
//    }

//    // Method to set paint color via PaintManager
//    void SetPaintColor(string colorName)
//    {
//        if (paintManager != null)
//        {
//            paintManager.SetPaintColor(colorName);
//        }
//        else
//        {
//            Debug.LogError("PaintManager reference is not set in GameManager!");
//        }
//    }

//    public void AddXP(float amount)
//    {
//        totalXP += amount;
//        UpdateXPText();
//    }

//    public void AddObstacleBonus()
//    {
//        totalXP += obstacleBonus;
//        UpdateXPText();
//    }

//    void UpdateXPText()
//    {
//        if (xpText != null)
//        {
//            xpText.text = $"XP: {totalXP}";
//        }
//        else
//        {
//            Debug.LogWarning("xpText reference not set in GameManager!");
//        }
//    }

//    public float GetTotalXP()
//    {
//        return totalXP;
//    }

//    void UpdateGameOverText()
//    {
//        if (gameOverText != null)
//        {
//            gameOverText.text = $"Final XP: {totalXP}";
//        }
//        else
//        {
//            Debug.LogWarning("gameOverText reference not set in GameManager!");
//        }
//    }
//}




//using UnityEngine;
//using UnityEngine.SceneManagement;
//using UnityEngine.UI;
//using TMPro;
//using System.Collections;

//public class GameManager : MonoBehaviour
//{
//    public Canvas mainMenuCanvas;      // Reference to your main menu canvas
//    public Button startButton;         // Reference to your start button
//    public Button pauseButton;         // Reference to your pause button
//    public Button resumeButton;        // Reference to your resume button
//    public Button restartButton;       // Reference to your restart button
//    public Button BMMutton;            // Reference to your back to main menu button
//    public Button RBMMutton;           // Reference to your back to main menu button
//    // Color selection buttons in pause menu
//    public Button purpleButton;
//    public Button blueButton;
//    public Button yellowButton;
//    public Button redButton;
//    public bool isGameStarted = false; // Tracks if game has started
//    public Canvas pauseMenu;
//    public Canvas resumeMenu;
//    public Canvas gameOverMenu;
//    public GameObject player;          // Reference to your player GameObject
//    public Camera mainCamera;          // Reference to your main camera
//    public PaintManager paintManager;  // Reference to your PaintManager
//    public TextMeshProUGUI xpText;
//    public TextMeshProUGUI gameOverText;
//    private Vector3 initialPlayerPosition; // Store player's starting position
//    private static bool shouldStartImmediately = false; // Flag for immediate start after restart
//    private float totalXP = 0f;        // Total XP earned
//    private float obstacleBonus = 10f; // XP bonus per obstacle passed

//    // Instruction UI fields
//    public Canvas instructionCanvas;    // Reference to instruction canvas
//    public Image instructionImage;      // Reference to the UI Image for displaying sprites
//    public Button nextButton;           // Reference to the Next button
//    public Button skipButton;           // Reference to the Skip button
//    public Sprite[] instructionSprites; // Array of instruction sprites
//    private int currentInstructionIndex = 0; // Track current instruction sprite
//    private bool showingInstructions = false; // Track if instructions are being shown

//    // Camera follow settings
//    public float smoothSpeed = 0.125f; // How smooth the camera follows (lower = smoother but slower)
//    public Vector3 cameraOffset = new Vector3(0f, 0f, -10f); // Offset from player (Z=-10 for 2D)

//    void Start()
//    {
//        // Store initial player position
//        if (player != null)
//        {
//            initialPlayerPosition = player.transform.position;
//        }

//        // Ensure correct UI state at start
//        if (mainMenuCanvas != null && pauseMenu != null && resumeMenu != null && gameOverMenu != null && instructionCanvas != null)
//        {
//            if (shouldStartImmediately)
//            {
//                // When restarting, hide all menus except pause menu and start game
//                instructionCanvas.gameObject.SetActive(false);
//                mainMenuCanvas.gameObject.SetActive(false);
//                pauseMenu.gameObject.SetActive(true);
//                resumeMenu.gameObject.SetActive(false);
//                gameOverMenu.gameObject.SetActive(false);
//                StartGame();
//            }
//            else
//            {
//                AudioManager.Instance.PlayMenuMusic();
//                // Normal start: show instructions first
//                showingInstructions = true;
//                instructionCanvas.gameObject.SetActive(falkse);
//                mainMenuCanvas.gameObject.SetActive(true);
//                pauseMenu.gameObject.SetActive(false);
//                resumeMenu.gameObject.SetActive(false);
//                gameOverMenu.gameObject.SetActive(false);
//                Time.timeScale = 0f;
//                ShowInstruction(); // Display first instruction
//            }
//        }
//        else
//        {
//            Debug.LogWarning("One or more Canvas references are not set in GameManager!");
//        }

//        // Initialize XP text
//        UpdateXPText();

//        // Add listeners to buttons
//        if (startButton != null)
//        {
//            startButton.onClick.AddListener(OnStartButtonClicked);
//        }

//        if (pauseButton != null)
//        {
//            pauseButton.onClick.AddListener(OnPauseButtonClicked);
//        }

//        if (resumeButton != null)
//        {
//            resumeButton.onClick.AddListener(OnResumeButtonClicked);
//        }

//        if (restartButton != null)
//        {
//            restartButton.onClick.AddListener(OnRestartButtonClicked);
//        }

//        if (BMMutton != null)
//        {
//            BMMutton.onClick.AddListener(OnBackToMainMenuButtonClicked);
//        }
//        if (RBMMutton != null)
//        {
//            RBMMutton.onClick.AddListener(OnBackToMainMenuButtonClicked);
//        }

//        // Add listeners for color selection buttons
//        if (purpleButton != null)
//        {
//            purpleButton.onClick.AddListener(() => OnColorButtonClicked("purple"));
//        }
//        if (blueButton != null)
//        {
//            blueButton.onClick.AddListener(() => OnColorButtonClicked("blue"));
//        }
//        if (yellowButton != null)
//        {
//            yellowButton.onClick.AddListener(() => OnColorButtonClicked("yellow"));
//        }
//        if (redButton != null)
//        {
//            redButton.onClick.AddListener(() => OnColorButtonClicked("red"));
//        }

//        // Add listeners for instruction buttons
//        if (nextButton != null)
//        {
//            nextButton.onClick.AddListener(OnNextInstructionClicked);
//        }
//        if (skipButton != null)
//        {
//            skipButton.onClick.AddListener(OnSkipInstructionsClicked);
//        }
//    }

//    void Update()
//    {
//        // Only follow player when game is started
//        if (isGameStarted && player != null && mainCamera != null)
//        {
//            FollowPlayer();
//        }
//    }

//    void FollowPlayer()
//    {
//        // Target position: follow player X, keep camera Y fixed, maintain Z offset
//        Vector3 desiredPosition = new Vector3(
//            player.transform.position.x + cameraOffset.x,
//            mainCamera.transform.position.y, // Keep Y fixed
//            cameraOffset.z
//        );

//        // Smoothly move camera to desired position
//        Vector3 smoothedPosition = Vector3.Lerp(
//            mainCamera.transform.position,
//            desiredPosition,
//            smoothSpeed * Time.deltaTime
//        );

//        mainCamera.transform.position = smoothedPosition;
//    }

//    void OnStartButtonClicked()
//    {
//        AudioManager.Instance.PlayClickSound();
//        StartGame();
//    }

//    void StartGame()
//    {
//        // Hide main menu and reset player position
//        if (mainMenuCanvas != null)
//        {
//            AudioManager.Instance.PlayGameplayMusic();
//            mainMenuCanvas.gameObject.SetActive(false);
//            if (pauseMenu != null)
//            {
//                pauseMenu.gameObject.SetActive(true);
//            }
//        }

//        if (player != null)
//        {
//            player.transform.position = initialPlayerPosition;
//        }

//        totalXP = 0f; // Reset XP on new game
//        UpdateXPText();

//        // Start the game
//        Time.timeScale = 1f;
//        isGameStarted = true;
//        shouldStartImmediately = false; // Reset the flag
//    }

//    void OnPauseButtonClicked()
//    {
//        AudioManager.Instance.PlayClickSound();
//        PauseGame();
//    }

//    void PauseGame()
//    {
//        AudioManager.Instance.PlayPauseMusic();
//        Time.timeScale = 0f;
//        pauseMenu.gameObject.SetActive(false);
//        resumeMenu.gameObject.SetActive(true);
//        UpdateXPText(); // Update XP display when pausing
//    }

//    void OnResumeButtonClicked()
//    {
//        AudioManager.Instance.PlayClickSound();
//        ResumeGame();
//    }

//    void ResumeGame()
//    {
//        AudioManager.Instance.PlayGameplayMusic();
//        Time.timeScale = 1f;
//        resumeMenu.gameObject.SetActive(false);
//        pauseMenu.gameObject.SetActive(true);
//    }

//    public void GameOver()
//    {
//        AudioManager.Instance.PlayGameOverMusic();
//        isGameStarted = false;
//        gameOverMenu.gameObject.SetActive(true);
//        pauseMenu.gameObject.SetActive(false);
//        Time.timeScale = 0f;
//        UpdateXPText(); // Update XP display on game over
//        UpdateGameOverText();
//    }

//    void OnRestartButtonClicked()
//    {
//        AudioManager.Instance.PlayClickSound();
//        Restart();
//    }

//    void Restart()
//    {
//        // Set flag to start immediately after reload
//        shouldStartImmediately = true;
//        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
//    }

//    void OnBackToMainMenuButtonClicked()
//    {
//        AudioManager.Instance.PlayClickSound();
//        BackToMainMenu();
//    }

//    public void BackToMainMenu()
//    {
//        // Fully reload the scene to reset everything
//        shouldStartImmediately = false; // Ensure we go to main menu
//        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
//    }

//    void OnColorButtonClicked(string colorName)
//    {
//        AudioManager.Instance.PlayClickSound();
//        SetPaintColor(colorName);
//    }

//    // Method to set paint color via PaintManager
//    void SetPaintColor(string colorName)
//    {
//        if (paintManager != null)
//        {
//            paintManager.SetPaintColor(colorName);
//        }
//        else
//        {
//            Debug.LogError("PaintManager reference is not set in GameManager!");
//        }
//    }

//    public void AddXP(float amount)
//    {
//        totalXP += amount;
//        UpdateXPText();
//    }

//    public void AddObstacleBonus()
//    {
//        totalXP += obstacleBonus;
//        UpdateXPText();
//    }

//    void UpdateXPText()
//    {
//        if (xpText != null)
//        {
//            xpText.text = $"XP: {totalXP}";
//        }
//        else
//        {
//            Debug.LogWarning("xpText reference not set in GameManager!");
//        }
//    }

//    public float GetTotalXP()
//    {
//        return totalXP;
//    }

//    void UpdateGameOverText()
//    {
//        if (gameOverText != null)
//        {
//            gameOverText.text = $"Final XP: {totalXP}";
//        }
//        else
//        {
//            Debug.LogWarning("gameOverText reference not set in GameManager!");
//        }
//    }

//    // Instruction handling methods
//    void ShowInstruction()
//    {
//        if (instructionSprites == null || instructionSprites.Length == 0)
//        {
//            Debug.LogWarning("No instruction sprites assigned in GameManager!");
//            EndInstructions();
//            return;
//        }

//        if (currentInstructionIndex >= instructionSprites.Length)
//        {
//            EndInstructions();
//            return;
//        }

//        if (instructionImage != null)
//        {
//            instructionImage.sprite = instructionSprites[currentInstructionIndex];
//            // Enable Next button only if there are more instructions
//            if (nextButton != null)
//            {
//                nextButton.gameObject.SetActive(currentInstructionIndex < instructionSprites.Length - 1);
//            }
//            // Optional: Uncomment to enable auto-advance
//            // StartCoroutine(AutoAdvanceInstruction(5f)); // Auto-advance after 5 seconds
//        }
//        else
//        {
//            Debug.LogWarning("InstructionImage reference not set in GameManager!");
//            EndInstructions();
//        }
//    }

//    void OnNextInstructionClicked()
//    {
//        AudioManager.Instance.PlayClickSound();
//        currentInstructionIndex++;
//        ShowInstruction();
//    }

//    void OnSkipInstructionsClicked()
//    {
//        AudioManager.Instance.PlayClickSound();
//        EndInstructions();
//    }

//    void EndInstructions()
//    {
//        showingInstructions = false;
//        instructionCanvas.gameObject.SetActive(false);
//        mainMenuCanvas.gameObject.SetActive(true);
//        currentInstructionIndex = 0; // Reset for next time
//    }

//    // Optional: Auto-advance coroutine
//    IEnumerator AutoAdvanceInstruction(float delay)
//    {
//        yield return new WaitForSecondsRealtime(delay); // Use real-time for paused game
//        if (showingInstructions) // Only advance if still showing instructions
//        {
//            currentInstructionIndex++;
//            ShowInstruction();
//        }
//    }
//}


using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public Canvas mainMenuCanvas;      // Reference to your main menu canvas
    public Button startButton;         // Reference to your start button
    public Button pauseButton;         // Reference to your pause button
    public Button resumeButton;        // Reference to your resume button
    public Button restartButton;       // Reference to your restart button
    public Button BMMutton;            // Reference to your back to main menu button
    public Button RBMMutton;           // Reference to your back to main menu button
    // Color selection buttons in pause menu
    public Button purpleButton;
    public Button blueButton;
    public Button yellowButton;
    public Button redButton;
    public bool isGameStarted = false; // Tracks if game has started
    public Canvas pauseMenu;
    public Canvas resumeMenu;
    public Canvas gameOverMenu;
    public GameObject player;          // Reference to your player GameObject
    public Camera mainCamera;          // Reference to your main camera
    public PaintManager paintManager;  // Reference to your PaintManager
    public TextMeshProUGUI xpText;
    public TextMeshProUGUI gameOverText;
    private Vector3 initialPlayerPosition; // Store player's starting position
    private static bool shouldStartImmediately = false; // Flag for immediate start after restart
    private float totalXP = 0f;        // Total XP earned
    private float obstacleBonus = 10f; // XP bonus per obstacle passed

    // Instruction UI fields
    public Canvas instructionCanvas;    // Reference to instruction canvas
    public Image instructionImage;      // Reference to the UI Image for displaying sprites
    public Button nextButton;           // Reference to the Next button
    public Button skipButton;           // Reference to the Skip button
    public Sprite[] instructionSprites; // Array of instruction sprites
    private int currentInstructionIndex = 0; // Track current instruction sprite
    private bool showingInstructions = false; // Track if instructions are being shown

    // Camera follow settings
    public float smoothSpeed = 0.125f; // How smooth the camera follows (lower = smoother but slower)
    public Vector3 cameraOffset = new Vector3(0f, 0f, -10f); // Offset from player (Z=-10 for 2D)

    void Start()
    {
        // Store initial player position
        if (player != null)
        {
            initialPlayerPosition = player.transform.position;
        }

        // Ensure correct UI state at start
        if (mainMenuCanvas != null && pauseMenu != null && resumeMenu != null && gameOverMenu != null)
        {
            if (shouldStartImmediately)
            {
                // When restarting, hide all menus except pause menu and start game
                mainMenuCanvas.gameObject.SetActive(false);
                pauseMenu.gameObject.SetActive(true);
                resumeMenu.gameObject.SetActive(false);
                gameOverMenu.gameObject.SetActive(false);
                if (instructionCanvas != null)
                {
                    instructionCanvas.gameObject.SetActive(false);
                }
                StartGame();
            }
            else
            {
                AudioManager.Instance.PlayMenuMusic();
                // Normal start: show main menu only
                mainMenuCanvas.gameObject.SetActive(true);
                pauseMenu.gameObject.SetActive(false);
                resumeMenu.gameObject.SetActive(false);
                gameOverMenu.gameObject.SetActive(false);
                if (instructionCanvas != null)
                {
                    instructionCanvas.gameObject.SetActive(false);
                }
                Time.timeScale = 0f;
            }
        }
        else
        {
            Debug.LogWarning("One or more Canvas references are not set in GameManager!");
        }

        // Initialize XP text
        UpdateXPText();

        // Add listeners to buttons
        if (startButton != null)
        {
            startButton.onClick.AddListener(OnStartButtonClicked);
        }

        if (pauseButton != null)
        {
            pauseButton.onClick.AddListener(OnPauseButtonClicked);
        }

        if (resumeButton != null)
        {
            resumeButton.onClick.AddListener(OnResumeButtonClicked);
        }

        if (restartButton != null)
        {
            restartButton.onClick.AddListener(OnRestartButtonClicked);
        }

        if (BMMutton != null)
        {
            BMMutton.onClick.AddListener(OnBackToMainMenuButtonClicked);
        }
        if (RBMMutton != null)
        {
            RBMMutton.onClick.AddListener(OnBackToMainMenuButtonClicked);
        }

        // Add listeners for color selection buttons
        if (purpleButton != null)
        {
            purpleButton.onClick.AddListener(() => OnColorButtonClicked("purple"));
        }
        if (blueButton != null)
        {
            blueButton.onClick.AddListener(() => OnColorButtonClicked("blue"));
        }
        if (yellowButton != null)
        {
            yellowButton.onClick.AddListener(() => OnColorButtonClicked("yellow"));
        }
        if (redButton != null)
        {
            redButton.onClick.AddListener(() => OnColorButtonClicked("red"));
        }

        // Add listeners for instruction buttons
        if (nextButton != null)
        {
            nextButton.onClick.AddListener(OnNextInstructionClicked);
        }
        if (skipButton != null)
        {
            skipButton.onClick.AddListener(OnSkipInstructionsClicked);
        }
    }

    void Update()
    {
        // Only follow player when game is started
        if (isGameStarted && player != null && mainCamera != null)
        {
            FollowPlayer();
        }
    }

    void FollowPlayer()
    {
        // Target position: follow player X, keep camera Y fixed, maintain Z offset
        Vector3 desiredPosition = new Vector3(
            player.transform.position.x + cameraOffset.x,
            mainCamera.transform.position.y, // Keep Y fixed
            cameraOffset.z
        );

        // Smoothly move camera to desired position
        Vector3 smoothedPosition = Vector3.Lerp(
            mainCamera.transform.position,
            desiredPosition,
            smoothSpeed * Time.deltaTime
        );

        mainCamera.transform.position = smoothedPosition;
    }

    void OnStartButtonClicked()
    {
        AudioManager.Instance.PlayClickSound();
        // Show instructions instead of starting game directly
        StartInstructions();
    }

    void StartInstructions()
    {
        if (mainMenuCanvas != null)
        {
            mainMenuCanvas.gameObject.SetActive(false);
        }
        if (instructionCanvas != null)
        {
            showingInstructions = true;
            instructionCanvas.gameObject.SetActive(true);
            currentInstructionIndex = 0;
            ShowInstruction();
        }
        else
        {
            Debug.LogWarning("InstructionCanvas reference not set in GameManager! Starting game directly.");
            StartGame();
        }
    }

    void StartGame()
    {
        // Hide instruction canvas and reset player position
        if (instructionCanvas != null)
        {
            instructionCanvas.gameObject.SetActive(false);
        }
        if (mainMenuCanvas != null)
        {
            AudioManager.Instance.PlayGameplayMusic();
            mainMenuCanvas.gameObject.SetActive(false);
            if (pauseMenu != null)
            {
                pauseMenu.gameObject.SetActive(true);
            }
        }

        if (player != null)
        {
            player.transform.position = initialPlayerPosition;
        }

        totalXP = 0f; // Reset XP on new game
        UpdateXPText();

        // Start the game
        Time.timeScale = 1f;
        isGameStarted = true;
        shouldStartImmediately = false; // Reset the flag
    }

    void OnPauseButtonClicked()
    {
        AudioManager.Instance.PlayClickSound();
        PauseGame();
    }

    void PauseGame()
    {
        AudioManager.Instance.PlayPauseMusic();
        Time.timeScale = 0f;
        pauseMenu.gameObject.SetActive(false);
        resumeMenu.gameObject.SetActive(true);
        UpdateXPText(); // Update XP display when pausing
    }

    void OnResumeButtonClicked()
    {
        AudioManager.Instance.PlayClickSound();
        ResumeGame();
    }

    void ResumeGame()
    {
        AudioManager.Instance.PlayGameplayMusic();
        Time.timeScale = 1f;
        resumeMenu.gameObject.SetActive(false);
        pauseMenu.gameObject.SetActive(true);
    }

    public void GameOver()
    {
        AudioManager.Instance.PlayGameOverMusic();
        isGameStarted = false;
        gameOverMenu.gameObject.SetActive(true);
        pauseMenu.gameObject.SetActive(false);
        Time.timeScale = 0f;
        UpdateXPText(); // Update XP display on game over
        UpdateGameOverText();
    }

    void OnRestartButtonClicked()
    {
        AudioManager.Instance.PlayClickSound();
        Restart();
    }

    void Restart()
    {
        // Set flag to start immediately after reload
        shouldStartImmediately = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void OnBackToMainMenuButtonClicked()
    {
        AudioManager.Instance.PlayClickSound();
        BackToMainMenu();
    }

    public void BackToMainMenu()
    {
        // Fully reload the scene to reset everything
        shouldStartImmediately = false; // Ensure we go to main menu
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void OnColorButtonClicked(string colorName)
    {
        AudioManager.Instance.PlayClickSound();
        SetPaintColor(colorName);
    }

    // Method to set paint color via PaintManager
    void SetPaintColor(string colorName)
    {
        if (paintManager != null)
        {
            paintManager.SetPaintColor(colorName);
        }
        else
        {
            Debug.LogError("PaintManager reference is not set in GameManager!");
        }
    }

    public void AddXP(float amount)
    {
        totalXP += amount;
        UpdateXPText();
    }

    public void AddObstacleBonus()
    {
        totalXP += obstacleBonus;
        UpdateXPText();
    }

    void UpdateXPText()
    {
        if (xpText != null)
        {
            xpText.text = $"XP: {totalXP}";
        }
        else
        {
            Debug.LogWarning("xpText reference not set in GameManager!");
        }
    }

    public float GetTotalXP()
    {
        return totalXP;
    }

    void UpdateGameOverText()
    {
        if (gameOverText != null)
        {
            gameOverText.text = $"Final XP: {totalXP}";
        }
        else
        {
            Debug.LogWarning("gameOverText reference not set in GameManager!");
        }
    }

    // Instruction handling methods
    void ShowInstruction()
    {
        if (instructionSprites == null || instructionSprites.Length == 0)
        {
            Debug.LogWarning("No instruction sprites assigned in GameManager!");
            EndInstructions();
            return;
        }

        if (currentInstructionIndex >= instructionSprites.Length)
        {
            EndInstructions();
            return;
        }

        if (instructionImage != null)
        {
            instructionImage.sprite = instructionSprites[currentInstructionIndex];
            // Enable Next button only if there are more instructions
            if (nextButton != null)
            {
                nextButton.gameObject.SetActive(currentInstructionIndex < instructionSprites.Length - 1);
            }
            // Optional: Uncomment to enable auto-advance
            // StartCoroutine(AutoAdvanceInstruction(5f)); // Auto-advance after 5 seconds
        }
        else
        {
            Debug.LogWarning("InstructionImage reference not set in GameManager!");
            EndInstructions();
        }
    }

    void OnNextInstructionClicked()
    {
        AudioManager.Instance.PlayClickSound();
        currentInstructionIndex++;
        ShowInstruction();
    }

    void OnSkipInstructionsClicked()
    {
        AudioManager.Instance.PlayClickSound();
        EndInstructions();
    }

    void EndInstructions()
    {
        showingInstructions = false;
        if (instructionCanvas != null)
        {
            instructionCanvas.gameObject.SetActive(false);
        }
        StartGame(); // Proceed to start the game
    }

    // Optional: Auto-advance coroutine
    IEnumerator AutoAdvanceInstruction(float delay)
    {
        yield return new WaitForSecondsRealtime(delay); // Use real-time for paused game
        if (showingInstructions) // Only advance if still showing instructions
        {
            currentInstructionIndex++;
            ShowInstruction();
        }
    }
}