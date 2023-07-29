using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour, ICardInteractionHandler
{
    /////////////SINGLETON IMPLEMENTATION/////////////////
    private static GameManager instance = null;
    public static GameManager Instance => instance;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;
        }
        DontDestroyOnLoad(this.gameObject);
        InitGame();
    }
    ///////////////////////////////////////////////////////

    public enum GameState
    {
        MAIN,
        NOINTERACTION,
        SCRY
    }

    private GameState currentGameState = GameState.MAIN;

    public GameObject sceneVFX;

    public GameObject deck;
    public GameObject scryStone;

    public GameObject cardPrefab;
    public GameObject objectCardPrefab;
    public GameObject monsterCardPrefab;
    public GameObject currentCard;

    public GameObject whiteDicePrefab;
    public GameObject redDicePrefab;
    public GameObject whiteDice;
    public GameObject redDice;
    private int whiteDiceResult = -1;
    private int redDiceResult = -1;

    public GameObject cardListPrefab;
    public GameObject scryCardList;

    public GameObject holdedCard;
    public int holdCardPlaneHeight = 5;
    public float holdCardScaleMultiplier = 0.8f;
    public float cardListScaleMultiplier = 0.5f;
    private Plane holdCardPlane; 

    //gameState
    bool canRevealNextCard = true;

    public GameObject lifeText;
    public GameObject strengthText;
    public GameObject coinText;

    public GameObject magicText;
    public GameObject deathIcon;

    public int playerStartingLife = 3;
    public int playerStartingStrength = 0;
    public int playerStartingCoin = 5;

    public int playerLife { get; private set; }
    public int playerStrength { get; private set; }
    public int playerCoin { get; private set; }

    public int playerStartingMagic = 3;
    public int playerMagic { get; private set; }

    private void InitGame()
    {
        playerLife = playerStartingLife;
        playerStrength = playerStartingStrength;
        playerCoin = playerStartingCoin;
        playerMagic = playerStartingMagic;
        lifeText.GetComponent<TMP_Text>().text = playerLife.ToString();
        strengthText.GetComponent<TMP_Text>().text = playerStrength.ToString();
        canRevealNextCard = true;
        currentGameState = GameState.MAIN;
        holdCardPlane = new Plane(Vector3.up, -holdCardPlaneHeight);
    }
    public void RestartGame()
    {
        if (currentCard)
            Destroy(currentCard);
        deck.GetComponent<Deck>().ReShuffle();
        scryStone.GetComponent<ScryStone>().Refill(100);
        InitGame();
    }


    public GameObject CreateCardObject(CardInstance cardInstance)
    {
        CardData data = cardInstance.dataInstance;
        GameObject cardObject;
        if (data is ObjectCardData)
        {
            cardObject = Instantiate(objectCardPrefab);
        }
        else if(data is MonsterCardData)
        {
            cardObject = Instantiate(monsterCardPrefab);
        }
        else
        {
            cardObject = Instantiate(cardPrefab);
        }

        cardObject.GetComponent<Card>().ApplyCardData(cardInstance);
        return cardObject;
    }

    public void OnDeckClicked(int deckSize)
    {
        if(canRevealNextCard && currentGameState == GameState.MAIN)
        {
            if(deckSize > 0)
            {
                CardInstance revealedCard = deck.GetComponent<Deck>().GetTopCard();
                currentCard = CreateCardObject(revealedCard);
                currentCard.GetComponent<Card>().interactionHandler = this;
                canRevealNextCard = false;
            }
        }
    }

    public void OnScryStoneClicked(bool isActivable)
    {
        if(isActivable && currentGameState == GameState.MAIN)
        {
            scryStone.GetComponent<ScryStone>().Activate();
            sceneVFX.GetComponent<SceneVFX>().SwitchMode(SceneVFX.Mode.SCRY);

            currentGameState = GameState.SCRY;
            scryCardList = Instantiate(cardListPrefab);
            InteractibleCardList cardListComponent = scryCardList.GetComponent<InteractibleCardList>();
            List<CardInstance> topCards = deck.GetComponent<Deck>().GetTopCards(scryStone.GetComponent<ScryStone>().cardsSeen);
            foreach(CardInstance card in topCards)
            {
                GameObject cardObject = CreateCardObject(card);
                Vector3 prefabScale = cardPrefab.transform.localScale;
                cardObject.transform.localScale = new Vector3(prefabScale.x * cardListScaleMultiplier, prefabScale.y, prefabScale.z * cardListScaleMultiplier);
                cardListComponent.AddCardToList(cardObject);
            }
        }
        else if(currentGameState == GameState.SCRY)
        {
            currentGameState = GameState.MAIN;
            sceneVFX.GetComponent<SceneVFX>().SwitchMode(SceneVFX.Mode.CANDLE);

            for(int i = scryCardList.GetComponent<InteractibleCardList>().cardList.Count - 1; i >= 0; i--)
            {
                GameObject card = scryCardList.GetComponent<InteractibleCardList>().cardList[i];
                deck.GetComponent<Deck>().ShuffleCardInDeck(card.GetComponent<Card>().data, 0, 0);
                Destroy(card);
            }

            Destroy(scryCardList);
        }
    }

    public void OnDeckRightClicked()
    {
        RestartGame();
    }

    public void AddCardToDeck(CardInstance card)
    {
        deck.GetComponent<Deck>().ShuffleCardInDeck(card);
    }

    public void AddLife(int amount)
    {
        playerLife += amount;
        lifeText.GetComponent<TMP_Text>().text = playerLife.ToString();
        if (playerLife <= 0)
        {
            playerLife = 0;
            deathIcon.SetActive(true);
            deathIcon.GetComponent<BasicAnimation>().PlayAnimation();
            Invoke("RestartGame", deathIcon.GetComponent<BasicAnimation>().duration);
            currentGameState = GameState.NOINTERACTION;
        }
    }

    public void AddStrength(int amount)
    {
        playerStrength += amount;
        if (playerStrength < 0) playerStrength = 0;
        strengthText.GetComponent<TMP_Text>().text = playerStrength.ToString();
    }

    public void AddCoin(int amount)
    {
        playerCoin += amount;
        if (playerCoin < 0) playerCoin = 0;
        coinText.GetComponent<TMP_Text>().text = playerCoin.ToString();
    }

    public void AddMagic(int amount)
    {
        playerMagic += amount;
        if (playerMagic < 0) playerMagic = 0;
        magicText.GetComponent<TMP_Text>().text = playerMagic.ToString();
    }

    public void OnCartHoldStart(GameObject card)
    {
        holdedCard = card;
        Vector3 prefabScale = cardPrefab.transform.localScale;
        card.transform.localScale = new Vector3(prefabScale.x * holdCardScaleMultiplier, prefabScale.y, prefabScale.z * holdCardScaleMultiplier);
    }

    public void OnCartHoldStop(GameObject card)
    {
        holdedCard = null;
        Vector3 prefabScale = cardPrefab.transform.localScale;
        card.transform.localScale = new Vector3(prefabScale.x * cardListScaleMultiplier, prefabScale.y, prefabScale.z * cardListScaleMultiplier);

        Vector3 targetPosition = new Vector3();
        Plane cardListPlane = new Plane(Vector3.up, -scryCardList.transform.position.y);
        float distance;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (cardListPlane.Raycast(ray, out distance))
        {
            targetPosition = ray.GetPoint(distance);
        }
        scryCardList.GetComponent<InteractibleCardList>().ReleaseCardAtPos(targetPosition, card);
    }

    public void StartBattleAgainstMonster()
    {
        currentGameState = GameState.NOINTERACTION;
        RollDice();
    }

    private void CompleteBattle()
    {
        MonsterCardData monsterData = (MonsterCardData)currentCard.GetComponent<Card>().data.dataInstance;
        if(monsterData.strength + redDiceResult > playerStrength + whiteDiceResult)
        {
            deck.GetComponent<Deck>().ShuffleCardInDeck(currentCard.GetComponent<Card>().data);
            AddLife(-1);
        }
        Destroy(currentCard);
        currentGameState = GameState.MAIN;
    }

    private void RollDice()
    {
        whiteDiceResult = -1;
        redDiceResult = -1;

        if (whiteDice)
            Destroy(whiteDice);

        if (redDice)
            Destroy(redDice);

        whiteDice = Instantiate(whiteDicePrefab);
        redDice = Instantiate(redDicePrefab);
        whiteDice.GetComponent<DiceRolling>().RollTheDice();
        redDice.GetComponent<DiceRolling>().RollTheDice();
    }
    public void OnDiceResult(GameObject dice, int result)
    {
        if(dice == whiteDice)
        {
            whiteDiceResult = result;
        }
        else if(dice == redDice)
        {
            redDiceResult = result;
        }

        if(whiteDiceResult > 0 && redDiceResult > 0)
            CompleteBattle();
    }

    private void Update()
    {
        if(holdedCard)
        {
            Vector3 targetPosition = new Vector3();
   
            float distance;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (holdCardPlane.Raycast(ray, out distance))
            {
                targetPosition = ray.GetPoint(distance);
            }
            holdedCard.transform.position = targetPosition;
        }
    }

    //////////////////// HANDLE CARD INTERACTION///////////////////////
    public void OnCardPressed(GameObject card)
    {

    }
    public void OnCardReleased(GameObject card)
    {

    }
    public void OnCardClicked(GameObject card)
    {
        if (currentGameState == GameState.MAIN)
        {
            currentCard.GetComponent<Card>().Resolve();
            canRevealNextCard = true;
            if (!scryStone.GetComponent<ScryStone>().isActivable)
                scryStone.GetComponent<ScryStone>().Refill(1);
        }
    }
    public void OnCardEntered(GameObject card)
    {

    }
    public void OnCardExited(GameObject card)
    {

    }
    ///////////////////////////////////////////////////////////////////

}
