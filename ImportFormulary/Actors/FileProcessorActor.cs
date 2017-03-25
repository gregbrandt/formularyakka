using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportFormulary.Actors
{
    public class FileProcessorActor : ReceiveActor
    {
        #region Message types

        /// <summary>
        /// Message sent by <see cref="FeedParserCoordinator"/> that begins the parsing process
        /// </summary>
        public class ProcessFile
        {
            public ProcessFile(string fileName)
            {
                FileName = fileName;
            }

            public string FileName { get; private set; }
        }

        #endregion
        
        private readonly string _consoleWriterActorPath;

        public FileProcessorActor() 
            : this(ActorNames.ConsoleWriterActor.Path)
        {
        }

        public FileProcessorActor( string consoleWriterActorPath)
        {
            _consoleWriterActorPath = consoleWriterActorPath;

            //Set our Receive functions
            Initialize();
        }

        public void Initialize()
        {
            //time to kick off the feed parsing process, and send the results to ourselves
            Receive<ProcessFile>(processFile =>
            {
                //_copayParserActor.Tell(new CopayParserActor.ParseFile(""));
                //_coverageParserActor.Tell(new CoverageParserActor.ParseFile(""));

                Context.ActorOf(Props.Create(() => new CopayParserActor())).Tell(new CopayParserActor.ParseFile(""));
                Context.ActorOf(Props.Create(() => new CoverageParserActor())).Tell(new CoverageParserActor.ParseFile(""));
                //Context.Parent.Tell(
                //    new DirectoryCrawler.FileProcessComplete(imagedownload.ImageDownloadCommand.FeedUri, 0, 1));
                SendMessage("File Process");
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
