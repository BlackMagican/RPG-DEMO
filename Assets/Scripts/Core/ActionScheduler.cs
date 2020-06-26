using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
    /// <summary>
    /// This class is a bridge which handling interactions between actions.
    /// </summary>
    public class ActionScheduler : MonoBehaviour
    {
        IAction currentAction = null;

        public void StartAction(IAction action)
        {
            /*
             * We don't need to change currentAction
             * if currentAction equals to new action.
            */
            if (currentAction == action) return;

            /*
             * If we already have a action but call this methon again,
             * we need to cancel the current action, 
             * then we can execute new action.
             */ 
            if (currentAction != null)
            {
                currentAction.Cancel();
            }

            // Change the current action
            currentAction = action;
        }

        public void CancelCurrentAction()
        {
            StartAction(null);
        }
    }
}

