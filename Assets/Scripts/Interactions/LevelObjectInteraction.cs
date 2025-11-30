using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelObjectInteraction : MonoBehaviour, IInteractable
{

    [SerializeField] private string loadToScene;
    [SerializeField] private float tpPlayerPosY = 3;
    private MeshRenderer meshRenderer;
    private Material cursedMaterial;


    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }
    public void Interact()
    {
        LevelManager.instance.ModifyFormerPositionForReturn(this.transform.position + (this.transform.forward * 3));
        Debug.Log($"Obje ile etkileşime geçildi {loadToScene} yükleniyor");

        StartCoroutine(LoadSceneAndTeleport());
        /*SceneManager.LoadScene(loadToScene, LoadSceneMode.Additive);

        playerSpawnPoint.transform.position = transform.Find("PlayerSpawnPoint").position + new Vector3(0, tpPlayerPosY, 0);
        ThirdPersonController.instance.characterController.Move(playerSpawnPoint.transform.position);*/
    }

    IEnumerator LoadSceneAndTeleport()
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(loadToScene, LoadSceneMode.Additive);
        LevelManager.instance.ModifyCurrentLevelName(loadToScene);
        while (!asyncOperation.isDone)
        {
            yield return null;
        }

        TeleportPlayerToLevel();

        Destroy(GetComponent<LevelObjectInteraction>());
        SetDefaultMaterial();
    }
    private void TeleportPlayerToLevel()
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

    public void TeleportPlayerBack()
    {

    }

    public void ChangeMaterialOfArtifact(Material brokenMaterial)
    {
        cursedMaterial = brokenMaterial;
        if (meshRenderer != null)
        {
            Debug.Log("Curslendi");
            List<Material> materialList = new List<Material>(meshRenderer.materials);
            materialList.Add(brokenMaterial);
            meshRenderer.materials = materialList.ToArray();
        }
        else
        {
            Debug.Log("Null");
        }
    }
    public void SetDefaultMaterial()
    {
        if (meshRenderer != null)
        {
            List<Material> materialList = new List<Material>(meshRenderer.materials);
            for (int i = materialList.Count - 1; i >= 0; i--)
            {
                if (materialList[i].name.StartsWith(cursedMaterial.name))
                {
                    materialList.RemoveAt(i);
                    break;
                }
            }
        }
        else
        {
            Debug.Log("Null");
        }
    }

    public void AppendLevelName(string levelName)
    {
        loadToScene = levelName;
    }
}
