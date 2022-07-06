using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardManager : MonoBehaviour
{
    [SerializeField] private GameObject cardPrefab;

    [SerializeField] private StatMapper[] StatSetter;
    [SerializeField] private SOCard[] HandSetter;

    private Dictionary<EStatType, int> Stats = new Dictionary<EStatType, int>();
    private List<ActiveCard> ActivePassives = new List<ActiveCard>();
    private List<RectTransform> ActivePassivesRT = new List<RectTransform>();

    private List<SOCard> Deck = new List<SOCard>();
    private List<SOCard> DiscardPile = new List<SOCard>();

    private List<SOCard> Hand = new List<SOCard>();
    private List<RectTransform> HandRT = new List<RectTransform>(); 

    private SOCard heldCard;
    private int heldIndex;
    
    private void Setup()
    {
        Deck = new List<SOCard>(HandSetter);
        foreach(var stat in StatSetter)
        {
            Stats.Add(stat.statType, stat.value);
        }
        DrawStartHand();
        heldCard = Hand[Hand.Count - 1];
    }
    private void Start()
    {
        Setup();
    }
    private void Update()
    {
        InputMechanics();
    }
    private void InputMechanics()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (heldIndex - 1 < 0)
                heldIndex = Hand.Count - 1;
            else
                heldIndex--;
            heldCard = Hand[heldIndex];
            MarkActiveCard();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (heldIndex + 1 >= Hand.Count)
                heldIndex = 0;
            else
                heldIndex++;
            heldCard = Hand[heldIndex];
            MarkActiveCard();
        }
        if (Input.GetMouseButtonDown(0))
        {
            UseCard();
        }
        if (Input.GetMouseButtonDown(1))
        {
            UseCard(true);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            DrawNewHand();
        }
    }
    private void MarkActiveCard()
    {
        int i = 0;
        foreach (RectTransform rt in HandRT)
        {
            if (i == heldIndex)
            {
                Canvas c = rt.GetComponent<Canvas>();
                c.sortingOrder = Stats[EStatType.HandLimit];
                rt.localScale = new Vector2(1.2f, 1.2f);
            }
            else
            {
                Canvas c = rt.GetComponent<Canvas>();
                c.sortingOrder = i;
                rt.localScale = Vector2.one;
                //rt.transform.SetSiblingIndex(i);
            }
            i++;
        }
    }

    private RectTransform SpawnRTCard(SOCard cardPreset, Transform parent, bool passiveCardMode = false, bool special = false)
    {
        CardScript cs = Instantiate(cardPrefab, parent.position, Quaternion.identity).GetComponent<CardScript>();
        cs.transform.SetParent(parent, false);
        cs.Setup(cardPreset, passiveCardMode, special);

        return cs.GetComponent<RectTransform>();
    }
    public void UseCard(bool special = false)
    {
        if (!HandRT[heldIndex].GetComponent<CardScript>().Ready)
            return;

        heldCard.UseCard(special); // use card
        DiscardPile.Add(heldCard); // add to DiscardPile
        
        if(special)
        {
            if(heldCard.cardPhaseSpecial != ECardPhase.Instant)
            {
                ActivePassives.Add(new ActiveCard(heldCard, special));
                RectTransform rt = SpawnRTCard(heldCard, GameManager.Instance.Passive, true, special);
                ActivePassivesRT.Add(rt);
            }
        }
        else
        {
            if(heldCard.cardPhaseNormal != ECardPhase.Instant)
            {
                ActivePassives.Add(new ActiveCard(heldCard, special));
                RectTransform rt = SpawnRTCard(heldCard, GameManager.Instance.Passive, true, special);
                ActivePassivesRT.Add(rt);
            }
        }

        Hand.RemoveAt(heldIndex);
        HandRT[heldIndex].GetComponent<CardScript>().DestroyCard();
        HandRT.RemoveAt(heldIndex);

        DrawCards();
        heldCard = Hand[heldIndex];
        MarkActiveCard();
    }
    private void ShuffleCards(List<SOCard> list)
    {
        System.Random rng = new System.Random();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            SOCard value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
    public void RemoveRtPassive(SOCard cardSO)
    {
        foreach (var cardRT in GameManager.Instance.CardManager.ActivePassivesRT)
        {
            if (cardRT.GetComponent<CardScript>().CardPreset == cardSO)
            {
                CardScript cs = cardRT.GetComponent<CardScript>();
                cs.DestroyCard();
                break;
            }
        }
        ActivePassivesRT.RemoveAll(card => card.GetComponent<CardScript>().CardPreset == cardSO || card == null);
    }
    private void RemovePassive(ECardPhase phase)
    {
        var toDest = new List<ActiveCard>();
        foreach (var activePassive in ActivePassives)
        {
            if (activePassive.special && activePassive.card.cardPhaseSpecial == phase)
            {
                toDest.Add(activePassive);
            }
            else if(!activePassive.special && activePassive.card.cardPhaseNormal == phase)
            {
                toDest.Add(activePassive);
            }
        }
        foreach (var item in toDest)
        {
            item.card.RemovePassive(item.special);
            Debug.Log("Removing: " + item.card.cardName);
        }
    }
    public void RemoveBuff(DefaultBuff buff, bool special)
    {
        if(special)
        {
            foreach (var stat in buff.specialBuffs)
            {
                ModifyStat(stat.statType, -stat.value);
            }
        }
        else
        {
            foreach (var stat in buff.normalBuffs)
            {
                ModifyStat(stat.statType, -stat.value);
            }
        }
        ActivePassives.RemoveAll(card => card.card == buff);
    }
    public void ModifyStat(EStatType statType, int value)
    {
        Stats[statType] += value;
    }

    #region AddCard
    public void AddAbbility()
    {
        RemovePassive(ECardPhase.TillNextAbbility);
    }
    public void AddBuffValue(DefaultBuff card, bool special)
    {
        RemovePassive(ECardPhase.TillNextBuff);
        if(special)
        {
            foreach (var stat in card.specialBuffs)
            {
                ModifyStat(stat.statType, stat.value);
            }
        }
        else
        {
            foreach (var stat in card.normalBuffs)
            {
                ModifyStat(stat.statType, stat.value);
            }
        }
    }
    #endregion

    #region Drawing
    private void DrawStartHand()
    {
        ShuffleCards(Deck);
        DrawCards();
        heldIndex = Hand.Count - 1;
        MarkActiveCard();
    }
    public void DrawCards()
    {
        int count = Stats[EStatType.HandLimit] - Hand.Count; // ile dobrac

        if (count == 0) // jesli zero to nie dobieraj, wyjdz z tej metody
            return;

        List<SOCard> addCards = new List<SOCard>();
        if (count > Deck.Count) // tutaj ZAWSZE count > 0
        {
            addCards.AddRange(Deck);
            Deck.Clear();
            count -= Deck.Count;
            ShuffleCards(DiscardPile);
            Deck.AddRange(DiscardPile);
            DiscardPile.Clear();
        }
        addCards.AddRange(Deck.GetRange(0, count));
        foreach (var card in addCards)
        {
            RectTransform rt = SpawnRTCard(card, GameManager.Instance.Hand);
            //RectTransform rt = GameManager.Instance.CardsSlotManager.SetCard(card);
            HandRT.Add(rt);
        }
        Hand.AddRange(addCards);
        Deck.RemoveRange(0, count);
        //heldIndex = Hand.Count - 1;
        MarkActiveCard();
    }
    public void DrawNewHand()
    {
        DiscardPile.AddRange(Hand);
        Hand.Clear();
        foreach (RectTransform rc in HandRT)
        {
            rc.GetComponent<CardScript>().DestroyCard();
        }
        HandRT.Clear();
        DrawCards();
    }
    #endregion
}
public class ActiveCard
{
    public ActiveCard(SOCard card, bool special)
    {
        this.card = card;
        this.special = special;
    }
    public SOCard card;
    public bool special;
}