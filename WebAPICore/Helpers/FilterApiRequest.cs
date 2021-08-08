using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using WebAPICore.Services;

namespace WebAPICore.Helpers
{
    public class FilterApiRequest : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
            //throw new NotImplementedException();
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var pathUreApi = context.HttpContext.Request.Path;
            var userService = context.HttpContext.RequestServices.GetRequiredService<IUserService>();
            var userId = context.HttpContext.User.Identity.Name;//lấy userID
            //var user = userService.GetById(userId);//compare với DB, thay bằng hàm khác truyền userID và pathUreApi để comnpare
            if (userId == "2")//check giá trị trả về từ user
            {
                dynamic obj = new ExpandoObject();
                obj.success = false;
                obj.message = "Bạn không có quyền truy cập";
                context.Result = new ObjectResult(obj) { StatusCode = 403 };
            }
        }
    }
}
