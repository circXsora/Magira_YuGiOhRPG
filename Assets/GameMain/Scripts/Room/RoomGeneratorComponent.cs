using GameFramework.Event;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityGameFramework.Runtime;
using static UnityEngine.Random;

namespace BBYGO
{

    public class RoomGeneratorComponent : GameFrameworkComponent
    {
        private Direction _currentDirection;
        private int _currentX = 0, _currentY = 0;
        private Dictionary<(int x, int y), RoomData> _roomPositionDic = new Dictionary<(int x, int y), RoomData>();

        [Header("������Ϣ")]
        public int RoomNumber;
        public Color StartColor, EndColor;
        public LayerMask RoomLayer;

        [Header("λ�ÿ���")]
        public Vector3 InitialRoomPosition = Vector3.zero;
        private Vector3 _generatorPoint;
        public float XOffset, YOffset;

        private readonly List<RoomData> _roomDatas = new List<RoomData>();
        public RoomData EndRoom;
        public MonsterInfo[] GenMonsterList;

        private void Start()
        {
            //GameEntry.Event.Subscribe(ShowEntitySuccessEventArgs.EventId, OnShowEntitySuccess);
            //GameEntry.Event.Subscribe(ShowEntityFailureEventArgs.EventId, OnShowEntityFailure);
        }


        private void Update()
        {
            //if (Input.GetKeyDown(KeyCode.R))
            //{
            //    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            //}
        }

        private void OnDestroy()
        {
            //GameEntry.Event.Unsubscribe(ShowEntitySuccessEventArgs.EventId, OnShowEntitySuccess);
            //GameEntry.Event.Unsubscribe(ShowEntityFailureEventArgs.EventId, OnShowEntityFailure);
        }

        private void OnShowEntityFailure(object sender, GameEventArgs e)
        {

        }

        private void OnShowEntitySuccess(object sender, GameEventArgs e)
        {

        }

        public void GenerateRooms()
        {
            _generatorPoint = InitialRoomPosition;

            for (int i = 0; i < RoomNumber; i++)
            {
                var roomData = new RoomData { Position = _generatorPoint, IndexPosition = new Vector2Int(_currentX, _currentY) };
                _roomDatas.Add(roomData);
                _roomPositionDic.Add((_currentX, _currentY), _roomDatas[i]);
                GetNextRoomPosition();
            }

            _roomDatas[0].Type = RoomType.Start;
            _roomDatas[0].StepFormOrigin = 0;

            SetupDoorsAndSteps(_roomDatas[0]);
            _roomDatas.ForEach(roomData => roomData.WallID = GetWallID(roomData));

            // TODO:���÷�����Ĺ�����Ϣ
            //for (int i = 1; i < RoomNumber; i++)
            //{
            //    var monsterNum = UnityEngine.Random.Range(1, 4);
            //    var roomCtrl = _roomDatas[i].Room.GetComponent<RoomController>();
            //    roomCtrl.Monsters = new MonsterInfo[monsterNum];
            //    for (int j = 0; j < monsterNum; j++)
            //    {
            //        roomCtrl.Monsters[j] = GenMonsterList.Random();
            //        roomCtrl.SetupBattle();
            //    }
            //}
            //ComputeRoomHard();

            _roomDatas.Sort((r, h) => r.StepFormOrigin.Value.CompareTo(h.StepFormOrigin.Value));

            EndRoom = null;
            var endlist = _roomDatas.FindAll(r => r.StepFormOrigin >= _roomDatas.Last().StepFormOrigin - 1);
            for (int i = endlist.Count - 1; i >= 0; i--)
            {
                if (endlist[i].WithRooms.Count(wr => wr != null) < 2)
                {
                    EndRoom = endlist[i];
                    break;
                }
            }

            if (EndRoom == null)
            {
                EndRoom = _roomDatas.Last();
            }
            if (EndRoom.StepFormOrigin < _roomDatas.Last().StepFormOrigin)
            {
                Log.Error($"���շ��䲽���������EndRoom����Ϊ��{EndRoom.StepFormOrigin}�����и���Ĳ������䣬����Ϊ{_roomDatas.Last().StepFormOrigin}");
            }
            EndRoom.Type = RoomType.End;
            foreach (var roomdata in _roomDatas)
            {
                GameEntry.Entity.ShowRoom(roomdata);
            }
        }

