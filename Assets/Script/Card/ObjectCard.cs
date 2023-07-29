using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ObjectCard : Card
{
    public GameObject lifeAddText;
    public GameObject strengthAddText;
    public GameObject coinAddText;

    public override void ApplyCardData(CardInstance data)
    {
        base.ApplyCardData(data);
        ObjectCardData cardData = (ObjectCardData)data.dataInstance;
        lifeAddText.GetComponent<TMP_Text>().text = cardData.lifeAdd.ToString();
        strengthAddText.GetComponent<TMP_Text>().text = cardData.strengthAdd.ToString();
        coinAddText.GetComponent<TMP_Text>().text = cardData.coinAdd.ToString();
    }

    public override void Resolve()
    {
        ObjectCardData cardData = (ObjectCardData)data.dataInstance;
        GameManager.Instance.AddStrength(cardData.strengthAdd);
        GameManager.Instance.AddCoin(cardData.coinAdd);
        GameManager.Instance.AddLife(cardData.lifeAdd);
        Destroy(gameObject);
    }
}
