using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO.AdministrationModule;
using Application.MainBoundedContext.DTO.RegistryModule;
using Infrastructure.Crosscutting.Framework.Utils;

namespace Application.MainBoundedContext.RegistryModule.Services
{
    internal static class CustomerRegistrationNotification
    {
        public static string EmailBody(CustomerDTO customer, BranchDTO branch, IEnumerable<CustomerAccountDTO> accounts)
        {
            var body = new StringBuilder();
            body.AppendFormat("<p>Dear {0},</p><p>Welcome to {1}. Your customer record has been created.</p>",
                Encode(customer.FullName), Encode(branch.CompanyDescription));
            body.Append("<dl>");
            Detail(body, "Customer number", customer.SerialNumber > 0 ? customer.PaddedSerialNumber : null);
            Detail(body, "Branch", branch.Description);
            if (customer.Type == (int)CustomerType.Individual)
                Detail(body, "Payroll number(s)", customer.IndividualPayrollNumbers);
            else
                Detail(body, "Organisation registration number", customer.NonIndividualRegistrationNumber);
            Detail(body, "Personal file number", customer.Reference3);
            body.Append("</dl>");
            body.Append(customer.RecordStatus == (byte)RecordStatus.Approved
                ? "<p>Customer verification status: Approved.</p>"
                : "<p>Customer verification status: Pending verification.</p>");

            // Only persisted accounts owned by this customer may appear. Use the
            // canonical DTO formatter, including leading zeros and product codes.
            var savedAccounts = (accounts ?? Enumerable.Empty<CustomerAccountDTO>())
                .Where(account => account != null && account.Id != Guid.Empty && account.CustomerId == customer.Id)
                .GroupBy(account => account.Id).Select(group => group.First())
                .OrderBy(account => account.FullAccountNumber).ToList();
            if (savedAccounts.Any())
            {
                body.Append("<h3>Accounts created</h3><table><thead><tr><th>Product</th><th>Account number</th></tr></thead><tbody>");
                foreach (var account in savedAccounts)
                    body.AppendFormat("<tr><td>{0}</td><td>{1}</td></tr>",
                        Encode(account.CustomerAccountTypeTargetProductDescription), Encode(account.FullAccountNumber));
                body.Append("</tbody></table>");
            }
            else
                body.Append("<p>No account numbers are available yet. Please contact your branch for assistance.</p>");
            body.Append("<p>Please quote your customer number when contacting us and the relevant account number for account enquiries.</p>");
            return body.ToString();
        }

        public static string TextBody(CustomerDTO customer, BranchDTO branch)
        {
            return string.Format("Dear {0},\nWelcome to {1}. Your customer record has been created.{2}{3}",
                customer.FullName, branch.CompanyDescription,
                customer.SerialNumber > 0 ? "\nYour customer number is " + customer.PaddedSerialNumber + "." : string.Empty,
                customer.RecordStatus == (byte)RecordStatus.Approved ? string.Empty : "\nPending customer verification.");
        }

        private static void Detail(StringBuilder body, string label, string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
                body.AppendFormat("<dt>{0}</dt><dd>{1}</dd>", Encode(label), Encode(value));
        }

        private static string Encode(string value)
        {
            return WebUtility.HtmlEncode(value ?? string.Empty);
        }
    }
}
