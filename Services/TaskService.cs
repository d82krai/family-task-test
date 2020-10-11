using AutoMapper;
using Core.Abstractions.Repositories;
using Core.Abstractions.Services;
using Domain.Commands;
using Domain.DataModels;
using Domain.Queries;
using Domain.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IMemberRepository _memberRepository;
        private readonly IMapper _mapper;

        public TaskService(IMapper mapper, ITaskRepository taskRepository, IMemberRepository memberRepository)
        {
            _mapper = mapper;
            _taskRepository = taskRepository;
            _memberRepository = memberRepository;
        }

        public async Task<CreateTaskCommandResult> CreateTaskCommandHandler(CreateTaskCommand command)
        {
            var task = _mapper.Map<Domain.DataModels.Task>(command);
            var persistedTask = await _taskRepository.CreateRecordAsync(task);

            var vm = _mapper.Map<TaskVm>(persistedTask);

            return new CreateTaskCommandResult()
            {
                Payload = vm
            };
        }

        public async Task<UpdateTaskCommandResult> UpdateTaskCommandHandler(UpdateTaskCommand command)
        {
            var isSucceed = true;
            var task = await _taskRepository.ByIdAsync(command.Id);

            _mapper.Map<UpdateTaskCommand, Domain.DataModels.Task>(command, task);

            var affectedRecordsCount = await _taskRepository.UpdateRecordAsync(task);

            if (affectedRecordsCount < 1)
                isSucceed = false;

            return new UpdateTaskCommandResult()
            {
                Succeed = isSucceed
            };
        }

        public async Task<GetAllTasksQueryResult> GetAllTasksQueryHandler()
        {
            IEnumerable<TaskVm> vm = new List<TaskVm>();

            var tasks = await _taskRepository.Reset().ToListAsync();

            if (tasks != null && tasks.Any())
                vm = _mapper.Map<IEnumerable<TaskVm>>(tasks);

            foreach (var item in vm)
            {
                if (item.AssignedToId != Guid.Empty)
                    item.AssignedTo = await _memberRepository.ByIdAsync(item.AssignedToId);
                else
                    item.AssignedTo = null;
            }

            return new GetAllTasksQueryResult()
            {
                Payload = vm
            };
        }

        public async Task<GetTaskByIdQueryResult> GetTaskByIdQueryHandler(Guid taskId)
        {
            TaskVm vm = new TaskVm();

            var task = await _taskRepository.Reset().ByIdAsync(taskId);

            if (task != null)
                vm = _mapper.Map<TaskVm>(task);

            return new GetTaskByIdQueryResult()
            {
                Payload = vm
            };
        }

        public async Task<GetTaskByIdQueryResult> UpdateTaskAssignedTo(Guid taskId, Guid memberId)
        {
            var task = await GetTaskByIdQueryHandler(taskId);
            if (task != null && task.Payload != null)
            {
                task.Payload.AssignedToId = memberId;
                UpdateTaskCommand command = new UpdateTaskCommand()
                {
                    Id = task.Payload.Id,
                    AssignedToId = memberId,
                    Subject = task.Payload.Subject,
                    AssignedTo = task.Payload.AssignedTo,
                    IsComplete = task.Payload.IsComplete
                };
                var result = await UpdateTaskCommandHandler(command);
                if (result.Succeed)
                {
                    return new GetTaskByIdQueryResult()
                    {
                        Payload = new TaskVm()
                        {
                            IsComplete = task.Payload.IsComplete,
                            AssignedToId = task.Payload.AssignedToId,
                            Id = task.Payload.Id,
                            Subject = task.Payload.Subject
                        }
                    };
                }
            }

            return new GetTaskByIdQueryResult()
            {
                Payload = null
            };
        }
    }
}
