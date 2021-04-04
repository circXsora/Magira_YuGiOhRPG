using GameFramework;
using GameFramework.Fsm;
using GameFramework.Procedure;
using mgo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace ygo
{

    public class ProcedureLaunch : ProcedureBase
    {
        protected override void OnEnter(IFsm<IProcedureManager> procedureOwner)
        {
            base.OnEnter(procedureOwner);
            string welcomeMessage = Utility.Text.Format("Hello Game Framework {0}.", Version.GameFrameworkVersion);

            // ��ӡ���Լ�����־�����ڼ�¼��������־��Ϣ
            //Log.Debug(welcomeMessage);

            // ��ӡ��Ϣ������־�����ڼ�¼��������������־��Ϣ
            Log.Info(welcomeMessage);

            MGOGameEntry.Base.EditorResourceMode = true;

            // ��ӡ���漶����־�������ڷ����ֲ������߼����󣬵��в��ᵼ����Ϸ�������쳣ʱʹ��
            //Log.Warning(welcomeMessage);

            // ��ӡ���󼶱���־�������ڷ��������߼����󣬵��в��ᵼ����Ϸ�������쳣ʱʹ��
            //Log.Error(welcomeMessage);

            // ��ӡ���ش��󼶱���־�������ڷ������ش��󣬿��ܵ�����Ϸ�������쳣ʱʹ�ã���ʱӦ�����������̻��ؽ���Ϸ���
            //Log.Fatal(welcomeMessage);
        }
        protected override void OnUpdate(IFsm<IProcedureManager> procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);

            // ����һ֡���л��� Splash չʾ����
            ChangeState<ProcedureSplash>(procedureOwner);
        }

    }

}