using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PA_DronePack
{
    public class LevelManager : MonoBehaviour
    {
        public void LoadScene(string _lvlname)
        {
            SceneManager.LoadScene(_lvlname);
        }
    }
}