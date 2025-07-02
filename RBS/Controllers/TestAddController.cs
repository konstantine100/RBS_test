using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RBS.Requests;
using RBS.Services.Interfaces;

namespace RBS.Controllers;

[Route("api/[controller]")]
[ApiController]

public class TestAddController : ControllerBase
{
    private readonly ITestingService _testingService;

    public TestAddController(ITestingService testingService)
    {
        _testingService = testingService;
    }


    [HttpPost("post-restaurant")]
    public ActionResult AddRestaurant(AddRestaurant request)
    {
        var response = _testingService.AddRestaurant(request);
        return Ok(response);
    }
    
    [HttpPost("post-space")]
    public ActionResult AddSpace(int restaurantId, AddSpace request)
    {
        var response = _testingService.AddSpace(restaurantId, request);
        return Ok(response);
    }
    
    [HttpPost("post-table")]
    public ActionResult AddTable(int spaceId, AddTable request)
    {
        var response = _testingService.AddTable(spaceId, request);
        return Ok(response);
    }
    
    [HttpPost("post-chair")]
    public ActionResult AddChair(int tableId, AddChair request)
    {
        var response = _testingService.AddChair(tableId, request);
        return Ok(response);
    }
    
    [HttpDelete("delete-restaurant")]
    public ActionResult DeleteRestaurant(int restaurantId)
    {
        var response = _testingService.DeleteRestaurant(restaurantId);
        return Ok(response);
    }
    
    [HttpDelete("delete-space")]
    public ActionResult DeleteSpace(int spaceId)
    {
        var response = _testingService.DeleteSpace(spaceId);
        return Ok(response);
    }
    
    [HttpDelete("delete-table")]
    public ActionResult DeleteTable(int tableId)
    {
        var response = _testingService.DeleteTable(tableId);
        return Ok(response);
    }
    
    [HttpDelete("delete-chair")]
    public ActionResult DeleteChair(int chairId)
    {
        var response = _testingService.DeleteChair(chairId);
        return Ok(response);
    }
}