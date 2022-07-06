using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Abbility/FireBall")]
public class FireBall : SOCard
{
    public override void UseCard(bool special)
    {
        GameManager.Instance.CardManager.AddAbbility();
    }
    public override void RemovePassive(bool special)
    {
        base.RemovePassive(special);
    }
}
