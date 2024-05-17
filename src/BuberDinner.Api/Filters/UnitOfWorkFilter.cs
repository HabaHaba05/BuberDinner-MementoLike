using BuberDinner.Application.Common.Interfaces.Persistence;

using Microsoft.AspNetCore.Mvc.Filters;

namespace BuberDinner.Api.Filters;

public class UnitOfWorkFilter : ActionFilterAttribute
{
    private readonly IUnitOfWork _unitOfWork;
    public UnitOfWorkFilter(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public override void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Exception == null)
        {
            _unitOfWork.Commit();
        }
        else
        {
            _unitOfWork.Rollback();
        }
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        _unitOfWork.Begin();
    }
}