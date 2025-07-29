using System.Collections.Generic;
using UnityEngine;

namespace Modules.SaveSystem.Repositories.SerializeStrategies
{
    public abstract class Serialization : MonoBehaviour, ISerialization
    {
        public abstract string Serialize(Dictionary<string, string> data);

        public abstract Dictionary<string, string> Deserialize(string json);
    }
}