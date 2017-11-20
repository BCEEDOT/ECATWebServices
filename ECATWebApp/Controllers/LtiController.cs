using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace Ecat.Web.Controllers
{
    //class to hold the LTI request information we know is coming across
    public class ECATLtiRequest
    {
        public string grant_type { get; set; }
        public string lti_message_type { get; set; }
        public string lti_version { get; set; }
        public string lis_person_contact_email_primary { get; set; }
        public string user_id { get; set; }
        public string roles { get; set; }
        public string lis_person_name_given { get; set; }
        public string lis_person_name_family { get; set; }
        public string custom_ecat_school { get; set; }
    }

    [Route("[controller]/[action]")]
    public class LtiController: Controller
    {
        [HttpPost]
        public async Task<ActionResult> LtiEntry(ECATLtiRequest req)
        {
            //form up a new httpclient to post over to the token endpoint
            //TODO: Update Uri (figure out how to read it from somewhere?)
            var client = new HttpClient();
            //dev
            client.BaseAddress = new Uri("http://localhost:62187");
            //aws testing
            //client.BaseAddress = new Uri("http://ec2-34-237-207-101.compute-1.amazonaws.com");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //form up all the LTI information to be put into the httpContent header for the post to the token endpoint
            var headList = new List<KeyValuePair<string, string>>();
            headList.Add(new KeyValuePair<string, string>("grant_type", "lti"));
            headList.Add(new KeyValuePair<string, string>("lti_message_type", req.lti_message_type));
            headList.Add(new KeyValuePair<string, string>("lti_version", req.lti_version));
            headList.Add(new KeyValuePair<string, string>("lis_person_contact_email_primary", req.lis_person_contact_email_primary));
            headList.Add(new KeyValuePair<string, string>("user_id", req.user_id));
            headList.Add(new KeyValuePair<string, string>("roles", req.roles));
            headList.Add(new KeyValuePair<string, string>("lis_person_name_given", req.lis_person_name_given));
            headList.Add(new KeyValuePair<string, string>("lis_person_name_family", req.lis_person_name_family));
            headList.Add(new KeyValuePair<string, string>("custom_ecat_school", req.custom_ecat_school));

            var content = new FormUrlEncodedContent(headList.AsEnumerable());

            //TODO: Error handling. Figure out how to read the error messages and context.Reject stuff from auth provider
            var response = await client.PostAsync("connect/token", content);
            var respString = await response.Content.ReadAsStringAsync();
            if (respString == "")
            {
                respString = response.ToString();
            }


            if (respString.Contains("Error"))
            {
                ViewBag.Error = respString;
                //ViewBag.Error = JsonConvert.DeserializeObject(respString);
            } else
            {
                ViewBag.Error = "ECAT Loading....";
                ViewBag.User = JsonConvert.DeserializeObject(respString);
            }

            return View();

        }
    }
}
