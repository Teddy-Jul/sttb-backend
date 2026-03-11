using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace sttbproject.Contracts.RequestModels.StudyPrograms
{
    public class DeleteStudyProgramRequest : IRequest<bool>
    {
        public int ProgramId { get; set; }
    }
}
