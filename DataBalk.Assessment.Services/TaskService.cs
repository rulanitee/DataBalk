using AutoMapper;
using DataBalk.Assessment.Data.Abstractions;
using DataBalk.Assessment.Services.Abstractions;
using DataBalk.Assessment.Services.Models;
using Microsoft.EntityFrameworkCore;
using Task = System.Threading.Tasks.Task;
using TaskEntity = DataBalk.Assessment.Data.Models.Task;

namespace DataBalk.Assessment.Services
{
    public class TaskService(IRepository repository, IMapper mapper) : ITaskService
    {

        private readonly IRepository _repository = repository;
        private readonly IMapper _mapper = mapper;

        public async Task<long> AssignTask(TaskDto dto)
        {
            var taskEntity = _mapper.Map<TaskEntity>(dto);
            var created = await _repository.Insert(taskEntity);
            return created.Id;
        }

        public async Task<List<TaskViewModel>> GetAllActiveTasks()
        {
            var now = DateTime.UtcNow;
            var tasks = await _repository.Get<TaskEntity>(
                t => t.DueDate >= now,
                include: query => query.Include(t => t.Assignee)
            );
            var activeTasks = _mapper.Map<IEnumerable<TaskViewModel>>(tasks);
            return activeTasks.ToList();
        }

        public async Task<List<TaskViewModel>> GetAllExpiredTasks()
        {
            var now = DateTime.UtcNow;
            var tasks = await _repository.Get<TaskEntity>(
                t => t.DueDate < now,
                include: query => query.Include(t => t.Assignee)
            );
            var expiredTasks = _mapper.Map<IEnumerable<TaskViewModel>>(tasks);
            return expiredTasks.ToList();
        }

        public async Task<List<TaskViewModel>> GetAllTasksByDate(DateTime dateTime)
        {
            var tasks = await _repository.Get<TaskEntity>(
                t => t.DueDate.Date == dateTime.Date,
                include: query => query.Include(t => t.Assignee)
            );
            var filteredTasks = _mapper.Map<IEnumerable<TaskViewModel>>(tasks);
            return filteredTasks.ToList();
        }

        public async Task RemoveTask(long id)
        {
            var task = await _repository.GetById<TaskEntity>(id);
            await _repository.Delete(task);
        }
    
    }
}
