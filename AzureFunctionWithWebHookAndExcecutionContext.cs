using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;

namespace ebaresearchdev
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req, ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

         //   log.LogInformation(requestBody);

            requestBody = FormatJson(requestBody);

            Microsoft.Xrm.Sdk.RemoteExecutionContext remoteExecutionContext = DeserializeJsonString<Microsoft.Xrm.Sdk.RemoteExecutionContext>(requestBody);

            log.LogInformation($"{remoteExecutionContext.PrimaryEntityName} record with Id: {remoteExecutionContext.PrimaryEntityId} has just been created!");
            return (ActionResult)new OkObjectResult(requestBody);
        }


        public static string FormatJson(string unformattedJson)
        {
            string formattedJson = string.Empty;
            try
            {
                formattedJson = unformattedJson.Trim('"');
                formattedJson = System.Text.RegularExpressions.Regex.Unescape(formattedJson);
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
            return formattedJson;
        }

        public static RemoteContextType DeserializeJsonString<RemoteContextType>(string jsonString)
        {
            //create an instance of generic type object
            RemoteContextType obj = Activator.CreateInstance<RemoteContextType>();
            MemoryStream ms = new MemoryStream(Encoding.Unicode.GetBytes(jsonString));
            System.Runtime.Serialization.Json.DataContractJsonSerializer serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(obj.GetType());
            obj = (RemoteContextType)serializer.ReadObject(ms);
            ms.Close();
            return obj;
        }
    }

}
