using Application.DTOs;
using Application.Interfaces;
using ApartmentManagement.Core.Domain.Entities;
using AutoMapper;
using Domain.Repository;
using System;
using System.Collections.Generic;

namespace Application.ServiceImpl
{
    public class PaymentRecordServiceImpl : IPaymentRecordService
    {
        private readonly IPaymentRecordRepository _paymentRecordRepository;
        private readonly IMapper _mapper;

        public PaymentRecordServiceImpl(IPaymentRecordRepository paymentRecordRepository, IMapper mapper)
        {
            _paymentRecordRepository = paymentRecordRepository;
            _mapper = mapper;
        }

        public void CreatePaymentRecord(PaymentRecordDto paymentRecordDto)
        {
            if (paymentRecordDto == null) throw new ArgumentNullException(nameof(paymentRecordDto));
            var paymentRecordEntity = _mapper.Map<PaymentRecord>(paymentRecordDto);
            _paymentRecordRepository.Add(paymentRecordEntity);
        }

        public PaymentRecordDto? GetPaymentRecordById(int id)
        {
            var paymentRecordEntity = _paymentRecordRepository.GetById(id);
            if (paymentRecordEntity == null) return null;
            return _mapper.Map<PaymentRecordDto>(paymentRecordEntity);
        }

        public IEnumerable<PaymentRecordDto> GetAllPaymentRecords()
        {
            var paymentRecordEntities = _paymentRecordRepository.GetAll();
            return _mapper.Map<IEnumerable<PaymentRecordDto>>(paymentRecordEntities);
        }

        public void UpdatePaymentRecord(PaymentRecordDto paymentRecordDto)
        {
            if (paymentRecordDto == null) throw new ArgumentNullException(nameof(paymentRecordDto));
            var paymentRecordEntity = _mapper.Map<PaymentRecord>(paymentRecordDto);
            _paymentRecordRepository.Update(paymentRecordEntity);
        }

        public void DeletePaymentRecord(int id)
        {
            _paymentRecordRepository.Delete(id);
        }
    }
}
