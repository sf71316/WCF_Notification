using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using WCF_Client.WcfDuplexService;

namespace WCF_Client
{
    class Program
    {
        Program()
        {

        }
        static void Main(string[] args)
        {
            PubSubServiceClient proxy = new PubSubServiceClient(new InstanceContext(new CallbackObject()));
            proxy.Subscribe();
            while (true) ;
        }


    }
    class CallbackObject : WCF_Client.WcfDuplexService.IPubSubServiceCallback
    {
        public void PublishMessage(string Message)
        {
            var data = JsonConvert.DeserializeObject<List<DataCollection>>(Message);
            foreach (var item in data)
            {
                Console.WriteLine(string.Format("[{0}] tot:{1}->{2} sbi {3}->{4} ",
                    item.Old.sna,
                    item.Old.tot,
                    item.New.tot,
                    item.Old.sbi,
                    item.New.sbi));
            }
        }
    }
    public class DataCollection
    {
        public Data Old { get; set; }
        public Data New { get; set; }
    }
    public class Data
    {
        public string sno { get; set; }
        public string sna { get; set; }
        public string tot { get; set; }
        public string sbi { get; set; }
        public string sarea { get; set; }
        public string mday { get; set; }
        public string lat { get; set; }
        public string lng { get; set; }
        public string ar { get; set; }
        public int bemp { get; set; }
        public int act { get; set; }
    }
}
