using System;
using System.IO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO.AdministrationModule;
using Application.MainBoundedContext.DTO.RegistryModule;
using Application.MainBoundedContext.RegistryModule.Services;
using Infrastructure.Crosscutting.Framework.Utils;

internal static class CustomerRegistrationNotificationTests
{
    private static void Check(bool condition, string message)
    {
        if (!condition) throw new Exception(message);
    }

    public static void Main(string[] args)
    {
        var customer = new CustomerDTO { Id = Guid.NewGuid(), SerialNumber = 42,
            Type = (int)CustomerType.Individual, IndividualFirstName = "Alice <script>", IndividualLastName = "O'Brien",
            IndividualPayrollNumbers = "007-A / 009-B", Reference2 = "FAKE-MEMBER-999", Reference3 = "FILE-004",
            RecordStatus = (byte)RecordStatus.New };
        var branch = new BranchDTO { Description = "Central <Branch>", CompanyDescription = "A & B SACCO" };
        var savings = new CustomerAccountDTO { Id = Guid.NewGuid(), CustomerId = customer.Id, BranchCode = 3,
            CustomerSerialNumber = 42, CustomerAccountTypeProductCode = 1, CustomerAccountTypeTargetProductCode = 7,
            CustomerAccountTypeTargetProductDescription = "Savings <main>" };
        var shares = new CustomerAccountDTO { Id = Guid.NewGuid(), CustomerId = customer.Id, BranchCode = 3,
            CustomerSerialNumber = 42, CustomerAccountTypeProductCode = 2, CustomerAccountTypeTargetProductCode = 9,
            CustomerAccountTypeTargetProductDescription = "Share capital" };
        var foreign = new CustomerAccountDTO { Id = Guid.NewGuid(), CustomerId = Guid.NewGuid(), CustomerSerialNumber = 999 };
        var body = CustomerRegistrationNotification.EmailBody(customer, branch, new[] { savings, shares, savings, foreign, null, new CustomerAccountDTO { CustomerId = customer.Id } });
        Check(body.Contains("0000042") && body.Contains("003-0000042-001-007") && body.Contains("003-0000042-002-009"), "Canonical saved numbers missing");
        Check(body.Split(new[] { "003-0000042-001-007" }, StringSplitOptions.None).Length == 2, "Duplicate account");
        Check(!body.Contains(foreign.FullAccountNumber) && !body.Contains("000-0000000-000-000"), "Foreign or unsaved account leaked");
        Check(body.Contains("007-A / 009-B") && body.Contains("FILE-004"), "Supplied identifiers missing");
        Check(!body.Contains("FAKE-MEMBER") && !body.Contains("membership number"), "Legacy fabricated number included");
        Check(body.Contains("&lt;script&gt;") && body.Contains("A &amp; B") && body.Contains("Savings &lt;main&gt;") && !body.Contains("<script>"), "HTML values not escaped");
        Check(body.Contains("Pending verification"), "Pending registration presented as approved");
        var sms = CustomerRegistrationNotification.TextBody(customer, branch);
        Check(sms.Contains("0000042") && !sms.Contains("FAKE-MEMBER") && sms.Contains("Pending"), "SMS identifiers or status incorrect");
        customer.Type = (int)CustomerType.Corporation;
        customer.NonIndividualDescription = "Example & Sons";
        customer.NonIndividualRegistrationNumber = "REG-007";
        customer.Reference3 = " ";
        customer.RecordStatus = (byte)RecordStatus.Approved;
        body = CustomerRegistrationNotification.EmailBody(customer, branch, null);
        Check(body.Contains("Example &amp; Sons") && body.Contains("REG-007"), "Organisation identifiers missing");
        Check(!body.Contains("Payroll number") && !body.Contains("Personal file number"), "Inapplicable or blank identifier included");
        Check(body.Contains("Approved") && body.Contains("No account numbers are available yet") && !body.Contains("Accounts created"), "Empty accounts or approval incorrect");
        var source = File.ReadAllText(args[0]);
        Check(!source.Contains("MAX(Reference2)"), "Fabricated membership generator remains");
        Check(source.IndexOf("#region Effect Mandatory + Additional Debit Types") < source.IndexOf("#region Send Email Notification"), "Email precedes charge-created accounts");
        Check(source.IndexOf("#region Auto-Create Mandatory + Additional Accounts") < source.IndexOf("#region Send Text Notification"), "SMS precedes account creation");
        Console.WriteLine("PASS: persisted identifiers, multiple accounts, ownership, duplicates, HTML escaping, optional fields, organisations, verification, SMS and notification order.");
    }
}
