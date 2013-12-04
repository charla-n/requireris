using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Requireris
{
    [Serializable]
    [XmlType(TypeName="MyKeyValuePair")]
    public class MyKeyValuePair<K, V>
    {
        public MyKeyValuePair()
        {
        }
        public MyKeyValuePair(K key, V value)
        {
            Key = key;
            Value = value;
        }
        public K Key { get; set; }
        public V Value { get; set; }
    }
}
