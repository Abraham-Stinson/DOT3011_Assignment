using Unity.VisualScripting;
using UnityEngine;

public class UIManager : MonoBehaviour, IGameStateListener
{
  public static UIManager instance { get; private set; }
  [Header("Panels")]
  [SerializeField] private GameObject menuPanel;
  [SerializeField] private GameObject gameIntro;
  [SerializeField] private GameObject characterSelectionPanel;
  [SerializeField] private GameObject inGamePanel;
  [SerializeField] private GameObject pausePanel;
  [SerializeField] private GameObject gameOverPanel;

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
    /*gameOverPanel.SetActive(gameState == EGameState.GAMEOVER);*/
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
}
