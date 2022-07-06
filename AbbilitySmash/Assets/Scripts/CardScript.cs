using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CardScript : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameTextField;
    [SerializeField] private TextMeshProUGUI upBonusTextField;
    [SerializeField] private TextMeshProUGUI downBonusTextField;
    [SerializeField] private TextMeshProUGUI passiveTextField;
    [SerializeField] private Image cardImage => GetComponent<Image>();
    [SerializeField] private Image specialSprite;

    [SerializeField] private Image coolDownImage;

    public bool Ready
    {
        get;
        private set;
    }
    private bool passive;

    public SOCard CardPreset { get; private set; }

    [SerializeField] private float prepTime;
    private float _timer = 0;

    public void Setup(SOCard cardPreset, bool passive = false, bool special = false)
    {
        Ready = false;
        this.CardPreset = cardPreset;
        this.passive = passive;

        if (passive)
            SetCardFields(special);
        else
            SetCardFields();
    }
    private void SetCardFields(bool special)
    {
        upBonusTextField.text = "";
        downBonusTextField.text = "";
        cardImage.sprite = CardPreset.passiveBackSprite;
        nameTextField.text = CardPreset.cardType.ToString();

        switch (CardPreset.cardType)
        {
            case ECardType.Buff:
                DefaultBuff db = CardPreset as DefaultBuff;
                if(special == false)
                {
                    passiveTextField.text = "";
                    foreach (var buff in db.normalBuffs)
                    {
                       passiveTextField.text += $"{buff.statType} {buff.value}. ";
                    }
                }
                else
                {
                    passiveTextField.text = "";
                    foreach (var buff in db.specialBuffs)
                    {
                        passiveTextField.text += $"{buff.statType} {buff.value}. ";
                    }
                }
                break;
            case ECardType.Abbility:

                break;
        }

        coolDownImage.fillAmount = 0;
        Color temp = cardImage.color;
        temp.a = 0.7f;
        cardImage.color = temp;

        foreach (var image in GetComponentsInChildren<Image>())
        {
            temp = image.color;
            temp.a = 0.7f;
            image.color = temp;
        }

        foreach(var textField in GetComponentsInChildren<TextMeshProUGUI>())
        {
            temp = textField.color;
            temp.a = 0.7f;
            textField.color = temp;
        }

        if (special)
        {
            temp = specialSprite.color;
            temp.a = 0.5f;
            specialSprite.color = temp; 
            specialSprite.sprite = CardPreset.soulSpritePassive;
        }
        else
            specialSprite.color = Vector4.zero;
    }
    private void SetCardFields()
    {
        passiveTextField.text = "";

        specialSprite.sprite = CardPreset.soulSprite;
        nameTextField.text = CardPreset.cardType.ToString();
        cardImage.sprite = CardPreset.cardBackSprite;

        switch (CardPreset.cardType)
        {
            case ECardType.Buff:
                DefaultBuff db = CardPreset as DefaultBuff;
                upBonusTextField.text = "";
                foreach (var buff in db.normalBuffs)
                {
                    upBonusTextField.text += $"{buff.statType} {buff.value}. ";
                }
                downBonusTextField.text = "";
                foreach (var buff in db.specialBuffs)
                {
                    downBonusTextField.text += $"{buff.statType} {buff.value}. ";
                }
                break;
            case ECardType.Abbility:

                break;
        }
    }
    public void DestroyCard()
    {
        Destroy(gameObject);
    }
    private void Start()
    {
        
    }
    private void Update()
    {
        if (!passive)
            CoolDownMechanics();
    }
    private void CoolDownMechanics()
    {
        if (_timer < prepTime)
        {
            _timer += Time.deltaTime;
            coolDownImage.fillAmount = 1 - _timer / prepTime;
        }
        else
        {
            Ready = true;
            coolDownImage.fillAmount = 0;
        }
    }
}
