using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
using UnityEngine.SceneManagement;
using Product = UnityEngine.Purchasing.Product;

public class IAPManager : MonoBehaviour, IDetailedStoreListener
{
    public static IAPManager instance;
    private SaveManager DataManager => SaveManager.Instance;
    public bool isRestored;
    public GameObject iapPanel;
    private static string GetIAPId(string addOn) => Application.identifier +"."+ addOn;
    
    private static string NoAdsId => GetIAPId("removeads");
    private static string WelcomePackId => GetIAPId("welcomepack");
    
    private static string ExtraMoves_50 => GetIAPId("50moves");
    private static string ExtraMoves_150 => GetIAPId("150moves");
    private static string ExtraMoves_300 => GetIAPId("300moves");
    private static string ExtraMoves_700 => GetIAPId("700moves");
    private static string ExtraMoves_1500 => GetIAPId("1500moves");
    
    private static string Coins_500 => GetIAPId("500coins");
    private static string Coins_1000 => GetIAPId("1000coins");
    private static string Coins_2500 => GetIAPId("2500coins");
    private static string Coins_5000 => GetIAPId("5000coins");
    private static string Coins_10000 => GetIAPId("10000coins");
    private static string Coins_25000 => GetIAPId("25000coins");
    
    public Button[] iapBtns;
    public bool isInitialized;
    
    private IStoreController _storeController;
    private IExtensionProvider _extensionProvider;
    private IGooglePlayStoreExtensions _iGooglePlayStoreExtensions;
    
    //private IAppleExtensions _iAppleExtensionProvider;
    private string _welcomePackTime = "";
    private TimeSpan _wpTimeDifference;
    public RectTransform purchasedText;

    private static int iapBoughtCount = 0;
    private static bool _isItemBought = false;
    private static int purchasedMoves;
    private static int movesCount;

    private void Awake()
    {
        if (!instance)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            DestroyImmediate(gameObject);
            return;
        }
        
