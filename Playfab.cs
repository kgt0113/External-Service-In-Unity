using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using System.IO;

public class DataManager : MonoBehaviour
{
    public int TotalCoin;
    public string myId;
    public static DataManager instance;
    public TextAsset CharaterDatabase;
    public List<SushiData> AllSushiList,  ResetSushiList;
    public List<Sprite> AllSushiSprite;
    public Text CoinText;

    public int nowSelectIdNum;


    //GameData
    string MyCharacterDatapath;
    string CoinDatapath;

    //Playfab Data
    public InputField EmailInput, PasswordInput, UsernameInput;
    public Text TestLogInPlayfab;
    public Text TestLogInBackground;


    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        string[] line = CharaterDatabase.text.Substring(0, CharaterDatabase.text.Length).Split('\n');
        for(int i = 1; i < line.Length ; i++)
        {
            string[] row = line[i].Split('\t');
            AllSushiList.Add(new SushiData(int.Parse(row[0]), row[1], row[2], row[3], row[4]));
            ResetSushiList.Add(new SushiData(int.Parse(row[0]), row[1], row[2], row[3], row[4])); //이것은 초기화를 위해 사용했음
            AllSushiList.Sort((p1, p2) => p1.ID.CompareTo(p2.ID));
        }

        AllSushiList[0].isLocked = 0;
        ResetSushiList[0].isLocked = 0;
        MyCharacterDatapath = Application.persistentDataPath + "/MyItemList.txt";
        CoinDatapath = Application.persistentDataPath + "/TotalCoin.txt";
        print("데이터 경로 : "+MyCharacterDatapath);
        Load();

    }

    public void AddCoin100()
    {
        TotalCoin += 100;
        CoinText.text = TotalCoin.ToString("N0");
    }

    public void BasicCharacter() //for Test
    {

        AllSushiList = ResetSushiList;
        Player.instance.spriteRenderer.sprite = AllSushiSprite[2];
        Player.instance.nowSelectID = 0;
        TotalCoin = 0;
        PlayerPrefs.DeleteAll();

        Save();
        Load();
    }

  
    //<<<Only For test, >>>
    public void GetCharacter2()
    {
        SushiData AddCharacter = AllSushiList.Find(x => x.ID == 2);
        AddCharacter.isLocked = 0;
        Save();
        Load();
    }

    public void GetCharacter4()
    {
        SushiData AddCharacter = AllSushiList.Find(x => x.ID == 4);
        AddCharacter.isLocked = 0;
        Save();
        Load();
    }

    public void GetAllCharacter()
    {
        int allsushicount = AllSushiList.Count;
        for(int i = 0; i < allsushicount; i++)
        {
            SushiData addCharacter = AllSushiList.Find(x => x.ID == i);
            addCharacter.isLocked = 0;
            Save();
            Load();
        }
    }

    public void GatchaCharacter()
    {
        int allsushicount = AllSushiList.Count;
        int getChrNum = Random.Range(0, allsushicount);
        print(getChrNum);
        select = getChrNum;

        int spriteLength = AllSushiSprite.Count;
        for (int i = 0; i < spriteLength; i++)
        {
            if (i % 3 == 0) // basic down image  2 5 8 
            {
                if (getChrNum * 3 == i)
                {
                    GameManager.instance.GatchaCharacter.sprite = AllSushiSprite[i];
                    GameManager.instance.gatchaText.text = AllSushiList[getChrNum].kName;
                }
            }
        }

        for (int i = 0; i < allsushicount; i++)
        {
            SushiData AddCharacter = AllSushiList.Find(x => x.ID == getChrNum);

            if(AddCharacter.isLocked ==0)
            {
                print(AddCharacter.cName + "이미 가지고 있음");
                Player.instance.nowSelectID = getChrNum;
                ScrollViewSnap.instance.minItemNum = getChrNum;
                GatchaManager.instance.ChrEffect.gameObject.SetActive(false);
                GatchaManager.instance.alreadyHaveButton.gameObject.SetActive(true);
                GatchaManager.instance.newChrButton.gameObject.SetActive(false);
                Save();
                Load();
                return;
            }

            if(AddCharacter.isLocked == 1)
            {
                print(AddCharacter.cName + "없음");
                AddCharacter.isLocked = 0;
                Player.instance.nowSelectID = getChrNum;
                ScrollViewSnap.instance.minItemNum = getChrNum;
                GatchaManager.instance.ChrEffect.gameObject.SetActive(true);
                GatchaManager.instance.alreadyHaveButton.gameObject.SetActive(false);
                GatchaManager.instance.newChrButton.gameObject.SetActive(true);
                Save();
                Load();
                return;
            }
        }
        

    }


    //Character Select Property -> local Save;
    private int _select;
    public int select
    {
        get
        {
            _select = PlayerPrefs.GetInt("SushiSelect", 0);
            return _select;
        }
        set
        {
            _select = value;
            PlayerPrefs.SetInt("SushiSelect", _select);
        }
    }
   
    /*
    Save LocalDisk
    */
    public void Save()
    {
        //Character Save
        string MyCharacterData = JsonUtility.ToJson(new Serialization<SushiData>(AllSushiList), true);
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(MyCharacterData);
        string code = System.Convert.ToBase64String(bytes);
        File.WriteAllText(MyCharacterDatapath, code);

        //Coin Save
        string CoinData = TotalCoin.ToString();
        byte[] coinbytes = System.Text.Encoding.UTF8.GetBytes(CoinData);
        string coincode = System.Convert.ToBase64String(coinbytes);
        File.WriteAllText(CoinDatapath, coincode);
    }

    public void Load()
    {
        if ((!File.Exists(MyCharacterDatapath)) || (!File.Exists(CoinDatapath)))
        {
            BasicCharacter();
        }
        //Character Load
        string code = File.ReadAllText(MyCharacterDatapath);
        byte[] bytes = System.Convert.FromBase64String(code);
        string MyCharacterData = System.Text.Encoding.UTF8.GetString(bytes);
        AllSushiList = JsonUtility.FromJson<Serialization<SushiData>>(MyCharacterData).target;
       
        //Coin Load
        string coincode = File.ReadAllText(CoinDatapath);
        byte[] coinbytes = System.Convert.FromBase64String(coincode);
        string CoinJsonData = System.Text.Encoding.UTF8.GetString(coinbytes);
        TotalCoin = int.Parse(CoinJsonData);
        CoinText.text = TotalCoin.ToString("N0");
    }

    /*
    This Section allocate Only about Playfab
    */
    public void PlayfabLoginButton()
    {
        var request = new LoginWithEmailAddressRequest { Email = EmailInput.text, Password = PasswordInput.text };
        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnLoginFailure);
         
    }   
    public void PlayfabRegisterButton()
    {
        var request = new RegisterPlayFabUserRequest { Email = EmailInput.text, Password = PasswordInput.text, Username = UsernameInput.text };
        PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnRegisterFailure);
    }
    void OnLoginSuccess(LoginResult result)
    {
        TestLogInPlayfab.text = "로그인 성공";
        myId = result.PlayFabId;
    }

    void OnLoginFailure(PlayFabError error) => TestLogInPlayfab.text = "로그인 실패";

    void OnRegisterSuccess(RegisterPlayFabUserResult result) => TestLogInPlayfab.text = "회원가입 성공";

    void OnRegisterFailure(PlayFabError error) => TestLogInPlayfab.text = "회원가입 실패";

    /*
    Before the Test, must be login!
    */
    public void CoinSaveInPlayfab() //int save
    {
        PlayFabClientAPI.UpdatePlayerStatistics(new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate {StatisticName = "CoinText", Value = int.Parse(CoinText.text)},
            }
        },
        (result) => { TestLogInBackground.text = "서버에 코인이 저장됨"; },
        (error) => { TestLogInBackground.text = "값 저장 실패함 ㅠ"; });
    }

    public void CoinGetFromPlayfab() 
    {
        PlayFabClientAPI.GetPlayerStatistics(
            new GetPlayerStatisticsRequest(),
            (result) =>
            {
                CoinText.text = "";
                foreach (var eachStat in result.Statistics) CoinText.text += eachStat.Value;
                TestLogInBackground.text = "코인 로딩 성공함";
            },
            (error) => { CoinText.text = "값 불러오기 실패함 ㅠ"; });
    }
    public void DataSaveInPlayfab()
    {
        string MyCharacterData = JsonUtility.ToJson(new Serialization<SushiData>(AllSushiList), true);
        var request = new UpdateUserDataRequest() { Data = new Dictionary<string, string>() { { "playerSushiList", MyCharacterData } } };
        PlayFabClientAPI.UpdateUserData(request, (result) => TestLogInBackground.text = "서버에 데이터 저장됨", (error) => TestLogInBackground.text = "데이터 저장 실패함");
    }

    public void DataLoadFromPlayfab()
    {
        var request = new GetUserDataRequest() { PlayFabId = myId };
        PlayFabClientAPI.GetUserData(request,
        (result) => { foreach (var eachData in result.Data)
            AllSushiList = JsonUtility.FromJson<Serialization<SushiData>>(eachData.Value.Value).target;
            TestLogInBackground.text = "데이터 로드 성공"; 
        },
        (error) => TestLogInBackground.text = "데이터 로딩 실패");
    }
}
   /*
   TestId : catcat@naver.com  //  61AB140E3ACB2124
   PW : 999999  //9 여섯9
   Username : cat9
   TestId : dogdog@naver.com
   PW : 999999
   Username : dog9
   */
