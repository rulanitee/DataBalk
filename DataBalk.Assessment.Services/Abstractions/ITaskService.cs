using DataBalk.Assessment.Services.Models;

namespace DataBalk.Assessment.Services.Abstractions
{
    public interface ITaskService
    {
        Task<List<TaskViewModel>> GetAllExpiredTasks();

        Task<List<TaskViewModel>> GetAllActiveTasks();

        Task<List<TaskViewModel>> GetAllTasksByDate(DateTime dateTime);        

        Task<long> AssignTask(TaskDto dto);

        Task RemoveTask(long id);
    }
}
