using Akka.Actor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportFormulary.Actors
{
    public class DirectoryValidatorActor : ReceiveActor
    {
        #region Message types
        
        public class ValidateDirectory
        {
            public ValidateDirectory(string directory)
            {
                Directory = directory;
            }

            public string Directory { get; private set; }
        }

        #endregion
        private readonly string _directory;
        private readonly string _consoleWriterActorPath;
        private IActorRef directoryCrawler;

        public DirectoryValidatorActor( string directory) 
            : this(directory,ActorNames.ConsoleWriterActor.Path)
        {
        }

        public DirectoryValidatorActor(string directory, string consoleWriterActorPath)
        {
            _consoleWriterActorPath = consoleWriterActorPath;
            _directory = directory;
            Initialize();
        }

        public void Initialize()
        {
            Receive<ValidateDirectory>(valDir =>
            {

                directoryCrawler =
                    Context.ActorOf(
                        Props.Create(
                            () => new DirectoryCrawlerActor()));

                directoryCrawler.Tell(new DirectoryCrawlerActor.BeginDirectoryCrawl(valDir.Directory));
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
