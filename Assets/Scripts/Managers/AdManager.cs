using UnityEngine;
using GoogleMobileAds.Api;
using System;

public class AdManager : MonoBehaviour
{
    public static AdManager Instance { get; private set; }
    
    [Header("Ad Unit IDs - Android")]
    public string androidBannerAdUnitId = "ca-app-pub-3940256099942544/6300978111"; // TEST
    public string androidInterstitialAdUnitId = "ca-app-pub-3940256099942544/1033173712"; // TEST
    public string androidRewardedAdUnitId = "ca-app-pub-3940256099942544/5224354917"; // TEST
    
    [Header("Ad Unit IDs - iOS")]
    public string iosBannerAdUnitId = "ca-app-pub-3940256099942544/2934735716"; // TEST
    public string iosInterstitialAdUnitId = "ca-app-pub-3940256099942544/4411468910"; // TEST
    public string iosRewardedAdUnitId = "ca-app-pub-3940256099942544/1712485313"; // TEST
    
    [Header("Settings")]
    public bool showBannerOnStart = false;
    public bool testMode = true;
    
    [Header("Ad Frequency")]
    public int deathsBeforeInterstitial = 3; // Cada 3 muertes
    
    private BannerView bannerView;
    private InterstitialAd interstitialAd;
    private RewardedAd rewardedAd;
    
    private Action<bool> currentRewardCallback;
    private string currentRewardType;
    private int deathCount = 0;
    
    private bool isInitialized = false;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    
    void Start()
    {
        InitializeAds();
    }
    
    void InitializeAds()
    {
        MobileAds.Initialize(initStatus =>
        {
            isInitialized = true;
            Debug.Log("[AdManager] AdMob initialized");
            
            // Precargar ads
            LoadInterstitialAd();
            LoadRewardedAd();
            
            if (showBannerOnStart)
            {
                ShowBanner();
            }
        });
    }
    
    // ═══════════════════════════════════════════════════════════════════
    // BANNER AD
    // ═══════════════════════════════════════════════════════════════════
    
    public void ShowBanner()
    {
        if (bannerView != null)
        {
            bannerView.Show();
            return;
        }
        
        string adUnitId = GetBannerAdUnitId();
        
        bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Bottom);
     AdRequest request = new AdRequest();
bannerView.LoadAd(request);
        
        Debug.Log("[AdManager] Banner loaded");
    }
    
    public void HideBanner()
    {
        if (bannerView != null)
        {
            bannerView.Hide();
        }
    }
    
    public void DestroyBanner()
    {
        if (bannerView != null)
        {
            bannerView.Destroy();
            bannerView = null;
        }
    }
    
    // ═══════════════════════════════════════════════════════════════════
    // INTERSTITIAL AD (Pantalla completa - después de morir)
    // ═══════════════════════════════════════════════════════════════════
    
    void LoadInterstitialAd()
    {
        if (interstitialAd != null)
        {
            interstitialAd.Destroy();
            interstitialAd = null;
        }
        
        string adUnitId = GetInterstitialAdUnitId();
        
     AdRequest request = new AdRequest();
        
        InterstitialAd.Load(adUnitId, request, (InterstitialAd ad, LoadAdError error) =>
        {
            if (error != null || ad == null)
            {
                Debug.LogError($"[AdManager] Interstitial failed to load: {error}");
                return;
            }
            
            interstitialAd = ad;
            RegisterInterstitialEvents(ad);
            Debug.Log("[AdManager] Interstitial loaded");
        });
    }
    
    void RegisterInterstitialEvents(InterstitialAd ad)
    {
        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("[AdManager] Interstitial closed");
            LoadInterstitialAd(); // Precargar siguiente
        };
        
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError($"[AdManager] Interstitial failed: {error}");
            LoadInterstitialAd();
        };
    }
    
    public void ShowInterstitialAd()
    {
        if (!isInitialized)
        {
            Debug.LogWarning("[AdManager] Not initialized yet");
            return;
        }
        
        if (interstitialAd != null && interstitialAd.CanShowAd())
        {
            interstitialAd.Show();
        }
        else
        {
            Debug.Log("[AdManager] Interstitial not ready");
            LoadInterstitialAd();
        }
    }
    
    public void OnPlayerDeath()
    {
        deathCount++;
        
        if (deathCount >= deathsBeforeInterstitial)
        {
            deathCount = 0;
            ShowInterstitialAd();
        }
    }
    
    // ═══════════════════════════════════════════════════════════════════
    // REWARDED AD (Con recompensa)
    // ═══════════════════════════════════════════════════════════════════
    
    void LoadRewardedAd()
    {
        if (rewardedAd != null)
        {
            rewardedAd.Destroy();
            rewardedAd = null;
        }
        
        string adUnitId = GetRewardedAdUnitId();
        
AdRequest request = new AdRequest();        
        RewardedAd.Load(adUnitId, request, (RewardedAd ad, LoadAdError error) =>
        {
            if (error != null || ad == null)
            {
                Debug.LogError($"[AdManager] Rewarded failed to load: {error}");
                return;
            }
            
            rewardedAd = ad;
            RegisterRewardedEvents(ad);
            Debug.Log("[AdManager] Rewarded ad loaded");
        });
    }
    
    void RegisterRewardedEvents(RewardedAd ad)
    {
        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("[AdManager] Rewarded ad closed");
            LoadRewardedAd(); // Precargar siguiente
        };
        
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError($"[AdManager] Rewarded ad failed: {error}");
            currentRewardCallback?.Invoke(false);
            currentRewardCallback = null;
            LoadRewardedAd();
        };
    }
    
    public void ShowRewardedAd(string rewardType, Action<bool> callback)
    {
        if (!isInitialized)
        {
            Debug.LogWarning("[AdManager] Not initialized yet");
            callback?.Invoke(false);
            return;
        }
        
        if (rewardedAd != null && rewardedAd.CanShowAd())
        {
            currentRewardType = rewardType;
            currentRewardCallback = callback;
            
            rewardedAd.Show((Reward reward) =>
            {
                Debug.Log($"[AdManager] User earned reward: {reward.Amount} {reward.Type}");
                currentRewardCallback?.Invoke(true);
                currentRewardCallback = null;
            });
        }
        else
        {
            Debug.Log("[AdManager] Rewarded ad not ready");
            callback?.Invoke(false);
            LoadRewardedAd();
        }
    }
    
    public bool IsRewardedAdReady()
    {
        return rewardedAd != null && rewardedAd.CanShowAd();
    }
    
    // ═══════════════════════════════════════════════════════════════════
    // HELPERS
    // ═══════════════════════════════════════════════════════════════════
    
    string GetBannerAdUnitId()
    {
        #if UNITY_ANDROID
            return androidBannerAdUnitId;
        #elif UNITY_IOS
            return iosBannerAdUnitId;
        #else
            return "unused";
        #endif
    }
    
    string GetInterstitialAdUnitId()
    {
        #if UNITY_ANDROID
            return androidInterstitialAdUnitId;
        #elif UNITY_IOS
            return iosInterstitialAdUnitId;
        #else
            return "unused";
        #endif
    }
    
    string GetRewardedAdUnitId()
    {
        #if UNITY_ANDROID
            return androidRewardedAdUnitId;
        #elif UNITY_IOS
            return iosRewardedAdUnitId;
        #else
            return "unused";
        #endif
    }
    
    void OnDestroy()
    {
        if (bannerView != null)
        {
            bannerView.Destroy();
        }
        
        if (interstitialAd != null)
        {
            interstitialAd.Destroy();
        }
        
        if (rewardedAd != null)
        {
            rewardedAd.Destroy();
        }
    }
}