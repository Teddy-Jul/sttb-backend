using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using sttbproject.Contracts.RequestModels.StudyPrograms;
using sttbproject.Contracts.ResponseModels.StudyPrograms;
using sttbproject.entities;

namespace sttbproject.Commons.RequestHandlers.StudyPrograms;

public class CreateStudyProgramRequestHandler : IRequestHandler<CreateStudyProgramRequest, StudyProgramDetailResponse>
{
    private readonly SttbprojectContext _context;
    private readonly ILogger<CreateStudyProgramRequestHandler> _logger;

    public CreateStudyProgramRequestHandler(
        SttbprojectContext context,
        ILogger<CreateStudyProgramRequestHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<StudyProgramDetailResponse> Handle(CreateStudyProgramRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating study program: {ProgramName}", request.ProgramName);

        var program = new StudyProgram
        {
            ProgramName = request.ProgramName,
            DegreeLevel = request.DegreeLevel,
            DegreeTitle = request.DegreeTitle,
            TotalCredits = request.TotalCredits,
            StudyDuration = request.StudyDuration,
            Description = request.Description,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.StudyPrograms.Add(program);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Study program created successfully with ID: {ProgramId}", program.ProgramId);

        return new StudyProgramDetailResponse
        {
            ProgramId = program.ProgramId,
            ProgramName = program.ProgramName,
            DegreeLevel = program.DegreeLevel,
            DegreeTitle = program.DegreeTitle,
            TotalCredits = program.TotalCredits,
            StudyDuration = program.StudyDuration,
            Description = program.Description,
            CreatedAt = program.CreatedAt,
            UpdatedAt = program.UpdatedAt
        };
    }
}