using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
//using Xamarin.Forms;

namespace ApaScoreKeeper
{
        //var team = new Team() { Name = "BlueTeam", Members = new List<Member>() { new Member() { Name = "Tim" }, new Member() { Name = "Bob" } } };
        //var str1 = JsonConvert.SerializeObject(team);
        //var str2 = JsonConvert.SerializeObject(team, new RecursiveJsonConverter());

        //var t1 = JsonConvert.DeserializeObject<Team>(str1);
        //var t2 = JsonConvert.DeserializeObject<Team>(str2, new RecursiveJsonConverter());
 
    public class RecursiveJsonConverter : JsonConverter
    {
        public List<IUniqueIdentifier> SeenDependencies { get; set; } = new List<IUniqueIdentifier>();
        public IUniqueIdentifier OriginalObject { get; set; }
        public IRecursiveJsonReader Reader { get; set; }
        public IRecursiveJsonWriter Writer { get; set; }

        public RecursiveJsonConverter(IRecursiveJsonReader reader, IRecursiveJsonWriter writer)
        {
            Reader = reader;
            Writer = writer;
        }

        public override bool CanConvert(Type objectType)
        {
            return (typeof(IUniqueIdentifier).GetTypeInfo().IsAssignableFrom(objectType.GetTypeInfo()));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            // TODO: rework
            if (reader.TokenType == JsonToken.String)
            {
                //return new Member();
            }


            serializer.Converters.Remove(this);
            serializer.Converters.Add(new RRJsonConverter(this));

            var b = Activator.CreateInstance(objectType);
            serializer.Populate(reader, b);
            return b;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Converters.Remove(this);
            serializer.Converters.Add(new RRJsonConverter(this));

            OriginalObject = (IUniqueIdentifier)value;

            var jo = new JObject();
            foreach (PropertyInfo prop in value.GetType().GetTypeInfo().DeclaredProperties)
            {
                if (prop.CanRead)
                {
                    object propVal = prop.GetValue(value, null);
                    if (propVal != null)
                    {
                        jo.Add(prop.Name, JToken.FromObject(propVal, serializer));
                    }
                }
            }
            jo.WriteTo(writer);
        }
    }

    public class RRJsonConverter : JsonConverter
    {
        public RecursiveJsonConverter ParentConverter { get; set; }

        public RRJsonConverter(RecursiveJsonConverter parentConverter)
        {
            ParentConverter = parentConverter;
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanConvert(Type objectType)
        {
            return (typeof(IUniqueIdentifier).GetTypeInfo().IsAssignableFrom(objectType.GetTypeInfo()));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var id = serializer.Deserialize<string>(reader);
            var obj = ParentConverter.SeenDependencies.FirstOrDefault(d => d.Id == id);
            if (obj == null)
            {
                obj = Task.Run(async () => await ParentConverter.Reader.Read(id, objectType)).Result; // TODO: this is ok...?
                ParentConverter.SeenDependencies.Add(obj);
            }

            return obj;
        }

        public override void WriteJson(JsonWriter writer, object objValue, JsonSerializer serializer)
        {
            // TODO: can we just load the original object into SeenDependencies? Then we don't need OriginalObject param
            var value = (IUniqueIdentifier)objValue;
            if (value != ParentConverter.OriginalObject && !ParentConverter.SeenDependencies.Any(d => d.Id == value.Id))
            {
                ParentConverter.SeenDependencies.Add(value);
                ParentConverter.Writer.Write(value.Id, value); // TODO: this won't work, as Write creates a new Converter...optional param seems messy...
            }

            var t = JToken.FromObject(value.Id);
            t.WriteTo(writer);
        }
    }

    public interface IUniqueIdentifier
    {
        string Id { get; set; }
    }

    public interface IRecursiveJsonWriter
    {
        Task Write(string key, IUniqueIdentifier obj);
    }

    public interface IRecursiveJsonReader
    {
        Task<IUniqueIdentifier> Read(string key, Type type);
    }

    public class UniqueIdentifierWrapper<T> : IUniqueIdentifier
    {
        public UniqueIdentifierWrapper()
        {

        }

        public UniqueIdentifierWrapper(string id, T data)
        {
            Id = id;
            Data = data;
        }

        public T Data { get; set; }
        public string Id { get; set; }        
    }

    static public class UniqueIdentifierWrapper
    {
        public static UniqueIdentifierWrapper<T> Create<T>(string id, T data)
        {
            return new UniqueIdentifierWrapper<T>(id, data);
        }
    }
}
