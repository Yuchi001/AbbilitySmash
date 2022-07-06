using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SOCard : ScriptableObject
{
    [Header("Card Settings")]
    public string cardName;
    public ECardType cardType;
    public ECardPhase cardPhaseNormal;
    public ECardPhase cardPhaseSpecial;
    public Sprite soulSprite;
    public Sprite soulSpritePassive;
    public Sprite cardBackSprite;
    public Sprite passiveBackSprite;
    public abstract void UseCard(bool special);
    public virtual void RemovePassive(bool special)
    {
        GameManager.Instance.CardManager.RemoveRtPassive(this);
    }
}
[System.Serializable]
public class StatMapper
{
    public EStatType statType;
    public int value;
}