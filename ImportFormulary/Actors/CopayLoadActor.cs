using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using ImportFormulary.Actors.Models;

namespace ImportFormulary.Actors
{
    public class CopayLoadActor: ReceiveActor
    {
        #region Message types

        public class LoadCopay
        {
            public LoadCopay(string data)
            {
                Data = data;
            }

            public string Data { get; private set; }
        }

        #endregion

        
        private readonly string _consoleWriterActorPath;

        public CopayLoadActor() 
            : this( ActorNames.ConsoleWriterActor.Path)
        {
        }

        public CopayLoadActor( string consoleWriterActorPath)
        {
            _consoleWriterActorPath = consoleWriterActorPath;

            //Set our Receive functions
            Initialize();
        }

        public void Initialize()
        {
            //time to kick off the feed parsing process, and send the results to ourselves
            Receive<LoadCopay>(processData =>
            {
                //Insert(processData).Wait();
                var model = new CopayDetailModel(processData.Data.Split('|'));
                IMongoClient client;
                IMongoDatabase database;

                client = new MongoClient();
                database = client.GetDatabase("copay");

                var collection = database.GetCollection<CopayDetailModel>("copay");

                collection.InsertOneAsync(model).PipeTo(Self,
                    Self,
                    ()=> { Console.WriteLine("Console WriteLine Copay Load data success"); return null; },
                    ex => {
                        Console.WriteLine("Console WriteLine Copay Load data failure" + ex.Message); return null;
            });
                SendMessage("Copay Load data " + processData.Data);

            });
        }

        private async Task Insert(LoadCopay processData)
        {
            var model = new CopayDetailModel(processData.Data.Split('|'));
            IMongoClient client;
            IMongoDatabase database;

            client = new MongoClient();
            database = client.GetDatabase("copay");

            var collection = database.GetCollection<CopayDetailModel>("copay");

            await collection.InsertOneAsync(model);
        }


        #region Messaging methods

        private void SendMessage(string message, PipeToSampleStatusCode pipeToSampleStatus = PipeToSampleStatusCode.Normal)
        {
            //create the message instance
            var consoleMsg = StatusMessageHelper.CreateMessage(message, pipeToSampleStatus);

            //Select the ConsoleWriterActor and send it a message
            Context.ActorSelection(_consoleWriterActorPath).Tell(consoleMsg);
        }

        #endregion
        

    }


    public class CopayDetailModel : BaseModel
    {

        public CopayDetailModel( string[] lineValues)
        {
            this.CopayId = lineValues[2];
            if (lineValues.Length > 9)
                this.PharmacyType = GetEmptyChar(lineValues[9], 0);
            if (lineValues.Length > 10)
                this.FlatCopayAmount = GetEmptyDouble(lineValues[10]);
            if (lineValues.Length > 11)
                this.PercentCopayRate = GetEmptyDouble(lineValues[11]);
            if (lineValues.Length > 12)
                this.FirstCopayTerm = GetEmptyChar(lineValues[10], 0);
            if (lineValues.Length > 13)
                this.MinimumCopay = GetEmptyDouble(lineValues[13]);
            if (lineValues.Length > 14)
                this.MaximumCopay = GetEmptyDouble(lineValues[14]);
            if (lineValues.Length > 15)
                this.DaysSupplyPerCopay = GetEmptyInt(lineValues[15]);
            if (lineValues.Length > 16)
                this.CopayTier = GetEmptyInt(lineValues[16]);
            if (lineValues.Length > 17)
                this.MaximumCopayTier = GetEmptyInt(lineValues[17]);


            Key = new ObjectId();
        }




        public string CopayId { get; set; }
        public string PharmacyType { get; set; }
        public double? FlatCopayAmount { get; set; }
        public double? PercentCopayRate { get; set; }
        public string FirstCopayTerm { get; set; }
        public double? MinimumCopay { get; set; }
        public double? MaximumCopay { get; set; }
        public int? DaysSupplyPerCopay { get; set; }
        public int? CopayTier { get; set; }
        public int? MaximumCopayTier { get; set; }

    }
}
