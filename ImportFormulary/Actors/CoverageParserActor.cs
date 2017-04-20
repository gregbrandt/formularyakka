using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportFormulary.Actors
{
    public class CoverageParserActor: ReceiveActor
    {
        #region Message types

        public class ParseFile
        {
            public ParseFile(string fileName)
            {
                FileName = fileName;
            }

            public string FileName { get; private set; }
        }

        #endregion

        
        private readonly string _consoleWriterActorPath;

        public CoverageParserActor() 
            : this( ActorNames.ConsoleWriterActor.Path)
        {
        }

        public CoverageParserActor( string consoleWriterActorPath)
        {
            _consoleWriterActorPath = consoleWriterActorPath;

            //Set our Receive functions
            Initialize();
        }

        public void Initialize()
        {
            //time to kick off the feed parsing process, and send the results to ourselves
            Receive<ParseFile>(processFile =>
            {
                Context.ActorOf(Props.Create(() => new CoverageLoadActor())).Tell(new CoverageLoadActor.LoadCoverage(""));
                SendMessage("Coverage Parser" + processFile.FileName);

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
