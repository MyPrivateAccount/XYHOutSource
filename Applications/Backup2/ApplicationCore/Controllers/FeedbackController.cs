using ApplicationCore.Dto;
using ApplicationCore.Filters;
using ApplicationCore.Managers;
using ApplicationCore.Models;
using AspNet.Security.OAuth.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Controllers
{
    [Authorize(AuthenticationSchemes = OAuthValidationDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    [Route("api/feedbacks")]
    public class FeedbackController : Controller
    {
        private readonly FeedbackManager _feedbackManager;
        public FeedbackController(FeedbackManager feedbackManager)
        {
            _feedbackManager = feedbackManager;
        }

        [HttpGet("{id}")]
        public async Task<ResponseMessage<Feedback>> GetFeedback([FromRoute] string id)
        {
            ResponseMessage<Feedback> response = new ResponseMessage<Feedback>();
            response.Extension = await _feedbackManager.FindByIdAsync(id, HttpContext.RequestAborted);
            return response;
        }

        [HttpPost("list")]
        public async Task<PagingResponseMessage<Feedback>> GetFeedbacks([FromBody] FeedBackSearchCondition condition)
        {
            return await _feedbackManager.Search(condition, HttpContext.RequestAborted);
        }

        [HttpPost]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]

        public async Task<ResponseMessage<Feedback>> PostBuildingBaseInfo(string userId, [FromBody]FeedbackRequest feedbackRequest)
        {
            ResponseMessage<Feedback> response = new ResponseMessage<Feedback>();
            if (!ModelState.IsValid)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                return response;
            }
            Feedback feedback = new Feedback();
            feedback.Content = feedbackRequest.Content;
            feedback.CreateTime = DateTime.Now;
            feedback.Id = Guid.NewGuid().ToString();
            feedback.UserId = userId;
            response.Extension = await _feedbackManager.CreateAsync(feedback, HttpContext.RequestAborted);
            return response;
        }





    }
}
