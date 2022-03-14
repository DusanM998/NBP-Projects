using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Driver.Builders;
using MongoDB.Driver.Linq;


namespace MongoDB_Repository
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var connectionString = "mongodb://localhost/?safe=true";
            var server = MongoServer.Create(connectionString);
            var database = server.GetDatabase("preduzece");
            
            var collection = database.GetCollection<Radnik>("radnici");

            MongoCursor<Radnik> radnici = collection.FindAll();

            foreach (Radnik r in radnici.ToArray<Radnik>())
            {
                MessageBox.Show(r.ime);
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            var connectionString = "mongodb://localhost/?safe=true";
            var server = MongoServer.Create(connectionString);
            var db = server.GetDatabase("preduzece");

            //db.CreateCollection("preduzece");

            var collection = db.GetCollection<Radnik>("radnici");

            Radnik radnik1 = new Radnik { ime = "Petar", prezime = "Petrović", Adresa = "Oblačića Rada 12", Plata = 25000, oznake = new List<string> { "muškarac", "ekonomista", "pripravnik" } };
            Radnik radnik2 = new Radnik { ime = "Sava", prezime = "Janković", Adresa = "Voždova 23", Plata = 40000, oznake = new List<string> { "muškarac", "pravnik", "rukovodilac" } };
            Radnik radnik3 = new Radnik { ime = "Milutin", prezime = "Simić", Adresa = "Dušanova 112", Plata = 33000 };
            Radnik radnik4 = new Radnik { ime = "Jelena", prezime = "Antić", Adresa = "Bulevar Nemanjića 134", Plata = 40000, oznake = new List<string> { "žena", "pravnik" } };
            Radnik radnik5 = new Radnik { ime = "Mirjana", prezime = "Vukić", Adresa = "Bulevar Nemanjića 23", Plata = 29000, oznake = new List<string> { "žena" } };

            collection.Insert(radnik1);
            collection.Insert(radnik2);
            collection.Insert(radnik3);
            collection.Insert(radnik4);
            collection.Insert(radnik5);

            foreach(Radnik radnik in collection.FindAll())
            {
                MessageBox.Show(radnik.ime);
            }
            
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //db.preduzece.update({ime:"Milutin"}, {$set:{oznake:["test"]}});

            var connectionString = "mongodb://localhost/?safe=true";
            var server = MongoServer.Create(connectionString);
            var db = server.GetDatabase("preduzece");

            var collection = db.GetCollection<Radnik>("radnici");

            var query = Query.EQ("ime", "Milutin");
            var update = MongoDB.Driver.Builders.Update.Set("oznake", BsonValue.Create(new List<string>{"test"}));

            collection.Update(query, update);

            foreach (Radnik r in collection.Find(Query.EQ("ime", BsonValue.Create("Milutin"))))
            {
                foreach (string oznaka in r.oznake)
                {
                    MessageBox.Show(r.ime + ":" + oznaka);
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            var connectionString = "mongodb://localhost/?safe=true";
            var server = MongoServer.Create(connectionString);
            var db = server.GetDatabase("preduzece");

            var collection = db.GetCollection<Radnik>("radnici");

            Radnik testRadnik = new Radnik { ime = "Test", prezime = "Test", Adresa = "Test adresa", Plata = 1000};

            collection.Insert(testRadnik);

            foreach (Radnik radnik in collection.FindAll())
            {
                MessageBox.Show(radnik.ime);
            }

            var query = Query.EQ("ime", "Test");

            collection.Remove(query);

            foreach (Radnik radnik in collection.FindAll())
            {
                MessageBox.Show(radnik.ime);
            }

        }

        private void button5_Click(object sender, EventArgs e)
        {

            var connectionString = "mongodb://localhost/?safe=true";
            var server = MongoServer.Create(connectionString);
            var db = server.GetDatabase("preduzece");

            var collection = db.GetCollection<Radnik>("radnici");

            MessageBox.Show("Broj radnika: " + collection.Count());

            MessageBox.Show("Prva dva radnika");

            foreach (Radnik r in collection.FindAll().SetLimit(2))
            {
                MessageBox.Show(r.ime + " " + r.prezime);
            }


            MessageBox.Show("3. i 4. radnik u kolekciji");

            foreach (Radnik r in collection.FindAll().SetSkip(2).SetLimit(2))
            {
                MessageBox.Show(r.ime + " " + r.prezime);
            }

            MessageBox.Show("Radnici sortirani po prezimenu");

            foreach (Radnik r in collection.FindAll().SetSortOrder(new string[] { "prezime" })) //ILI: //collection.FindAll().SetSortOrder(SortBy.Descending("prezime"));
            {
                MessageBox.Show(r.ime + " " + r.prezime);
            }

            MessageBox.Show("Radnici koji se ne zovu Jelena i imaju platu manju od 40000");

            var query = Query.And(
                            Query.Not(Query.EQ("ime", "Jelena")), 
                            Query.LT("Plata", 40000)
                            );

            foreach (Radnik r in collection.Find(query))
            {
                MessageBox.Show(r.ime + " " + r.prezime);
            }

           
        }

        private void button6_Click(object sender, EventArgs e)
        {
            var connectionString = "mongodb://localhost/?safe=true";
            var server = MongoServer.Create(connectionString);
            var db = server.GetDatabase("preduzece");

            var radniciCollection = db.GetCollection<Radnik>("radnici");

            var sektoriCollection = db.GetCollection<Sektor>("sektori");

            Sektor sek = new Sektor{Naziv="sektor 1"};
            sektoriCollection.Insert(sek);

            //svi radnici osim prva 2 rade u sektoru 1
            foreach (Radnik r in radniciCollection.FindAll().SetSkip(2))
            {
                sek.Radnici.Add(new MongoDBRef("radnici", r.Id));
                
                r.Sektor = new MongoDBRef("sektori", sek.Id);

                radniciCollection.Save(r);
            }

            sektoriCollection.Save(sek);

            foreach (Sektor s in sektoriCollection.FindAll())
            {
                foreach (MongoDBRef radnikRef in s.Radnici.ToList())
                {
                    Radnik radnik = db.FetchDBRefAs<Radnik>(radnikRef);
                    MessageBox.Show(radnik.ime + " " + radnik.prezime + " sektor: " + s.Naziv);
                }
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            var connectionString = "mongodb://localhost/?safe=true";
            var server = MongoServer.Create(connectionString);
            var db = server.GetDatabase("preduzece");

            var sektoriCollection = db.GetCollection<Sektor>("sektori");
            sektoriCollection.RemoveAll();

            //MessageBox.Show("");
            var radnici = db.GetCollection<Radnik>("radnici");
            radnici.RemoveAll();

        }

        private void button8_Click(object sender, EventArgs e)
        {
            var connectionString = "mongodb://localhost/?safe=true";
            var server = MongoServer.Create(connectionString);
            var db = server.GetDatabase("preduzece");

            var radniciCollection = db.GetCollection("radnici");

            var sektoriCollection = db.GetCollection("sektori");

            
            //simple
            var query1 = from radnik in radniciCollection.AsQueryable<Radnik>()
                        where radnik.ime == "Petar"
                        select radnik;
            
            foreach (Radnik r in query1)
            {
                MessageBox.Show(r.ime + " " + r.prezime);
            }

            //simple count
            var result1 = (from radnik in radniciCollection.AsQueryable<Radnik>()
                         select radnik).Count();

            MessageBox.Show("Broj radnika: " + result1.ToString());

            //simple
            var result2 = (from radnik in radniciCollection.AsQueryable<Radnik>()
                           where radnik.Plata > 20000
                           orderby radnik.prezime descending 
                           select radnik).FirstOrDefault();
            MessageBox.Show("Prvi radnik po opadajucem redosledu po prezimenu koji ima platu vecu od 20000: " + result2.ime + " " + result2.prezime);

            
        }
    }
}
