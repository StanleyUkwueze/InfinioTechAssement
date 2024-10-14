using IfinionBackendAssessment.DataAccess.Common;
using IfinionBackendAssessment.Entity.Entities;
using IfinionBackendAssessment.Service.DataTransferObjects.Requests;
using IfinionBackendAssessment.Service.DataTransferObjects.Responses;
using PayStack.Net;

namespace IfinionBackendAssessment.Service.TransactionServices
{
    public interface ITransactionService
    {
        Task<PaymentVerificationResponse> VerifyPayment(string trxref);
        Task<APIResponse<TransactionInitializeResponse>> InitiatePayment(TransactionDto transactionDto, int userId);
    }
}