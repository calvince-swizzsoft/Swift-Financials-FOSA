using System;
using System.Collections.Generic;
using Application.MainBoundedContext.DTO.AccountsModule;
using Infrastructure.Crosscutting.Framework.Utils;
namespace Application.MainBoundedContext.AccountsModule.Services
{
    public interface ISasraInsiderAppService
    {
        Form9Page<InsiderAppointmentDTO> GetAppointments(string text,int page,int size,ServiceHeader h);
        InsiderAppointmentDTO SaveAppointment(InsiderAppointmentDTO input,ServiceHeader h);
        Form9Page<Form9Candidate> GetCandidates(string text,int page,int size,ServiceHeader h);
        Form9Page<Form9Source> GetLoans(string text,int page,int size,ServiceHeader h);
        InsiderDecisionDTO GetDecision(Guid id,ServiceHeader h);
        InsiderDecisionDTO SaveDecision(InsiderDecisionDTO input,ServiceHeader h);
        Form9PolicyDTO GetPolicy(ServiceHeader h);
        Form9PolicyDTO SavePolicy(Form9PolicyDTO input,ServiceHeader h);
        List<Form9Product> GetProducts(ServiceHeader h);
        Form9Result Preview(Form9Request input,ServiceHeader h);
        Form9Result SaveDraft(Form9Request input,ServiceHeader h);
        Form9Page<Form9Result> GetRuns(int page,int size,ServiceHeader h);
        Form9Result GetRun(Guid id,ServiceHeader h);
        Form9Export Export(Guid id,ServiceHeader h);
        List<Form9Revision> History(string kind,Guid id,ServiceHeader h);
    }
}
