using GameFramework;
using GameFramework.Fsm;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using mgo;
using GameFramework.Procedure;
using UnityGameFramework.Runtime;

namespace ygo
{

    public class ProcedureSplash : ProcedureBase
    {
        protected override void OnUpdate(IFsm<IProcedureManager> procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);

            // TODO: ������Բ���һ�� Splash ����
            // ...

            //if (mgo.GameEntry.Base.EditorResourceMode)
            //{
            //    // �༭��ģʽ
            //    Log.Info("Editor resource mode detected.");
            //    ChangeState<ProcedurePreload>(procedureOwner);
            //}
            //else if (mgo.GameEntry.Resource.ResourceMode == ResourceMode.Package)
            //{
            //    // ����ģʽ
            //    Log.Info("Package resource mode detected.");
            //    ChangeState<ProcedureInitResources>(procedureOwner);
            //}
            //else
            //{
            //    // �ɸ���ģʽ
            //    Log.Info("Updatable resource mode detected.");
            //    ChangeState<ProcedureCheckVersion>(procedureOwner);
            //}

            Log.Info("Editor resource mode detected.");
            ChangeState<ProcedurePreload>(procedureOwner);
        }
    }

}