        private void ComputeRoomHard()
        {
            //for (int i = 0; i < RoomNumber; i++)
            //{
            //    var roomCtrl = _roomDatas[i].Room.GetComponent<RoomController>();
            //    for (int j = 0; j < _roomDatas[i].WithRooms.Length; j++)
            //    {
            //        if (_roomDatas[i].WithRooms[j] == null)
            //        {
            //            continue;
            //        }
            //        var withRoomCtrl = _roomDatas[i].WithRooms[j].Room.GetComponent<RoomController>();
            //        roomCtrl.Doors[j].GetComponentInChildren<HardDisplay>().SetHard(withRoomCtrl.Monsters.Length);
            //    }
            //}
        }

        private void SetupDoorsAndSteps(RoomData roomData)
        {
            for (int i = 0; i < 4; i++)
            {
                var direction = (Direction)i;
                var linkedRoomData = GetRoomLinkedRoom(roomData, direction);
                roomData.DoorsActiveInfos[i] = linkedRoomData != null;
                if (linkedRoomData != null)
                {
                    roomData.WithRooms[i] = linkedRoomData;
                    if (linkedRoomData.StepFormOrigin.HasValue)
                    {
                        if (linkedRoomData.StepFormOrigin.Value > roomData.StepFormOrigin.Value + 1)
                        {
                            linkedRoomData.StepFormOrigin = roomData.StepFormOrigin.Value + 1;
                            SetupDoorsAndSteps(linkedRoomData);
                        }
                    }
                    else
                    {
                        linkedRoomData.StepFormOrigin = roomData.StepFormOrigin.Value + 1;
                        SetupDoorsAndSteps(linkedRoomData);
                    }
                }
            }
        }

        private Vector3 GetNextRoomPosition()
        {
            var directions = new Direction[4] { Direction.Down, Direction.Up, Direction.Left, Direction.Right };
            Array.Sort(directions, (l, r) => Range(-1, 2) - 0); // ϴ��
            foreach (var direction in directions)
            {
                _currentDirection = direction;
                int x = _currentX;
                int y = _currentY;
                Vector3 p = _generatorPoint;

                switch (_currentDirection)
                {
                    case Direction.Up:
                        y += 1;
                        p += new Vector3(0, YOffset, 0);
                        break;
                    case Direction.Down:
                        y -= 1;
                        p += new Vector3(0, -YOffset, 0);
                        break;
                    case Direction.Left:
                        x -= 1;
                        p += new Vector3(-XOffset, 0, 0);
                        break;
                    case Direction.Right:
                        x += 1;
                        p += new Vector3(XOffset, 0, 0);
                        break;
                    default:
                        break;
                }

                if (!_roomPositionDic.ContainsKey((x, y)))
                {
                    _currentX = x;
                    _currentY = y;
                    _generatorPoint = p;
                    return _generatorPoint;
                }
            }
            throw new GameFramework.GameFrameworkException("�ĸ������϶��Ѿ��з��䣬��������ʧ�ܣ�");

        }

        private RoomData GetRoomLinkedRoom(RoomData roomData, Direction direction)
        {
            var indexPosition = roomData.IndexPosition;
            switch (direction)
            {
                case Direction.Up:
                    indexPosition.y += 1;
                    break;
                case Direction.Down:
                    indexPosition.y -= 1;
                    break;
                case Direction.Left:
                    indexPosition.x -= 1;
                    break;
                case Direction.Right:
                    indexPosition.x += 1;
                    break;
                default:
                    Debug.LogError("no such direction!");
                    return null;
            }
            if (_roomPositionDic.ContainsKey((indexPosition.x, indexPosition.y)))
                return _roomPositionDic[(indexPosition.x, indexPosition.y)];
            else
                return null;
        }

        //����������������
        private int GetWallID(RoomData newRoom)
        {
            string up = (newRoom.WithRooms[(int)Direction.Up] != null ? 1 : 0).ToString();
            string down = (newRoom.WithRooms[(int)Direction.Down] != null ? 1 : 0).ToString();
            string left = (newRoom.WithRooms[(int)Direction.Left] != null ? 1 : 0).ToString();
            string right = (newRoom.WithRooms[(int)Direction.Right] != null ? 1 : 0).ToString();
            return int.Parse("1" + up + down + left + right);
        }
    }
}