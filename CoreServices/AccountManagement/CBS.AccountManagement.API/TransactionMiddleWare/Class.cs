//using Microsoft.AspNetCore.Mvc.Filters;
//using System.Text;
//using System;
//using CBS.AccountManagement.Data;
//using CBS.AccountManagement.Domain;
//using CBS.TransactionManagement.Helper;

//public class TransactionRequestTrackerAttribute : ActionFilterAttribute
//{
//    private readonly UserInfoToken _userInfoToken;
//    private readonly POSContext _dbContext;

//    public TransactionRequestTrackerAttribute()
//    {
//    }

//    public TransactionRequestTrackerAttribute(UserInfoToken userInfoToken, POSContext dbContext)
//    {
//        _userInfoToken = userInfoToken;
//    }

//    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
//    {
//        // Read the request body
//        context.HttpContext.Request.EnableBuffering();
//        var requestBody = await new StreamReader(context.HttpContext.Request.Body, Encoding.UTF8).ReadToEndAsync();
//        context.HttpContext.Request.Body.Position = 0;
//        // Extract controller and action names
//        var controllerName = context.ActionDescriptor.RouteValues["controller"];
//        var actionName = context.ActionDescriptor.RouteValues["action"];
//        // Log the request with status as false
//        var log = new TransactionTracker
//        {
//            CommandJsonObject = requestBody,
//            HasPassed = false,
//            BankId = _userInfoToken.BankId,
//            BranchId = _userInfoToken.BranchId,
//            CreatedBy = _userInfoToken.Id,
//            CreatedDate = DateTime.UtcNow,
//            ModifiedBy = "SYSTEM",
//            ModifiedDate = DateTime.UtcNow,
//            CommandDataType = $"{controllerName}@{actionName}",
//            NumberOfRetry = 1,

//        };

//        _dbContext.TransactionTrackers.Add(log);
//        await _dbContext.SaveChangesAsync();

//        // Execute the action
//        var result = await next();

//        // Update the status to true if the action succeeds
//        if (result.Exception == null)
//        {
//            log.HasPassed = true;
//            _dbContext.TransactionTrackers.Update(log);
//            await _dbContext.SaveChangesAsync();
//        }
//    }
//}