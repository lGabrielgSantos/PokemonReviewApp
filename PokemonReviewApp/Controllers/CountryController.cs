using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using PokemonReviewApp.Dto;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Controllers
{
    [Route("api/[controller]")] //pass route
    [ApiController]
    public class CountryController : Controller
    {
        private readonly ICountryRepository _countryRepository;
        private readonly IMapper _mapper;
        public CountryController(ICountryRepository countryRepository, IMapper mapper)
        {
            _countryRepository = countryRepository;
            _mapper = mapper;
        }
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Country>))]
        public IActionResult GetCountries()
        {
            var countries = _mapper.Map<List<CountryDto>>(_countryRepository.GetCountries());

            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            return Ok(countries);
        }

        [HttpGet("{countryId}")]
        [ProducesResponseType(200, Type = typeof(Country))]
        [ProducesResponseType(400)]
        public IActionResult GetCountry(int countryId) 
        { 
            if(!_countryRepository.CountryExists(countryId))
                return NotFound();
            var country = _mapper.Map<CountryDto>(_countryRepository.GetCountry(countryId));
           if(!ModelState.IsValid)
                return BadRequest(ModelState);
           return Ok(country);
        }

        [HttpGet("/{ownerId}/country")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Country>))]
        [ProducesResponseType(400)]
        public IActionResult GetCountryByOwner(int ownerId)
        {
            var country = _mapper.Map<CountryDto>(_countryRepository.GetCountryByOwner(ownerId));

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(country);
        }


        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public IActionResult CreatedCountry(CountryDto countryCreated)
        {
            if(countryCreated == null)
                return BadRequest(ModelState);

            if (countryCreated.Id != null)
                return BadRequest();

            var coutryRepeat = _countryRepository.GetCountries()
                .Where(c => c.Name.Trim().ToUpper() == countryCreated.Name.TrimEnd().ToUpper())
                .FirstOrDefault();

            if(coutryRepeat != null)
            {
                ModelState.AddModelError("", "Coutry exist!");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return NotFound(ModelState);

            var coutryMap = _mapper.Map<Country>(countryCreated);

            var sucess = _countryRepository.CreateCountry(coutryMap);

            if (!sucess)
            {
                ModelState.AddModelError("", "Failed Saved.");
                return StatusCode(500, ModelState);
            }

            return Ok("Sucess Created");
        }

        [HttpPut("{countryId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult UpdateCountry(int countryId, [FromBody] CountryDto updateCountry)
        {
            if (updateCountry == null) 
                return BadRequest();

            if(countryId != updateCountry.Id) 
                return BadRequest();

            if(!_countryRepository.CountryExists(countryId)) 
                return NotFound();

            if(!ModelState.IsValid) 
                return BadRequest();

            var coutryMap = _mapper.Map<Country>(updateCountry);

            var sucess = _countryRepository.UpdateCountry(coutryMap);

            if (!sucess)
            {
                return BadRequest(ModelState);
            }

            return Ok("Coutry Update!");
        }

        [HttpDelete("{coutryId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult DeleteCoutry(int coutryId)
        {
            if(coutryId == null) 
                return BadRequest();

            if (!_countryRepository.CountryExists(coutryId))
                return NotFound(ModelState);


            var coutryDelete = _countryRepository.GetCountry(coutryId);

            if (!ModelState.IsValid)
                return NotFound(ModelState);

            var sucess = _countryRepository.DeleteCountry(coutryDelete);

            if(!sucess)
            {
                ModelState.AddModelError("", "Error in delete coutry");
                return StatusCode(500, ModelState);
            }
            return Ok("Coutry Deleted!");

        }
    }
}
