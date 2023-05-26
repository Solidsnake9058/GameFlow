using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityGame.App.Actor
{
    public class PlayerAnimatorEvent : MonoBehaviour
    {
        PlayerDirectionMoveTemp m_Player;
        private void Start()
        {
            m_Player = GetComponentInParent<PlayerDirectionMoveTemp>();
        }

        public void TurnEnd()
        {
            m_Player.TurnEnd();
        }
    }
}