using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using TODOList.GrpcService;

namespace TODOList.GrpcService.Services
{
    public class DoingsApiService : DoingsService.DoingsServiceBase 
    {
        private readonly ILogger<DoingsApiService> _logger;
        private readonly TodolistDbContext _dbContext;
        public DoingsApiService(ILogger<DoingsApiService> logger, TodolistDbContext dbContext)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }
        public override async Task<ListReply> GetDoings(GetDoingsRequest request, ServerCallContext context)
        {
            var response = new ListReply();
            var doingsFromDb = await _dbContext.Doings.Select(item => new DoingReply { Id = item.Id, AddedDate = Timestamp.FromDateTime(item.AddedDate), CompletionDate = Timestamp.FromDateTime(item.CompletionDate), IsComplete = item.Iscomplete, Name = item.Name }).OrderBy(item => item.Id).ToListAsync();
            if (request.OnlyIsCompleteDoings)
            {
                doingsFromDb = doingsFromDb.Where(item => item.IsComplete == true).ToList();
            }
            response.Doings.AddRange(doingsFromDb);
            return await Task.FromResult(response);
        }
        public override async Task<DoingReply> CreateDoing(CreateDoingRequest request, ServerCallContext context)
        {
            var doingForDb = new Doing() { };
            doingForDb.Name = request.Name;
            doingForDb.Iscomplete = request.IsComplete;
            doingForDb.AddedDate = request.AddedDate.ToDateTime();
            doingForDb.CompletionDate = request.CompletionDate.ToDateTime();

            await _dbContext.Doings.AddAsync(doingForDb);
            await _dbContext.SaveChangesAsync();

            var response = new DoingReply() { Id = doingForDb.Id, Name = doingForDb.Name, IsComplete = doingForDb.Iscomplete, AddedDate = request.AddedDate, CompletionDate = request.CompletionDate};

            return await Task.FromResult(response);
        }
        public override async Task<DoingReply> DeleteDoing(DeleteDoingRequest request, ServerCallContext context)
        {
            var doing = await _dbContext.Doings.FindAsync(request.Id);
            if (doing == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "User not found"));
            }
            _dbContext.Doings.Remove(doing);
            await _dbContext.SaveChangesAsync();

            var response = new DoingReply() { Id = doing.Id, Name = doing.Name, IsComplete = doing.Iscomplete, AddedDate = doing.AddedDate.ToTimestamp(), CompletionDate = doing.CompletionDate.ToTimestamp()};
            return await Task.FromResult(response);
        }
        public override async Task<DoingReply> GetDoing(GetDoingRequest request, ServerCallContext context)
        {
            var doing = await _dbContext.Doings.FindAsync(request.Id);
            if (doing == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "User not found"));
            }

            var response = new DoingReply() { Id = doing.Id, Name = doing.Name, IsComplete = doing.Iscomplete, AddedDate = doing.AddedDate.ToTimestamp(), CompletionDate = doing.CompletionDate.ToTimestamp() };
            return await Task.FromResult(response);
        }
        public override async Task<DoingReply> UpdateDoing(UpdateDoingRequst request, ServerCallContext context)
        {
            var doing = await _dbContext.Doings.FindAsync(request.Id);

            if (doing == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "User not found"));
            }

            doing.Name = request.Name;
            doing.Iscomplete = request.IsComplete;
            doing.AddedDate = request.AddedDate.ToDateTime();
            doing.CompletionDate = request.CompletionDate.ToDateTime();

            await _dbContext.SaveChangesAsync();

            var responce = new DoingReply() { Id = doing.Id, Name = doing.Name, IsComplete = doing.Iscomplete, AddedDate = doing.AddedDate.ToTimestamp(), CompletionDate = doing.CompletionDate.ToTimestamp()};

            return await Task.FromResult(responce);
        }
    }
}
