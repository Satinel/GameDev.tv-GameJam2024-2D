using System.IO;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    [SerializeField] Wallet _wallet; // TODO Rather than getting reference to Wallet etc. we make an interface for things we want to have save data
    string _path;

    void OnEnable()
    {
        Campaign.OnReturnToTown += Save;
    }

    void OnDisable()
    {
        Campaign.OnReturnToTown -= Save;
    }

    public void Save() // TODO Prompt about overwriting save data
    {
        
#if UNITY_WEBGL
{
        _path = "/idbfs/MangoKamenLastStand" + "/save.txt";
        // if(!Directory.Exists(_path))
        // {
        //     Directory.CreateDirectory("/idbfs/MangoKamenLastStand");
        // }
}
#else
{
        _path = Application.persistentDataPath + "/save.txt";
}
#endif
        int savedMoney = _wallet.SaveMoney();
        File.WriteAllText(_path, savedMoney.ToString());
    }

    public void Load() // TODO Prompt about loading save data
    {
#if UNITY_WEBGL
{
        _path = "/idbfs/MangoKamenLastStand" + "/save.txt";
}
#else
{
        _path = Application.persistentDataPath + "/save.txt";
}
#endif
        if(File.Exists(_path))
        {
            int savedMoney = int.Parse(File.ReadAllText(_path));
            _wallet.LoadMoney(savedMoney);
        }
    }

}
