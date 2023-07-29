using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICardInteractionHandler
{
    public void OnCardPressed(GameObject card);
    public void OnCardReleased(GameObject card);
    public void OnCardClicked(GameObject card);
    public void OnCardEntered(GameObject card);
    public void OnCardExited(GameObject card);
}
