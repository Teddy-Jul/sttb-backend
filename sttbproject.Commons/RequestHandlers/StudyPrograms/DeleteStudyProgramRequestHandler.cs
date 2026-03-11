using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using sttbproject.Contracts.RequestModels.StudyPrograms;
using sttbproject.entities;

namespace sttbproject.Commons.RequestHandlers.StudyPrograms;

public class DeleteStudyProgramRequestHandler : IRequestHandler<DeleteStudyProgramRequest, bool>
{
    private readonly SttbprojectContext _context;
    private readonly ILogger<DeleteStudyProgramRequestHandler> _logger;

    public DeleteStudyProgramRequestHandler(
        SttbprojectContext context,
        ILogger<DeleteStudyProgramRequestHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<bool> Handle(DeleteStudyProgramRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting study program: {ProgramId}", request.ProgramId);

        var program = await _context.StudyPrograms
            .Include(p => p.ProgramCourseCategories)
            .Include(p => p.ProgramFees)
            .FirstOrDefaultAsync(p => p.ProgramId == request.ProgramId, cancellationToken);

        if (program == null)
        {
            _logger.LogWarning("Study program not found: {ProgramId}", request.ProgramId);
            throw new InvalidOperationException("Study program not found");
        }

        // CASCADE DELETE will handle related records automatically
        _context.StudyPrograms.Remove(program);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Study program deleted successfully: {ProgramId}", request.ProgramId);

        return true;
    }
}