        SetUpBuilder();
    }

    private void CheckOutSideIAPButtons()
    {
        if (DataManager.state.noAds)
        {
            // turn off No Ads buttons
            iapBtns[0].gameObject.SetActive(false);
            
        }
    }
    
    private async void Start()
    {
        try
        {
            var options = new InitializationOptions().SetEnvironmentName("production");
            await UnityServices.InitializeAsync(options);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        
        iapBoughtCount = DataManager.state.iapBoughtCount;
        CheckOutSideIAPButtons();
    }
    private void SetUpBuilder()
    {
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        
        builder.AddProduct(NoAdsId, ProductType.NonConsumable);
        builder.AddProduct(WelcomePackId, ProductType.Consumable);
        
        builder.AddProduct(Coins_500, ProductType.Consumable);
        builder.AddProduct(Coins_1000, ProductType.Consumable);
        builder.AddProduct(Coins_2500, ProductType.Consumable);
        builder.AddProduct(Coins_5000, ProductType.Consumable);
        builder.AddProduct(Coins_10000, ProductType.Consumable);
        builder.AddProduct(Coins_25000, ProductType.Consumable);
        
        builder.AddProduct(ExtraMoves_50, ProductType.Consumable);
        builder.AddProduct(ExtraMoves_150, ProductType.Consumable);
        builder.AddProduct(ExtraMoves_300, ProductType.Consumable);
        builder.AddProduct(ExtraMoves_700, ProductType.Consumable);
        builder.AddProduct(ExtraMoves_1500, ProductType.Consumable);
        
        
        UnityPurchasing.Initialize(this,builder);
        
    }
    
    
    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        isInitialized = true;
        _storeController = controller;
        _extensionProvider = extensions;
        _iGooglePlayStoreExtensions = _extensionProvider.GetExtension<IGooglePlayStoreExtensions>();
        SetIAPButtonCurrency();
        RestoreTransactions(_iGooglePlayStoreExtensions);
    }
    
    private void SetIAPButtonCurrency()
    {
        if(iapBtns.Length<=0)return;
        
        iapBtns[0].GetComponentInChildren<TMP_Text>().text = ReturnLocalizedPrice(NoAdsId);
        
        iapBtns[1].GetComponentInChildren<TMP_Text>().text = ReturnLocalizedPrice(Coins_500);
        iapBtns[2].GetComponentInChildren<TMP_Text>().text = ReturnLocalizedPrice(Coins_1000);
        iapBtns[3].GetComponentInChildren<TMP_Text>().text = ReturnLocalizedPrice(Coins_2500);
        iapBtns[4].GetComponentInChildren<TMP_Text>().text = ReturnLocalizedPrice(Coins_10000);
        iapBtns[5].GetComponentInChildren<TMP_Text>().text = ReturnLocalizedPrice(Coins_25000);
        
        /*iapBtns[1].GetComponentInChildren<TMP_Text>().text = ReturnLocalizedPrice(WelcomePackId);

        iapBtns[2].GetComponentInChildren<TMP_Text>().text = ReturnLocalizedPrice(ExtraMoves_50);
        iapBtns[3].GetComponentInChildren<TMP_Text>().text = ReturnLocalizedPrice(ExtraMoves_150);
        iapBtns[4].GetComponentInChildren<TMP_Text>().text = ReturnLocalizedPrice(ExtraMoves_300);
        iapBtns[5].GetComponentInChildren<TMP_Text>().text = ReturnLocalizedPrice(ExtraMoves_700);
        iapBtns[6].GetComponentInChildren<TMP_Text>().text = ReturnLocalizedPrice(ExtraMoves_1500);*/
        
    }
    
    private string ReturnLocalizedPrice(string id)
    {
        var p = _storeController.products.WithID(id);
        var price = p.metadata.localizedPrice;
        var code = p.metadata.isoCurrencyCode;
        if (!Currencies.ContainsKey(code)) return price + " " + code;
        var code2 = Currencies[code];
        return code2 + " " + price;
    }
    
    private void RestoreTransactions(IGooglePlayStoreExtensions googleExtensions)
    {
        if (googleExtensions is null)
        {
            print("Google Extension is null at RestoreTransactions @OnInitialized");
            return;
        } 
        print("Calling OnRestoreTransactions: " + googleExtensions);
        googleExtensions.RestoreTransactions(OnTransactionsRestored);
    }


    private void OnTransactionsRestored(bool success, string error)
    {
        print("Calling OnTransactionsRestore: " + isInitialized +"::"+success );
        if (!isInitialized)
        {
            print("Not Initialized IAP");
            return;
        }

        if (!success)
        {
            print("IAP  Not Restore Success Fully:: " + error);
            return;
        }
        isRestored = true;
        CheckNonConsumableProductsReceipt();
    }

    private void CheckNonConsumableProductsReceipt()
    {
        if (_storeController is null)
        {
            print("No Store Controller found IAP");
            return;
        }
        var noAdsProduct = _storeController.products.WithID(NoAdsId);

        if (noAdsProduct != null)
        {
            print(noAdsProduct.hasReceipt +" ::RemoveAdsProduct  IAP");
            if (noAdsProduct.hasReceipt)
            {
                IAP_ButtonStatus(true);
                print("IAP  Purchased: " + NoAdsId);
                if (!DataManager.state.noAds)
                {
                    AddCoins(200);
                    DataManager.state.noAds = true;
                    //ApplovinManager.HideBannerAds();
                }
            } 
        }
        
        if(DataManager.state.noAds) IAP_ButtonStatus(true);
    }

    private void IAP_ButtonStatus(bool isRemoveAds)
    {
       iapBtns[0].gameObject.SetActive(!isRemoveAds);
    }
    
    
    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
    {
        var product = purchaseEvent.purchasedProduct;
        print("IAP PurchaseProcessingResult:: "+ product.definition.id);
        OnProductPurchased(product);
        //AdjustManager.IAPEvent(product);
        return PurchaseProcessingResult.Complete;
    }

    private void OnProductPurchased(Product product)
    {
        var productID = product.definition.id;

        if (productID == NoAdsId)
        {
            // Remove Ads
            if(DataManager.state.noAds) return;
            
            AddCoins(200);
            DataManager.state.noAds = true;
            // ApplovinManager.HideBannerAds();
            IAP_ButtonStatus(true);
           
            // LionStudiosManager.IAPEvent(product,1,0,200);
            ByteBrewManager.IAPEvents(product,SaveData.GetSpecialLevelNumber()+"");
            iapBoughtCount++;
            CheckOutSideIAPButtons();
        }
        else if (productID == WelcomePackId)
        {
            AddMoves(50);
            AddCoins(500);
            // LionStudiosManager.IAPEvent(product,0,50,500);
            ByteBrewManager.IAPEvents(product,SaveData.GetSpecialLevelNumber()+"");
            iapBoughtCount++;
        }
        else if (productID == ExtraMoves_50)
        {
            AddMoves(50);
            // LionStudiosManager.IAPEvent(product,0,50,0);
            ByteBrewManager.IAPEvents(product,SaveData.GetSpecialLevelNumber()+"");
            iapBoughtCount++;
           
        }
        else if (productID == ExtraMoves_150)
        {
            AddMoves(150);
            // LionStudiosManager.IAPEvent(product,0,150,0);
            ByteBrewManager.IAPEvents(product,SaveData.GetSpecialLevelNumber()+"");
            iapBoughtCount++;
            
        }
        else if (productID == ExtraMoves_300)
        {
            AddMoves(300);
            // LionStudiosManager.IAPEvent(product,0,300,0);
            ByteBrewManager.IAPEvents(product,SaveData.GetSpecialLevelNumber()+"");
            iapBoughtCount++;
           
        }
        else if (productID == ExtraMoves_700)
        {
            AddMoves(700);
            // LionStudiosManager.IAPEvent(product,0,700,0);
            ByteBrewManager.IAPEvents(product,SaveData.GetSpecialLevelNumber()+"");
            iapBoughtCount++;
          
        }
        else if (productID == ExtraMoves_1500)
        {
            AddMoves(1500);
            // LionStudiosManager.IAPEvent(product,0,1500,0);
            ByteBrewManager.IAPEvents(product,SaveData.GetSpecialLevelNumber()+"");
            iapBoughtCount++;
          
        }
        else if (productID == Coins_500)
        {
            AddCoins(500);
            // LionStudiosManager.IAPEvent(product,0,0,500);
            ByteBrewManager.IAPEvents(product,SaveData.GetSpecialLevelNumber()+"");
            iapBoughtCount++;
            
        }
        else if (productID == Coins_1000)
        {
            AddCoins(1000);
            //LionStudiosManager.IAPEvent(product,0,0,1000);
            ByteBrewManager.IAPEvents(product,SaveData.GetSpecialLevelNumber()+"");
            iapBoughtCount++;
        }
        else if (productID == Coins_2500)
        {
            AddCoins(2500);
            //LionStudiosManager.IAPEvent(product,0,0,2500);
            ByteBrewManager.IAPEvents(product,SaveData.GetSpecialLevelNumber()+"");
            iapBoughtCount++;
        }
        else if (productID == Coins_5000)
        {
            AddCoins(5000);
           // LionStudiosManager.IAPEvent(product,0,0,5000);
            ByteBrewManager.IAPEvents(product,SaveData.GetSpecialLevelNumber()+"");
            iapBoughtCount++;
        }
        else if (productID == Coins_10000)
        {
            AddCoins(10000);
            //LionStudiosManager.IAPEvent(product,0,0,10000);
            ByteBrewManager.IAPEvents(product,SaveData.GetSpecialLevelNumber()+"");
            iapBoughtCount++;
        }  
        else if (productID == Coins_25000)
        {
            AddCoins(25000);
            //LionStudiosManager.IAPEvent(product,0,0,25000);
            ByteBrewManager.IAPEvents(product,SaveData.GetSpecialLevelNumber()+"");
            iapBoughtCount++;
        }
        
        DataManager.state.iapBoughtCount = iapBoughtCount;
        DataManager.UpdateState();
    }
    
    private static void AddMoves(int moves)
    {
        SaveData.SetMovesCount(SaveData.GetMovesCount() + moves);
    }
    
    private static void AddCoins(int coins)
    {
       SaveData.SetCoinCount(coins + SaveData.GetCoinsCount());
    }

    private void PlaySound()
    {
        //  sounds here
        Vibration.VibrateAndroid(30);
    }

    public void OnPurchaseInitiate_NoAds()
    {
        _storeController.InitiatePurchase(NoAdsId);
        PlaySound();
    }
   
    public void OnPurchaseInitiate_WelcomePack()
    {
        _storeController.InitiatePurchase(WelcomePackId);
        PlaySound();
    }
    
    public void OnPurchaseInitiate_50Moves()
    {
        _storeController.InitiatePurchase(ExtraMoves_50);
        PlaySound();
    }
    public void OnPurchaseInitiate_150Moves()
    {
        _storeController.InitiatePurchase(ExtraMoves_150);
        PlaySound();
    }
    public void OnPurchaseInitiate_300Moves()
    {
        _storeController.InitiatePurchase(ExtraMoves_300);
        PlaySound();
    }
    public void OnPurchaseInitiate_700Moves()
    {
        _storeController.InitiatePurchase(ExtraMoves_700);
        PlaySound();
    }
    public void OnPurchaseInitiate_1500Moves()
    {
        _storeController.InitiatePurchase(ExtraMoves_1500);
        PlaySound();
    }
    public void OnPurchaseInitiate_500Coins()
    {
        _storeController.InitiatePurchase(Coins_500);
        PlaySound();
    }
    public void OnPurchaseInitiate_1000Coins()
    {
        _storeController.InitiatePurchase(Coins_1000);
        PlaySound();
    }
    public void OnPurchaseInitiate_2500Coins()
    {
        _storeController.InitiatePurchase(Coins_2500);
        PlaySound();
    }
    public void OnPurchaseInitiate_5000Coins()
    {
        _storeController.InitiatePurchase(Coins_5000);
        PlaySound();
    }
    public void OnPurchaseInitiate_10000Coins()
    {
        _storeController.InitiatePurchase(Coins_10000);
        PlaySound();
    }
    public void OnPurchaseInitiate_25000Coins()
    {
        _storeController.InitiatePurchase(Coins_25000);
        PlaySound();
    }
    

    public void OnRestore()
    {
        if(_extensionProvider is null) 
        {
            print("ExtensionProvider is null IAP");
            return;
        }

        if (_iGooglePlayStoreExtensions is null)
        {
            print("IGoogleExtension is null IAP");
            return;
        }
      
        _iGooglePlayStoreExtensions.RestoreTransactions((result, error) =>
        {
            if (result)
            {
                isRestored = true;
                print("Restored:::   IAP");
                CheckNonConsumableProductsReceipt();
            }
            else
            {
                print("Not Restored::"+error);
            }
        });
    }
    

    public void OnInitializeFailed(InitializationFailureReason error)
    {
       print("IAP  InitializeFailed::" + error);
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        print("IAP   InitializeFailed::" + error+":"+message);
    }
    
    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        print("IAP    OnPurchaseFailed Reason: "+failureReason);
    }
    
    public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
    {
        print("IAP     OnPurchaseFailed description: "+failureDescription);
    }

    public void OpenIAP()
    {
        iapPanel.SetActive(true);
        PlaySound();
    }
    

    public void CloseIAP()
    {
        iapPanel.SetActive(false);
        PlaySound();
    }
    
    

    private static readonly Dictionary<string, string> Currencies = new Dictionary<string, string>() {
                                                    {"AED", "د.إ."},
                                                    {"AFN", "؋ "},
                                                    {"ALL", "Lek"},
                                                    {"AMD", "դր."},
                                                    {"ARS", "$"},
                                                    {"AUD", "$"}, 
                                                    {"AZN", "man."}, 
                                                    {"BAM", "KM"}, 
                                                    {"BDT", "৳"}, 
                                                    {"BGN", "лв."}, 
                                                    {"BHD", "د.ب."},
                                                    {"BND", "$"}, 
                                                    {"BOB", "$b"}, 
                                                    {"BRL", "R$"}, 
                                                    {"BYR", "р."}, 
                                                    {"BZD", "BZ$"}, 
                                                    {"CAD", "$"}, 
                                                    {"CHF", "fr."}, 
                                                    {"CLP", "$"}, 
                                                    {"CNY", "¥"}, 
                                                    {"COP", "$"}, 
                                                    {"CRC", "₡"}, 
                                                    {"CSD", "Din."}, 
                                                    {"CZK", "Kč"}, 
                                                    {"DKK", "kr."}, 
                                                    {"DOP", "RD$"}, 
                                                    {"DZD", "DZD"}, 
                                                    {"EEK", "kr"}, 
                                                    {"EGP", "ج.م."},
                                                    {"ETB", "ETB"}, 
                                                    {"EUR", "€"}, 
                                                    {"GBP", "£"}, 
                                                    {"GEL", "Lari"}, 
                                                    {"GTQ", "Q"}, 
                                                    {"HKD", "HK$"}, 
                                                    {"HNL", "L."}, 
                                                    {"HRK", "kn"}, 
                                                    {"HUF", "Ft"}, 
                                                    {"IDR", "Rp"}, 
                                                    {"ILS", "₪"}, 
                                                    {"INR", "₹"}, 
                                                    {"IQD", "د.ع."},
                                                    {"IRR", "ريال"},
                                                    {"ISK", "kr."}, 
                                                    {"JMD", "J$"}, 
                                                    {"JOD", "د.ا."},
                                                    {"JPY", "¥"}, 
                                                    {"KES", "S"}, 
                                                    {"KGS", "сом"}, 
                                                    {"KHR", "៛"}, 
                                                    {"KRW", "₩"}, 
                                                    {"KWD", "د.ك."},
                                                    {"KZT", "Т"}, 
                                                    {"LAK", "₭"}, 
                                                    {"LBP", "ل.ل."},
                                                    {"LKR", "රු."}, 
                                                    {"LTL", "Lt"}, 
                                                    {"LVL", "Ls"}, 
                                                    {"LYD", "د.ل."},
                                                    {"MAD", "د.م."},
                                                    {"MKD", "ден."}, 
                                                    {"MNT", "₮"}, 
                                                    {"MOP", "MOP"}, 
                                                    {"MVR", "ރ."}, 
                                                    {"MXN", "$"}, 
                                                    {"MYR", "RM"}, 
                                                    {"NIO", "N"}, 
                                                    {"NOK", "kr"}, 
                                                    {"NPR", "रु"}, 
                                                    {"NZD", "$"}, 
                                                    {"OMR", "ر.ع."},
                                                    {"PAB", "B/."}, 
                                                    {"PEN", "S/."}, 
                                                    {"PHP", "PhP"}, 
                                                    {"PKR", "Rs"}, 
                                                    {"PLN", "zł"}, 
                                                    {"PYG", "Gs"}, 
                                                    {"QAR", "ر.ق."},
                                                    {"RON", "lei"}, 
                                                    {"RSD", "Din."}, 
                                                    {"RUB", "р."}, 
                                                    {"RWF", "RWF"}, 
                                                    {"SAR", "ر.س."},
                                                    {"SEK", "kr"}, 
                                                    {"SGD", "$"}, 
                                                    {"SYP", "ل.س."},
                                                    {"THB", "฿"}, 
                                                    {"TJS", "т.р."}, 
                                                    {"TMT", "m."}, 
                                                    {"TND", "د.ت."},
                                                    {"TRY", "TL"}, 
                                                    {"TTD", "TT$"}, 
                                                    {"TWD", "NT$"}, 
                                                    {"UAH", "₴"}, 
                                                    {"USD", "$"}, 
                                                    {"UYU", "$U"}, 
                                                    {"UZS", "so'm"}, 
                                                    {"VEF", "Bs. F."}, 
                                                    {"VND", "₫"}, 
                                                    {"XOF", "XOF"}, 
                                                    {"YER", "ر.ي."},
                                                    {"ZAR", "R"}, 
                                                    {"ZWL", "Z$"} 
  };


}
