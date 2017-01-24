﻿using Dlp.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Dlp.Sdk.Tests {

    [ExcludeFromCodeCoverage]
    public sealed class SerializableObject {

        public SerializableObject() { }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ObjectName { get; set; }

        public int ObjectValue { get; set; }

        public string ObjectCreationDate { get; set; }
    }

    [ExcludeFromCodeCoverage]
    public sealed class CamelCaseSerializableObject {

        public CamelCaseSerializableObject() { }

        public string objectName { get; set; }

        public int objectValue { get; set; }

        public string objectCreationDate { get; set; }
    }

    [ExcludeFromCodeCoverage]
    [TestClass]
    public class SerializerTest {

        [TestMethod]
        public void BinaryObjectDesserialize() {

            DateTime source = DateTime.Now;

            byte[] actual;

            // Formatter utilizado para gerenciar a serialização binária.
            BinaryFormatter binaryFormatter = new BinaryFormatter();

            // Serializa o valor do item a ser adicionano no cache.
            using (MemoryStream stream = new MemoryStream()) {

                binaryFormatter.Serialize(stream, source);
                actual = stream.ToArray();
            }

            Assert.IsNotNull(actual);

            DateTime result = Serializer.BinaryDeserialize<DateTime>(actual);

            Assert.AreEqual(source, result);
        }

        [TestMethod]
        public void BinaryNullObjectDesserialize() {

            Hashtable actual = Serializer.BinaryDeserialize<Hashtable>(null);

            Assert.IsNull(actual);
        }

        [TestMethod]
        public void BinarySerializeDateTimeToByteArray() {

            DateTime source = DateTime.Now;

            byte[] actual = Serializer.BinarySerialize(source);

            Assert.IsNotNull(actual);

            // Objetos DateTime sempre tem 78 bytes.
            Assert.AreEqual(78, actual.Length);

            // Converte novamente o array de bytes para um DateTime.
            DateTime result = Serializer.BinaryDeserialize<DateTime>(actual);

            Assert.AreEqual(source, result);
        }

        [TestMethod]
        public void BinarySerializeNullObject() {

            byte[] actual = Serializer.BinarySerialize(null);

            Assert.IsNull(actual);
        }

        [TestMethod]
        public void BinarySerializeSimpleType() {

            short source = 12;

            byte[] actual = Serializer.BinarySerialize(source);

            Assert.IsNotNull(actual);

            short result = Serializer.BinaryDeserialize<short>(actual);

            Assert.AreEqual(source, result);
        }

        [TestMethod]
        public void JsonSerializeObject() {

            DateTime creationDate = DateTime.Now;

            SerializableObject serializableObject = new SerializableObject();

            serializableObject.ObjectName = "Objeto para serialização";
            serializableObject.ObjectValue = 1;
            serializableObject.ObjectCreationDate = creationDate.ToString("yyyy-MM-ddTHH:mm:ss.fffffffzzz");

            // Desserializa o objeto.
            string serializedString = Serializer.JsonSerialize(serializableObject);

            Assert.IsNotNull(serializedString);
        }

        [TestMethod]
        public void JsonDeserialize() {

            DateTime creationDate = DateTime.Now;

            SerializableObject serializableObject = new SerializableObject();

            serializableObject.ObjectName = "Objeto para serialização";
            serializableObject.ObjectValue = 1;
            serializableObject.ObjectCreationDate = creationDate.ToString("yyyy-MM-ddTHH:mm:ss.fffffffzzz");

            // Desserializa o objeto.
            string serializedString = Serializer.JsonSerialize(serializableObject);

            SerializableObject newObject = Serializer.JsonDeserialize<SerializableObject>(serializedString);

            Assert.IsNotNull(newObject);
            Assert.AreEqual(serializableObject.ObjectName, newObject.ObjectName);
            Assert.AreEqual(serializableObject.ObjectValue, newObject.ObjectValue);
            Assert.AreEqual(serializableObject.ObjectCreationDate, newObject.ObjectCreationDate);
        }

        [TestMethod]
        public void NewtonsoftSerialize() {

            DateTime creationDate = DateTime.Now;

            SerializableObject serializableObject = new SerializableObject();

            serializableObject.ObjectName = "Objeto para serialização";
            serializableObject.ObjectValue = 1;
            serializableObject.ObjectCreationDate = creationDate.ToString("yyyy-MM-ddTHH:mm:ss.fffffffzzz");

            // Desserializa o objeto.
            string serializedString = Serializer.NewtonsoftSerialize(serializableObject);

            Assert.IsNotNull(serializedString);
            Assert.AreEqual("{\"ObjectName\":\"Objeto para serialização\",\"ObjectValue\":1,\"ObjectCreationDate\":\"" + creationDate.ToString("yyyy-MM-ddTHH:mm:ss.fffffffzzz") + "\"}", serializedString);
        }

        [TestMethod]
        public void NewtonsoftSerializeIgnoreNull() {

            DateTime creationDate = DateTime.Now;

            SerializableObject serializableObject = new SerializableObject();

            serializableObject.ObjectName = null;
            serializableObject.ObjectValue = 1;
            serializableObject.ObjectCreationDate = creationDate.ToString("yyyy-MM-ddTHH:mm:ss.fffffffzzz");

            // Desserializa o objeto.
            string serializedString = Serializer.NewtonsoftSerialize(serializableObject);

            Assert.IsNotNull(serializedString);
            Assert.AreEqual("{\"ObjectValue\":1,\"ObjectCreationDate\":\"" + creationDate.ToString("yyyy-MM-ddTHH:mm:ss.fffffffzzz") + "\"}", serializedString);
        }

        [TestMethod]
        public void JavaScriptDeserialize() {

            DateTime creationDate = DateTime.Now;

            SerializableObject serializableObject = new SerializableObject();

            serializableObject.ObjectName = "Objeto para serialização";
            serializableObject.ObjectValue = 1;
            serializableObject.ObjectCreationDate = creationDate.ToString("yyyy-MM-ddTHH:mm:ss.fffffffzzz");

            // Desserializa o objeto.
            string serializedString = Serializer.NewtonsoftSerialize(serializableObject);

            CamelCaseSerializableObject newObject = Serializer.NewtonsoftDeserialize<CamelCaseSerializableObject>(serializedString);

            Assert.IsNotNull(newObject);
            Assert.AreEqual(serializableObject.ObjectName, newObject.objectName);
            Assert.AreEqual(serializableObject.ObjectValue, newObject.objectValue);
            Assert.AreEqual(serializableObject.ObjectCreationDate, newObject.objectCreationDate);
        }

        [TestMethod]
        public void JavaScriptDynamicDeserialize() {

            #region Serialização
            DateTime creationDate = DateTime.Now;

            SerializableObject serializableObject = new SerializableObject();

            serializableObject.ObjectName = "Objeto para serialização";
            serializableObject.ObjectValue = 1;
            serializableObject.ObjectCreationDate = creationDate.ToString("yyyy-MM-ddTHH:mm:ss.fffffffzzz");

            // Desserializa o objeto.
            string serializedString = Serializer.NewtonsoftSerialize(serializableObject);
            #endregion

            dynamic newObject = Serializer.DynamicDeserialize(serializedString);

            string name = newObject.ObjectName;
            int value = newObject.ObjectValue;
            string date = newObject.ObjectCreationDate;

            Assert.IsNotNull(newObject);
            Assert.AreEqual(serializableObject.ObjectName, name);
            Assert.AreEqual(serializableObject.ObjectValue, value);
            //Assert.AreEqual(serializableObject.ObjectCreationDate, date);
        }

        [TestMethod]
        public void JavaScriptSerialize() {

            DateTime creationDate = DateTime.Now;

            SerializableObject serializableObject = new SerializableObject();

            serializableObject.ObjectName = "Objeto para serialização";
            serializableObject.ObjectValue = 1;
            serializableObject.ObjectCreationDate = creationDate.ToString("yyyy-MM-ddTHH:mm:ss.fffffffzzz");

            // Desserializa o objeto.
            string serializedString = Serializer.JavaScriptSerialize(serializableObject);

            Assert.IsNotNull(serializedString);
            Assert.AreEqual("{\"ObjectName\":\"Objeto para serialização\",\"ObjectValue\":1,\"ObjectCreationDate\":\"" + creationDate.ToString("yyyy-MM-ddTHH:mm:ss.fffffffzzz") + "\"}", serializedString);
        }

        [TestMethod]
        public void JavaScriptSerializeIgnoreNull() {

            DateTime creationDate = DateTime.Now;

            SerializableObject serializableObject = new SerializableObject();

            serializableObject.ObjectName = null;
            serializableObject.ObjectValue = 1;
            serializableObject.ObjectCreationDate = creationDate.ToString("yyyy-MM-ddTHH:mm:ss.fffffffzzz");

            // Desserializa o objeto.
            string serializedString = Serializer.JavaScriptSerialize(serializableObject);

            Assert.IsNotNull(serializedString);
            Assert.AreEqual("{\"ObjectValue\":1,\"ObjectCreationDate\":\"" + creationDate.ToString("yyyy-MM-ddTHH:mm:ss.fffffffzzz") + "\"}", serializedString);
        }

        [TestMethod]
        public void JavaScriptSerializeIncludeNull() {

            DateTime creationDate = DateTime.Now;

            SerializableObject serializableObject = new SerializableObject();

            serializableObject.ObjectName = null;
            serializableObject.ObjectValue = 1;
            serializableObject.ObjectCreationDate = creationDate.ToString("yyyy-MM-ddTHH:mm:ss.fffffffzzz");

            // Desserializa o objeto.
            string serializedString = Serializer.JavaScriptSerialize(serializableObject, false);

            Assert.IsNotNull(serializedString);
            Assert.AreEqual("{\"ObjectName\":null,\"ObjectValue\":1,\"ObjectCreationDate\":\"" + creationDate.ToString("yyyy-MM-ddTHH:mm:ss.fffffffzzz") + "\"}", serializedString);
        }

        [TestMethod]
        public void NewtonsoftDeserialize() {

            DateTime creationDate = DateTime.Now;

            SerializableObject serializableObject = new SerializableObject();

            serializableObject.ObjectName = "Objeto para serialização";
            serializableObject.ObjectValue = 1;
            serializableObject.ObjectCreationDate = creationDate.ToString("yyyy-MM-ddTHH:mm:ss.fffffffzzz");

            // Desserializa o objeto.
            string serializedString = Serializer.JavaScriptSerialize(serializableObject);

            CamelCaseSerializableObject newObject = Serializer.JavaScriptDeserialize<CamelCaseSerializableObject>(serializedString);

            Assert.IsNotNull(newObject);
            Assert.AreEqual(serializableObject.ObjectName, newObject.objectName);
            Assert.AreEqual(serializableObject.ObjectValue, newObject.objectValue);
            Assert.AreEqual(serializableObject.ObjectCreationDate, newObject.objectCreationDate);
        }

        [TestMethod]
        public void JavascriptDictionarySerialization() {

            Dictionary<string, string> userDictionary = new Dictionary<string, string>();

            userDictionary.Add("Nome", "Jonssen");
            userDictionary.Add("Sexo", "Masculino");
            userDictionary.Add("Idade", "33");

            string serializedUserData = Serializer.JsonSerialize(userDictionary);

            Assert.IsNotNull(serializedUserData);
        }

        [TestMethod]
        public void NewtonsoftDictionarySerialization() {

            Dictionary<string, string> userDictionary = new Dictionary<string, string>();

            userDictionary.Add("Nome", "Jonssen");
            userDictionary.Add("Sexo", "Masculino");
            userDictionary.Add("Idade", "33");

            string serializedUserData = Serializer.NewtonsoftSerialize(userDictionary);

            Assert.IsNotNull(serializedUserData);
        }

        [TestMethod]
        public void XmlSerializeObject() {

            DateTime creationDate = DateTime.Now;

            SerializableObject serializableObject = new SerializableObject();

            serializableObject.ObjectName = "Objeto para serialização";
            serializableObject.ObjectValue = 1;
            serializableObject.ObjectCreationDate = creationDate.ToString("yyyy-MM-ddTHH:mm:ss.fffffffzzz");

            // Desserializa o objeto.
            string serializedString = Serializer.XmlSerialize(serializableObject, true);

            // Falha caso a string seja nula.
            Assert.IsNotNull(serializedString);

            // Obtém o byte de especificação de encoding (default UTF-8).
            string byteOrder = Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble());

            // Remove o caracter de especificação de encoding.
            if (serializedString.StartsWith(byteOrder) == true) { serializedString = serializedString.Remove(0, byteOrder.Length); }

            // Separa todas as linhas do xml.
            string[] lines = serializedString.Split(new string[] { "\r\n" }, StringSplitOptions.None);

            // Verifica se foram retornadas 6 linhas.
            Assert.AreEqual(6, lines.Length);

            // Verifica se o header é válido.
            Assert.AreEqual("<?xml version=\"1.0\" encoding=\"utf-8\"?>", lines[0]);

            // Verifica se o namespace é válido.
            Assert.AreEqual("<SerializableObject xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">", lines[1]);

            // Verifica se o nome do objeto é válido.
            Assert.AreEqual("\t<ObjectName>Objeto para serialização</ObjectName>", lines[2]);

            // Verifica se o valor do objeto é válido.
            Assert.AreEqual("\t<ObjectValue>1</ObjectValue>", lines[3]);

            // Verifica se a data de criação do objeto é válida.
            Assert.AreEqual(string.Format("\t<ObjectCreationDate>{0}</ObjectCreationDate>", creationDate.ToString("yyyy-MM-ddTHH:mm:ss.fffffffzzz")), lines[4]);

            // Verifica se o fechamento do objeto é válido.
            Assert.AreEqual("</SerializableObject>", lines[5]);
        }

        [TestMethod]
        public void XmlSerializeObjectWithCustomEncoding() {

            Encoding encoding = Encoding.GetEncoding("ISO-8859-1");

            DateTime creationDate = DateTime.Now;

            SerializableObject serializableObject = new SerializableObject();

            serializableObject.ObjectName = "Objeto para serialização";
            serializableObject.ObjectValue = 2;
            serializableObject.ObjectCreationDate = creationDate.ToString("yyyy-MM-ddTHH:mm:ss.fffffffzzz");

            // Serializa o objeto.
            string serializedString = Serializer.XmlSerialize(serializableObject, true, encoding);

            // Falha caso o objeto seja nulo.
            Assert.IsNotNull(serializedString);

            // Obtém o byte de especificação de encoding.
            string byteOrder = encoding.GetString(encoding.GetPreamble());

            // Remove o caracter de especificação de encoding.
            if (serializedString.StartsWith(byteOrder) == true) { serializedString = serializedString.Remove(0, byteOrder.Length); }

            // Separa todas as linhas do xml.
            string[] lines = serializedString.Split(new string[] { "\r\n" }, StringSplitOptions.None);

            // Verifica se foram retornadas 6 linhas.
            Assert.AreEqual(6, lines.Length);

            // Verifica se o header é válido.
            Assert.AreEqual(string.Format("<?xml version=\"1.0\" encoding=\"{0}\"?>", encoding.BodyName), lines[0]);

            // Verifica se o namespace é válido.
            Assert.AreEqual("<SerializableObject xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">", lines[1]);

            // Verifica se o nome do objeto é válido.
            Assert.AreEqual("\t<ObjectName>Objeto para serialização</ObjectName>", lines[2]);

            // Verifica se o valor do objeto é válido.
            Assert.AreEqual("\t<ObjectValue>2</ObjectValue>", lines[3]);

            // Verifica se a data de criação do objeto é válida.
            Assert.AreEqual(string.Format("\t<ObjectCreationDate>{0}</ObjectCreationDate>", creationDate.ToString("yyyy-MM-ddTHH:mm:ss.fffffffzzz")), lines[4]);

            // Verifica se o fechamento do objeto é válido.
            Assert.AreEqual("</SerializableObject>", lines[5]);
        }

        [TestMethod]
        public void SerializeNullObject() {

            string serializedString = Serializer.XmlSerialize(null);

            Assert.IsNull(serializedString);
        }

        [TestMethod]
        public void DeserializeXmlString() {

            string source = "﻿<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<SerializableObject xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"><ObjectName>Objeto para serialização</ObjectName><ObjectValue>1</ObjectValue><ObjectCreationDate>2014-07-24T14:08:36.2336326-03:00</ObjectCreationDate></SerializableObject>";

            SerializableObject actual = Serializer.XmlDeserialize<SerializableObject>(source);

            Assert.IsNotNull(actual);

            Assert.AreEqual(actual.ObjectName, "Objeto para serialização");
        }

        [TestMethod]
        public void DeserializeCustomEncodingXmlString() {

            string source = "﻿<?xml version=\"1.0\" encoding=\"iso-8859-1\"?>\r\n<SerializableObject xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"><ObjectName>Objeto para serialização</ObjectName><ObjectValue>1</ObjectValue><ObjectCreationDate>2014-07-24T14:08:36.2336326-03:00</ObjectCreationDate></SerializableObject>";

            SerializableObject actual = Serializer.XmlDeserialize<SerializableObject>(source);

            Assert.IsNotNull(actual);

            Assert.AreEqual(actual.ObjectName, "Objeto para serializaÃ§Ã£o");
        }

        [TestMethod]
        public void DeserializeNullXmlString() {

            SerializableObject actual = Serializer.XmlDeserialize<SerializableObject>(null);

            Assert.IsNull(actual);
        }
    }
}
