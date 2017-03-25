using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            Receive<LoadCopay>(processFile =>
            {
                //_copayLoadActor.Tell(new CopayLoadActor.(""));
                SendMessage("Copay Load");

            });
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
}
