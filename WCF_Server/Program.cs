using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace WCF_Server
{
    class Program
    {

        static Dictionary<string,Data> _Data;
        static bool ISRUN = false;
        static void Main(string[] args)
        {
            ExcuteNotificationService();
            Timer t = new Timer();
            t.Enabled = true;
            t.Interval = 3 * 1000;
            t.Elapsed += t_Elapsed;
            while (true) ;
        }

        private static void ExcuteNotificationService()
        {
            ServiceHost host = new ServiceHost(typeof(NotificationService));
            host.Open();
        }

        static void t_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (!ISRUN)
            {
                ISRUN = true;
                Console.WriteLine("Sync data.......");
                GetiBikeData();
                ISRUN = false;
            }
        }

        private static void GetiBikeData()
        {
            List<DataCollection> _change = new List<DataCollection>();
            JsonSerializer json = new JsonSerializer();
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://ybjson01.youbike.com.tw:1002/gwjs.json");
            HttpResponseMessage resp = client.GetAsync("").Result;
            DataContainer data = null;
            if (resp.IsSuccessStatusCode)
            {
                data = JsonConvert.DeserializeObject<DataContainer>(resp.Content.ReadAsStringAsync().Result);
                if (_Data != null)
                {
                    foreach (KeyValuePair<string, Data> item in data.retVal)
                    {
                        var _source = _Data[item.Key];
                        if (_source.tot != item.Value.tot ||
                           _source.sbi != item.Value.sbi)
                        {
                            DataCollection c = new DataCollection();
                            c.Old = _source;
                            c.New = item.Value;
                            _change.Add(c);
                        }
                    }
                    if (_change.Count > 0)
                    {
                        _Data = data.retVal;
                        Console.WriteLine("follow location value changed.....");
                        foreach (var item in _change)
                        {
                            Console.WriteLine(string.Format("[{0}] tot:{1}->{2} sbi {3}->{4} ",
                                item.Old.sna,
                                item.Old.tot,
                                item.New.tot,
                                item.Old.sbi,
                                item.New.sbi));
                        }
                        NotificationClient(_change);
                    }
                    else
                    {
                        Console.WriteLine("no data changed....");
                    }
                }
                else
                {
                    _Data = data.retVal;
                    Console.WriteLine("has new data...");
                }
            }
        }

        private static void NotificationClient(IEnumerable<DataCollection> _change)
        {
            if (NotificationService.ClientCallbackList.Count > 0)
            {

                string data = JsonConvert.SerializeObject(_change);
                var list = NotificationService.ClientCallbackList;
                lock (list)
                {
                    foreach (var client in list)
                    {
                        client.PublishMessage(data);
                    }
                }
            }
        }
    }
    class DataContainer
    {
        public int retCode { get; set; }

        public Dictionary<string, Data> retVal { get; set; }
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
    public class DataCollection
    {
        public Data Old { get; set; }
        public Data New { get; set; }
    }
}
