﻿using Akka.Actor;
using ImportFormulary.Actors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportFormulary
{
    class Program
    {
        public static ActorSystem MyActorSystem;

        static void Main(string[] args)
        {
            MyActorSystem = ActorSystem.Create("MyFirstActorSystem");

            //Create the actors who are going to validate RSS / ATOM feeds and start the parsing process
            IActorRef feedValidator =
                MyActorSystem.ActorOf(Props.Create(() => new DirectoryCrawlerActor(ActorNames.ConsoleWriterActor.Path)),
                    ActorNames.DirectoryCrawlerActor.Name);



            //Create the actors who are going to read from and write to the console
            IActorRef consoleWriter = MyActorSystem.ActorOf(Props.Create<ConsoleWriterActor>(), ActorNames.ConsoleWriterActor.Name);
            IActorRef consoleReader = MyActorSystem.ActorOf(Props.Create<ConsoleReaderActor>(), ActorNames.ConsoleReaderActor.Name);

            Instructions.PrintWelcome();

            //Tell the console reader that we're ready to begin
            consoleReader.Tell(new ConsoleReaderActor.ReadFromConsoleClean());

            // This blocks the current thread from exiting until MyActorSystem is shut down
            // The ConsoleReaderActor will shut down the ActorSystem once it receives an 
            // "exit" command from the user
            MyActorSystem.WhenTerminated.Wait();
            
            
        }
    }
}
