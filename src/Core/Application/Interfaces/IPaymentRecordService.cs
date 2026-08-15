using Application.DTOs;
using System.Collections.Generic;

namespace Application.Interfaces
{
    public interface IPaymentRecordService
    {
        void CreatePaymentRecord(PaymentRecordDto paymentRecordDto);
        PaymentRecordDto? GetPaymentRecordById(int id);
        IEnumerable<PaymentRecordDto> GetAllPaymentRecords();
        void UpdatePaymentRecord(PaymentRecordDto paymentRecordDto);
        void DeletePaymentRecord(int id);
    }
}
