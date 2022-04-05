using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class SavingData : MonoBehaviour
{
    private string DATA_PATH = "/MyGame.dat";

    private Player myPlayer;

    void Start()
    {
        SaveData();
        print("DATA PATH IS " + Application.persistentDataPath + DATA_PATH);

        LoadData();
        if (myPlayer != null)
        {
            print("Player Name: " + myPlayer.Name);
            print("Player Power: " + myPlayer.Power);
            print("Player Health: " + myPlayer.Health);
        }
    }

    private void SaveData()
    {
        FileStream file = null;

        try
        {
            BinaryFormatter bf = new BinaryFormatter();

            file = File.Create(Application.persistentDataPath + DATA_PATH);

            Player p = new Player("Warrior", 67, 100);

            // encrypt and save the data, 부호화
            bf.Serialize(file, p); // p를 byte로 바꿔(직렬화) file에 저장
        }
        catch (Exception e)
        {
            if (e != null)
            {
                // handle exception
            }
        }
        finally
        {
            if (file != null)
            {
                file.Close();
            }
        }
    }

    private void LoadData()
    {
        FileStream file = null;

        try
        {
            BinaryFormatter bf = new BinaryFormatter();

            file = File.Open(Application.persistentDataPath + DATA_PATH, FileMode.Open);

            // decrypting and loading data, 복호화
            myPlayer = bf.Deserialize(file) as Player; // byte를 다시 객체화함(역직렬화). as Player 써줘야함.
        }
        catch (Exception e)
        {
            if (e != null)
            {
                // handle exception
            }
        }
        finally
        {
            if (file != null)
            {
                file.Close();
            }
        }
    }
}
