using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using sttbproject.Contracts.RequestModels.StudyPrograms;
using sttbproject.Contracts.ResponseModels.StudyPrograms;
using sttbproject.entities;

namespace sttbproject.Commons.RequestHandlers.StudyPrograms;

public class GetStudyProgramListRequestHandler : IRequestHandler<GetStudyProgramListRequest, GetStudyProgramListResponse>
{
    private readonly SttbprojectContext _context;
    private readonly ILogger<GetStudyProgramListRequestHandler> _logger;

    public GetStudyProgramListRequestHandler(
        SttbprojectContext context,
        ILogger<GetStudyProgramListRequestHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<GetStudyProgramListResponse> Handle(GetStudyProgramListRequest request, CancellationToken cancellationToken)
    {
        var query = _context.StudyPrograms.AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            query = query.Where(p => p.ProgramName.Contains(request.SearchTerm) || 
                                     p.Description!.Contains(request.SearchTerm));
        }

        if (!string.IsNullOrWhiteSpace(request.DegreeLevel))
        {
            query = query.Where(p => p.DegreeLevel == request.DegreeLevel);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var programs = await query
            .OrderBy(p => p.ProgramName)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(p => new StudyProgramListItem
            {
                ProgramId = p.ProgramId,
                ProgramName = p.ProgramName,
                DegreeLevel = p.DegreeLevel,
                DegreeTitle = p.DegreeTitle,
                TotalCredits = p.TotalCredits,
                StudyDuration = p.StudyDuration,
                CreatedAt = p.CreatedAt
            })
            .ToListAsync(cancellationToken);

        _logger.LogInformation("Retrieved {Count} study programs", programs.Count);

        return new GetStudyProgramListResponse
        {
            Programs = programs,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}