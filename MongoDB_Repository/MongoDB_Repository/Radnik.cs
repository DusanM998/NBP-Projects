using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MongoDB.Bson;
using MongoDB.Driver;

namespace MongoDB_Repository
{
    public class Radnik
    {
        public ObjectId Id { get; set; }
        public string ime { get; set; }
        public string prezime { get; set; }
        public string Adresa { get; set; }
        public float Plata { get; set; }
        public List<string> oznake { get; set; }
        public MongoDBRef Sektor { get; set; }

    }
}
