using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace WCF_Server
{
    [ServiceContract(
SessionMode = SessionMode.Required, CallbackContract = typeof
(IPublisherEvents))]
    public interface IPubSubService
    {
        [OperationContract(IsOneWay = false, IsInitiating = true)]
        void Subscribe();
        [OperationContract(IsOneWay = false, IsInitiating = true)]
        void Unsubscribe();
        

    }
    public interface IPublisherEvents
    {
        [OperationContract(IsOneWay = true)]
        void PublishMessage(string Message);
    }
}
