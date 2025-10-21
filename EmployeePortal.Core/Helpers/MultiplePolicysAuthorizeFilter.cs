//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Filters;

//namespace UrlaubsPortal.Core.Helpers
//{
//    public class MultiplePoliciesAuthorizeAttribute : TypeFilterAttribute
//    {
//        public MultiplePoliciesAuthorizeAttribute(string policies, bool isAnd = false) : base(typeof(MultiplePoliciesAuthorizeFilter))
//        {
//            Arguments = new object[] { policies, isAnd };
//        }
//    }
//    public class MultiplePoliciesAuthorizeFilter : IAsyncAuthorizationFilter
//    {
//        private readonly IAuthorizationService _authorization;
//        public string Policies { get; private set; }
//        public bool IsAnd { get; private set; }

//        public MultiplePoliciesAuthorizeFilter(string policies, bool isAnd, IAuthorizationService authorization)
//        {
//            Policies = policies;
//            IsAnd = isAnd;
//            _authorization = authorization;
//        }

//        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
//        {
//            var policies = Policies.Split(";").ToList();
//            if (IsAnd)
//            {
//                foreach (var policy in policies)
//                {
//                    var authorized = await _authorization.AuthorizeAsync(context.HttpContext.User, policy);
//                    if (!authorized.Succeeded)
//                    {
//                        context.Result = new JsonResult(new
//                        {
//                            Message = GetForbiddenMessage(policies)
//                        })
//                        {
//                            StatusCode = StatusCodes.Status403Forbidden
//                        };
//                        return;
//                    }
//                }
//            }
//            else
//            {
//                foreach (var policy in policies)
//                {
//                    var authorized = await _authorization.AuthorizeAsync(context.HttpContext.User, policy);
//                    if (authorized.Succeeded)
//                    {
//                        return;
//                    }

//                }
//                context.Result = new JsonResult(new
//                {
//                    Message = GetForbiddenMessage(policies)
//                })
//                {
//                    StatusCode = StatusCodes.Status403Forbidden
//                };
//                return;
//            }
//        }
//        public List<string> GetForbiddenMessage(List<string> policies)
//        {
//            for (int i = 0; i < policies.Count; i++)
//            {
//                var feature = policies[i].Split(".")[0];
//                policies[i] = policies[i].Replace(".", " ");
//                //string name = ((Enum)Enum.Parse(typeof(RolePermission.PermissionFeatures), feature)).GetAttribute<NameAttribute>().Name;
//                //policies[i] = policies[i].Replace(feature, name);

//            }

//            return policies;
//        }
//    }
//}
