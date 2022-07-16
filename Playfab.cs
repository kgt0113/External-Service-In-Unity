using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using System.IO;

public class DataManager : MonoBehaviour {
    public List<UserData> userData;
    string MyCharacterDatapath;

    //Playfab Data
    public InputField EmailInput, PasswordInput, UsernameInput;
    public Text CoinText;
    public Text TestLogInPlayfab;
    public Text TestLogInBackground;

    public void PlayfabLoginButton() {
        var request = new LoginWithEmailAddressRequest { Email = EmailInput.text, Password = PasswordInput.text };
        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnLoginFailure);  
    }   
    
    public void PlayfabRegisterButton() {
        var request = new RegisterPlayFabUserRequest { Email = EmailInput.text, Password = PasswordInput.text, Username = UsernameInput.text };
        PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnRegisterFailure);
    }
    void OnLoginSuccess(LoginResult result) {
        TestLogInPlayfab.text = "Login Success";
        myId = result.PlayFabId;
    }

    void OnLoginFailure(PlayFabError error) => TestLogInPlayfab.text = "Login Fail";

    void OnRegisterSuccess(RegisterPlayFabUserResult result) => TestLogInPlayfab.text = "Register Success";

    void OnRegisterFailure(PlayFabError error) => TestLogInPlayfab.text = "Resgister Failed";


    public void IntegerSave_InPlayfab() { //int save
        PlayFabClientAPI.UpdatePlayerStatistics(new UpdatePlayerStatisticsRequest {
            Statistics = new List<StatisticUpdate> {
                new StatisticUpdate {StatisticName = "Coin", Value = int.Parse(CoinText.text)},
            }
        },
        (result) => { TestLogInBackground.text = "Integer Saved"; },
        (error) => { TestLogInBackground.text = "Integer Save Failed"; });
    }

    public void IntegerLoad_FromPlayfab() {
        PlayFabClientAPI.GetPlayerStatistics(
            new GetPlayerStatisticsRequest(),
            (result) => {
                CoinText.text = "";
                foreach (var eachStat in result.Statistics) CoinText.text += eachStat.Value;
                TestLogInBackground.text = "Integer Load Success";
            },
            (error) => { CoinText.text = "Integer Load failed"; });
    }
    
    public void DataSaveInPlayfab() {
        string myData = JsonUtility.ToJson(new Serialization<Lists>(Lists_var), true);
        var request = new UpdateUserDataRequest() { Data = new Dictionary<string, string>() { { "userData", myData } } };
        PlayFabClientAPI.UpdateUserData(request, (result) => TestLogInBackground.text = "Data Saved success", (error) => TestLogInBackground.text = "Data Save Failed");
    }

    public void DataLoadFromPlayfab() {
        var request = new GetUserDataRequest() { PlayFabId = myId };
        PlayFabClientAPI.GetUserData(request,
        (result) => { foreach (var eachData in result.Data)
            Lists = JsonUtility.FromJson<Serialization<UserData>>(eachData.Value.Value).target;
            TestLogInBackground.text = "Data Load Success"; 
        },
        (error) => TestLogInBackground.text = "Data Load Failed");
    }
}
   
