#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityGame.App.Manager
{
    public class CreatorGameManager : GameManager
    {
        private void Start()
        {

        }

        public override bool StartGame()
        {
            if (m_IsMainState)
            {
                Debug.Log("Play");
                int temp = 0;
                if (GameMediator.m_StageManager.LoadStage(ref temp, ref temp))
                {
                    ChangeState(GameState.Play);
                    return true;
                }
            }
            return false;
        }

        protected override void MainUpdate()
        {
        }

        public override void GameClear()
        {
            CreatorMediator.m_CreatorUIManager.SetConfirmAction("ステージクリア", "リプレイ", "戻る", CreatorMediator.m_CreateShapeManager.ReTestStage, CreatorMediator.m_CreateShapeManager.StopTest);
        }

        public override void GameOver()
        {
            CreatorMediator.m_CreatorUIManager.SetConfirmAction("ゲームオーバー", "リプレイ", "戻る", CreatorMediator.m_CreateShapeManager.ReTestStage, CreatorMediator.m_CreateShapeManager.StopTest);
        }
    }
}
#endif