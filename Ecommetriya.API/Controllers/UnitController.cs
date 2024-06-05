
using Ecommetriya.API.Models;
using Ecommetriya.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommetriya.API.Controllers
{
    //unitt
    [ApiController]
    [Route("api/[controller]")]
    public class UnitController : ControllerBase
    {
        
        //private readonly IUnitService _unitService;

        //public UnitController(IUnitService unitService)
        //{
        //    _unitService = unitService ?? throw new ArgumentNullException(nameof(unitService));
        //}

        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<Unit>>> GetAllUnits()
        //{
        //    var units = await _unitService.GetAllUnitsAsync();
        //    return Ok(units);
        //}

        //[HttpGet("{id}")]
        //public async Task<ActionResult<Unit>> GetUnitById(int id)
        //{
        //    var unit = await _unitService.GetUnitByIdAsync(id);
        //    if (unit == null)
        //    {
        //        return NotFound();
        //    }
        //    return Ok(unit);
        //}

        //[HttpPost]
        //public async Task<ActionResult> AddUnit([FromBody] Unit unit)
        //{
        //    await _unitService.AddUnitAsync(unit);
        //    return CreatedAtAction(nameof(GetUnitById), new { id = unit.nmID1 }, unit);
        //}

        //[HttpPut("{id}")]
        //public async Task<ActionResult> UpdateUnit(int id, [FromBody] Unit unit)
        //{
        //    if (id != unit.nmID1)
        //    {
        //        return BadRequest();
        //    }
        //    await _unitService.UpdateUnitAsync(unit);
        //    return NoContent();
        //}

        //[HttpDelete("{id}")]
        //public async Task<ActionResult> DeleteUnit(int id)
        //{
        //    await _unitService.DeleteUnitAsync(id);
        //    return NoContent();
        //}
    }
}
