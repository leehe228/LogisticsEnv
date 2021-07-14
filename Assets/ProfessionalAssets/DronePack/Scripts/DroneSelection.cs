using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PocketRocket
{
    public class DroneSelection : MonoBehaviour
    {
        [System.Serializable]
        public class SelectableDrone
        {
            public GameObject skin1;
            public GameObject skin2;
            public GameObject skin3;
            public GameObject skin4;
        }

        public Text nameLabel;
        [Space(10)]
        public List<SelectableDrone> selectableDrones = new List<SelectableDrone>();

        public static int droneIndex = 0;
        public static int skinIndex = 0;

        private void Start()
        {
            RefreshSelection();
        }

        private void RefreshSelection()
        {
            foreach (SelectableDrone drone in selectableDrones)
            {
                drone.skin1.SetActive((droneIndex == selectableDrones.IndexOf(drone) && skinIndex == 0) ? true : false);
                drone.skin2.SetActive((droneIndex == selectableDrones.IndexOf(drone) && skinIndex == 1) ? true : false);
                drone.skin3.SetActive((droneIndex == selectableDrones.IndexOf(drone) && skinIndex == 2) ? true : false);
                drone.skin4.SetActive((droneIndex == selectableDrones.IndexOf(drone) && skinIndex == 3) ? true : false);
                if(nameLabel) {
                    if(droneIndex == selectableDrones.IndexOf(drone))
                    {
                        nameLabel.text = drone.skin1.transform.parent.name;
                    }
                }
            }
        }

        public void CycleGroupsUp()
        {
            if (droneIndex < selectableDrones.Count - 1)
            {
                droneIndex += 1;
            }
            else
            {
                droneIndex = 0;
            }
            RefreshSelection();
        }
        public void CycleGroupsDown()
        {
            if (droneIndex > 0)
            {
                droneIndex -= 1;
            }
            else
            {
                droneIndex = selectableDrones.Count - 1;
            }
            RefreshSelection();
        }
        public void CycleSkinsUp()
        {
            if (skinIndex < 3)
            {
                skinIndex += 1;
            }
            else
            {
                skinIndex = 0;
            }
            RefreshSelection();
        }
    }
}