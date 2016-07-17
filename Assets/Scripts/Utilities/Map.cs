using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.Assertions;

namespace Utilities
{
    public class Map<T1, T2> : IXmlSerializable
    {
        private SerializableDictionary<T1, T2> _forward = new SerializableDictionary<T1, T2>();
        private SerializableDictionary<T2, T1> _reverse = new SerializableDictionary<T2, T1>();

        public int Count
        {
            get
            {
                Assert.AreEqual(_forward.Count, _reverse.Count);
                return _forward.Count;
            }
        }

        public Map()
        {
            Init();
        }

        private void Init()
        {
            this.Forward = new Indexer<T1, T2>(_forward);
            this.Reverse = new Indexer<T2, T1>(_reverse);
        }

        public class Indexer<T3, T4>
        {
            private SerializableDictionary<T3, T4> _dictionary;

            public SerializableDictionary<T3, T4>.KeyCollection Keys
            {
                get
                {
                    return _dictionary.Keys;
                }
            }

            public Indexer()
            {

            }

            public Indexer(SerializableDictionary<T3, T4> dictionary)
            {
                _dictionary = dictionary;
            }
            public T4 this[T3 index]
            {
                get { return _dictionary[index]; }
                set { _dictionary[index] = value; }
            }
        }

        public void Add(T1 t1, T2 t2)
        {
            _forward.Add(t1, t2);
            _reverse.Add(t2, t1);
        }

        public bool ContainsKey(T1 key)
        {
            return _forward.ContainsKey(key);
        }

        public bool ContainsKey(T2 key)
        {
            return _reverse.ContainsKey(key);
        }

        public XmlSchema GetSchema()
        {
            return ((IXmlSerializable)_forward).GetSchema();
        }

        public void ReadXml(XmlReader reader)
        {
            XmlSerializer forwardSerializer = new XmlSerializer(typeof(SerializableDictionary<T1, T2>));
            XmlSerializer reverseSerializer = new XmlSerializer(typeof(SerializableDictionary<T2, T1>));
            bool wasEmpty = reader.IsEmptyElement;
            reader.Read();

            if (wasEmpty)
                return;

            reader.ReadStartElement("forward");
            _forward = (SerializableDictionary<T1, T2>)forwardSerializer.Deserialize(reader);
            reader.ReadEndElement();

            reader.ReadStartElement("reverse");
            _reverse = (SerializableDictionary<T2,T1>)reverseSerializer.Deserialize(reader);
            reader.ReadEndElement();

            Init();
        }

        public void WriteXml(XmlWriter writer)
        {
            XmlSerializer forwardSerializer = new XmlSerializer(typeof(SerializableDictionary<T1, T2>));
            XmlSerializer reverseSerializer = new XmlSerializer(typeof(SerializableDictionary<T2, T1>));

            writer.WriteStartElement("forward");
            forwardSerializer.Serialize(writer, _forward);
            writer.WriteEndElement();

            writer.WriteStartElement("reverse");
            reverseSerializer.Serialize(writer, _reverse);
            writer.WriteEndElement();
        }

        public Indexer<T1, T2> Forward { get; set; }
        public Indexer<T2, T1> Reverse { get; set; }
    }
}
