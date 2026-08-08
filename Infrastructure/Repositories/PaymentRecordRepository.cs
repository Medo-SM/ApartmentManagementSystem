using System.Collections.Generic;
using System.Linq;
using ApartmentManagement.Core.Domain.Entities;
using Domain.Repository;
using Infrastructure.Data;

namespace ApartmentManagementSystem.Infrastructure.Repositories
{
    public class PaymentRecordRepository : IPaymentRecordRepository
    {
        private readonly AppDbContext _context;

        public PaymentRecordRepository(AppDbContext context)
        {
            _context = context;
        }

        public void Add(PaymentRecord paymentRecord)
        {
            _context.PaymentRecords.Add(paymentRecord);
            _context.SaveChanges();
        }

        public void Update(PaymentRecord paymentRecord)
        {
            _context.PaymentRecords.Update(paymentRecord);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var paymentRecord = _context.PaymentRecords.Find(id);
            if (paymentRecord != null)
            {
                _context.PaymentRecords.Remove(paymentRecord);
                _context.SaveChanges();
            }
        }

        public PaymentRecord? GetById(int id)
        {
            return _context.PaymentRecords.Find(id);
        }

        public List<PaymentRecord> GetAll()
        {
            return _context.PaymentRecords.ToList();
        }
    }
}