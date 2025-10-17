using UnityEngine;

public class UIManager : MonoBehaviour,IGameStateListener
{
    [Header("Panels")] 
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject gameIntro;
    [SerializeField] private GameObject characterSelectionPanel;
    [SerializeField] private GameObject inGamePanel;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject gameOverPanel;
    

  public void GameStateChangedCallBack(EGameState gameState)
  {
    menuPanel.SetActive(gameState == EGameState.MAINMENU);
    //gameIntro.SetActive(gameState == EGameState.GAMEINTRO);
    characterSelectionPanel.SetActive(gameState == EGameState.CHRACTERSELECTION);
    /*inGamePanel.SetActive(gameState == EGameState.INGAME);
    pausePanel.SetActive(gameState == EGameState.PAUSE);
    gameOverPanel.SetActive(gameState == EGameState.GAMEOVER);*/
  }
}
