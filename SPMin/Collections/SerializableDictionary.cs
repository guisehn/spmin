using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SPMin.Collections
{
    public class SerializableDictionary : Dictionary<string, string>
    {
        private const char Separator = ':';

        public SerializableDictionary() : base() { }

        public SerializableDictionary(string serializedDictionary) : base()
        {
            serializedDictionary.Split('\n')
                .Select(s => s.Trim())
                .Where(s => s != "")
                .Select(s => s.Split(Separator))
                .ToList()
                .ForEach(parts => { Add(parts[0], parts[1]); });
        }

        public override string ToString()
        {
            return String.Join("\n",
                this.Select(kv => kv.Key + Separator + kv.Value)
                .ToArray());
        }
    }
}
