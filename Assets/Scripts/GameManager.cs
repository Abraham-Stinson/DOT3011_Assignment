using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum CharacterType
    {
        WARRIOR,//jop male
        MAGE,//electro shock female
        RANGER//flashlight non selected
    }
    [System.Serializable]
    public class CharacterData
    {
        public CharacterType characterType;
        public string characterName;
        public GameObject characterPrefab;
        //public Sprite weaponIcon;
    }

    [SerializeField] private CharacterData[] characters = new CharacterData[3];

    private GameObject player;
    private GameObject freeLookCamera;
    public EGameState gameState;

    void Start()
    {
        /*Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;*/
        SetGameState(EGameState.MAINMENU);
        freeLookCamera = GameObject.Find("FreeLook Camera");
        
        ActiveControl(false);
        /*player = GameObject.Find("Third_Person_Player");
        player.GetComponent<ThirdPersonController>().enabled = false;
        ThirdPersonController.instance.playerInputActions.Player.Disable();*/

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
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        SetGameState(EGameState.INGAME);
        SpawnChrahterAtIndex(selectedCharacterIndex);
    }


    public void SpawnChrahterAtIndex(int index)
    {
        //TO DO: SPAWN CHARACTER AT INDEX
        switch (index)
        {
            case (int)CharacterType.WARRIOR:
                Debug.Log("Spawn Warrior");
                break;
            case (int)CharacterType.MAGE:
                Debug.Log("Spawn Mage");
                break;
            case (int)CharacterType.RANGER:
                Debug.Log("Spawn Ranger");
                break;
                /*default:
                    Debug.Log("Spawn Default Character");
                    break;*/
        }
        ActiveControl(true);
        /*player.GetComponent<ThirdPersonController>().enabled = true;
        freeLookCamera.GetComponent<Cinemachine.CinemachineFreeLook>().enabled = true;
        ThirdPersonController.instance.playerInputActions.Player.Enable();*/

    }

    public void ActiveControl(bool isActive)
    {
        if (isActive)
        {
            freeLookCamera.GetComponent<Cinemachine.CinemachineFreeLook>().enabled = true;
            ThirdPersonController.instance.playerInputActions.Player.Enable();
        }
        else
        {
            freeLookCamera.GetComponent<Cinemachine.CinemachineFreeLook>().enabled = false;
            ThirdPersonController.instance.playerInputActions.Player.Disable();
        }
    }
    void OnDisable()
    {
        ThirdPersonController.instance.playerInputActions.Player.Disable();
    }
}
