﻿//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using GameFramework.DataTable;
using System;
using UnityEngine;

namespace BBYGO
{
    [Serializable]
    public class PlayerData : EntityData
    {
        public MonsterData[] MonsterDatas { get; set; }
        public DRPlayer GetEntryData() => GameEntry.DataTable.GetDataTable<DRPlayer>().GetDataRow(TypeId);
        public PlayerData(int entityId, int typeId)
            : base(entityId, typeId)
        {

        }
    }
}
