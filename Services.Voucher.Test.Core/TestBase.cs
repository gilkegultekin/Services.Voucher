using Microsoft.AspNetCore.Mvc;

namespace Services.Voucher.Test.Core
{
    public class TestBase
    {
        protected virtual TResult ParseActionResultAsOk<TResult>(ActionResult<TResult> actionResult)
        {
            var okResult = actionResult.Result as OkObjectResult;
            return (TResult)okResult.Value;
        }

    }
}
