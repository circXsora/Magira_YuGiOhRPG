using System.IO;
using UnityEditor;
using UnityEngine;

namespace BBYGO
{

    public static class GameEntityIDGenerator
    {
        public static string EntityConfigPath = "GameMain/DataTables/Entity.txt";

        [MenuItem("Tools/������Ϸʵ��ID����")]
        public static void GenerateGameEntityIDs()
        {
            var path = Path.Combine(Application.dataPath, EntityConfigPath);
            Debug.Log("������Ϸʵ��ID����, ·��"+ path);
            var lines = File.ReadAllLines(path);
            foreach (var line in lines)
            {

            }
        }
    }

}