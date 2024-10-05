using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Post.API.Models
{
    public class Image
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } // MongoDB will auto-generate this if not set
        public string Name { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
