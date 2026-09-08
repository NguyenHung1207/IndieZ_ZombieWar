using UnityEngine;
using UnityEngine.UI;

public sealed class ShopController : MonoBehaviour
{
    [SerializeField] private WeaponDefinition[] definitions;
    [SerializeField] private int[] prices;
    private GameObject panel;
    private Text coinsText;
    private Text feedbackText;
    private Text[] primaryLabels;
    private Text[] secondaryLabels;
    private CurrencyWallet wallet;

    private void Start()
    {
        wallet = FindFirstObjectByType<CurrencyWallet>();
        CreateShopButton();
    }

    private void CreateShopButton()
    {
        GameObject buttonObject = CreateUI("SHOP", transform);
        RectTransform rect = buttonObject.GetComponent<RectTransform>(); rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f); rect.anchoredPosition = new Vector2(220f, -45f); rect.sizeDelta = new Vector2(180f, 64f);
        Button button = buttonObject.GetComponent<Button>(); button.onClick.AddListener(OpenShop);
    }

    private void OpenShop()
    {
        if (panel != null) return;
        panel = new GameObject("ShopPanel", typeof(RectTransform), typeof(Image)); panel.transform.SetParent(transform, false);
        RectTransform root = panel.GetComponent<RectTransform>(); root.anchorMin = Vector2.zero; root.anchorMax = Vector2.one; root.offsetMin = root.offsetMax = Vector2.zero; panel.GetComponent<Image>().color = new Color(0.025f,0.035f,0.05f,0.97f);
        GameObject title = CreateText("WEAPON SHOP", panel.transform, 30); title.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, 250f);
        GameObject coins = CreateText("COINS", panel.transform, 24); coinsText = coins.GetComponent<Text>(); coins.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, 205f);
        if (wallet != null) { wallet.Changed += RefreshCoins; RefreshCoins(wallet.Coins); }
        primaryLabels = new Text[definitions.Length]; secondaryLabels = new Text[definitions.Length];
        for (int i=0;i<definitions.Length;i++) if (definitions[i] != null) CreateCard(i, definitions[i]);
        GameObject back = CreateUI("BACK", panel.transform); back.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, -285f); back.GetComponent<RectTransform>().sizeDelta = new Vector2(180f, 56f); back.GetComponent<Button>().onClick.AddListener(CloseShop);
        feedbackText = CreateText("", panel.transform, 18).GetComponent<Text>(); feedbackText.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, -235f);
    }

    private void CreateCard(int index, WeaponDefinition definition)
    {
        float x = (index - (definitions.Length - 1) * 0.5f) * 190f;
        GameObject card = new GameObject("Card_" + definition.DisplayName, typeof(RectTransform), typeof(Image)); card.transform.SetParent(panel.transform,false); var r=card.GetComponent<RectTransform>();r.anchorMin=r.anchorMax=new Vector2(.5f,.5f);r.anchoredPosition=new Vector2(x,55f);r.sizeDelta=new Vector2(175f,250f);card.GetComponent<Image>().color=new Color(.12f,.15f,.19f,.98f);
        GameObject label=CreateText(definition.DisplayName.ToUpperInvariant()+"\nDMG "+definition.Damage+"  MAG "+definition.MagazineSize+"\nRANGE "+definition.Range+" m",card.transform,16);label.GetComponent<RectTransform>().anchoredPosition=new Vector2(0,65);label.GetComponent<Text>().resizeTextMaxSize=20;
        GameObject action=CreateUI("BUY",card.transform);action.GetComponent<RectTransform>().anchoredPosition=new Vector2(0,-65);action.GetComponent<RectTransform>().sizeDelta=new Vector2(150,44);int captured=index;action.GetComponent<Button>().onClick.AddListener(()=>HandleAction(captured,0));primaryLabels[index]=action.GetComponentInChildren<Text>();RefreshCard(primaryLabels[index],index,0);
        GameObject slot2=CreateUI("SLOT 2",card.transform);slot2.GetComponent<RectTransform>().anchoredPosition=new Vector2(0,-112);slot2.GetComponent<RectTransform>().sizeDelta=new Vector2(150,38);slot2.GetComponent<Button>().onClick.AddListener(()=>HandleAction(captured,1));secondaryLabels[index]=slot2.GetComponentInChildren<Text>();RefreshCard(secondaryLabels[index],index,1);
    }
    private void RefreshCard(Text text,int index,int slot){var d=definitions[index];text.text=!WeaponOwnership.IsOwned(d.WeaponId)?slot==0?"BUY "+prices[index]:"LOCKED":WeaponOwnership.GetSlot(slot)==d.WeaponId?"EQUIPPED":"EQUIP S"+(slot+1);}
    private void HandleAction(int index,int slot){var d=definitions[index];if(!WeaponOwnership.IsOwned(d.WeaponId)){if(slot!=0)return;if(wallet==null||!wallet.TrySpend(prices[index])){if(feedbackText)feedbackText.text="NOT ENOUGH COINS";return;}WeaponOwnership.Unlock(d.WeaponId);if(feedbackText)feedbackText.text="PURCHASED "+d.DisplayName.ToUpperInvariant();RefreshAllCards();return;}string other=WeaponOwnership.GetSlot(1-slot);if(other==d.WeaponId){if(feedbackText)feedbackText.text="ALREADY EQUIPPED";return;}WeaponOwnership.SetSlot(slot,d.WeaponId);if(feedbackText)feedbackText.text="EQUIPPED "+d.DisplayName.ToUpperInvariant();RefreshAllCards();}
    private void RefreshAllCards(){if(primaryLabels==null)return;for(int i=0;i<definitions.Length;i++){if(primaryLabels[i]!=null)RefreshCard(primaryLabels[i],i,0);if(secondaryLabels[i]!=null)RefreshCard(secondaryLabels[i],i,1);}}
    private void RefreshCoins(int value){if(coinsText)coinsText.text="COINS  "+value;}
    private void CloseShop(){if(wallet!=null)wallet.Changed-=RefreshCoins;if(panel)Destroy(panel);panel=null;feedbackText=null;}
    private GameObject CreateUI(string text,Transform parent){GameObject go=new GameObject(text,typeof(RectTransform),typeof(Image),typeof(Button));go.transform.SetParent(parent,false);go.GetComponent<Image>().color=new Color(.18f,.24f,.3f,.98f);GameObject label=CreateText(text,go.transform,20);label.GetComponent<RectTransform>().anchoredPosition=Vector2.zero;return go;}
    private GameObject CreateText(string text,Transform parent,int size){GameObject go=new GameObject("Text",typeof(RectTransform),typeof(Text));go.transform.SetParent(parent,false);var t=go.GetComponent<Text>();t.text=text;t.alignment=TextAnchor.MiddleCenter;t.color=Color.white;t.fontSize=size;t.font=Resources.GetBuiltinResource<Font>("Arial.ttf");t.raycastTarget=false;var r=t.rectTransform;r.sizeDelta=new Vector2(500,100);return go;}
    private void OnDestroy(){if(wallet!=null)wallet.Changed-=RefreshCoins;}
}
