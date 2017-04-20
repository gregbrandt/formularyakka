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
                if (Directory.Exists(beginDirectoryCrawl.Directory))
                {

                    SendMessage("Directory crawl");
                    ProcessDirectory(beginDirectoryCrawl.Directory);

                }
                else
                {
                    SendMessage("Invalid directory " + beginDirectoryCrawl.Directory);
                }
                
                Context.Self.Tell(PoisonPill.Instance);

            });
        }

        private static void ProcessDirectory(string directory)
        {
            foreach (var file in Directory.GetFiles(directory))
            {
                ProcessFile(file);
            }
            foreach (var childDirectory in Directory.GetDirectories(directory))
            {
                ProcessDirectory(childDirectory);
            }
        }

        private static void ProcessFile(string file)
        {
            Context.ActorOf(Props.Create(() => new FileProcessorActor())).Tell(new FileProcessorActor.ProcessFile(file));
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
