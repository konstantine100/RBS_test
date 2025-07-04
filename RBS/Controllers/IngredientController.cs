using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RBS.Requests;
using RBS.Services.Interfaces;

namespace RBS.Controllers;

[Route("api/[controller]")]
[ApiController]

public class IngredientController : ControllerBase
{
    private readonly IIngridientService _ingridientService;

    public IngredientController(IIngridientService ingridientService)
    {
        _ingridientService = ingridientService;
    }

    [HttpPost("create-ingredient")]
    public async Task<ActionResult> AddIngredient(int foodId, AddIngredient request)
    {
        var response = _ingridientService.AddIngredient(foodId, request);
        return Ok(response);
    }
    
    [HttpPut("update-ingredient")]
    public async Task<ActionResult> UpdateIngredient(int ingridientId, string changeTo)
    {
        var response = _ingridientService.UpdateIngredient(ingridientId, changeTo);
        return Ok(response);
    }
    
    [HttpDelete("delete-ingredient")]
    public async Task<ActionResult> DeleteIngredient(int ingridientId)
    {
        var response = _ingridientService.DeleteIngredient(ingridientId);
        return Ok(response);
    }
}