using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PokemonReviewApp.Dto;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewerController : Controller
    {
        private readonly IReviewerRepository _reviewerRepository;
        private readonly IMapper _mapper;
        public ReviewerController(IReviewerRepository reviewerRepository, IMapper mapper)
        {
            _reviewerRepository = reviewerRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Reviewer>))]
        public IActionResult GetReviewers()
        {
            var reviewers = _mapper.Map<List<ReviewerDto>>(_reviewerRepository.GetReviewers());
            if(!ModelState.IsValid)
                return NotFound();

            return Ok(reviewers);
        }

        [HttpGet("reviewerId")]
        [ProducesResponseType(200, Type = typeof(Reviewer))]
        public IActionResult GetReviewer(int reviewerId) 
        {
            if (!_reviewerRepository.ReviewerExist(reviewerId))
                return BadRequest();

            var reviewer = _mapper.Map<ReviewerDto>(_reviewerRepository.GetReviewer(reviewerId));

            if (!ModelState.IsValid)
                return NotFound();
            return Ok(reviewer);  
        }

        [HttpGet("review/{reviewerId}")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Review>))]
        public IActionResult GetReviewsByReviewer(int reviewerId)
        {
            var reviews = _mapper.Map<List<ReviewDto>>(_reviewerRepository.GetReviewsByReviewer(reviewerId));
            if (!ModelState.IsValid)
                return NotFound();

            return Ok(reviews);
        }

        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public IActionResult CreateReviewer([FromBody] ReviewerDto newReviewer)
        {
            if(newReviewer == null)
            {
                return BadRequest(ModelState);
            }

            var reviewerExist = _reviewerRepository.GetReviewers()
                .Where(r => r.LastName.Trim().ToUpper() == newReviewer.LastName.TrimEnd().ToUpper())
                .FirstOrDefault();

            if (reviewerExist != null)
            {
                ModelState.AddModelError("", "Reviewer exist!");
                return StatusCode(402, ModelState);
            }

            if (!ModelState.IsValid)
                return NotFound(ModelState);

            var reviewerMap = _mapper.Map<Reviewer>(newReviewer);

            var sucess = _reviewerRepository.CreateReviewer(reviewerMap);

            if (!sucess)
            {
                ModelState.AddModelError("", "Error!");
                return StatusCode(402, ModelState);
            }

            return Ok("Reviewer created!");

        }

    }
}
