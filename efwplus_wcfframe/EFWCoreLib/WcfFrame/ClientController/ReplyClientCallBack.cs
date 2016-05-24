using EFWCoreLib.WcfFrame.WcfService.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace EFWCoreLib.WcfFrame.ClientController
{

    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    public class ReplyClientCallBack: IClientService
    {
        //回调委托
        public Action<string> ReplyClientAction {
            get; 
            set; 
        }

        //超级回调委托
        public Func<string,string,string,string,string> SuperReplyClientAction
        {
            get;
            set;
        }

        #region IClientService 成员

        public void ReplyClient(string jsondata)
        {
            if (ReplyClientAction != null)
            {
                ReplyClientAction(jsondata);
            }
        }

        public string SuperReplyClient(string plugin, string controller, string method, string jsondata)
        {
            if (SuperReplyClientAction != null)
            {
                return SuperReplyClientAction(plugin, controller, method, jsondata);
            }
            return null;
        }

        #endregion
    }
}
