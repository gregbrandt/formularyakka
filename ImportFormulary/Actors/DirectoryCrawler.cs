using Akka.Actor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportFormulary.Actors
{
    public class DirectoryCrawlerActor : ReceiveActor
    {
        #region Message types
        
        public class BeginDirectoryCrawl
        {
            public BeginDirectoryCrawl(string directory)
            {
                Directory = directory;
            }

            public string Directory { get; private set; }
        }

        #endregion
        
        private readonly string _consoleWriterActorPath;

        public DirectoryCrawlerActor( ) 
            : this(ActorNames.ConsoleWriterActor.Path)
        {
        }

        public DirectoryCrawlerActor(  string consoleWriterActorPath)
        {
            _consoleWriterActorPath = consoleWriterActorPath;

            //Set our Receive functions
            Initialize();
        }

        public void Initialize()
        {

            //time to kick off the feed parsing process, and send the results to ourselves
            Receive<BeginDirectoryCrawl>(beginDirectoryCrawl =>
            {
                //if (Directory.Exists(beginDirectoryCrawl.Directory))
                //{

                //}
                //else
                //{
                //    SendMessage(string.Format("Downloading {0} for RSS/ATOM processing...", feed.FeedUri));
                //    _feedFactory.CreateFeedAsync(feed.FeedUri).PipeTo(Self);
                //}
                
                Context.ActorOf(Props.Create(() => new FileProcessorActor())).Tell(new FileProcessorActor.ProcessFile(beginDirectoryCrawl.Directory));

                SendMessage("Directory crawl");

                Context.Self.Tell(PoisonPill.Instance);

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
