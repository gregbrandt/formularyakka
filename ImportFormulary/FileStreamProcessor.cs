using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportFormulary
{
    class FileStreamProcessor
    {
        public static IObservable<string> GetFileSource(string path, Func<string, Task<string>> processor, IScheduler scheduler = null)
        {

            scheduler = scheduler ?? Scheduler.Default;

            return Observable.Create<string>(obs =>
            {
                //Grab the enumerator as our iteration state.
                var enumerator = File.Open(path,FileMode.Open);
                return scheduler.Schedule(new StreamReader(enumerator), async (e, recurse) =>
                {
                    if (e.EndOfStream)
                    {
                        obs.OnCompleted();
                        return;
                    }

                    var line = e.ReadLine();
                        //Wait here until processing is done before moving on
                        obs.OnNext(await processor(line));
                        

                    //Recursively schedule
                    recurse(e);
                });
            });

        }

        public static IObservable<string> GetFileSourceAsync(string path, IScheduler scheduler = null)
        {

            scheduler = scheduler ?? Scheduler.Default;

            return Observable.Create<string>(obs =>
            {
                //Grab the enumerator as our iteration state.
                var enumerator = File.Open(path, FileMode.Open);
                return scheduler.Schedule(new StreamReader(enumerator), async (e, recurse) =>
                {
                    if (e.EndOfStream)
                    {
                        obs.OnCompleted();
                        return;
                    }

                    var line = e.ReadLine();
                    //Wait here until processing is done before moving on
                    await Task.Run(() => obs.OnNext(line));


                    //Recursively schedule
                    recurse(e);
                });
            });

        }



        public static IObservable<string> GetFileSource(string path, IScheduler scheduler = null)
        {

            scheduler = scheduler ?? Scheduler.Default;

            return Observable.Create<string>(obs =>
            {
                //Grab the enumerator as our iteration state.
                var enumerator = File.Open(path, FileMode.Open);
                return scheduler.Schedule(new StreamReader(enumerator), (e, recurse) =>
                {
                    if (e.EndOfStream)
                    {
                        obs.OnCompleted();
                        return;
                    }

                    var line = e.ReadLine();
                    //Wait here until processing is done before moving on
                    obs.OnNext(line);


                    //Recursively schedule
                    recurse(e);
                });
            });

        }

    }
}
