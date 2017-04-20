using Akka.Actor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportFormulary.Actors
{
    public class CopayParserActor : ReceiveActor
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

        public CopayParserActor() 
            : this(ActorNames.ConsoleWriterActor.Path)
        {
        }

        public CopayParserActor( string consoleWriterActorPath)
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

                var sendMsgActor = Context.ActorSelection(_consoleWriterActorPath);
                SendMessage(sendMsgActor, "Copay parser" + processFile.FileName);
                var copayLoadActor = Context.ActorOf(Props.Create(() => new CopayLoadActor()));
                using (var filestream = File.Open(processFile.FileName, FileMode.Open))
                using (var reader = new StreamReader(filestream))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();

                        copayLoadActor.Tell(new CopayLoadActor.LoadCopay(line));
                    }
                }

            //var source = FileStreamProcessor.GetFileSource(processFile.FileName)
            //                                .Subscribe(x => {
            //                                    copayLoadActor.Tell(new CopayLoadActor.LoadCopay(x));
            //                                });



            });
        }


        #region Messaging methods

        private void SendMessage(ActorSelection sendMsgActor, string message, PipeToSampleStatusCode pipeToSampleStatus = PipeToSampleStatusCode.Normal)
        {
            //create the message instance
            var consoleMsg = StatusMessageHelper.CreateMessage(message, pipeToSampleStatus);

            //Select the ConsoleWriterActor and send it a message
            sendMsgActor.Tell(consoleMsg);
        }

        #endregion


    }
}
