using System.Collections.Generic;
using Newtonsoft.Json;

namespace Modules.SaveSystem.Repositories.SerializeStrategies
{
    public sealed class JsonSerialization : Serialization
    {
        public override string Serialize(Dictionary<string, string> data) => JsonConvert.SerializeObject(data);

        public override Dictionary<string, string> Deserialize(string json) =>
            JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
    }
}