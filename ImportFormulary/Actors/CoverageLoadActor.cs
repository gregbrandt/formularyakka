using Akka.Actor;

namespace ImportFormulary.Actors
{
    public class CoverageLoadActor: ReceiveActor
    {

        #region Message types

        public class LoadCoverage
        {
            public LoadCoverage(string data)
            {
                Data = data;
            }

            public string Data { get; private set; }
        }

        #endregion


        private readonly string _consoleWriterActorPath;

        public CoverageLoadActor() 
            : this( ActorNames.ConsoleWriterActor.Path)
        {
        }

        public CoverageLoadActor(string consoleWriterActorPath)
        {
            _consoleWriterActorPath = consoleWriterActorPath;

            //Set our Receive functions
            Initialize();
        }

        public void Initialize()
        {
            //time to kick off the feed parsing process, and send the results to ourselves
            Receive<LoadCoverage>(processFile =>
            {
                //_copayLoadActor.Tell(new CopayLoadActor.(""));

                SendMessage("Coverage Load ");
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
