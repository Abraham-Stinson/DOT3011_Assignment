using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelObjectInteraction : MonoBehaviour, IInteractable
{

    [SerializeField] private string loadToScene;
    [SerializeField] private float tpPlayerPosY = 3;
    public void Interact()
    {
        Debug.Log($"Obje ile etkileşime geçildi {loadToScene} yükleniyor");

        StartCoroutine(LoadSceneAndTeleport());
        /*SceneManager.LoadScene(loadToScene, LoadSceneMode.Additive);

        playerSpawnPoint.transform.position = transform.Find("PlayerSpawnPoint").position + new Vector3(0, tpPlayerPosY, 0);
        ThirdPersonController.instance.characterController.Move(playerSpawnPoint.transform.position);*/
    }

    IEnumerator LoadSceneAndTeleport()
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(loadToScene, LoadSceneMode.Additive);

        while (!asyncOperation.isDone)
        {
            yield return null;
        }

        TeleportPlayer();
    }
    private void TeleportPlayer()
    {
        var player = ThirdPersonController.instance;
        GameObject spawnObj = GameObject.Find("PlayerSpawnPoint");
        Vector3 targetPos = spawnObj.transform.position + new Vector3(0, tpPlayerPosY, 0);

        CharacterController cc = player.GetComponent<CharacterController>();
        if (cc != null)
        {
            cc.enabled = false;
        }

        player.transform.position = targetPos;

        if (cc != null)
        {
            cc.enabled = true;
        }
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(loadToScene.ToString());
    }
}
