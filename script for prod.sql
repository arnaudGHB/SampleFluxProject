

--AccountCategory
INSERT [dbo].[AccountCategory] ([Id], [Name], [Description], [CreatedDate], [BankId], [BranchId], [CreatedBy], [ModifiedDate], [ModifiedBy], [DeletedDate], [DeletedBy], [IsDeleted], [TempData]) VALUES (N'CT95750177', N'THIRD PARTY ACCOUNTS AND ACCRUALS(Recievables)', N'Record receivables at fair value when the right to receive payment is established', CAST(N'2025-01-12T00:29:29.5817938' AS DateTime2), N'NoBankId', N'717373842079233', N'81ddb9c6-f937-4f92-8171-8b1524bf6dc1', CAST(N'0001-01-01T01:00:00.0000000' AS DateTime2), N'81ddb9c6-f937-4f92-8171-8b1524bf6dc1', CAST(N'0001-01-01T01:00:00.0000000' AS DateTime2), NULL, 0, N'')

--AccountCategory
Update [dbo].[AccountCategory] Set [Name]='CAPITAL FUNDS ACCOUNTS' where id ='AC88741310'
Update [dbo].[AccountCategory] Set [Name]='FIXED ASSETS ACCOUNTS' where id ='AC74825435'
Update [dbo].[AccountCategory] Set [Name]='CUSTOMER ACCOUNTS' where id ='AC74825100'
Update [dbo].[AccountCategory] Set [Name]='THIRD PARTY ACCOUNTS AND ACCRUALS(Payabels)' where id ='AC64912072'
Update [dbo].[AccountCategory] Set [Name]='TREASURY AND INTER BANK TRANSACTIONS ACCOUNTS'  where id ='AC59489857'
Update [dbo].[AccountCategory] Set [Name]='EXPENSE ACCOUNTS' where id ='AC45569308'
Update [dbo].[AccountCategory] Set [Name]='REVENUE ACCOUNTS'  where id ='AC073078925'
Update [dbo].[AccountCategory] Set [Name]='INTERMEDIATE MANAGEMENT ACCOUNT'  where id ='AC26059376'
Update [dbo].[AccountCategory] Set [Name]='OFF BALANCE SHEET ACCOUNTS'  where id ='AC38528827'

--AccountClasss*******************
Update [dbo].[AccountClasses] Set [AccountNumber]='1', [AccountCategoryId]='AC88741310' where id ='AC07432684'
Update [dbo].[AccountClasses] Set [AccountNumber]='2', [AccountCategoryId]='AC74825435' where id ='AC29922660'
Update [dbo].[AccountClasses] Set [AccountNumber]='3', [AccountCategoryId]='AC74825100'  where id ='AC98669676'
Update [dbo].[AccountClasses] Set [AccountNumber]='4', [AccountCategoryId]='AC64912072'  where id ='AC09073525'
Update [dbo].[AccountClasses] Set [AccountNumber]='5', [AccountCategoryId]='AC59489857'  where id ='AC27896694'
Update [dbo].[AccountClasses] Set [AccountNumber]='6', [AccountCategoryId]='AC45569308'  where id ='AC23890516'
Update [dbo].[AccountClasses] Set [AccountNumber]='7', [AccountCategoryId]='AC073078925'  where id ='AC62144456'
Update [dbo].[AccountClasses] Set [AccountNumber]='8' , [AccountCategoryId]='AC26059376' where id ='AC62144456'
Update [dbo].[AccountClasses] Set [AccountNumber]='9' , [AccountCategoryId]='AC38528827' where id ='AC22178527'
Update [dbo].[AccountClasses] Set [AccountNumber]='4', [AccountCategoryId]='CT95750177'  where id ='AC05480085'

UPDATE Accounts SET Account1 = LEFT(Account2, 1);

update Accounts set AccountCategoryId= 'AC88741310' where Account1 ='1'
update Accounts set AccountCategoryId= 'AC74825435' where Account1 ='2'
update Accounts set AccountCategoryId= 'AC74825100' where Account1 ='3'
update Accounts set AccountCategoryId= 'AC64912072'  where Account1 ='4'
update Accounts set AccountCategoryId= 'AC59489857' where Account1 ='5'
update Accounts set AccountCategoryId= 'AC45569308' where Account1 ='6'
update Accounts set AccountCategoryId= 'AC073078925' where Account1 ='7'
update Accounts set AccountCategoryId= 'AC26059376' where Account1 ='8'
update Accounts set AccountCategoryId= 'AC38528827' where Account1 ='9'

UPDATE ChartOfAccount SET AccountCartegoryId = 'AC38528827'where  Account1= '9'
UPDATE ChartOfAccount SET AccountCartegoryId = 'AC26059376'where  Account1= '8'
UPDATE ChartOfAccount SET AccountCartegoryId = 'AC073078925'where  Account1= '7'
UPDATE ChartOfAccount SET AccountCartegoryId = 'AC45569308'where  Account1= '6'
UPDATE ChartOfAccount SET AccountCartegoryId = 'AC59489857'where  Account1= '5'
UPDATE ChartOfAccount SET AccountCartegoryId = 'AC64912072'where  Account1= '4'
UPDATE ChartOfAccount SET AccountCartegoryId = 'AC74825100'where  Account1= '3'
UPDATE ChartOfAccount SET AccountCartegoryId = 'AC74825435'where  Account1= '2'
UPDATE ChartOfAccount SET AccountCartegoryId = 'AC88741310'where  Account1= '1