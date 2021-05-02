using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomInfo : MonoBehaviour
{
    public static int curRoom = -1, lastRoom = -1;
    [ConditionalShow(true)] public int roomIndex;
    public Dictionary<int, GameObject> roomEntranceDic;

    [System.Serializable]
    struct RoomEntrance
    {
        public int lastRoomIndex;
        public GameObject EntrancePos;
    }
    [SerializeField]
    RoomEntrance[] entrances;

    public void InitRoomInfo()
    {
        roomIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
        // dic
        roomEntranceDic = new Dictionary<int, GameObject>();
        for (int i = 0; i < entrances.Length; i++)
        {
            //因为忘记mainmenu了，所以要+1
            if (!roomEntranceDic.ContainsKey(entrances[i].lastRoomIndex+1))
            {
                roomEntranceDic.Add(entrances[i].lastRoomIndex+1, entrances[i].EntrancePos);
            }
        }
        lastRoom = curRoom;
        curRoom = roomIndex;
    }
}
