using UnityEngine;
using UnityEngine.UI;

public sealed class ShopController : MonoBehaviour
{
    [SerializeField] private WeaponDefinition[] definitions;
    [SerializeField] private int[] prices;
    [Header("Persistent Main Menu UI")]
    [SerializeField] private Button shopButton;
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private Text coinsText;
    [SerializeField] private Text feedbackText;
    [SerializeField] private Button backButton;
    [SerializeField] private Button[] primaryButtons;
    [SerializeField] private Button[] secondaryButtons;
    [SerializeField] private Text[] primaryLabels;
    [SerializeField] private Text[] secondaryLabels;
    private CurrencyWallet wallet;

    private void Start()
    {
        wallet = FindFirstObjectByType<CurrencyWallet>();
        if (shopButton != null) shopButton.onClick.AddListener(OpenShop); else CreateShopButtonFallback();
        if (backButton != null) backButton.onClick.AddListener(CloseShop);
        if (primaryButtons != null) for (int i = 0; i < primaryButtons.Length; i++) { int captured = i; if (primaryButtons[i] != null) primaryButtons[i].onClick.AddListener(() => HandleAction(captured, 0)); if (secondaryButtons != null && i < secondaryButtons.Length && secondaryButtons[i] != null) secondaryButtons[i].onClick.AddListener(() => HandleAction(captured, 1)); }
        if (wallet != null) { wallet.Changed += RefreshCoins; RefreshCoins(wallet.Coins); }
        if (shopPanel != null) shopPanel.SetActive(false); RefreshAllCards();
    }
    public void OpenShop() { if (shopPanel == null) BuildRuntimePanel(); if (mainMenuPanel != null) mainMenuPanel.SetActive(false); if (shopPanel != null) shopPanel.SetActive(true); RefreshCoins(wallet != null ? wallet.Coins : 0); RefreshAllCards(); }
    public void CloseShop() { if (shopPanel != null) shopPanel.SetActive(false); if (mainMenuPanel != null) mainMenuPanel.SetActive(true); }
    private void CreateShopButtonFallback() { GameObject go = CreateUI("SHOP", transform); RectTransform r = go.GetComponent<RectTransform>(); r.anchorMin = r.anchorMax = new Vector2(.5f,.5f); r.anchoredPosition = new Vector2(220,-45); r.sizeDelta = new Vector2(180,64); shopButton = go.GetComponent<Button>(); shopButton.onClick.AddListener(OpenShop); }
    private void BuildRuntimePanel() { shopPanel = new GameObject("ShopPanel", typeof(RectTransform), typeof(Image)); shopPanel.transform.SetParent(transform,false); RectTransform r=shopPanel.GetComponent<RectTransform>(); r.anchorMin=Vector2.zero; r.anchorMax=Vector2.one; r.offsetMin=r.offsetMax=Vector2.zero; shopPanel.GetComponent<Image>().color=new Color(.025f,.035f,.05f,.97f); coinsText=CreateText("COINS",shopPanel.transform,24).GetComponent<Text>(); coinsText.rectTransform.anchoredPosition=new Vector2(0,205); primaryButtons=new Button[definitions.Length]; secondaryButtons=new Button[definitions.Length]; primaryLabels=new Text[definitions.Length]; secondaryLabels=new Text[definitions.Length]; for(int i=0;i<definitions.Length;i++) CreateCard(i,definitions[i]); backButton=CreateUI("BACK",shopPanel.transform).GetComponent<Button>(); backButton.GetComponent<RectTransform>().anchoredPosition=new Vector2(0,-285); backButton.GetComponent<RectTransform>().sizeDelta=new Vector2(180,56); backButton.onClick.AddListener(CloseShop); feedbackText=CreateText("",shopPanel.transform,18).GetComponent<Text>(); feedbackText.rectTransform.anchoredPosition=new Vector2(0,-235); }
    private void CreateCard(int i, WeaponDefinition d) { float x=(i-(definitions.Length-1)*.5f)*190; GameObject card=new GameObject("Card_"+d.DisplayName,typeof(RectTransform),typeof(Image)); card.transform.SetParent(shopPanel.transform,false); RectTransform r=card.GetComponent<RectTransform>(); r.anchorMin=r.anchorMax=new Vector2(.5f,.5f); r.anchoredPosition=new Vector2(x,55); r.sizeDelta=new Vector2(175,250); card.GetComponent<Image>().color=new Color(.12f,.15f,.19f,.98f); GameObject info=CreateText(d.DisplayName.ToUpperInvariant()+"\nDMG "+d.Damage+"  MAG "+d.MagazineSize+"\nRANGE "+d.Range+" m",card.transform,16); info.GetComponent<RectTransform>().anchoredPosition=new Vector2(0,65); primaryButtons[i]=CreateUI("BUY",card.transform).GetComponent<Button>(); primaryButtons[i].GetComponent<RectTransform>().anchoredPosition=new Vector2(0,-65); primaryButtons[i].GetComponent<RectTransform>().sizeDelta=new Vector2(150,44); primaryLabels[i]=primaryButtons[i].GetComponentInChildren<Text>(); secondaryButtons[i]=CreateUI("SLOT 2",card.transform).GetComponent<Button>(); secondaryButtons[i].GetComponent<RectTransform>().anchoredPosition=new Vector2(0,-112); secondaryButtons[i].GetComponent<RectTransform>().sizeDelta=new Vector2(150,38); secondaryLabels[i]=secondaryButtons[i].GetComponentInChildren<Text>(); int captured=i; primaryButtons[i].onClick.AddListener(()=>HandleAction(captured,0)); secondaryButtons[i].onClick.AddListener(()=>HandleAction(captured,1)); RefreshCard(primaryLabels[i],i,0); RefreshCard(secondaryLabels[i],i,1); }
    private void RefreshCard(Text t,int i,int slot) { if(!t||definitions==null||i>=definitions.Length||!definitions[i])return; WeaponDefinition d=definitions[i]; t.text=!WeaponOwnership.IsOwned(d.WeaponId)?(slot==0?"BUY "+prices[i]:"LOCKED"):(WeaponOwnership.GetSlot(slot)==d.WeaponId?"EQUIPPED":"EQUIP S"+(slot+1)); }
    private void HandleAction(int i,int slot) { if(definitions==null||i>=definitions.Length||!definitions[i])return; WeaponDefinition d=definitions[i]; if(!WeaponOwnership.IsOwned(d.WeaponId)){if(slot!=0)return; if(wallet==null||!wallet.TrySpend(prices[i])){if(feedbackText)feedbackText.text="NOT ENOUGH COINS";return;} WeaponOwnership.Unlock(d.WeaponId);if(feedbackText)feedbackText.text="PURCHASED "+d.DisplayName.ToUpperInvariant();RefreshAllCards();return;} string other=WeaponOwnership.GetSlot(1-slot);if(other==d.WeaponId){if(feedbackText)feedbackText.text="ALREADY EQUIPPED";return;} WeaponOwnership.SetSlot(slot,d.WeaponId);if(feedbackText)feedbackText.text="EQUIPPED "+d.DisplayName.ToUpperInvariant();RefreshAllCards(); }
    private void RefreshAllCards(){if(definitions==null)return;for(int i=0;i<definitions.Length;i++){if(primaryLabels!=null&&i<primaryLabels.Length)RefreshCard(primaryLabels[i],i,0);if(secondaryLabels!=null&&i<secondaryLabels.Length)RefreshCard(secondaryLabels[i],i,1);}}
    private void RefreshCoins(int value){if(coinsText)coinsText.text="COINS  "+value;}
    private GameObject CreateUI(string text,Transform parent){GameObject go=new GameObject(text,typeof(RectTransform),typeof(Image),typeof(Button));go.transform.SetParent(parent,false);go.GetComponent<Image>().color=new Color(.18f,.24f,.3f,.98f);GameObject label=CreateText(text,go.transform,20);label.GetComponent<RectTransform>().anchoredPosition=Vector2.zero;return go;}
    private GameObject CreateText(string text,Transform parent,int size){GameObject go=new GameObject("Text",typeof(RectTransform),typeof(Text));go.transform.SetParent(parent,false);Text t=go.GetComponent<Text>();t.text=text;t.alignment=TextAnchor.MiddleCenter;t.color=Color.white;t.fontSize=size;t.font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");t.raycastTarget=false;t.rectTransform.sizeDelta=new Vector2(500,100);return go;}
    private void OnDestroy(){if(wallet!=null)wallet.Changed-=RefreshCoins;}
}
