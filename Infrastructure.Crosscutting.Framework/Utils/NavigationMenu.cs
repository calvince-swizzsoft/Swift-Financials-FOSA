using System;
using System.Collections.Generic;

namespace Infrastructure.Crosscutting.Framework.Utils
{
    public class NavigationMenu
    {
        public Guid? ParentId { get; private set; }

        public bool IsArea { get; private set; }

        public string Description { get; private set; }

        public string Icon { get; private set; }

        public string ControllerName { get; private set; }

        public string ActionName { get; private set; }

        public string AreaName { get; private set; }

        public int Code { get; private set; }

        public int AreaCode { get; private set; }

        public List<NavigationMenu> GetMenus()
        {
            var menus = new List<NavigationMenu>()
            {
                #region Administration Module
                //Administration Module...
                new NavigationMenu { Description = "Administration", IsArea = true, Code = 0x00004E20 },
                new NavigationMenu { AreaCode = 0x00004E20, IsArea = true, Description = "Setup", Icon = "fa fa-bars", Code = 0x00004E20 + 1 },
                new NavigationMenu { AreaCode = 0x00004E20, IsArea = true, Description = "Operations", Icon = "fa fa-bars", Code = 0x00004E20 + 2 },

                // Setup
                new NavigationMenu { AreaCode = 0x00004E20 + 1, IsArea = false, Description = "Companies", Icon = "fa fa-university", ControllerName = "Company", ActionName = "Index", AreaName = "Admin",
                    Code = 0x00004E20 + 3 },
                new NavigationMenu { AreaCode = 0x00004E20 + 1, IsArea = false, Description = "Branches", Icon = "fa fa-list", ControllerName = "Branch", ActionName = "Index", AreaName = "Admin",
                    Code = 0x00004E20 + 4 },
                new NavigationMenu { AreaCode = 0x00004E20 + 1, IsArea = false, Description = "Banks", Icon = "fa fa-university", ControllerName = "Bank", ActionName = "Index", AreaName = "Admin",
                    Code = 0x00004E20 + 5 },
                new NavigationMenu { AreaCode = 0x00004E20 + 1, IsArea = false, Description = "Locations", Icon = "fa fa-map-marker", ControllerName = "Location", ActionName = "Index", AreaName = "Admin",
                    Code = 0x00004E20 + 6 },

                //Operations
                new NavigationMenu { AreaCode = 0x00004E20 + 2, IsArea = true, Description = "Security", Icon = "", Code = 0x00004E20 + 7 },
                new NavigationMenu { AreaCode = 0x00004E20 + 7, IsArea = false, Description = "Audit Logs", Icon = "fa fa-file-text-o", ControllerName = "AuditLogs", ActionName = "Index", AreaName = "Admin",
                    Code = 0x00004E20 + 8 },
                new NavigationMenu { AreaCode = 0x00004E20 + 7, IsArea = false, Description = "Roles", Icon = "fa fa-tags", ControllerName = "Role", ActionName = "Index", AreaName = "Admin",
                    Code = 0x00004E20 + 9 },
                new NavigationMenu { AreaCode = 0x00004E20 + 7, IsArea = false, Description = "Users", Icon = "fa fa-users", ControllerName = "Membership", ActionName = "Index", AreaName = "Admin",
                    Code = 0x00004E20 + 10 },

                new NavigationMenu { AreaCode = 0x00004E20 + 2, IsArea = true, Description = "Access Control List", Icon = "", Code = 0x00004E20 + 11 },
                new NavigationMenu { AreaCode = 0x00004E20 + 11, IsArea = false, Description = "Modules", Icon = "fa fa-cog", ControllerName = "Module", ActionName = "Index", AreaName = "Admin",
                    Code = 0x00004E20 + 12 },
                new NavigationMenu { AreaCode = 0x00004E20 + 11, IsArea = false, Description = "Operations", Icon = "fa fa-code-fork", ControllerName = "SystemTransactiontypes", ActionName = "Create", AreaName = "Admin",
                    Code = 0x00004E20 + 13 },
                new NavigationMenu { AreaCode = 0x00004E20 + 2, IsArea = false, Description = "Workflow", Icon = "fa fa-table", ControllerName = "Workflow", ActionName = "Create",
                    AreaName = "Admin", Code = 0x00004E20 + 14 },
                new NavigationMenu { AreaCode = 0x00004E20 + 2, IsArea = false, Description = "Report Setting", Icon = "fa fa-table", ControllerName = "SSRSReportSetting", ActionName = "Create",
                    AreaName = "Admin", Code = 0x00004E20 + 15 },
                #endregion

                #region Accounts Module
                // Accounts...
                new NavigationMenu{Description = "Accounts", IsArea = true, Code = 0x000059D8},
                new NavigationMenu { AreaCode = 0x000059D8, IsArea = true, Description = "Setup", Icon = "fa fa-bars", Code = 0x000059D8 + 1 },
                new NavigationMenu { AreaCode = 0x000059D8, IsArea = true, Description = "Operations", Icon = "fa fa-bars", Code = 0x000059D8 + 2 },

                // Setup
                new NavigationMenu { AreaCode = 0x000059D8 + 1, IsArea = true, Description = "G/L Accounts", Icon = "", Code = 0x000059D8 + 3 },
                new NavigationMenu { AreaCode = 0x000059D8 + 3, IsArea = false, Description = "Cost Centers", Icon = "fa fa-briefcase", ControllerName = "CostCenter", ActionName = "Index", AreaName = "Accounts",
                    Code = 0x000059D8 + 4 },
                new NavigationMenu { AreaCode = 0x000059D8 + 3, IsArea = false, Description = "Chart Of Accounts", Icon = "fa fa-address-book", ControllerName = "ChartOfAccount", ActionName = "Index", AreaName = "Accounts",
                    Code = 0x000059D8 + 5 },
                new NavigationMenu { AreaCode = 0x000059D8 + 3, IsArea = false, Description = "G/L Account Determination", Icon = "fa fa-cogs", ControllerName = "SystemGeneralLedgerAccountMapping", ActionName = "Index", AreaName = "Accounts",
                    Code = 0x000059D8 + 6 },

                new NavigationMenu { AreaCode = 0x000059D8 + 1, IsArea = true, Description = "Levies & Charges", Icon = "", Code = 0x000059D8 + 7 },
                new NavigationMenu { AreaCode = 0x000059D8 + 7, IsArea = false, Description = "Levies", Icon = "fa fa-money", ControllerName = "Levy", ActionName = "Index", AreaName = "Accounts",
                    Code = 0x000059D8 + 8 },
                new NavigationMenu { AreaCode = 0x000059D8 + 7, IsArea = false, Description = "Charges", Icon = "fa fa-money", ControllerName = "Charges", ActionName = "Index", AreaName = "Accounts",
                    Code = 0x000059D8 + 9 },

                new NavigationMenu { AreaCode = 0x000059D8 + 7, IsArea = true, Description = "Charge Determination", Icon = "", Code = 0x000059D8 + 10 },
                new NavigationMenu { AreaCode = 0x000059D8 + 10, IsArea = false, Description = "Well-Known Charges", Icon = "fa fa-money", ControllerName = "WellKnownCharges", ActionName = "Index", AreaName = "Accounts",
                    Code = 0x000059D8 + 11 },
                new NavigationMenu { AreaCode = 0x000059D8 + 10, IsArea = false, Description = "Indefinite Charges", Icon = "fa fa-money", ControllerName = "IndefiniteCharges", ActionName = "Index", AreaName = "Accounts",
                    Code = 0x000059D8 + 12 },
                new NavigationMenu { AreaCode = 0x000059D8 + 10, IsArea = false, Description = "Text Alert Charges", Icon = "fa fa-money", ControllerName = "TextAlertCharges", ActionName = "Index", AreaName = "Accounts",
                    Code = 0x000059D8 + 13 },
                new NavigationMenu { AreaCode = 0x000059D8 + 10, IsArea = false, Description = "Alternate Channels", Icon = "fa fa-money", ControllerName = "AlternateChannels", ActionName = "Index", AreaName = "Accounts",
                    Code = 0x000059D8 + 14 },

                new NavigationMenu { AreaCode = 0x000059D8 + 1, IsArea = true, Description = "Financial Products", Icon = "", Code = 0x000059D8 + 15 },
                new NavigationMenu { AreaCode = 0x000059D8 + 15, IsArea = false, Description = "Savings", Icon = "fa fa-money", ControllerName = "SavingsProduct", ActionName = "Index", AreaName = "Accounts",
                    Code = 0x000059D8 + 16 },
                new NavigationMenu { AreaCode = 0x000059D8 + 15, IsArea = false, Description = "Investments", Icon = "fa fa-car", ControllerName = "InvestmentProduct", ActionName = "Index", AreaName = "Accounts",
                    Code = 0x000059D8 + 17 },
                new NavigationMenu { AreaCode = 0x000059D8 + 15, IsArea = false, Description = "Loans", Icon = "fa fa-handshake-o", ControllerName = "LoanProduct", ActionName = "Index", AreaName = "Accounts",
                    Code = 0x000059D8 + 18 },

                new NavigationMenu { AreaCode = 0x000059D8 + 1, IsArea = false, Description = "Posting Periods", Icon = "fa fa-calendar-check-o", ControllerName = "PostingPeriod", ActionName = "Index", AreaName = "Accounts",
                    Code = 0x000059D8 + 19 },
                new NavigationMenu { AreaCode = 0x000059D8 + 1, IsArea = false, Description = "Treasuries", Icon = "fa fa-users", ControllerName = "Treasuries", ActionName = "Index", AreaName = "Accounts",
                    Code = 0x000059D8 + 20 },
                new NavigationMenu { AreaCode = 0x000059D8 + 1, IsArea = false, Description = "Tellers", Icon = "fa fa-users", ControllerName = "Teller", ActionName = "Index", AreaName = "FrontOffice",
                    Code = 0x000059D8 + 21 },
                new NavigationMenu { AreaCode = 0x000059D8 + 1, IsArea = false, Description = "Bank Linkages", Icon = "fa fa-university", ControllerName = "BankLinkage", ActionName = "Index", AreaName = "Accounts",
                    Code = 0x000059D8 + 22 },
                new NavigationMenu { AreaCode = 0x000059D8 + 1, IsArea = false, Description = "Cheque Types", Icon = "fa fa-envelope-open-o", ControllerName = "ChequeType", ActionName = "Index", AreaName = "Accounts",
                    Code = 0x000059D8 + 23 },
                new NavigationMenu { AreaCode = 0x000059D8 + 1, IsArea = false, Description = "Direct Debits", Icon = "fa fa-money", ControllerName = "DirectDebit", ActionName = "Index", AreaName = "Accounts",
                    Code = 0x000059D8 + 24 },
                new NavigationMenu { AreaCode = 0x000059D8 + 1, IsArea = false, Description = "Credit Types", Icon = "fa fa-money", ControllerName = "CreditType", ActionName = "Index", AreaName = "Accounts",
                    Code = 0x000059D8 + 25 },
                new NavigationMenu { AreaCode = 0x000059D8 + 1, IsArea = false, Description = "Debit Types", Icon = "fa fa-money", ControllerName = "DebitType", ActionName = "Index", AreaName = "Accounts",
                    Code = 0x000059D8 + 26 },
                new NavigationMenu { AreaCode = 0x000059D8 + 1, IsArea = false, Description = "Insurers", Icon = "fa fa-building", ControllerName = "Insurers", ActionName = "Index", AreaName = "Accounts",
                    Code = 0x000059D8 + 27 },
                new NavigationMenu { AreaCode = 0x000059D8 + 1, IsArea = false, Description = "Unpay Reasons", Icon = "fa fa-hand-paper-o", ControllerName = "UnpayReason", ActionName = "Index", AreaName = "Accounts",
                    Code = 0x000059D8 + 28 },
                new NavigationMenu { AreaCode = 0x000059D8 + 1, IsArea = false, Description = "Fixed Deposit Types", Icon = "fa fa-money", ControllerName = "FixedDepositType", ActionName = "Index", AreaName = "Accounts",
                    Code = 0x000059D8 + 29 },
                new NavigationMenu { AreaCode = 0x000059D8 + 1, IsArea = false, Description = "Wire Transfer Types", Icon = "fa fa-money", ControllerName = "WireTransferType", ActionName = "Index", AreaName = "Accounts",
                    Code = 0x000059D8 + 30 },


                // Operations
                //Transactions Journal
                new NavigationMenu { AreaCode = 0x000059D8 + 2, IsArea = true, Description = "Transactions Journal", Icon = "", Code = 0x000059D8 + 31 },
                new NavigationMenu { AreaCode = 0x000059D8 + 31, IsArea = false, Description = "System", Icon = "fa fa-cog", ControllerName = "TransactionJournals", ActionName = "Index", AreaName = "Accounts",
                    Code = 0x000059D8 + 32 },
                new NavigationMenu { AreaCode = 0x000059D8 + 31, IsArea = false, Description = "G/L Account", Icon = "fa fa-address-book", ControllerName = "TransactionJournals", ActionName = "Index", AreaName = "Accounts",
                    Code = 0x000059D8 + 33 },
                new NavigationMenu { AreaCode = 0x000059D8 + 31, IsArea = false, Description = "Posting Period Closing", Icon = "fa fa-calendar-times-o", ControllerName = "ClosingPostingPeriod", ActionName = "Index", AreaName = "Accounts",
                    Code = 0x000059D8 + 34 },

                //Recurring Procedures
                new NavigationMenu { AreaCode = 0x000059D8 + 2, IsArea = true, Description = "Recurring Procedures", Icon = "", Code = 0x000059D8 + 35 },
                new NavigationMenu { AreaCode = 0x000059D8 + 35, IsArea = false, Description = "Loan Indefinite Charges", Icon = "fa fa-money", ControllerName = "LoanIndefiniteCharges", ActionName = "Index", AreaName = "Accounts",
                    Code = 0x000059D8 + 36 },
                new NavigationMenu { AreaCode = 0x000059D8 + 35, IsArea = false, Description = "Savings Dynamic Fees", Icon = "fa fa-money", ControllerName = "SavingsDynamicFees", ActionName = "Index", AreaName = "Accounts",
                    Code = 0x000059D8 + 37 },
                new NavigationMenu { AreaCode = 0x000059D8 + 35, IsArea = false, Description = "Loan Interest Capitalization", Icon = "fa fa-money", ControllerName = "LoanInterestCapitalization", ActionName = "Index", AreaName = "Accounts",
                    Code = 0x000059D8 + 38 },
                new NavigationMenu { AreaCode = 0x000059D8 + 35, IsArea = false, Description = "Standing Order Execution", Icon = "fa fa-tasks", ControllerName = "SatndingOrderExecution", ActionName = "Index", AreaName = "Accounts",
                    Code = 0x000059D8 + 39 },
                //...Discrepancy Matching
                new NavigationMenu { AreaCode = 0x000059D8 + 35, IsArea = true, Description = "Discrepancy Matching", Icon = "", Code = 0x000059D8 + 40 },
                new NavigationMenu { AreaCode = 0x000059D8 + 40, IsArea = false, Description = "Matching Singly", Icon = "fa fa-cog", ControllerName = "Controller", ActionName = "Index", AreaName = "Accounts",
                    Code = 0x000059D8 + 41 },
                new NavigationMenu { AreaCode = 0x000059D8 + 40, IsArea = false, Description = "Matching Jointly", Icon = "fa fa-cogs", ControllerName = "Controller", ActionName = "Index", AreaName = "Accounts",
                    Code = 0x000059D8 + 42 },

                //Customer Accounts
                new NavigationMenu { AreaCode = 0x000059D8 + 2, IsArea = true, Description = "Customer Accounts", Icon = "", Code = 0x000059D8 + 43 },
                new NavigationMenu { AreaCode = 0x000059D8 + 43, IsArea = false, Description = "Register", Icon = "fa fa-clipboard", ControllerName = "CustomerAccounts", ActionName = "Index", AreaName = "Accounts",
                    Code = 0x000059D8 + 44 },
                new NavigationMenu { AreaCode = 0x000059D8 + 43, IsArea = false, Description = "Management", Icon = "fa fa-cogs", ControllerName = "CoA_Management", ActionName = "Index", AreaName = "Accounts",
                    Code = 0x000059D8 + 45 },
                new NavigationMenu { AreaCode = 0x000059D8 + 43, IsArea = false, Description = "Signatories", Icon = "fa fa-users", ControllerName = "CoA_Signatories", ActionName = "Index", AreaName = "Accounts",
                    Code = 0x000059D8 + 46 },
                new NavigationMenu { AreaCode = 0x000059D8 + 43, IsArea = false, Description = "Cheque Books", Icon = "fa fa-envelope-square", ControllerName = "CoA_ChequeBooks", ActionName = "Index", AreaName = "Accounts",
                    Code = 0x000059D8 + 47 },
                new NavigationMenu { AreaCode = 0x000059D8 + 43, IsArea = false, Description = "Standing Orders", Icon = "fa fa-tasks", ControllerName = "CustomerAccountStandingOrder", ActionName = "Index", AreaName = "Accounts",
                    Code = 0x000059D8 + 48 },
                new NavigationMenu { AreaCode = 0x000059D8 + 43, IsArea = false, Description = "Intra Account Transfer", Icon = "fa fa-exchange", ControllerName = "IntraAccountTransfer", ActionName = "Index", AreaName = "Accounts",
                    Code = 0x000059D8 + 49 },
                new NavigationMenu { AreaCode = 0x000059D8 + 43, IsArea = false, Description = "Mobile To Bank", Icon = "fa fa-exchange", ControllerName = "MobileToBank", ActionName = "Index", AreaName = "Accounts",
                    Code = 0x000059D8 + 50 },
                new NavigationMenu { AreaCode = 0x000059D8 + 43, IsArea = false, Description = "E-Statements", Icon = "fa fa-envelope-o", ControllerName = "eStatements", ActionName = "Index", AreaName = "Accounts",
                    Code = 0x000059D8 + 51 },

                //Alternate Channels
                new NavigationMenu { AreaCode = 0x000059D8 + 2, IsArea = true, Description = "Alternate Channels", Icon = "", Code = 0x000059D8 + 52 },
                new NavigationMenu { AreaCode = 0x000059D8 + 52, IsArea = false, Description = "Register", Icon = "fa fa-clipboard", ControllerName = "Register", ActionName = "Index", AreaName = "Accounts",
                    Code = 0x000059D8 + 53 },
                new NavigationMenu { AreaCode = 0x000059D8 + 52, IsArea = false, Description = "Management", Icon = "fa fa-cogs", ControllerName = "AlternatechannelManagement", ActionName = "Index", AreaName = "Accounts",
                    Code = 0x000059D8 + 54 },
                 //...Reconciliation
                new NavigationMenu { AreaCode = 0x000059D8 + 52, IsArea = true, Description = "Alternate Channel Reconciliation", Icon = "", Code = 0x000059D8 + 55 },
                new NavigationMenu { AreaCode = 0x000059D8 + 55, IsArea = false, Description = "Periods", Icon = "fa fa-calendar-o", ControllerName = "AlternatePeriods", ActionName = "Create", AreaName = "Accounts",
                    Code = 0x000059D8 + 56 },
                new NavigationMenu { AreaCode = 0x000059D8 + 55, IsArea = false, Description = "Processing", Icon = "fa fa-cogs", ControllerName = "AlternatePeriods", ActionName = "Processing", AreaName = "Accounts",
                    Code = 0x000059D8 + 57 },
                new NavigationMenu { AreaCode = 0x000059D8 + 55, IsArea = false, Description = "Closing", Icon = "fa fa-calendar-times-o", ControllerName = "AlternatePeriods", ActionName = "Clos", AreaName = "Accounts",
                    Code = 0x000059D8 + 58 },
                new NavigationMenu { AreaCode = 0x000059D8 + 55, IsArea = false, Description = "Catalogue", Icon = "fa fa-table", ControllerName = "AlternatePeriods", ActionName = "Index", AreaName = "Accounts",
                    Code = 0x000059D8 + 59 },


                //Bank Reconciliation
                new NavigationMenu { AreaCode = 0x000059D8 + 2, IsArea = true, Description = "Bank Reconciliation", Icon = "", Code = 0x000059D8 + 60 },
                new NavigationMenu { AreaCode = 0x000059D8 + 60, IsArea = false, Description = "Periods", Icon = "fa fa-calendar-o", ControllerName = "BankReconciliation_Periods", ActionName = "Index", AreaName = "Accounts",
                    Code = 0x000059D8 + 61 },
                new NavigationMenu { AreaCode = 0x000059D8 + 60, IsArea = false, Description = "Processing", Icon = "fa fa-cogs", ControllerName = "BankReconciliation_Periods", ActionName = "Processing", AreaName = "Accounts",
                    Code = 0x000059D8 + 62 },
                new NavigationMenu { AreaCode = 0x000059D8 + 60, IsArea = false, Description = "Closing", Icon = "fa fa-calendar-times-o", ControllerName = "BankReconciliation_Periods", ActionName = "Closing", AreaName = "Accounts",
                    Code = 0x000059D8 + 63 },
                new NavigationMenu { AreaCode = 0x000059D8 + 60, IsArea = false, Description = "Catalogue", Icon = "fa fa-table", ControllerName = "BankReconciliation_Periods", ActionName = "Index", AreaName = "Accounts",
                    Code = 0x000059D8 + 64 },

                //Budget Management
                new NavigationMenu { AreaCode = 0x000059D8 + 2, IsArea = true, Description = "Budget Management", Icon = "", Code = 0x000059D8 + 65 },
                new NavigationMenu { AreaCode = 0x000059D8 + 65, IsArea = false, Description = "Periods", Icon = "fa fa-calendar-o", ControllerName = "Budget", ActionName = "Index", AreaName = "Accounts",
                    Code = 0x000059D8 + 66 },
                new NavigationMenu { AreaCode = 0x000059D8 + 65, IsArea = false, Description = "Appropriation", Icon = "fa fa-table", ControllerName = "Budget", ActionName = "Create", AreaName = "Accounts",
                    Code = 0x000059D8 + 67 },

                 //Batch Procedures
                new NavigationMenu { AreaCode = 0x000059D8 + 2, IsArea = true, Description = "Batch Procedures", Icon = "", Code = 0x000059D8 + 68 },

                //... Batch Origination
                new NavigationMenu { AreaCode = 0x000059D8 + 68, IsArea = true, Description = "Batch Origination", Icon = "", Code = 0x000059D8 + 69 },
                new NavigationMenu { AreaCode = 0x000059D8 + 69, IsArea = false, Description = "Voucher", Icon = "fa fa-calendar-o", ControllerName = "BatchOrigination_Voucher", ActionName = "Index", AreaName = "Accounts",
                    Code = 0x000059D8 + 70},
                new NavigationMenu { AreaCode = 0x000059D8 + 69, IsArea = false, Description = "Credit", Icon = "fa fa-money", ControllerName = "BatchOrigination_Credit", ActionName = "Index", AreaName = "Accounts",
                    Code = 0x000059D8 + 71 },
                new NavigationMenu { AreaCode = 0x000059D8 + 69, IsArea = false, Description = "Debit", Icon = "fa fa-money", ControllerName = "BatchOrigination_Debit", ActionName = "Index", AreaName = "Accounts",
                    Code = 0x000059D8 + 72},
                new NavigationMenu { AreaCode = 0x000059D8 + 69, IsArea = false, Description = "Refund", Icon = "fa fa-exchange", ControllerName = "BatchOrigination_Refund", ActionName = "Index", AreaName = "Accounts",
                    Code = 0x000059D8 + 73},
                new NavigationMenu { AreaCode = 0x000059D8 + 69, IsArea = false, Description = "Wire Transfer", Icon = "fa fa-exchange", ControllerName = "BatchOrigination_WireTransfer", ActionName = "Index", AreaName = "Accounts",
                    Code = 0x000059D8 + 74},
                new NavigationMenu { AreaCode = 0x000059D8 + 69, IsArea = false, Description = "Disbursement", Icon = "fa fa-share-square-o", ControllerName = "BatchOrigination_Disbursement", ActionName = "Index", AreaName = "Accounts",
                    Code = 0x000059D8 + 75},
                new NavigationMenu { AreaCode = 0x000059D8 + 69, IsArea = false, Description = "Reversal", Icon = "fa fa-exchange", ControllerName = "BatchOrigination_Reversal", ActionName = "Index", AreaName = "Accounts",
                    Code = 0x000059D8 + 76},
                new NavigationMenu { AreaCode = 0x000059D8 + 69, IsArea = false, Description = "Inter Account Transfer", Icon = "fa fa-exchange", ControllerName = "InterAccountTransfer", ActionName = "Index", AreaName = "Accounts",
                    Code = 0x000059D8 + 77},
                new NavigationMenu { AreaCode = 0x000059D8 + 69, IsArea = false, Description = "General Ledger", Icon = "fa fa-book", ControllerName = "AddGeneralLedger", ActionName = "Index", AreaName = "Accounts",
                    Code = 0x000059D8 + 78},

                 //... Batch Verification
                new NavigationMenu { AreaCode = 0x000059D8 + 68, IsArea = true, Description = "Batch Verification", Icon = "", Code = 0x000059D8 + 79 },
                new NavigationMenu { AreaCode = 0x000059D8 + 79, IsArea = false, Description = "Voucher", Icon = "fa fa-calendar-o", ControllerName = "BatchVerification_Voucher", ActionName = "Index", AreaName = "Accounts",
                    Code = 0x000059D8 + 80},
                new NavigationMenu { AreaCode = 0x000059D8 + 79, IsArea = false, Description = "Credit", Icon = "fa fa-money", ControllerName = "BatchVerification_Credit", ActionName = "Index", AreaName = "Accounts",
                    Code = 0x000059D8 + 81 },
                new NavigationMenu { AreaCode = 0x000059D8 + 79, IsArea = false, Description = "Debit", Icon = "fa fa-money", ControllerName = "BatchVerification_Debit", ActionName = "Index", AreaName = "Accounts",
                    Code = 0x000059D8 + 82},
                new NavigationMenu { AreaCode = 0x000059D8 + 79, IsArea = false, Description = "Refund", Icon = "fa fa-exchange", ControllerName = "BatchVerification_Refund", ActionName = "Index", AreaName = "Accounts",
                    Code = 0x000059D8 + 83},
                new NavigationMenu { AreaCode = 0x000059D8 + 79, IsArea = false, Description = "Wire Transfer", Icon = "fa fa-exchange", ControllerName = "BatchVerification_WireTransfer", ActionName = "Index", AreaName = "Accounts",
                    Code = 0x000059D8 + 84},
                new NavigationMenu { AreaCode = 0x000059D8 + 79, IsArea = false, Description = "Disbursement", Icon = "fa fa-share-square-o", ControllerName = "BatchVerification_Disbursement", ActionName = "Index", AreaName = "Accounts",
                    Code = 0x000059D8 + 85},
                new NavigationMenu { AreaCode = 0x000059D8 + 79, IsArea = false, Description = "Reversal", Icon = "fa fa-exchange", ControllerName = "BatchVerification_Reversal", ActionName = "Index", AreaName = "Accounts",
                    Code = 0x000059D8 + 86},
                new NavigationMenu { AreaCode = 0x000059D8 + 79, IsArea = false, Description = "Inter Account Transfer", Icon = "fa fa-exchange", ControllerName = "BatchVerification_InterAccountTransfer", ActionName = "Index", AreaName = "Accounts",
                    Code = 0x000059D8 + 87},
                new NavigationMenu { AreaCode = 0x000059D8 + 79, IsArea = false, Description = "General Ledger", Icon = "fa fa-book", ControllerName = "BatchVerification_GeneralLedger", ActionName = "Index", AreaName = "Accounts",
                    Code = 0x000059D8 + 88},

                 //... Batch Authorization
                new NavigationMenu { AreaCode = 0x000059D8 + 68, IsArea = true, Description = "Batch Authorization", Icon = "", Code = 0x000059D8 + 89 },
                new NavigationMenu { AreaCode = 0x000059D8 + 89, IsArea = false, Description = "Voucher", Icon = "fa fa-calendar-o", ControllerName = "BatchAuthorization_Voucher", ActionName = "Index", AreaName = "Accounts",
                    Code = 0x000059D8 + 90},
                new NavigationMenu { AreaCode = 0x000059D8 + 89, IsArea = false, Description = "Credit", Icon = "fa fa-money", ControllerName = "BatchAuthorization_Credit", ActionName = "Index", AreaName = "Accounts",
                    Code = 0x000059D8 + 91 },
                new NavigationMenu { AreaCode = 0x000059D8 + 89, IsArea = false, Description = "Debit", Icon = "fa fa-money", ControllerName = "BatchAuthorization_Debit", ActionName = "Index", AreaName = "Accounts",
                    Code = 0x000059D8 + 92},
                new NavigationMenu { AreaCode = 0x000059D8 + 89, IsArea = false, Description = "Refund", Icon = "fa fa-exchange", ControllerName = "BatchAuthorization_Refund", ActionName = "Index", AreaName = "Accounts",
                    Code = 0x000059D8 + 93},
                new NavigationMenu { AreaCode = 0x000059D8 + 89, IsArea = false, Description = "Wire Transfer", Icon = "fa fa-exchange", ControllerName = "BatchAuthorization_WireTransfer", ActionName = "Index", AreaName = "Accounts",
                    Code = 0x000059D8 + 94},
                new NavigationMenu { AreaCode = 0x000059D8 + 89, IsArea = false, Description = "Disbursement", Icon = "fa fa-share-square-o", ControllerName = "BatchAuthorization_Disbursement", ActionName = "Index", AreaName = "Accounts",
                    Code = 0x000059D8 + 95},
                new NavigationMenu { AreaCode = 0x000059D8 + 89, IsArea = false, Description = "Reversal", Icon = "fa fa-exchange", ControllerName = "BatchAuthorization_Reversal", ActionName = "Index", AreaName = "Accounts",
                    Code = 0x000059D8 + 96},
                new NavigationMenu { AreaCode = 0x000059D8 + 89, IsArea = false, Description = "Inter Account Transfer", Icon = "fa fa-exchange", ControllerName = "BatchAuthorization_InterAccountTransfer", ActionName = "Index", AreaName = "Accounts",
                    Code = 0x000059D8 + 97},
                new NavigationMenu { AreaCode = 0x000059D8 + 89, IsArea = false, Description = "General Ledger", Icon = "fa fa-book", ControllerName = "BatchAuthorization_GeneralLedger", ActionName = "Index", AreaName = "Accounts",
                    Code = 0x000059D8 + 98},
                #endregion

                #region Loan Origination Module
                //Loan Origination Module...
                new NavigationMenu { Description = "Back Office", IsArea = true, Code = 0x00011170 },
                new NavigationMenu { AreaCode = 0x00011170, IsArea = true, Description = "Setup", Icon = "fa fa-bars", Code = 0x00011170 + 1 },
                new NavigationMenu { AreaCode = 0x00011170, IsArea = true, Description = "Operations", Icon = "fa fa-bars", Code = 0x00011170 + 2 },

                //... Setup
                new NavigationMenu { AreaCode = 0x00011170 + 1, IsArea = false, Description = "Loan Purpose", Icon = "fa fa-question-circle-o", ControllerName = "LoanPurpose", ActionName = "Index", AreaName = "Loaning",
                    Code = 0x00011170 + 3 },
                new NavigationMenu { AreaCode = 0x00011170 + 1, IsArea = false, Description = "Loaning Remarks", Icon = "fa fa-commenting-o", ControllerName = "LoaningRemark", ActionName = "Index", AreaName = "Loaning",
                    Code = 0x00011170 + 4 },
                new NavigationMenu { AreaCode = 0x00011170 + 1, IsArea = false, Description = "Income Adjustments", Icon = "fa fa-adjust", ControllerName = "IncomeAdjustments", ActionName = "Index", AreaName = "Loaning",
                    Code = 0x00011170 + 5 },

                //... Operations
                //..Application
                new NavigationMenu { AreaCode = 0x00011170 + 2, IsArea = true, Description = "Application", Icon = "", Code = 0x00011170 + 6 },
                new NavigationMenu { AreaCode = 0x00011170 + 6, IsArea = false, Description = "Registration", Icon = "fa fa-money", ControllerName = "LoanRegistration", ActionName = "Index", AreaName = "Loaning",
                    Code = 0x00011170 + 7 },
                new NavigationMenu { AreaCode = 0x00011170 + 6, IsArea = false, Description = "Appraisal", Icon = "fa fa-money", ControllerName = "AppraiseLoan", ActionName = "Index", AreaName = "Loaning",
                    Code = 0x00011170 + 8 },
                new NavigationMenu { AreaCode = 0x00011170 + 6, IsArea = false, Description = "Approval", Icon = "fa fa-money", ControllerName = "ApproveLoan", ActionName = "Index", AreaName = "Loaning",
                    Code = 0x00011170 + 9 },
                new NavigationMenu { AreaCode = 0x00011170 + 6, IsArea = false, Description = "Verification", Icon = "fa fa-money", ControllerName = "LoanVerification", ActionName = "Index", AreaName = "Loaning",
                    Code = 0x00011170 + 10 },
                new NavigationMenu { AreaCode = 0x00011170 + 6, IsArea = false, Description = "Cancellation", Icon = "fa fa-money", ControllerName = "LoanCancellation", ActionName = "Index", AreaName = "Loaning",
                    Code = 0x00011170 + 11 },

                //..Loaning
                new NavigationMenu { AreaCode = 0x00011170 + 2, IsArea = true, Description = "Loaning", Icon = "", Code = 0x00011170 + 12 },
                new NavigationMenu { AreaCode = 0x00011170 + 12, IsArea = false, Description = "Restructuring", Icon = "fa fa-adjust", ControllerName = "LoanRestructuring", ActionName = "Create", AreaName = "Loaning",
                    Code = 0x00011170 + 13 },
                new NavigationMenu { AreaCode = 0x00011170 + 12, IsArea = false, Description = "Guarantor Attachment", Icon = "fa fa-users", ControllerName = "GuarantorAttachment", ActionName = "Create", AreaName = "Loaning",
                    Code = 0x00011170 + 14 },
                new NavigationMenu { AreaCode = 0x00011170 + 12, IsArea = false, Description = "Guarantor Substitution", Icon = "fa fa-users", ControllerName = "GuarantorSubstitution", ActionName = "Create", AreaName = "Loaning",
                    Code = 0x00011170 + 15 },
                new NavigationMenu { AreaCode = 0x00011170 + 12, IsArea = false, Description = "Guarantor Relieving", Icon = "fa fa-users", ControllerName = "GuarantorRelieving", ActionName = "Index", AreaName = "Loaning",
                    Code = 0x00011170 + 16 },
                new NavigationMenu { AreaCode = 0x00011170 + 12, IsArea = false, Description = "Guarantor Management", Icon = "fa fa-users", ControllerName = "GuarantorManagement", ActionName = "Create", AreaName = "Loaning",
                    Code = 0x00011170 + 17 },

                //..Data Capture
                new NavigationMenu { AreaCode = 0x00011170 + 2, IsArea = true, Description = "Data Capture", Icon = "", Code = 0x00011170 + 18 },
                new NavigationMenu { AreaCode = 0x00011170 + 18, IsArea = false, Description = "Data Periods", Icon = "fa fa-cogs", ControllerName = "GuarantorManagement", ActionName = "Create", AreaName = "Loaning",
                    Code = 0x00011170 + 19 },
                new NavigationMenu { AreaCode = 0x00011170 + 18, IsArea = false, Description = "Data Processing", Icon = "fa fa-cogs", ControllerName = "GuarantorManagement", ActionName = "Create", AreaName = "Loaning",
                    Code = 0x00011170 + 20 },
                new NavigationMenu { AreaCode = 0x00011170 + 18, IsArea = false, Description = "Closing", Icon = "fa fa-cogs", ControllerName = "GuarantorManagement", ActionName = "Create", AreaName = "Loaning",
                    Code = 0x00011170 + 21 },
                new NavigationMenu { AreaCode = 0x00011170 + 18, IsArea = false, Description = "Catalogue", Icon = "fa fa-cogs", ControllerName = "GuarantorManagement", ActionName = "Create", AreaName = "Loaning",
                    Code = 0x00011170 + 22 },
                #endregion

                #region Registry Module
                // Registry Module...
                new NavigationMenu { Description = "Registry", IsArea = true, Code = 0x00005208 },
                new NavigationMenu { AreaCode = 0x00005208, IsArea = true, Description = "Setup", Icon = "fa fa-bars", Code = 0x00005208 + 1 },
                new NavigationMenu { AreaCode = 0x00005208, IsArea = true, Description = "Operations", Icon = "fa fa-bars", Code = 0x00005208 + 2 },

                //Setup
                new NavigationMenu { AreaCode = 0x00005208 + 1, IsArea = false, Description = "Employers", Icon = "fa fa-users", ControllerName = "Employer", ActionName = "Index", AreaName = "Registry",
                    Code = 0x00005208 + 3},
                new NavigationMenu { AreaCode = 0x00005208 + 1, IsArea = false, Description = "Zones", Icon = "fa fa-map-pin", ControllerName = "Zone", ActionName = "Index", AreaName = "Registry",
                    Code =  0x00005208 + 4},

                //Operations
                //..Customers
                new NavigationMenu { AreaCode = 0x00005208 + 2, IsArea = true, Description = "Customers", Icon = "", Code = 0x00005208 + 5 },
                new NavigationMenu { AreaCode = 0x00005208 + 5, IsArea = false, Description = "Documents", Icon = "fa fa-file-o", ControllerName = "Document", ActionName = "Index", AreaName = "Registry",
                    Code =  0x00005208 + 6},
                new NavigationMenu { AreaCode = 0x00005208 + 5, IsArea = false, Description = "Register", Icon = "fa fa-clipboard", ControllerName = "Customer", ActionName = "Index", AreaName = "Registry",
                    Code =  0x00005208 + 7},
                new NavigationMenu { AreaCode = 0x00005208 + 5, IsArea = false, Description = "Next-Of-Kin", Icon = "fa fa-users", ControllerName = "NextOfKin", ActionName = "Index", AreaName = "Registry",
                    Code =  0x00005208 + 8},
                new NavigationMenu { AreaCode = 0x00005208 + 5, IsArea = false, Description = "Account Alerts", Icon = "fa fa-bell-o", ControllerName = "Controller", ActionName = "Index", AreaName = "Registry",
                    Code =  0x00005208 + 9},
                new NavigationMenu { AreaCode = 0x00005208 + 5, IsArea = false, Description = "Charges Exemptions", Icon = "fa fa-money", ControllerName = "ChargeExemptions", ActionName = "Index", AreaName = "Registry",
                    Code =  0x00005208 + 10},
                new NavigationMenu { AreaCode = 0x00005208 + 5, IsArea = false, Description = "Delegates", Icon = "fa fa-users", ControllerName = "Delegate", ActionName = "Index", AreaName = "Registry",
                    Code =  0x00005208 + 11},
                new NavigationMenu { AreaCode = 0x00005208 + 5, IsArea = false, Description = "Directors", Icon = "fa fa-users", ControllerName = "Director", ActionName = "Index", AreaName = "Registry",
                    Code =  0x00005208 + 12},
                new NavigationMenu { AreaCode = 0x00005208 + 5, IsArea = false, Description = "Station Linkage", Icon = "fa fa-building-o", ControllerName = "Station", ActionName = "Index", AreaName = "Registry",
                    Code =  0x00005208 + 13},
                new NavigationMenu { AreaCode = 0x00005208 + 5, IsArea = false, Description = "Branch Linkage", Icon = "fa fa-building-o", ControllerName = "BranchLinkage", ActionName = "Index", AreaName = "Registry",
                    Code =  0x00005208 + 14},
                new NavigationMenu { AreaCode = 0x00005208 + 5, IsArea = false, Description = "Conditional Lendging", Icon = "fa fa-handshake-o", ControllerName = "ConditionalLending", ActionName = "Index", AreaName = "Registry",
                    Code =  0x00005208 + 15},

                //..File Tracking
                new NavigationMenu { AreaCode = 0x00005208 + 2, IsArea = true, Description = "File Tracking", Icon = "", Code = 0x00005208 + 16 },
                //..Dispatch
                new NavigationMenu { AreaCode = 0x00005208 + 16, IsArea = true, Description = "Dispatch", Icon = "", Code = 0x00005208 + 17 },
                new NavigationMenu { AreaCode = 0x00005208 + 17, IsArea = false, Description = "Multi-Destination", Icon = "fa fa-plane", ControllerName = "Controller", ActionName = "Index", AreaName = "Registry",
                    Code =  0x00005208 + 18},
                new NavigationMenu { AreaCode = 0x00005208 + 17, IsArea = false, Description = "Single-Destination", Icon = "fa fa-plane", ControllerName = "Controller", ActionName = "Index", AreaName = "Registry",
                    Code =  0x00005208 + 19},

                new NavigationMenu { AreaCode = 0x00005208 + 16, IsArea = false, Description = "Receive", Icon = "fa fa-cogs", ControllerName = "Controller", ActionName = "Index", AreaName = "Registry",
                    Code =  0x00005208 + 20},
                new NavigationMenu { AreaCode = 0x00005208 + 16, IsArea = false, Description = "Recall", Icon = "fa fa-cogs", ControllerName = "Controller", ActionName = "Index", AreaName = "Registry",
                    Code =  0x00005208 + 21},
                new NavigationMenu { AreaCode = 0x00005208 + 16, IsArea = false, Description = "Catalogue", Icon = "fa fa-cogs", ControllerName = "Controller", ActionName = "Index", AreaName = "Registry",
                    Code =  0x00005208 + 22},

                //..Membership Termination
                new NavigationMenu { AreaCode = 0x00005208 + 2, IsArea = true, Description = "Membership Termination", Icon = "", Code = 0x00005208 + 23 },
                new NavigationMenu { AreaCode = 0x00005208 + 23, IsArea = false, Description = "Registration", Icon = "fa fa-clipboard", ControllerName = "Controller", ActionName = "Index", AreaName = "Registry",
                    Code =  0x00005208 + 24},
                new NavigationMenu { AreaCode = 0x00005208 + 23, IsArea = false, Description = "Approval", Icon = "fa fa-thumbs-o-up", ControllerName = "Controller", ActionName = "Index", AreaName = "Registry",
                    Code =  0x00005208 + 25},
                new NavigationMenu { AreaCode = 0x00005208 + 23, IsArea = false, Description = "Verification", Icon = "fa fa-check-square-o", ControllerName = "Controller", ActionName = "Index", AreaName = "Registry",
                    Code =  0x00005208 + 26},
                new NavigationMenu { AreaCode = 0x00005208 + 23, IsArea = false, Description = "Settlement", Icon = "fa fa-cogs", ControllerName = "Controller", ActionName = "Index", AreaName = "Registry",
                    Code =  0x00005208 + 27},
                new NavigationMenu { AreaCode = 0x00005208 + 23, IsArea = false, Description = "Death Claim", Icon = "fa fa-stop", ControllerName = "Controller", ActionName = "Index", AreaName = "Registry",
                    Code =  0x00005208 + 28},

                //..Education
                new NavigationMenu { AreaCode = 0x00005208 + 2, IsArea = true, Description = "Education", Icon = "", Code = 0x00005208 + 29 },
                new NavigationMenu { AreaCode = 0x00005208 + 29, IsArea = false, Description = "Venues", Icon = "fa fa-home", ControllerName = "EducationVenue", ActionName = "Index", AreaName = "Registry",
                    Code =  0x00005208 + 30},
                new NavigationMenu { AreaCode = 0x00005208 + 29, IsArea = false, Description = "Registration", Icon = "fa fa-clipboard", ControllerName = "Controller", ActionName = "Index", AreaName = "Registry",
                    Code =  0x00005208 + 31},
                #endregion

                #region Human Resource Module
                // Human Resource Module...
                new NavigationMenu { Description = "Human Resource", IsArea = true, Code = 0x000055F0 },
                new NavigationMenu { AreaCode = 0x000055F0, IsArea = true, Description = "Setup", Icon = "fa fa-bars", Code = 0x000055F0 + 1 },
                new NavigationMenu { AreaCode = 0x000055F0, IsArea = true, Description = "Operations", Icon = "fa fa-bars", Code = 0x000055F0 + 2 },

                //Setup
                new NavigationMenu { AreaCode = 0x000055F0 + 1, IsArea = false, Description = "Departments", Icon = "fa fa-building", ControllerName = "Department", ActionName = "Index", AreaName = "HumanResource",
                    Code =  0x000055F0 + 3},
                new NavigationMenu { AreaCode = 0x000055F0 + 1, IsArea = false, Description = "Designations", Icon = "fa fa-briefcase", ControllerName = "Designation", ActionName = "Index", AreaName = "HumanResource",
                    Code =  0x000055F0 + 4},
                new NavigationMenu { AreaCode = 0x000055F0 + 1, IsArea = false, Description = "Holidays", Icon = "fa fa-tree", ControllerName = "Holiday", ActionName = "Index", AreaName = "HumanResource",
                    Code =  0x000055F0 + 5},
                new NavigationMenu { AreaCode = 0x000055F0 + 1, IsArea = false, Description = "Employee Types", Icon = "fa fa-users", ControllerName = "EmployeeType", ActionName = "Index", AreaName = "HumanResource",
                    Code =  0x000055F0 + 6},

                //Operations
                //..Employees
                new NavigationMenu { AreaCode = 0x000055F0 + 2, IsArea = true, Description = "Employees", Icon = "", Code = 0x000055F0 + 7 },
                new NavigationMenu { AreaCode = 0x000055F0 + 7, IsArea = false, Description = "Register", Icon = "fa fa-clipboard", ControllerName = "Employee", ActionName = "Index", AreaName = "HumanResource",
                    Code =  0x000055F0 + 8},
                new NavigationMenu { AreaCode = 0x000055F0 + 7, IsArea = false, Description = "Document", Icon = "fa fa-file-o", ControllerName = "EmployeeDocuments", ActionName = "Index", AreaName = "HumanResource",
                    Code =  0x000055F0 + 9},

                //..Roster
                new NavigationMenu { AreaCode = 0x000055F0 + 2, IsArea = true, Description = "Roster", Icon = "", Code = 0x000055F0 + 10 },
                new NavigationMenu { AreaCode = 0x000055F0 + 10, IsArea = false, Description = "Regular Day Program", Icon = "fa fa-calendar-check-o", ControllerName = "Controller", ActionName = "Index", AreaName = "HumanResource",
                    Code =  0x000055F0 + 11},

                //..Attendance
                new NavigationMenu { AreaCode = 0x000055F0 + 2, IsArea = true, Description = "Attendance", Icon = "", Code = 0x000055F0 + 12 },
                new NavigationMenu { AreaCode = 0x000055F0 + 12, IsArea = false, Description = "Register", Icon = "fa fa-clipboard", ControllerName = "Controller", ActionName = "Index", AreaName = "HumanResource",
                    Code =  0x000055F0 + 13},
                new NavigationMenu { AreaCode = 0x000055F0 + 12, IsArea = false, Description = "Import Data", Icon = "fa fa-upload", ControllerName = "Controller", ActionName = "Index", AreaName = "HumanResource",
                    Code =  0x000055F0 + 14},

                //..Leave
                new NavigationMenu { AreaCode = 0x000055F0 + 2, IsArea = true, Description = "Leave", Icon = "", Code = 0x000055F0 + 15 },
                new NavigationMenu { AreaCode = 0x000055F0 + 15, IsArea = false, Description = "Application", Icon = "fa fa-clipboard", ControllerName = "LeaveApplication", ActionName = "Index", AreaName = "HumanResource",
                    Code =  0x000055F0 + 16},
                new NavigationMenu { AreaCode = 0x000055F0 + 15, IsArea = false, Description = "Approval", Icon = "fa fa-thumbs-o-up", ControllerName = "LeaveApproval", ActionName = "Index", AreaName = "HumanResource",
                    Code =  0x000055F0 + 17},
                new NavigationMenu { AreaCode = 0x000055F0 + 15, IsArea = false, Description = "Recall", Icon = "fa fa-cogs", ControllerName = "LeaveRecall", ActionName = "Index", AreaName = "HumanResource",
                    Code =  0x000055F0 + 18},

                //..Salary
                new NavigationMenu { AreaCode = 0x000055F0 + 2, IsArea = true, Description = "Salary", Icon = "", Code = 0x000055F0 + 19 },
                new NavigationMenu { AreaCode = 0x000055F0 + 19, IsArea = false, Description = "Salary Heads", Icon = "fa fa-clipboard", ControllerName = "Salary", ActionName = "Index", AreaName = "HumanResource",
                    Code =  0x000055F0 + 20},
                new NavigationMenu { AreaCode = 0x000055F0 + 19, IsArea = false, Description = "Salary Groups", Icon = "fa fa-thumbs-o-up", ControllerName = "SalaryGroups", ActionName = "Index", AreaName = "HumanResource",
                    Code =  0x000055F0 + 21},
                new NavigationMenu { AreaCode = 0x000055F0 + 19, IsArea = false, Description = "Salary Cards", Icon = "fa fa-cogs", ControllerName = "SalaryCards", ActionName = "Index", AreaName = "HumanResource",
                    Code =  0x000055F0 + 22},
                new NavigationMenu { AreaCode = 0x000055F0 + 19, IsArea = false, Description = "Salary Periods", Icon = "fa fa-cogs", ControllerName = "SalaryPeriods", ActionName = "Index", AreaName = "HumanResource",
                    Code =  0x000055F0 + 23},
                new NavigationMenu { AreaCode = 0x000055F0 + 19, IsArea = false, Description = "Salary Processing", Icon = "fa fa-cogs", ControllerName = "SalaryProcessing", ActionName = "Index", AreaName = "HumanResource",
                    Code =  0x000055F0 + 24},
                new NavigationMenu { AreaCode = 0x000055F0 + 19, IsArea = false, Description = "Payslips", Icon = "fa fa-cogs", ControllerName = "Payslips", ActionName = "Index", AreaName = "HumanResource",
                    Code =  0x000055F0 + 25},
                new NavigationMenu { AreaCode = 0x000055F0 + 19, IsArea = false, Description = "Period Closing", Icon = "fa fa-cogs", ControllerName = "PeriodClosing", ActionName = "Index", AreaName = "HumanResource",
                    Code =  0x000055F0 + 26},
                new NavigationMenu { AreaCode = 0x000055F0 + 2, IsArea = false, Description = "Training Period", Icon = "fa fa-bicycle", ControllerName = "TrainingPeriod", ActionName = "Index", AreaName = "HumanResource",
                    Code =  0x000055F0 + 27},
                #endregion

                #region Front-Office Module
                // Front-Office Module...
                new NavigationMenu { Description = "Front-Office", IsArea = true, Code = 0x000061A8 },
                new NavigationMenu { AreaCode = 0x000061A8, IsArea = true, Description = "Operations", Icon = "fa fa-bars", Code = 0x000061A8 + 1 },

                //Operations
                //..Treasury
                new NavigationMenu { AreaCode = 0x000061A8 + 1, IsArea = true, Description = "Treasury", Icon = "", Code = 0x000061A8 + 2 },
                new NavigationMenu { AreaCode = 0x000061A8 + 2, IsArea = false, Description = "Cash Management", Icon = "fa fa-cogs", ControllerName = "CashManagement", ActionName = "Create", AreaName = "FrontOffice",
                    Code =  0x000061A8 + 3},
                new NavigationMenu { AreaCode = 0x000061A8 + 2, IsArea = false, Description = "Authorizations", Icon = "fa fa-cogs", ControllerName = "CashWithdrawalRequest", ActionName = "Create", AreaName = "FrontOffice",
                    Code =  0x000061A8 + 4},
                new NavigationMenu { AreaCode = 0x000061A8 + 2, IsArea = false, Description = "Fiscal Counts", Icon = "fa fa-table", ControllerName = "FiscalCount", ActionName = "Index", AreaName = "FrontOffice",
                    Code =  0x000061A8 + 15},


                //..Teller
                new NavigationMenu { AreaCode = 0x000061A8 + 1, IsArea = true, Description = "Teller", Icon = "", Code = 0x000061A8 + 5 },
                //..Savings Receipts/Payments
                new NavigationMenu { AreaCode = 0x000061A8 + 5, IsArea = false, Description = "Savings Receipts/Payments", Icon = "fa fa-money", ControllerName = "CashDeposit", ActionName = "Create", AreaName = "FrontOffice",
                    Code =  0x000061A8 + 6},
                //..Sundry Receipts Payments
                new NavigationMenu { AreaCode = 0x000061A8 + 5, IsArea = false, Description = "Sundry Receipts/Payments", Icon = "fa fa-money", ControllerName = "SundryPayments", ActionName = "Create", AreaName = "FrontOffice",
                    Code =  0x000061A8 + 7},
                new NavigationMenu { AreaCode = 0x000061A8 + 5, IsArea = false, Description = "Customer Receipts", Icon = "fa fa-money", ControllerName = "CustomerReceipts", ActionName = "Create", AreaName = "FrontOffice",
                    Code =  0x000061A8 + 8},
                new NavigationMenu { AreaCode = 0x000061A8 + 5, IsArea = false, Description = "Cheques/Cash Transfer", Icon = "fa fa-exchange", ControllerName = "Transfers", ActionName = "Create", AreaName = "FrontOffice",
                    Code =  0x000061A8 + 9},
                new NavigationMenu { AreaCode = 0x000061A8 + 5, IsArea = false, Description = "End-Of-Day", Icon = "fa fa-hourglass-end", ControllerName = "EndOfDay", ActionName = "Create", AreaName = "FrontOffice",
                    Code =  0x000061A8 + 10},

                new NavigationMenu{AreaCode = 0x000061A8 + 1, IsArea = false, Description = "Cheques", Icon="fa fa-bars", ControllerName="Cheques", ActionName="Index", AreaName = "FrontOffice", Code = 0x000061A8 + 11},
                new NavigationMenu{AreaCode = 0x000061A8 + 1, IsArea = false, Description = "Fixed Deposits", Icon="fa fa-calculator", ControllerName="FixedDeposit", ActionName="Index", AreaName = "FrontOffice", Code = 0x000061A8 + 12},
                new NavigationMenu{AreaCode = 0x000061A8 + 1, IsArea = false, Description = "Expense Payables", Icon="fa fa-bars", ControllerName="ExpensePayable", ActionName="Index", AreaName = "FrontOffice", Code = 0x000061A8 + 13},
                new NavigationMenu{AreaCode = 0x000061A8 + 1, IsArea = false, Description = "Account Closure", Icon="fa fa-calculator", ControllerName="AccountClosure", ActionName="Index", AreaName = "FrontOffice", Code = 0x000061A8 + 14},
                ////...Cheques
                //new NavigationMenu { AreaCode = 0x000061A8 + 1, IsArea = true, Description = "Cheques", Icon = "", Code = 0x000061A8 + 22 },
                ////..External
                //new NavigationMenu { AreaCode = 0x000061A8 + 22, IsArea = true, Description = "External", Icon = "", Code = 0x000061A8 + 23 },
                //new NavigationMenu { AreaCode = 0x000061A8 + 23, IsArea = false, Description = "Banking", Icon = "fa fa-university", ControllerName = "Controller", ActionName = "Index", AreaName = "FrontOffice",
                //    Code =  0x000061A8 + 24},
                //new NavigationMenu { AreaCode = 0x000061A8 + 23, IsArea = false, Description = "Clearance", Icon = "fa fa-eraser", ControllerName = "Controller", ActionName = "Index", AreaName = "FrontOffice",
                //    Code =  0x000061A8 + 25},
                //new NavigationMenu { AreaCode = 0x000061A8 + 23, IsArea = false, Description = "Catalogue", Icon = "fa fa-cogs", ControllerName = "Controller", ActionName = "Index", AreaName = "FrontOffice",
                //    Code =  0x000061A8 + 26},

                //..In-House
                new NavigationMenu{AreaCode = 0x000061A8 + 1, IsArea = false, Description = "In-House Cheques", Icon = "fa fa-print", ControllerName = "InHouse", ActionName = "Index", AreaName = "FrontOffice",
                    Code = 0x000061A8 + 16},

                //..Automated Clearing
                new NavigationMenu{AreaCode = 0x000061A8 + 1, IsArea = false, Description = "Automated Clearing", Icon = "fa fa-refresh", ControllerName = "AutomatedClearing", ActionName = "Index", AreaName = "FrontOffice",
                    Code = 0x000061A8 + 17},

                //...Fixed Deposits
                //new NavigationMenu { AreaCode = 0x000061A8 + 1, IsArea = true, Description = "Fixed Deposits", Icon = "", Code = 0x000061A8 + 35 },
                //new NavigationMenu { AreaCode = 0x000061A8 + 35, IsArea = false, Description = "Fixing", Icon = "fa fa-wrench", ControllerName = "Controller", ActionName = "Index", AreaName = "FrontOffice",
                //    Code =  0x000061A8 + 36},
                //new NavigationMenu { AreaCode = 0x000061A8 + 35, IsArea = false, Description = "Verification", Icon = "fa fa-check-square-o", ControllerName = "Controller", ActionName = "Index", AreaName = "FrontOffice",
                //    Code =  0x000061A8 + 37},
                //new NavigationMenu { AreaCode = 0x000061A8 + 35, IsArea = false, Description = "Termination", Icon = "fa fa-cogs", ControllerName = "Controller", ActionName = "Index", AreaName = "FrontOffice",
                //    Code =  0x000061A8 + 38},
                //new NavigationMenu { AreaCode = 0x000061A8 + 35, IsArea = false, Description = "Liquidation", Icon = "fa fa-cogs", ControllerName = "Controller", ActionName = "Index", AreaName = "FrontOffice",
                //    Code =  0x000061A8 + 39},
                //new NavigationMenu { AreaCode = 0x000061A8 + 35, IsArea = false, Description = "Catalogue", Icon = "fa fa-cogs", ControllerName = "Controller", ActionName = "Index", AreaName = "FrontOffice",
                //    Code =  0x000061A8 + 40},

                ////...Expense Payables
                //new NavigationMenu { AreaCode = 0x000061A8 + 1, IsArea = true, Description = "Expense Payables", Icon = "", Code = 0x000061A8 + 41 },
                //new NavigationMenu { AreaCode = 0x000061A8 + 41, IsArea = false, Description = "Origination", Icon = "fa fa-cogs", ControllerName = "Controller", ActionName = "Index", AreaName = "FrontOffice",
                //    Code =  0x000061A8 + 42},
                //new NavigationMenu { AreaCode = 0x000061A8 + 41, IsArea = false, Description = "Verification", Icon = "fa fa-check-square-o", ControllerName = "Controller", ActionName = "Index", AreaName = "FrontOffice",
                //    Code =  0x000061A8 + 43},
                //new NavigationMenu { AreaCode = 0x000061A8 + 41, IsArea = false, Description = "Authorization", Icon = "fa fa-check-square-o", ControllerName = "Controller", ActionName = "Index", AreaName = "FrontOffice",
                //    Code =  0x000061A8 + 44},

                ////...Account Closure
                //new NavigationMenu { AreaCode = 0x000061A8 + 1, IsArea = true, Description = "Account Closure", Icon = "", Code = 0x000061A8 + 45 },
                //new NavigationMenu { AreaCode = 0x000061A8 + 45, IsArea = false, Description = "Registration", Icon = "fa fa-clipboard", ControllerName = "Controller", ActionName = "Index", AreaName = "FrontOffice",
                //    Code =  0x000061A8 + 46},
                //new NavigationMenu { AreaCode = 0x000061A8 + 45, IsArea = false, Description = "Approval", Icon = "fa fa-thumbs-o-up", ControllerName = "Controller", ActionName = "Index", AreaName = "FrontOffice",
                //    Code =  0x000061A8 + 47},
                //new NavigationMenu { AreaCode = 0x000061A8 + 45, IsArea = false, Description = "Verification", Icon = "fa fa-check-square-o", ControllerName = "Controller", ActionName = "Index", AreaName = "FrontOffice",
                //    Code =  0x000061A8 + 48},
                #endregion

                            
                    #region Command Hub Module
                // Command Hub Module...
                new NavigationMenu { Description = "Command Hub", IsArea = true, Code = 0x00006590 },
                new NavigationMenu { AreaCode = 0x00006590, IsArea = true, Description = "Setup", Icon = "fa fa-bars", Code = 0x00006590 + 1 },
                new NavigationMenu { AreaCode = 0x00006590, IsArea = true, Description = "Operations", Icon = "fa fa-bars", Code = 0x00006590 + 2 },

                // Setup
                new NavigationMenu { AreaCode = 0x00006590 + 1, IsArea = false, Description = "Messaging Groups", Icon = "fa fa-users", ControllerName = "MessagingGroups", ActionName = "Index", AreaName = "Dashboard",
                    Code =  0x00006590 + 3},

                //..Messaging
                new NavigationMenu { AreaCode = 0x00006590 + 2, IsArea = true, Description = "Messaging", Icon = "", Code = 0x00006590 + 7 },
                new NavigationMenu { AreaCode = 0x00006590 + 7, IsArea = false, Description = "Text Alerts", Icon = "fa fa-comment", ControllerName = "TextAlerts", ActionName = "Index", AreaName = "Dashboard",
                    Code =  0x00006590 + 8},
                new NavigationMenu { AreaCode = 0x00006590 + 7, IsArea = false, Description = "E-mail Alerts", Icon = "fa fa-envelope", ControllerName = "EmailAlerts", ActionName = "Index", AreaName = "Dashboard",
                    Code =  0x00006590 + 9},
                new NavigationMenu { AreaCode = 0x00006590 + 7, IsArea = false, Description = "Instant Messaging", Icon = "fa fa-comment", ControllerName = "InstantMessaging", ActionName = "Index", AreaName = "Dashboard",
                    Code =  0x00006590 + 10},

                //..Utilities
                new NavigationMenu { AreaCode = 0x00006590 + 2, IsArea = true, Description = "Utilities", Icon = "", Code = 0x00006590 + 11 },
                new NavigationMenu { AreaCode = 0x00006590 + 11, IsArea = false, Description = "Financial Position", Icon = "fa fa-money", ControllerName = "FinancialPosition", ActionName = "Index", AreaName = "Dashboard",
                    Code =  0x00006590 + 12},
                new NavigationMenu { AreaCode = 0x00006590 + 11, IsArea = false, Description = "Account Statuses", Icon = "fa fa-user", ControllerName = "AccountStatuses", ActionName = "Index", AreaName = "Dashboard",
                    Code =  0x00006590 + 13},
                new NavigationMenu { AreaCode = 0x00006590 + 11, IsArea = false, Description = "User-Defined Reports", Icon = "fa fa-table", ControllerName = "SSRSReports", ActionName = "Index", AreaName = "Reports",
                    Code =  0x00006590 + 14},
                new NavigationMenu { AreaCode = 0x00006590 + 11, IsArea = false, Description = "Approval Requests", Icon = "fa fa-table", ControllerName = "Workflow", ActionName = "Index", AreaName = "Workflows",
                    Code =  0x00006590 + 15},
                #endregion
                //
            #region Control Module
// Control Module...
new NavigationMenu { Description = "Control (Procurement)", IsArea = true, Code = 0x00007530 },

// Setup & Operations Categories
new NavigationMenu { AreaCode = 0x00007530, IsArea = true, Description = "Setup", Icon = "fa fa-cogs", Code = 0x00007530 + 1 },
new NavigationMenu { AreaCode = 0x00007530, IsArea = true, Description = "Operations", Icon = "fa fa-tasks", Code = 0x00007530 + 2 },

// Setup
new NavigationMenu { AreaCode = 0x00007530 + 1, IsArea = false, Description = "Suppliers", Icon = "fa fa-truck", ControllerName = "Supplier", ActionName = "Index", AreaName = "Control",
    Code =  0x00007530 + 3 },
new NavigationMenu { AreaCode = 0x00007530 + 1, IsArea = false, Description = "Asset Types", Icon = "fa fa-cubes", ControllerName = "AssetTypes", ActionName = "Index", AreaName = "Control",
    Code =  0x00007530 + 4 },
new NavigationMenu { AreaCode = 0x00007530 + 1, IsArea = false, Description = "Inventory Categories", Icon = "fa fa-boxes", ControllerName = "InventoryCategory", ActionName = "Index", AreaName = "Control",
    Code =  0x00007530 + 5 },
new NavigationMenu { AreaCode = 0x00007530 + 1, IsArea = false, Description = "Package Types", Icon = "fa fa-box-open", ControllerName = "PackageType", ActionName = "Index", AreaName = "Control",
    Code =  0x00007530 + 6 },
new NavigationMenu { AreaCode = 0x00007530 + 1, IsArea = false, Description = "Unit Of Measure", Icon = "fa fa-ruler-combined", ControllerName = "UnitOfMeasurement", ActionName = "Index", AreaName = "Control",
    Code =  0x00007530 + 7 },
new NavigationMenu { AreaCode = 0x00007530 + 1, IsArea = false, Description = "Tenders", Icon = "fa fa-file-signature", ControllerName = "Tenders", ActionName = "Index", AreaName = "Control",
    Code =  0x00007530 + 8 },
new NavigationMenu { AreaCode = 0x00007530 + 1, IsArea = false, Description = "Evaluation", Icon = "fa fa-clipboard-check", ControllerName = "Evaluation", ActionName = "Index", AreaName = "Control",
    Code =  0x00007530 + 9 },
new NavigationMenu { AreaCode = 0x00007530 + 1, IsArea = false, Description = "Bidings", Icon = "fa fa-gavel", ControllerName = "Bidings", ActionName = "Index", AreaName = "Control",
    Code =  0x00007530 + 10 },
new NavigationMenu { AreaCode = 0x00007530 + 1, IsArea = false, Description = "Contracts", Icon = "fa fa-file-contract", ControllerName = "Contracts", ActionName = "Index", AreaName = "Control",
    Code =  0x00007530 + 11 },

// Operations
// Assets
new NavigationMenu { AreaCode = 0x00007530 + 2, IsArea = true, Description = "Assets", Icon = "fa fa-landmark", Code = 0x00007530 + 8 },
new NavigationMenu { AreaCode = 0x00007530 + 8, IsArea = false, Description = "Catalogue", Icon = "fa fa-book", ControllerName = "Controller", ActionName = "Index", AreaName = "Control",
    Code =  0x00007530 + 9 },
new NavigationMenu { AreaCode = 0x00007530 + 8, IsArea = false, Description = "Depreciation", Icon = "fa fa-percentage", ControllerName = "Controller", ActionName = "Index", AreaName = "Control",
    Code =  0x00007530 + 10 },

// Inventory
new NavigationMenu { AreaCode = 0x00007530 + 2, IsArea = true, Description = "Inventory", Icon = "fa fa-warehouse", Code = 0x00007530 + 11 },
new NavigationMenu { AreaCode = 0x00007530 + 11, IsArea = false, Description = "Catalogue", Icon = "fa fa-book-open", ControllerName = "Controller", ActionName = "Index", AreaName = "Control",
    Code =  0x00007530 + 12 },
#endregion




                #region Micro-Credit Module
                // Micro-Credit Module...
                new NavigationMenu { Description = "Micro-Credit", IsArea = true, Code = 0x00007918 },
                new NavigationMenu { AreaCode = 0x00007918, IsArea = true, Description = "Setup", Icon = "fa fa-bars", Code = 0x00007918 + 1 },
                new NavigationMenu { AreaCode = 0x00007918, IsArea = true, Description = "Operations", Icon = "fa fa-bars", Code = 0x00007918 + 2 },

                //Setup
                new NavigationMenu { AreaCode = 0x00007918 + 1, IsArea = false, Description = "Officers", Icon = "fa fa-users", ControllerName = "MicroCreditOfficers", ActionName = "Index", AreaName = "MicroCredit",
                    Code =  0x00007918 + 3},

                //Operations
                new NavigationMenu { AreaCode = 0x00007918 + 2, IsArea = false, Description = "Groups", Icon = "fa fa-users", ControllerName = "MicroCreditGroups", ActionName = "Index", AreaName = "MicroCredit",
                    Code =  0x00007918 + 4},
                new NavigationMenu { AreaCode = 0x00007918 + 2, IsArea = false, Description = "Apportionment", Icon = "fa fa-cogs", ControllerName = "MicroCreditApportionment", ActionName = "Index", AreaName = "MicroCredit",
                    Code =  0x00007918 + 5}
                #endregion

                #region Commented Section
                /*test - area 62,000
                 new NavigationMenu{Description = "Grandfather", IsArea = true, Code = 0xF230},
                 //test menu-items (children)
                 new NavigationMenu{AreaCode = 0xF230, IsArea = false, Description = "Father1", Icon="fa fa-users", Code = 0xF230 + 1},
                 new NavigationMenu{AreaCode = 0xF230, IsArea = false, Description = "Father2", Icon="fa fa-users", Code = 0xF230 + 2},

                 new NavigationMenu{AreaCode = 0xF230 + 1, IsArea = false, Description = "Child1_F1", Icon="fa fa-users", ControllerName="Home", ActionName="IndexOne", AreaName = "", Code = 0xF230 + 3},
                 new NavigationMenu{AreaCode = 0xF230 + 1, IsArea = false, Description = "Child2_F1", Icon="fa fa-users", ControllerName="Home", ActionName="IndexTwo", AreaName = "", Code = 0xF230 + 4},

                 new NavigationMenu{AreaCode = 0xF230 + 2, IsArea = false, Description = "Child1_F2", Icon="fa fa-users", Code = 0xF230 + 5},
                 new NavigationMenu{AreaCode = 0xF230 + 2, IsArea = false, Description = "Child2_F2", Icon="fa fa-users", ControllerName="Home", ActionName="IndexThree", AreaName = "", Code = 0xF230 + 6},

                 new NavigationMenu{AreaCode =  0xF230 + 5, IsArea = false, Description = "Child_Child1_F2", Icon="fa fa-users", Code = 0xF230 + 7},
                 new NavigationMenu{AreaCode =  0xF230 + 5, IsArea = false, Description = "Child_Child2_F2", Icon="fa fa-users", ControllerName="Home", ActionName="IndexFour", AreaName = "", Code = 0xF230 + 8},

                 new NavigationMenu{AreaCode =  0xF230 + 7, IsArea = false, Description = "Child_Child1_F2_2", Icon="fa fa-users", ControllerName="Home", ActionName="IndexFive", AreaName = "", Code = 0xF230 + 9},*/
                #endregion
            };

            return menus;
        }
    }
}