using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ScryStone : MonoBehaviour, IPointerClickHandler
{
    public GameObject particleSpread;
    public GameObject particleGlow;
    public GameObject particleBurst;

    public uint cardsSeen = 3;
    public uint minCardsToDiscard = 0;
    public uint maxCardsToDiscard = 0;
    public uint minCardsToBottom = 0;
    public uint maxCardsToBottom = 3;
    public bool canOrderBottomCards = false;
    public uint minCardsToTop = 0;
    public uint maxCardsToTop = 3;
    public bool canOrderTopCards = true;
    public bool isActivable = true;
    public int amountNeededToRefill = 3;

    private int currentRefillAmount = 0;

    public void Activate()
    {
        isActivable = false;
        ParticleSystem.EmissionModule particleSpreadEmission = particleSpread.GetComponent<ParticleSystem>().emission;
        ParticleSystem.EmissionModule particleGlowEmission = particleGlow.GetComponent<ParticleSystem>().emission;
        particleSpreadEmission.enabled = false;
        particleGlowEmission.enabled = false;
        Material scryStoneMat = GetComponent<MeshRenderer>().material;
        scryStoneMat.color = new Color(0.5f, 0.5f, 0.5f);
        currentRefillAmount = 0;
    }

    public void Refill(int amount)
    {
        currentRefillAmount += amount;
        if(currentRefillAmount >= amountNeededToRefill)
        {
            isActivable = true;
            ParticleSystem.EmissionModule particleSpreadEmission = particleSpread.GetComponent<ParticleSystem>().emission;
            ParticleSystem.EmissionModule particleGlowEmission = particleGlow.GetComponent<ParticleSystem>().emission;
            particleSpreadEmission.enabled = true;
            particleGlowEmission.enabled = true;
            particleBurst.GetComponent<ParticleSystem>().Play();
            Material scryStoneMat = GetComponent<MeshRenderer>().material;
            scryStoneMat.color = new Color(1, 1, 1);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        GameManager.Instance.OnScryStoneClicked(isActivable);
    }
}
