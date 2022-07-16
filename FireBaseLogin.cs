using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using Firebase.Auth;
using UnityEngine.UI;

[System.Serializable]
public class DataManager : Singleton<DataManager> {
    [SerializeField]
    public InputField emailField;
    [SerializeField]
    public InputField passField;

    [SerializeField]
    public string user_id_UID;
    [SerializeField]
    public string user_email;
    [SerializeField]
    public static DataManager instance;

    private bool isLoad = false;

    private DatabaseReference databaseReference;
    private FirebaseAuth firebaseAuth;
 
    private void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else {
            Destroy(this.gameObject);
        }

        //Firebase
        firebaseAuth = FirebaseAuth.DefaultInstance;
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
        
    }

    public void OnClickFirebaseCharacterDataSave() {
        string jsonData = JsonUtility.ToJson(new Serialization<Characters>(characters));
        databaseReference.Child("Character_Database").SetRawJsonValueAsync(jsonData);
    }

    public void FirebaseCharacterDataLoad() {
        // Set CharacterData
        FirebaseDatabase.DefaultInstance.GetReference("Reference").GetValueAsync().ContinueWith(task => {
            if (task.IsCanceled) {
                Debug.Log("Canceled");
            }
            else if (task.IsFaulted) {
                Debug.Log("Faulted");
            }
            else {
                DataSnapshot snapshot = task.Result;
                string jsonData = snapshot.GetRawJsonValue();
                characters = JsonUtility.FromJson<Serialization<Characters>>(jsonData).target;
            }
        });
    }
    private void Update() {
        if(!isLoad){
            FirebaseUserDataLoad();
        }
    }
    public void TestLogin_firebase(){
        // userid = bbB7UnATcVOK9zYVcQC0Cgt6ZMi1;
        // email = test.@test.coom;
        emailField.text = "test@test.com";
        passField.text = "qwerasdf12!";
        firebaseAuth.SignInWithEmailAndPasswordAsync(emailField.text, passField.text).ContinueWith(
            task => {
                if (task.IsCompleted && !task.IsFaulted && !task.IsCanceled) {
                    Debug.Log(emailField.text + " 로 자동 로그인 하셨습니다.");
                }
                else {
                    Debug.Log("로그인에 실패하셨습니다.");
                }
                Firebase.Auth.FirebaseUser alreadyUser = task.Result;
                user_id_UID = alreadyUser.UserId;
                user_email = alreadyUser.Email;
                FirebaseUserDataLoad();
               
            }
        );

        // StartCoroutine(FirebaseUserDataLoad()); 
    }

    public void LoginInFirebase() {
        emailField.text = "test@test.com";
        passField.text = "qwerasdf12!";
        firebaseAuth.SignInWithEmailAndPasswordAsync(emailField.text, passField.text).ContinueWith(
            task => {
                if (task.IsCompleted && !task.IsFaulted && !task.IsCanceled) {
                    Debug.Log(emailField.text + " 로 로그인 하셨습니다.");
                }
                else {
                    Debug.Log("로그인에 실패하셨습니다.");
                }
                Firebase.Auth.FirebaseUser alreadyUser = task.Result;
                user_id_UID = alreadyUser.UserId;
                user_email = alreadyUser.Email;
            }
        );
        // FirebaseUserDataLoad();
        // StartCoroutine(FirebaseUserDataLoad()); 
    }

    public void FirebaseUserDataLoad() {
        string refer = user_id_UID;
        if(refer == "") return;
        
        var DBtask = FirebaseDatabase.DefaultInstance.GetReference("Refer").Child(refer).GetValueAsync();
        // while(!DBtask.IsCompleted){
            DBtask.ContinueWith(task => {
                if (task.IsCanceled) {
                    Debug.Log("Canceled");
                    Debugtext.text = "Load Canceled";
                    FirebaseUserDataLoad();
                }
                else if (task.IsFaulted) {
                    Debug.Log("Faulted");
                    Debugtext.text = "Load Faulted";
                    FirebaseUserDataLoad();
                }
                else {
                    DataSnapshot snapshot = task.Result;
                    string jsonData = snapshot.GetRawJsonValue();
                    User_Info user_Info0 = JsonUtility.FromJson<User_Info>(jsonData);
                    User_Info user_Info2 = new User_Info("","",0,0, new List<Characters>(){});
                    JsonUtility.FromJsonOverwrite(jsonData, user_Info2);
                    user_Info = user_Info2;
                    Debugtext.text = "Database Load Complete!!";
                    isLoad = true;
                } 
            });
        // }
        // yield return new WaitUntil(predicate: () => DBtask.IsCompleted);
    }

    public void RegisterInFirebase() {
        firebaseAuth.CreateUserWithEmailAndPasswordAsync(emailField.text, passField.text).ContinueWith(
            task => {
                if (!task.IsCanceled && !task.IsFaulted) {
                    Debug.Log(emailField.text + " Regitser\n");
                    Firebase.Auth.FirebaseUser newUser = task.Result;
                    user_Info = (new User_Info(newUser.UserId, newUser.Email, 0, 0, characters));
                    WriteNewUser_InFireBase(newUser.UserId);
                }
                else
                    Debug.Log("회원가입 실패\n");
            });
    }

    void WriteNewUser_InFireBase(string userid) {
        string json = JsonUtility.ToJson((user_Info)); 
        databaseReference.Child("Refer").Child(userid).SetRawJsonValueAsync(json);
    }
}
