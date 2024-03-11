using API.Data;
using API.DTOs;
using API.Helper;
using API.Interface.Repository;
using API.Param;
using API.Param.Enums;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Repository
{
    public class TaskRepository : BaseRepository<Entity.Task>, ITaskRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public TaskRepository(DataContext context, IMapper mapper) : base(context)
        {
            _context = context;
            _mapper = mapper;
        }

        public Task<bool> CheckAccountAssignedValid(int accountId)
        {
            return _context.Account.AnyAsync(c => c.AccountId.Equals(accountId) && c.RoleId.Equals((int)RoleEnum.Staff)); 
        }

        public async Task CreateTaskAsync(CreateUpdateTaskDto taskDto, int adminId)
        {
            var accountCreate = _context.Account.SingleOrDefault(c => c.AccountId.Equals(adminId));
            var accountAssigned = _context.Account.SingleOrDefault(c => c.AccountId.Equals(taskDto.AccountAssignedId));

            var newTask = new Entity.Task();
            newTask.AccountAssignedName = accountAssigned.AccountName;
            newTask.AccountAssignedId = accountAssigned.AccountId;
            newTask.AccountCreateId = accountCreate.AccountId;
            newTask.AccountCreateName = accountCreate.AccountName;
            newTask.TaskTitle = taskDto.TaskTitle;
            newTask.TaskContent = taskDto.TaskContent;
            newTask.Status = taskDto.Status;
            newTask.TaskNotes = taskDto.TaskNotes;
            newTask.DateCreated = DateTime.Now;

            _context.Set<Entity.Task>().Add(newTask);
            await _context.SaveChangesAsync();
        }

        public async Task EditTaskAsync(CreateUpdateTaskDto taskDto, int adminId, int taskId)
        {
            var existingTask = await _context.Task.FindAsync(taskId);

            var accountCreate = _context.Account.SingleOrDefault(c => c.AccountId.Equals(adminId));
            var accountAssigned = _context.Account.SingleOrDefault(c => c.AccountId.Equals(taskDto.AccountAssignedId));

            existingTask.AccountAssignedName = accountAssigned.AccountName;
            existingTask.AccountAssignedId = accountAssigned.AccountId;
            existingTask.AccountCreateId = accountCreate.AccountId;
            existingTask.AccountCreateName = accountCreate.AccountName;
            existingTask.TaskTitle = taskDto.TaskTitle;
            existingTask.TaskContent = taskDto.TaskContent;
            existingTask.Status = taskDto.Status;
            existingTask.TaskNotes = taskDto.TaskNotes;
            existingTask.DateUpdated = DateTime.Now;

            _context.Entry(existingTask).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task<TaskDetailDto> GetTaskDetailAsync(int accountId, int taskId, string role)
        {

            if (role.Equals("ADMIN"))
            {
                var task = await _context.Task.SingleOrDefaultAsync(t => t.AccountCreateId == accountId &&
                                    t.TaskId == taskId);
                return _mapper.Map<TaskDetailDto>(task);
            }
            else
            {
                var task = await _context.Task.SingleOrDefaultAsync(t => t.AccountAssignedId == accountId &&
                                    t.TaskId == taskId);
                return _mapper.Map<TaskDetailDto>(task);
            }
        }

        public async Task<PageList<TaskDto>> GetTasksAsync(
            PaginationParams paginationParams, 
            TaskRequest taskParam, 
            int accountId,
            string role)
        {
            var query = _context.Task.AsQueryable();

            if (role.Equals("ADMIN"))
            {
                query = query.Where(t => t.AccountCreateId == accountId);
            }
            else
            {
                query = query.Where(t => t.AccountAssignedId == accountId);
            }

            //if (taskParam.Status != -1)
            //{
            //    query = query.Where(t => t.Status == taskParam.Status);
            //}

            if (!string.IsNullOrEmpty(taskParam.AccountAssignedName))
            {
                query = query.Where(t =>
                    t.AccountAssignedName.ToLower().Contains(taskParam.AccountAssignedName.ToLower()));
            }

            if (!string.IsNullOrEmpty(taskParam.AccountCreateName))
            {
                query = query.Where(t =>
                    t.AccountCreateName.ToLower().Contains(taskParam.AccountCreateName.ToLower()));
            }

            query = query.OrderByDescending(r => r.DateCreated);

            return await PageList<TaskDto>.CreateAsync(
            query.AsNoTracking().ProjectTo<TaskDto>(_mapper.ConfigurationProvider),
            paginationParams.PageNumber,
            paginationParams.PageSize);
        }

        public async Task UpdateTaskStatus(int taskId, int taskStatus)
        {
            var existingTask = await _context.Task.FindAsync(taskId);
            if (existingTask != null)
            {
                existingTask.Status = taskStatus;
                _context.Entry(existingTask).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
        }
    }
}
