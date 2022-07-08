using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CHIS.NotificationCenter.Domain.Enum;
using Newtonsoft.Json.Linq;

namespace CHIS.NotificationCenter.Application.Services
{
    public class OneSignalInterfaceService : IOneSignalInterfaceService
    {
        public OneSignalInterfaceService() { }

        public async Task<string> SendMessage(string title, string contents, Dictionary<string, string> data, List<string> employeeList)
        {
            //수신대상자가 없으면 중단
            if (employeeList == null)
            {
                return null;
            }

            string OneSignalAddress         = CHIS.Framework.Core.Configuration.ConfigurationManager.AppSettings.GetDictionary()["OneSignalAddress"      ];
            string OneSignalAppId           = CHIS.Framework.Core.Configuration.ConfigurationManager.AppSettings.GetDictionary()["OneSignalAppId"        ];
            string OneSignalAuthorization   = CHIS.Framework.Core.Configuration.ConfigurationManager.AppSettings.GetDictionary()["OneSignalAuthorization"];
            JObject param = new JObject();
            param.Add("app_id"  , OneSignalAppId);                              //AppId
            param.Add("headings", JObject.Parse($"{{ en:'{title}'}}"));         //제목
            param.Add("contents", JObject.Parse($"{{ en:'{contents}'}}"));      //본문

            //수신자 필터
            JArray filters = new JArray();
            for (int i = 0; i < employeeList.Count; i++)
            {
                filters.Add(JObject.Parse($"{{ field:'tag', key:'employeeId', value:'{employeeList[i]}'}}"));

                if (i < employeeList.Count - 1)
                {
                    filters.Add(JObject.Parse("{ operator : 'OR' }"));
                }
            }
//#if DEBUG
//            //테스트용 임시 코드
//            //filters.Add(JObject.Parse("{ operator : 'OR' }"));
//            //filters.Add(JObject.Parse("{ field:'tag', key:'employeeId', value:'GUID_10009' }"));
//#endif
            param.Add("filters", filters);
            JObject AdditionalData = JObject.FromObject(data?? new Dictionary<string, string>());
            //AdditionalData.Add("pushMessageType", $"{messageType}");
            param.Add("data", AdditionalData);

            // The notification will be expired if the device does not come back online within this time/time to live : 1day  (60 * 60 * 24)
            // Supported Platform : iOS, ANDROID, CHROME, SAFARI, CHROMEWEB
            param.Add("ttl", 86400);

            using (HttpClient client = new HttpClient())
            {
                //client.BaseAddress = new Uri(url);
                //client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json;charset=utf-8"));
                //HTTPS 호출시 예외가 떨어지는 현상을 막기위한 코드
                client.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/36.0.1985.143 Safari/537.36");
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", OneSignalAuthorization);

                var content = new StringContent(param.ToString(), Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(OneSignalAddress, content).ConfigureAwait(false);

                string result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                return result;
            }
        }
    }
}
