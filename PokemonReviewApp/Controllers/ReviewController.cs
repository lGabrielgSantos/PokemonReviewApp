using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PokemonReviewApp.Dto;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;
using PokemonReviewApp.Repository;

namespace PokemonReviewApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : Controller
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IMapper _mapper;
        private readonly IPokemonRepository _pokemonRepository;
        private readonly IReviewerRepository _reviewerRepository;

        public ReviewController(IReviewRepository reviewRepository, IMapper mapper, IPokemonRepository pokemonRepository, IReviewerRepository reviewerRepository)
        {
            _reviewRepository = reviewRepository;
            _mapper = mapper;
            _pokemonRepository = pokemonRepository;
            _reviewerRepository = reviewerRepository;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Review>))]
        public IActionResult GetReviews() 
        {
            if (!ModelState.IsValid)
                return NotFound(ModelState);
            
            var reviews = _mapper.Map<List<ReviewDto>>(_reviewRepository.GetReviews());

            return Ok(reviews);

        }

        [HttpGet("{reviewId}")]
        [ProducesResponseType(200, Type = typeof(Review))]
        public IActionResult GetReview(int reviewId) 
        {
            if (!_reviewRepository.ReviewExists(reviewId))
                return BadRequest();
            var review = _mapper.Map<ReviewDto>(_reviewRepository.GetReview(reviewId));
            if (!ModelState.IsValid)
                return NotFound(ModelState);
            return Ok(review);
        }
        [HttpGet("pokemon/{pokeId}")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Review>))]
        public IActionResult GetReviewsOfAPokemon(int pokeId)
        {
            var reviews = _mapper.Map<List<ReviewDto>>(_reviewRepository.GetReviewsOfAPokemon(pokeId));
            if (!ModelState.IsValid)
                return NotFound(ModelState);
            return Ok(reviews);

        }

        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public IActionResult CreateReview([FromQuery] int reviewerId, [FromQuery] int pokeId, ReviewDto newReview)
        {
            if(newReview == null || reviewerId == null || pokeId == null){
                return BadRequest();
            }
            var reviewExist = _reviewRepository.GetReviews()
                .Where(r => r.Id== newReview.Id)
                .FirstOrDefault();
            if (reviewExist != null)
            {
                ModelState.AddModelError("", "Review exist");
                return StatusCode(422, ModelState);
            }
            
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var reviewMap = _mapper.Map<Review>(newReview);

            reviewMap.Reviewer = _reviewerRepository.GetReviewer(reviewerId);
            reviewMap.Pokemon = _pokemonRepository.GetPokemon(pokeId);

            var sucess = _reviewRepository.CreateReview(reviewMap);

            if (!sucess)
            {
                ModelState.AddModelError("", "Error in save review");
                return StatusCode(422, ModelState);
            }

            return Ok("Review Saved!");

        }

        [HttpPut("{reviewId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult UpdateOwner(int reviewId, [FromBody] ReviewDto updateReview)
        {
            if (updateReview == null)
                return BadRequest();

            if (reviewId != updateReview.Id)
                return BadRequest(ModelState);

            if (!_reviewerRepository.ReviewerExist(reviewId))
                return BadRequest();

            if (!ModelState.IsValid)
                return NotFound();

            var reviewMap = _mapper.Map<Review>(updateReview);

            var sucess = _reviewRepository.UpdateReview(reviewMap);

            if (!sucess)
            {
                ModelState.AddModelError("", "Error in updatee.");
                return StatusCode(500, ModelState);
            }

            return Ok("Review update");
        }


    }
}
