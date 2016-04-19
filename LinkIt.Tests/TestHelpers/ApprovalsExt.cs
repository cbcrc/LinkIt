using ApprovalTests;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace LinkIt.Tests.TestHelpers {
    public static class ApprovalsExt {
        private static readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings {
            DateTimeZoneHandling = DateTimeZoneHandling.Utc,
            Formatting = Formatting.Indented,
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            TypeNameHandling = TypeNameHandling.Auto
        };

        public static void VerifyPublicProperties(object toVerify) {
            string json = JsonConvert.SerializeObject(toVerify, _jsonSerializerSettings);
            Approvals.Verify(json);
        }
    }
}
