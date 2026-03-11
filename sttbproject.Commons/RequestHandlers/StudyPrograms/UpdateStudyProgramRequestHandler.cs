using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using sttbproject.Contracts.RequestModels.StudyPrograms;
using sttbproject.Contracts.ResponseModels.StudyPrograms;
using sttbproject.entities;

namespace sttbproject.Commons.RequestHandlers.StudyPrograms;

public class UpdateStudyProgramRequestHandler : IRequestHandler<UpdateStudyProgramRequest, StudyProgramDetailResponse>
{
    private readonly SttbprojectContext _context;
    private readonly ILogger<UpdateStudyProgramRequestHandler> _logger;

    public UpdateStudyProgramRequestHandler(
        SttbprojectContext context,
        ILogger<UpdateStudyProgramRequestHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<StudyProgramDetailResponse> Handle(UpdateStudyProgramRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating study program: {ProgramId}", request.ProgramId);

        var program = await _context.StudyPrograms
            .FirstOrDefaultAsync(p => p.ProgramId == request.ProgramId, cancellationToken);

        if (program == null)
        {
            _logger.LogWarning("Study program not found: {ProgramId}", request.ProgramId);
            throw new InvalidOperationException("Study program not found");
        }

        program.ProgramName = request.ProgramName;
        program.DegreeLevel = request.DegreeLevel;
        program.DegreeTitle = request.DegreeTitle;
        program.TotalCredits = request.TotalCredits;
        program.StudyDuration = request.StudyDuration;
        program.Description = request.Description;
        program.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Study program updated successfully: {ProgramId}", program.ProgramId);

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