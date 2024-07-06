using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using SortaKinda.Classes;

namespace SortaKinda.Controllers;

public unsafe class SortingThreadController : IDisposable {
    private readonly List<Task> sortingTasks = [];
    
    private bool SortPending => sortingTasks.Any(task => task.Status is TaskStatus.Created);
    
    private readonly CancellationTokenSource cancellationTokenSource = new();

    public void Dispose() {
        cancellationTokenSource.Cancel();
    }

    public void AddSortingTask(InventoryType type, params InventoryGrid[] grids) {
        var sortingTask = new Task(() => {
            InventorySorter.SortInventory(type, grids);
        }, cancellationTokenSource.Token);
        
        sortingTasks.Add(sortingTask);
    }

    public void Update() {
        if (SortPending) {
            Service.Log.Verbose($"Launching sorting tasks. {sortingTasks.Count(task => task.Status is TaskStatus.Created)} Tasks Pending.");
            
            foreach (var task in sortingTasks.Where(task => task.Status is TaskStatus.Created)) {
                Service.Log.Verbose("Starting Task");
                task.Start();
            }

            Service.Log.Verbose("Scheduling Continuation");
            Task.WhenAll(sortingTasks).ContinueWith(_ => OnCompletion(), cancellationTokenSource.Token);
        }
    }

    private void OnCompletion() {
        Service.Log.Verbose("Continuing!");
        
        Service.Framework.RunOnTick(() => {
            Service.Log.Debug("Marked ItemODR as changed.");

            ItemOrderModule.Instance()->UserFileEvent.IsSavePending = true;
        }, delayTicks: 5);

        sortingTasks.RemoveAll(task => task.Status is TaskStatus.RanToCompletion or TaskStatus.Canceled or TaskStatus.Faulted);
    }
}