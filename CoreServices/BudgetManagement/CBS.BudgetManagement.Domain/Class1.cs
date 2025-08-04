//FiscalYear

//Another foundational entity that's referenced by many other entities.
//It's independent of other entities.

//BudgetCategory

//This is a simple entity that BudgetItems will reference.
//It doesn't depend on other entities.


//Project

//Projects are standalone entities at this point.
//They'll be referenced by ProjectBudget later.


//BudgetPlan

//Depends on Department, FiscalYear, and User.
//Many subsequent entities will reference BudgetPlan.


//BudgetItem

//Depends on BudgetPlan and BudgetCategory.
//Represents the details of a BudgetPlan.


//Expenditure

//Depends on Department and BudgetItem.
//Tracks actual spending against budget items.


//BudgetAdjustment

//Depends on BudgetPlan and User.
//Allows for modifications to existing budget plans.


//SpendingLimit

//Depends on Department and FiscalYear.
//Sets constraints on departmental spending.


//ProjectBudget

//Depends on Project and FiscalYear.
//Links projects to specific fiscal years for budgeting purposes.
