using CBS.TransactionManagement.Common.GenericRespository;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Data.Dto.RemittanceP;
using CBS.TransactionManagement.Data.Entity.RemittanceP;
using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Repository.SalaryManagement.SalaryFiles;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Repository.RemittanceP
{
    // Remittance Repository
    public class RemittanceRepository : GenericRepository<Remittance, TransactionContext>, IRemittanceRepository
    {
        private readonly UserInfoToken _userInfoToken;
        private readonly ILogger<SalaryProcessingRepository> _logger;

        public RemittanceRepository(IUnitOfWork<TransactionContext> unitOfWork, UserInfoToken userInfoToken, ILogger<SalaryProcessingRepository> logger) : base(unitOfWork)
        {
            _userInfoToken = userInfoToken;
            _logger = logger;
        }

        /// <summary>
        /// Validates the receiver's identity by matching the provided details with the remittance record.
        /// Performs an 80% match for the name and address, and exact match for other details.
        /// </summary>
        /// <param name="remittance">The remittance object containing stored details.</param>
        /// <param name="receiverName">The receiver's name provided for validation.</param>
        /// <param name="receiverAddress">The receiver's address provided for validation.</param>
        /// <param name="receiverPhoneNumber">The receiver's phone number provided for validation.</param>
        /// <param name="secreteCode">The secret code provided for validation.</param>
        /// <returns>Empty string if validation passes, otherwise throws an exception.</returns>
        public string ValidateReceiverIdentity(Remittance remittance, string receiverName, string receiverAddress, string receiverPhoneNumber, string secreteCode)
        {
            if (remittance == null)
            {
                throw new ArgumentNullException(nameof(remittance), "Remittance data cannot be null.");
            }

            List<string> errors = new List<string>();

            // Validate inputs before comparison
            if (string.IsNullOrWhiteSpace(receiverName))
            {
                errors.Add("Receiver name is required.");
            }
            if (string.IsNullOrWhiteSpace(receiverAddress))
            {
                errors.Add("Receiver address is required.");
            }
            if (string.IsNullOrWhiteSpace(receiverPhoneNumber))
            {
                errors.Add("Receiver phone number is required.");
            }
            if (string.IsNullOrWhiteSpace(secreteCode))
            {
                errors.Add("Secret code is required.");
            }

            // Perform validation only if inputs are provided
            if (!errors.Any())
            {
                if (!BaseUtilities.IsFuzzyMatch(remittance.ReceiverName, receiverName, 0.8))
                {
                    errors.Add("1. Receiver name does not sufficiently match the records.");
                }

                if (!BaseUtilities.IsFuzzyMatch(remittance.ReceiverAddress, receiverAddress, 0.8))
                {
                    errors.Add("2. Receiver address does not sufficiently match the records.");
                }

                if (!remittance.ReceiverPhoneNumber.Equals(receiverPhoneNumber, StringComparison.OrdinalIgnoreCase))
                {
                    errors.Add("3. Receiver phone number does not match the records.");
                }

                if (!remittance.SenderSecreteCode.Equals(secreteCode, StringComparison.OrdinalIgnoreCase))
                {
                    errors.Add("4. Secret code is incorrect.");
                }
            }

            if (errors.Any())
            {
                throw new InvalidOperationException(string.Join("\n", errors));
            }

            return string.Empty;
        }


        /// <summary>
        /// Validates the sender's identity by matching the provided details with the remittance record.
        /// Performs an 80% match for the name and address, and validates the transaction amount and date.
        /// </summary>
        /// <param name="remittance">The remittance object containing stored details.</param>
        /// <param name="senderName">The sender's name provided for validation.</param>
        /// <param name="senderAddress">The sender's address provided for validation.</param>
        /// <param name="senderPhoneNumber">The sender's phone number provided for validation.</param>
        /// <param name="secreteCode">The secret code provided for validation.</param>
        /// <param name="amount">The amount sent in the transaction.</param>
        /// <param name="transactionDate">The date of the transaction.</param>
        /// <returns>Empty string if validation passes, otherwise throws an exception.</returns>
        public string ValidateSenderIdentity(Remittance remittance, string senderName, string senderPhoneNumber, string secreteCode, decimal amount, DateTime transactionDate)
        {
            if (remittance == null)
            {
                throw new ArgumentNullException(nameof(remittance), "Remittance data cannot be null.");
            }

            List<string> errors = new List<string>();

            // Validate inputs before comparison
            if (string.IsNullOrWhiteSpace(senderName))
            {
                errors.Add("Sender name is required.");
            }
           
            if (string.IsNullOrWhiteSpace(senderPhoneNumber))
            {
                errors.Add("Sender phone number is required.");
            }
            if (string.IsNullOrWhiteSpace(secreteCode))
            {
                errors.Add("Secret code is required.");
            }
            if (amount <= 0)
            {
                errors.Add("Transaction amount must be greater than zero.");
            }
            if (transactionDate == DateTime.MinValue)
            {
                errors.Add("Transaction date is required.");
            }

            // Perform validation only if all required fields are provided
            if (!errors.Any())
            {
                if (!BaseUtilities.IsFuzzyMatch(remittance.SenderName, senderName, 0.8))
                {
                    errors.Add("1. Sender name does not sufficiently match the records.");
                }

                

                if (!remittance.SenderPhoneNumber.Equals(senderPhoneNumber, StringComparison.OrdinalIgnoreCase))
                {
                    errors.Add("3. Sender phone number does not match the records.");
                }

                if (!remittance.SenderSecreteCode.Equals(secreteCode, StringComparison.OrdinalIgnoreCase))
                {
                    errors.Add("4. Secret code is incorrect.");
                }

                if (remittance.Amount != amount)
                {
                    errors.Add($"5. Transaction amount does not match the records. Expected: {remittance.Amount}, Provided: {amount}");
                }

                if (remittance.InitiationDate.Date != transactionDate.Date)
                {
                    errors.Add($"6. Transaction date does not match the records. Expected: {remittance.InitiationDate:yyyy-MM-dd}, Provided: {transactionDate:yyyy-MM-dd}");
                }
            }

            if (errors.Any())
            {
                throw new InvalidOperationException(string.Join("\n", errors));
            }

            return string.Empty;
        }
    }
}
