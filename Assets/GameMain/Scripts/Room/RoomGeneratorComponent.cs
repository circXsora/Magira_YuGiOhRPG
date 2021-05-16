using GameFramework.Entity;
using GameFramework.Event;
using MGO;
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
        private int _currentX = 0, _currentY = 0;
        private Dictionary<(int x, int y), RoomData> _roomPositionDic = new Dictionary<(int x, int y), RoomData>();

        [Header("������Ϣ")]
        public int RoomNumber;
        public Color StartColor, EndColor;
        public LayerMask RoomLayer;

        [Header("λ�ÿ���")]
        public Vector3 InitialRoomPosition = Vector3.zero;
        public float XOffset, YOffset;
        private Vector3 _generatorPoint;

        private readonly List<RoomData> _roomDatas = new List<RoomData>();
        public int[] GenerateMonsterIds;

        private IEntityGroup _roomGroup;
        public IEntityGroup RoomGroup
        {
            get
            {
                if (_roomGroup == null)
                {
                    _roomGroup = GameEntry.Entity.GetEntityGroup("Room");
                }
                return _roomGroup;
            }
        }

        private void Start()
        {
            //GameEntry.Event.Subscribe(ShowEntitySuccessEventArgs.EventId, OnShowEntitySuccess);
            //GameEntry.Event.Subscribe(ShowEntityFailureEventArgs.EventId, OnShowEntityFailure);
        }


        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                ClearAllRooms();
                GenerateRooms();
            }
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


        public void HideAllRooms()
        {
            var roomEntities = RoomGroup.GetAllEntities();
            for (int i = 0; i < roomEntities.Length; i++)
            {
                Log.Info(roomEntities[i].GetType());
                var entity = roomEntities[i] as Entity;
                entity.gameObject.SetActive(false);
            }
        }

        public void ShowAllRooms()
        {
            var roomEntities = RoomGroup.GetAllEntities();
            for (int i = 0; i < roomEntities.Length; i++)
            {
                var entity = roomEntities[i] as UnityGameFramework.Runtime.Entity;
                entity.gameObject.SetActive(true);
            }
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

            // ���÷�����Ĺ�����Ϣ
            for (int i = 1; i < RoomNumber; i++)
            {
                var monsterNum = Range(1, 4);
                var roomData = _roomDatas[i];
                roomData.MonsterDatas = new MonsterData[monsterNum];
                roomData.DifficultValue = monsterNum;
                for (int j = 0; j < monsterNum; j++)
                {
                    roomData.MonsterDatas[j] = new MonsterData(GenerateMonsterIds.Random1());
                }
            }

            _roomDatas.Sort((r, h) => r.StepFormOrigin.Value.CompareTo(h.StepFormOrigin.Value));

            RoomData endRoom = null;
            var endlist = _roomDatas.FindAll(r => r.StepFormOrigin >= _roomDatas.Last().StepFormOrigin - 1);
            for (int i = endlist.Count - 1; i >= 0; i--)
            {
                if (endlist[i].WithRooms.Count(wr => wr != null) < 2)
                {
                    endRoom = endlist[i];
                    break;
                }
            }

            if (endRoom == null)
            {
                endRoom = _roomDatas.Last();
            }
            if (endRoom.StepFormOrigin < _roomDatas.Last().StepFormOrigin)
            {
                Log.Error($"���շ��䲽���������EndRoom����Ϊ��{endRoom.StepFormOrigin}�����и���Ĳ������䣬����Ϊ{_roomDatas.Last().StepFormOrigin}");
            }
            endRoom.Type = RoomType.End;
            foreach (var roomdata in _roomDatas)
            {
                GameEntry.Entity.ShowRoom(roomdata);
            }
        }

        public void ClearAllRooms()
        {
            var roomEntities = RoomGroup.GetAllEntities();
            for (int i = 0; i < roomEntities.Length; i++)
            {
                GameEntry.Entity.HideEntity(roomEntities[i].Id);
            }
            _roomPositionDic.Clear();
            _roomDatas.Clear();
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
            var directions = new List<Direction>() { Direction.Down, Direction.Up, Direction.Left, Direction.Right };
            directions.Shuffle();
            foreach (var direction in directions)
            {
                int x = _currentX;
                int y = _currentY;
                Vector3 p = _generatorPoint;

                switch (direction)
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