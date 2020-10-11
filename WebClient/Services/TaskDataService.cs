using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using WebClient.Abstractions;
using WebClient.Shared.Models;
using System.Threading.Tasks;
using Domain.Commands;
using Microsoft.AspNetCore.Components;
using Core.Extensions.ModelConversion;
using Domain.Queries;
using Domain.ViewModel;

namespace WebClient.Services
{
    public class TaskDataService: ITaskDataService
    {
        private readonly HttpClient httpClient;

        public TaskDataService(IHttpClientFactory clientFactory)
        {
            httpClient = clientFactory.CreateClient("FamilyTaskAPI");
            tasks = new List<TaskVm>();
            LoadTasks();
        }
        private IEnumerable<TaskVm> tasks;



        public IEnumerable<TaskVm> Tasks => tasks;
        public TaskVm SelectedTask { get; private set; }

        public event EventHandler TasksChanged;
        public event EventHandler TasksUpdated;
        public event EventHandler TaskSelected;
        public event EventHandler<string> UpdateTaskFailed;

        private async void LoadTasks()
        {
            tasks = (await GetAllTasks()).Payload;
            TasksChanged?.Invoke(this, null);
        }

        public void SelectTask(Guid id)
        {
            SelectedTask = tasks.SingleOrDefault(t => t.Id == id);
            TasksUpdated?.Invoke(this, null);
        }

        private async Task<CreateTaskCommandResult> Create(CreateTaskCommand command)
        {
            return await httpClient.PostJsonAsync<CreateTaskCommandResult>("tasks", command);
        }

        private async Task<UpdateTaskCommandResult> Update(UpdateTaskCommand command)
        {
            return await httpClient.PutJsonAsync<UpdateTaskCommandResult>($"tasks/{command.Id}", command);
        }

        private async Task<GetAllTasksQueryResult> GetAllTasks()
        {
            return await httpClient.GetJsonAsync<GetAllTasksQueryResult>("tasks");
        }

        public async System.Threading.Tasks.Task ToggleTask(Guid id)
        {
            foreach (var taskModel in Tasks)
            {
                if (taskModel.Id == id)
                {
                    taskModel.IsComplete = !taskModel.IsComplete;
                    await UpdateTask(taskModel);
                }
            }

            TasksUpdated?.Invoke(this, null);
        }

        public async System.Threading.Tasks.Task AddTask(TaskVm model)
        {
            //Tasks.Add(model);
            //TasksUpdated?.Invoke(this, null);

            var result = await Create(model.ToCreateTaskCommand());
            if (result != null)
            {
                var updatedList = (await GetAllTasks()).Payload;

                if (updatedList != null)
                {
                    tasks = updatedList;
                    TasksChanged?.Invoke(this, null);
                    return;
                }
                UpdateTaskFailed?.Invoke(this, "The creation was successful, but we can no longer get an updated list of tasks from the server.");
            }

            UpdateTaskFailed?.Invoke(this, "Unable to create record.");
        }

        public async Task UpdateTask(TaskVm model)
        {
            var result = await Update(model.ToUpdateTaskCommand());

            Console.WriteLine(JsonSerializer.Serialize(result));

            if (result != null)
            {
                var updatedList = (await GetAllTasks()).Payload;

                if (updatedList != null)
                {
                    tasks = updatedList;
                    TasksChanged?.Invoke(this, null);
                    return;
                }
                UpdateTaskFailed?.Invoke(this, "The save was successful, but we can no longer get an updated list of tasks from the server.");
            }

            UpdateTaskFailed?.Invoke(this, "Unable to save changes.");
        }

        public async Task UpdateAssignedTo(Guid taskId, Guid memberId)
        {
            if (tasks.All(taskVm => taskVm.Id != taskId && taskVm.AssignedToId != memberId)) return;
            {
                var task = tasks.FirstOrDefault(m => m.Id == taskId);
                if (task != null)
                {
                    task.AssignedToId = memberId;
                    await UpdateTask(task);
                }
            }
        }
    }
}