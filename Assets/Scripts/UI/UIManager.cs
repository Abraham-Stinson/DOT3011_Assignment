using Microsoft.Unity.VisualStudio.Editor;
using Unity.VisualScripting;
using UnityEngine;
using Image = UnityEngine.UI.Image;

public class UIManager : MonoBehaviour, IGameStateListener
{
  public static UIManager instance { get; private set; }
  [Header("Panels")]
  [SerializeField] private GameObject menuPanel;
  [SerializeField] private GameObject gameIntro;
  [SerializeField] private GameObject characterSelectionPanel;
  [SerializeField] private GameObject inGamePanel;
  [SerializeField] private GameObject pausePanel;
  [SerializeField] private GameObject gameOverWinPanel;
  [SerializeField] private GameObject gameOverLosePanel;

  [Header("Player Health Bar In Game UI")]
  [SerializeField] private Image playerHealthBarInGameUI;

  void Awake()
  {
    instance = this;
  }
  public void GameStateChangedCallBack(EGameState gameState)
  {
    menuPanel.SetActive(gameState == EGameState.MAINMENU);
    //gameIntro.SetActive(gameState == EGameState.GAMEINTRO);
    characterSelectionPanel.SetActive(gameState == EGameState.CHRACTERSELECTION);
    inGamePanel.SetActive(gameState == EGameState.INGAME);
    pausePanel.SetActive(gameState == EGameState.PAUSE);
    gameOverWinPanel.SetActive(gameState == EGameState.GAMEOVERWIN);
    gameOverLosePanel.SetActive(gameState == EGameState.GAMEOVERLOSE);
  }
  private bool isGamePaused = false;
  public void PauseMenuToggle()
  {
    isGamePaused = !isGamePaused;
    if (isGamePaused && GameManager.instance.gameState == EGameState.INGAME)
    {
      GameManager.instance.SetGameState(EGameState.PAUSE);
      Time.timeScale = 0f;
      Cursor.lockState = CursorLockMode.None;
      Cursor.visible = true;
    }
    else if (!isGamePaused && GameManager.instance.gameState == EGameState.PAUSE)
    {
      GameManager.instance.SetGameState(EGameState.INGAME);
      Time.timeScale = 1f;
      Cursor.lockState = CursorLockMode.Locked;
      Cursor.visible = false;
    }

  }

  public Image GetInGameHealthBar()
    {
        return playerHealthBarInGameUI;
    }
}
