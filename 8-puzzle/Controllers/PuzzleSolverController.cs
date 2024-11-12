using _8_puzzle.Responses;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace _8_puzzle.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PuzzleSolverController(IPuzzleSolverService puzzleSolverService) : Controller
{
    [HttpGet]
    public ActionResult<PuzzleSolverResponse> Get()
    {
        var totalResult = puzzleSolverService.Solve();

        var finalResult = new List<List<int>>();
        
        finalResult.AddRange(totalResult.Take(50));
        finalResult.AddRange(totalResult.Skip(totalResult.Count - 50));
        
        var response = new PuzzleSolverResponse()
        {
            Data = finalResult
        };
        
        return Ok(response);
    }
}