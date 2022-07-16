using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using Firebase.Auth;
using UnityEngine.UI;

[System.Serializable]
public class Serialization<T> {
    public Serialization(List<T> _target) => target = _target;
    public List<T> target;
}

[System.Serializable]
public class Characters {
    public int _index;
    public string chr_english_name;
    public int inGame_price;
    [SerializeField]
    public Characters(int _index, string chr_english_name, int inGame_price) {
        this._index = _index;
        this.chr_english_name = chr_english_name;
        this.inGame_price = inGame_price;
    }
}

[System.Serializable]
public class DataManager : Singleton<DataManager> {
    public Text Debugtext;
    public string user_email;
    
    [SerializeField]
    public static DataManager instance;
    [SerializeField]
    public List<Characters> characters;
 
    private void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else {
            Destroy(this.gameObject);
        }
        characters = new List<Characters>();
        SetData();
    }
    
    public void SetData() {
        characters.Add(new Characters(0, "Black", 0);
        characters.Add(new Characters(1, "Cheese", 100);
    }

    public void JsonDataSave() {
        string jsonData = JsonUtility.ToJson(new Serialization<Characters>(characters));
        print(jsonData);
    }

    public void JsonDataLoad() {
        characters = JsonUtility.FromJson<Serialization<Characters>>(jsonData).target;
    }
}
