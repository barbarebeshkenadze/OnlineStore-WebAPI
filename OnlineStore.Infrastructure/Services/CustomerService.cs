using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using OnlineStore.Core.Entities;
using OnlineStore.Core.Interfaces;

namespace OnlineStore.Infrastructure.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomerService(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<List<Customer>> GetAllAsync()
        {
            return await _customerRepository.GetAllAsync();
        }

        public async Task<Customer?> GetByIdAsync(int id)
        {
            return await _customerRepository.GetByIdAsync(id);
        }

        public async Task<Customer> CreateAsync(Customer customer)
        {
            ValidateCustomer(customer);

            customer.Email = customer.Email.Trim();

            var existing = await _customerRepository.GetByEmailAsync(
                customer.Email);

            if (existing != null)
                throw new InvalidOperationException(
                    "A customer with this email already exists.");

            await _customerRepository.AddAsync(customer);
            return customer;
        }

        public async Task<bool> UpdateAsync(int id, Customer customer)
        {
            ValidateCustomer(customer);

            var existing = await _customerRepository.GetByIdAsync(id);

            if (existing == null)
                return false;

            var email = customer.Email.Trim();

            var emailOwner = await _customerRepository.GetByEmailAsync(email);

            if (emailOwner != null && emailOwner.Id != id)
                throw new InvalidOperationException(
                    "A customer with this email already exists.");

            existing.FirstName = customer.FirstName;
            existing.LastName = customer.LastName;
            existing.Email = email;
            existing.Phone = customer.Phone;

            await _customerRepository.UpdateAsync(existing);
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var customer = await _customerRepository.GetByIdAsync(id);

            if (customer == null)
                return false;

            if (await _customerRepository.HasOrdersAsync(id))
                throw new InvalidOperationException(
                    "Cannot delete a customer who has orders.");

            await _customerRepository.DeleteAsync(customer);
            return true;
        }

        private static void ValidateCustomer(Customer customer)
        {
            if (string.IsNullOrWhiteSpace(customer.FirstName))
                throw new ArgumentException("First name is required.");

            if (string.IsNullOrWhiteSpace(customer.LastName))
                throw new ArgumentException("Last name is required.");

            if (string.IsNullOrWhiteSpace(customer.Email))
                throw new ArgumentException("Email is required.");

            if (customer.FirstName.Length > 100 ||
                customer.LastName.Length > 100)
                throw new ArgumentException(
                    "First and last names cannot exceed 100 characters.");

            if (customer.Email.Length > 200)
                throw new ArgumentException(
                    "Email cannot exceed 200 characters.");

            if (customer.Phone?.Length > 30)
                throw new ArgumentException(
                    "Phone cannot exceed 30 characters.");

            try
            {
                var address = new MailAddress(customer.Email.Trim());

                if (!string.Equals(address.Address, customer.Email.Trim(),
                    StringComparison.OrdinalIgnoreCase))
                    throw new FormatException();
            }
            catch (FormatException)
            {
                throw new ArgumentException("Invalid email address.");
            }
        }
    }
}