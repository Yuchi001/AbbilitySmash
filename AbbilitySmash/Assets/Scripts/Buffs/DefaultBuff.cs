using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Cards/Buff/DefaultBuff")]
public class DefaultBuff : SOCard
{
    [Header("Buff Values")]
    public StatMapper[] normalBuffs;
    public StatMapper[] specialBuffs;
    public override void UseCard(bool special)
    {
        GameManager.Instance.CardManager.AddBuffValue(this, special);
    }
    public override void RemovePassive(bool special)
    {
        base.RemovePassive(special);
        GameManager.Instance.CardManager.RemoveBuff(this, special);
    }
}
