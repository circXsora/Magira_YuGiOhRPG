using UnityEngine;

namespace mgo
{
    /// <summary>
    /// ��Ϸ��ڡ�
    /// </summary>
    public partial class MGOGameEntry : MonoBehaviour
    {
        private void Start()
        {
            // ��ʼ���������
            InitBuiltinComponents();

            // ��ʼ���Զ������
            InitCustomComponents();

            // ��ʼ���Զ��������
            InitCustomDebuggers();
        }
    }
}