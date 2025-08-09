using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Identity.Client;
using Newtonsoft.Json;

namespace SailfishCallPOC
{

    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Hello, World! From Sailfish Call POC Project");

            var armService = new AzureArmService("1f847e97-a9e0-4335-9e1a-531c116d57e0");

            var res = armService.GetResourceGroupsAsync().GetAwaiter().GetResult();
            Console.WriteLine("Resource Groups: " + JsonConvert.SerializeObject(res));
        }
    }
}