using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }
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

    [SerializeField] private GameObject player;
    private GameObject freeLookCamera;
    public EGameState gameState;

    [SerializeField] public Transform spawnObjectParent;
    void Awake()
    {
        instance = this;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
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

    public void ToMainMenu()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        RemoveCurrentPlayer();
        SetGameState(EGameState.MAINMENU);
    }
    public void CharacterSelection()
    {
        SetGameState(EGameState.CHRACTERSELECTION);
    }
    public void InGame(int selectedCharacterIndex)
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        SetGameState(EGameState.INGAME);
        SpawnChrahterAtIndex(selectedCharacterIndex);
    }


    public void SpawnChrahterAtIndex(int index)
    {
        Debug.Log(characters[index].characterName + " spawned");
        player = Instantiate(characters[index].characterPrefab, spawnObjectParent.position, spawnObjectParent.rotation, spawnObjectParent.transform);

        ThirdPersonController.instance.GetAnimatorCompononet();
        GameObject heroNameUI = GameObject.Find("HeroName");
        heroNameUI.GetComponent<TextMeshProUGUI>().text = characters[index].characterName;
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

    void RemoveCurrentPlayer()
    {
        if (player != null)
        {
            Destroy(player);
        }
    }
}
