using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace WCF_Server
{
    // 注意: 您可以使用 [重構] 功能表上的 [重新命名] 命令同時變更程式碼和組態檔中的類別名稱 "NotificationService"。
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class NotificationService : IPubSubService
    {
        public static List<IPublisherEvents> ClientCallbackList { get; set; }
        public NotificationService()
        {
            ClientCallbackList = new List<IPublisherEvents>();
        }
        public void Subscribe()
        {
            var client = OperationContext.Current.GetCallbackChannel<IPublisherEvents>();
            var sessionid = OperationContext.Current.SessionId;
            OperationContext.Current.Channel.Closing += Channel_Closing;
            ClientCallbackList.Add(client);
        }
        public void Unsubscribe()
        {
            var client = OperationContext.Current.GetCallbackChannel<IPublisherEvents>();
            var sessionid = OperationContext.Current.SessionId;
            ClientCallbackList.Remove(client);
        }
        void Channel_Closing(object sender, EventArgs e)
        {
            lock (ClientCallbackList)
            {
                ClientCallbackList.Remove((IPublisherEvents)sender);
            }
        }
    }
}
