module TaskToAsync

open System.Threading.Tasks

let awaitTask (task : Task<'a>) =
    async {
        do! task |> Async.AwaitIAsyncResult |> Async.Ignore
        if task.IsFaulted then raise task.Exception
        return task.Result
    }
