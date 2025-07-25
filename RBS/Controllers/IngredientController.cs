using Microsoft.AspNetCore.Authorization;
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
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult> AddIngredient(int foodId, AddIngredient request)
    {
        try
        {
            var response = await _ingridientService.AddIngredient(foodId, request);
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while creating ingredient.", ex);
        }
    }
    
    [HttpPut("update-ingredient/{ingredientId}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult> UpdateIngredient(int ingredientId, bool isEnglish, string changeTo)
    {
        try
        {
            var response = await _ingridientService.UpdateIngredient(ingredientId, isEnglish, changeTo);
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while updating ingredient.", ex);
        }
    }
    
    [HttpDelete("delete-ingredient/{ingredientId}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult> DeleteIngredient(int ingredientId)
    {
        try
        {
            var response = await _ingridientService.DeleteIngredient(ingredientId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while deleting ingredient.", ex);
        }
    }
}