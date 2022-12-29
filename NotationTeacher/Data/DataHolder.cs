using System.Xml.Serialization;

namespace NotationTeacher
{
    // Used to store all data that needs to be saved and serialized.
    public class DataHolder
    {
        public NoteLearningData NoteLearningData { get; set; } = new();

        // This must not be serialized because it refers to when this instance was created.
        [XmlIgnore]
        public DateTime TimeLoaded { get;} = DateTime.Now;

        public DateTime TimeCreated { get; set; } = DateTime.Now;

        public DateTime? TimeSaved {get; set; } = null;

        public string ToXml()
        {
            var serializer = new XmlSerializer(typeof(DataHolder));
            using var writer = new StringWriter();
            serializer.Serialize(writer, this);
            return writer.ToString();
        }

        public static DataHolder FromXml(string xml)
        {
            var serializer = new XmlSerializer(typeof(DataHolder));
            using (var reader = new StringReader(xml))
            {
                if (serializer.Deserialize(reader) is DataHolder holder)
                {
                    return holder;
                }
            }

            Console.WriteLine("Valid Xml was not given.");
            throw new ArgumentException("Valid Xml was not given.");
        }
    }
}
