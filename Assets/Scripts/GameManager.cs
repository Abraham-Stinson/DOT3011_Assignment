using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private GameObject player;
    private GameObject freeLookCamera;
    //public static GameManager instance;
    public EGameState gameState;
    void Start()
    {
        /*Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;*/
        SetGameState(EGameState.MAINMENU);
        player = GameObject.Find("Third_Person_Player");
        freeLookCamera = GameObject.Find("FreeLook Camera");
        player.GetComponent<ThirdPersonController>().enabled = false;
        freeLookCamera.GetComponent<Cinemachine.CinemachineFreeLook>().enabled = false;
        
    }
    void Update()
    {
        
    }

    public void SetGameState(EGameState gameState)
    {
        this.gameState = gameState;
        IEnumerable<IGameStateListener> gameStateListeners = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<IGameStateListener>();

        foreach (IGameStateListener dependency in gameStateListeners)
        {
            dependency.GameStateChangedCallBack(gameState);
        }
    }

    /*public void GameIntro()
    {
        SetGameState(EGameState.GAMEINTRO);
    }*/
    public void CharacterSelection()
    {
        SetGameState(EGameState.CHRACTERSELECTION);
    }
    public void InGame(int selectedCharacterIndex)
    {
        SetGameState(EGameState.INGAME);
        SpawnChrahterAtIndex(selectedCharacterIndex);
    }

    enum PlayerType
    {
        WARRIOR,//jop male
        MAGE,//electro shock female
        RANGER//flashlight non selected
    }
    public void SpawnChrahterAtIndex(int index)
    {
        //TO DO: SPAWN CHARACTER AT INDEX
        switch (index)
        {
            case (int)PlayerType.WARRIOR:
                Debug.Log("Spawn Warrior");
                break;
            case (int)PlayerType.MAGE:
                Debug.Log("Spawn Mage");
                break;
            case (int)PlayerType.RANGER:
                Debug.Log("Spawn Ranger");
                break;
            default:
                Debug.Log("Spawn Default Character");
                break;
        }
        player.GetComponent<ThirdPersonController>().enabled = true;
        freeLookCamera.GetComponent<Cinemachine.CinemachineFreeLook>().enabled = true;
        
    }
}
