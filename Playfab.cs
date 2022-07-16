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
        string MyCharacterData = JsonUtility.ToJson(new Serialization<SushiData>(AllSushiList), true);
        var request = new UpdateUserDataRequest() { Data = new Dictionary<string, string>() { { "playerSushiList", MyCharacterData } } };
        PlayFabClientAPI.UpdateUserData(request, (result) => TestLogInBackground.text = "서버에 데이터 저장됨", (error) => TestLogInBackground.text = "데이터 저장 실패함");
    }

    public void DataLoadFromPlayfab() {
        var request = new GetUserDataRequest() { PlayFabId = myId };
        PlayFabClientAPI.GetUserData(request,
        (result) => { foreach (var eachData in result.Data)
            AllSushiList = JsonUtility.FromJson<Serialization<SushiData>>(eachData.Value.Value).target;
            TestLogInBackground.text = "데이터 로드 성공"; 
        },
        (error) => TestLogInBackground.text = "데이터 로딩 실패");
    }
}
   
