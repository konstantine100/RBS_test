using RBS.CORE;
using RBS.DTOs;
using RBS.Requests;

namespace RBS.Services.Interfaces;

public interface IIngridientService
{
    Task<ApiResponse<IngredientDTO>> AddIngredient(int foodId, AddIngredient request);
    Task<ApiResponse<IngredientDTO>> UpdateIngredient(int ingridientId, string changeTo);
    Task<ApiResponse<IngredientDTO>> DeleteIngredient(int ingridientId);
    
}