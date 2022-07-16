using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RestartBtn : MonoBehaviour {
    public Canvas myCanvas;
    public static RestartBtn instance;
    public Button restartBtn;
    // Start is called before the first frame update
    private InterstitialAd interstitial;
    private void Start() {
        instance = this;
        restartBtn.onClick.AddListener(BtnClickRestart);
    }
    private void RequestInterstitial() {
        #if UNITY_ANDROID
            string adUnitId = "ca-app-pub-3940256099942544/1033173712";
        #elif UNITY_IPHONE
            string adUnitId = "ca-app-pub-3940256099942544/4411468910";
        #else
             string adUnitId = "unexpected_platform";
        #endif

        // Initialize an InterstitialAd.
        this.interstitial = new InterstitialAd(adUnitId);
        // Called when the ad is closed.
        this.interstitial.OnAdClosed += HandleOnAdClosed_LoadScene;

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the interstitial with the request.
        this.interstitial.LoadAd(request);
    }

    public void HandleOnAdClosed_LoadScene(object sender, System.EventArgs args) {
        SceneManager.LoadScene("Sample Scene");
    }


    //admob
    public void BtnClickRestart() {
        RequestInterstitial();
        
        StartCoroutine(showInterstitial());
        IEnumerator showInterstitial() {
            while (!this.interstitial.IsLoaded()) {
                yield return new WaitForSeconds(0.2f);
            }
            this.interstitial.Show();
            // sortingOrder = -1 => Canvas can't be cover ADs
            myCanvas.sortingOrder = -1;
        }
    }
}
