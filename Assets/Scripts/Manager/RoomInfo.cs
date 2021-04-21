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
            if (!roomEntranceDic.ContainsKey(entrances[i].lastRoomIndex))
            {
                roomEntranceDic.Add(entrances[i].lastRoomIndex, entrances[i].EntrancePos);
            }
        }
        lastRoom = curRoom;
        curRoom = roomIndex;
    }
}
