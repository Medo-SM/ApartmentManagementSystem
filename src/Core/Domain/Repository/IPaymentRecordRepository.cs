using ApartmentManagement.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Repository
{
    public interface IPaymentRecordRepository
    {
        void Add(PaymentRecord paymentRecord);
        void Update(PaymentRecord payment);
        void Delete(int Id);
        PaymentRecord? GetById(int Id);
        List<PaymentRecord> GetAll();
    }
}
