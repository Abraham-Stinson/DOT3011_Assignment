using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeInteraction : MonoBehaviour, IInteractable
{

    public void Interact()
    {

        if (LevelManager.instance != null)
        {
            Debug.Log($"Intreaction with tree and museum artifacts: {LevelManager.instance.isMuseumArtifactsCursedCheck()}");
            if (!LevelManager.instance.isMuseumArtifactsCursedCheck())
            {
                LevelManager.instance.ArtifectsInMuseum();
                Debug.Log("Interaction: Agacla etkilesime gecildi");
            }

        }


    }
}
