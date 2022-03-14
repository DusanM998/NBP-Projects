using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MongoDB_Repository
{
    public class Sektor
    {
        public ObjectId Id { get; set; }
        public string Naziv { get; set; }
        public List<MongoDBRef> Radnici { get; set; }

        public Sektor()
        {
            Radnici = new List<MongoDBRef>();
        }
    }
}
