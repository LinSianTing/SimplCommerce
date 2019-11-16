using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimplCommerce.Infrastructure.Data;
using SimplCommerce.Infrastructure.Web.SmartTable;
using SimplCommerce.Module.Catalog.Models;
using SimplCommerce.Module.Core.Events;
using SimplCommerce.Module.Core.Extensions;
using SimplCommerce.Module.Reviews.Data;
using SimplCommerce.Module.Reviews.Models;

namespace SimplCommerce.Module.Reviews.Areas.Reviews.Controllers
{
    [Area("Reviews")]
    [Authorize(Roles = "admin,vendor, farmer")]
    [Route("api/reviews")]
    public class ReviewApiController : Controller
    {
        private readonly IRepository<Product> _productRepository;
        private readonly IReviewRepository _reviewRepository;
        private readonly IMediator _mediator;
        private readonly IWorkContext _workContext;

        public ReviewApiController(IRepository<Product> productRepository, IReviewRepository reviewRepository, IMediator mediator, IWorkContext workContext)
        {
            _productRepository = productRepository;
            _reviewRepository = reviewRepository;
            _mediator = mediator;
            _workContext = workContext;
        }

        [HttpGet]
        public async Task<IActionResult> Get(int status, int numRecords)
        {
            var reviewStatus = (ReviewStatus)status;

            if ((numRecords <= 0) || (numRecords > 100))
            {
                numRecords = 5;
            }

            if (User.IsInRole("admin"))
            {
                var model = _reviewRepository
                                      .List()
                                      .Where(x => x.Status == reviewStatus)
                                      .OrderByDescending(x => x.CreatedOn)
                                      .Take(numRecords)
                                      .Select(x => new
                                      {
                                          x.Id,
                                          x.ReviewerName,
                                          x.EntityName,
                                          x.EntitySlug,
                                          x.Rating,
                                          x.Title,
                                          x.Comment,
                                          Status = x.Status.ToString(),
                                          x.CreatedOn
                                      });

                return Json(model);
            }
            else
            {
                var currentUser = await _workContext.GetCurrentUser();

                var query = _productRepository.Query().Where(x => x.BrandId == currentUser.Id);

                var thisFarmerProductList = query.Select(a=>a.Id);

                var model = _reviewRepository
                    .List()
                    .Where(x => x.Status == reviewStatus && thisFarmerProductList.Contains(x.EntityId))
                    .OrderByDescending(x => x.CreatedOn)
                    .Take(numRecords)
                    .Select(x => new
                    {
                        x.Id,
                        x.ReviewerName,
                        x.EntityName,
                        x.EntitySlug,
                        x.Rating,
                        x.Title,
                        x.Comment,
                        Status = x.Status.ToString(),
                        x.CreatedOn
                    });

                return Json(model);
            }
        }

        [HttpPost("grid")]
        public async Task<IActionResult> List([FromBody] SmartTableParam param)
        {
            var query = _reviewRepository.List();

            if (param.Search.PredicateObject != null)
            {
                dynamic search = param.Search.PredicateObject;
                if (search.Id != null)
                {
                    long id = search.Id;
                    query = query.Where(x => x.Id == id);
                }

                if (search.EntityName != null)
                {
                    string entityName = search.EntityName;
                    query = query.Where(x => x.EntityName == entityName);
                }

                if (search.Status != null)
                {
                    var status = (ReviewStatus)search.Status;
                    query = query.Where(x => x.Status == status);
                }

                if (search.CreatedOn != null)
                {
                    if (search.CreatedOn.before != null)
                    {
                        DateTimeOffset before = search.CreatedOn.before;
                        query = query.Where(x => x.CreatedOn <= before);
                    }

                    if (search.CreatedOn.after != null)
                    {
                        DateTimeOffset after = search.CreatedOn.after;
                        query = query.Where(x => x.CreatedOn >= after);
                    }
                }
            }

            var currentUser = await _workContext.GetCurrentUser();

            var thisFarmerProductList = _productRepository.Query().Where(x => x.BrandId == currentUser.Id).Select(a => a.Id);

            var reviews = query
                .Where(a => thisFarmerProductList.Contains(a.EntityId))
                .ToSmartTableResult(
                param,
                x => new
                {
                    x.Id,
                    x.ReviewerName,
                    x.Rating,
                    x.Title,
                    x.Comment,
                    x.EntityName,
                    x.EntitySlug,
                    Status = x.Status.ToString(),
                    x.CreatedOn
                });

            return Json(reviews);
        }

        [HttpPost("change-status/{id}")]
        public async Task<IActionResult> ChangeStatus(long id, [FromBody] int statusId)
        {
            var review = _reviewRepository.Query().FirstOrDefault(x => x.Id == id);
            if (review == null)
            {
                return NotFound();
            }

            if (Enum.IsDefined(typeof(ReviewStatus), statusId))
            {
                review.Status = (ReviewStatus)statusId;
                _reviewRepository.SaveChanges();

                var rattings = _reviewRepository.Query()
                    .Where(x => x.EntityId == review.EntityId && x.EntityTypeId == review.EntityTypeId && x.Status == ReviewStatus.Approved);

                var reviewSummary = new ReviewSummaryChanged
                {
                    EntityId = review.EntityId,
                    EntityTypeId = review.EntityTypeId,
                    ReviewsCount = rattings.Count()
                };
                if (reviewSummary.ReviewsCount == 0)
                {
                    reviewSummary.RatingAverage = null;
                }
                else
                {
                    var grouped = rattings.GroupBy(x => x.Rating).Select(x => new { Rating = x.Key, Count = x.Count() }).ToList();
                    reviewSummary.RatingAverage = grouped.Select(x => x.Rating * x.Count).Sum() / (double)reviewSummary.ReviewsCount;
                }

                await _mediator.Publish(reviewSummary);
                await _reviewRepository.SaveChangesAsync();
                return Accepted();
            }
            return BadRequest(new { Error = "unsupported order status" });
        }
    }
}
