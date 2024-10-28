
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectBase.Helpers;
using ProjectBase.Models;
using System;
using System.Threading.Tasks;

public class SubjectRegister : Controller
{
    private readonly DataContext _context;

    public SubjectRegister(DataContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> Register(int subjectId, int userId, int selectedPackage, int packageId)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        Console.WriteLine($"subjectId: {subjectId}, userId: {userId}, selectedPackage: {selectedPackage}");

        var buyAt = DateTime.Now;
        DateTime endAt;

        switch (selectedPackage)
        {
            case 1:
                endAt = buyAt.AddMonths(3);
                break;
            case 2:
                endAt = buyAt.AddMonths(6);
                break;
            case 3:
                endAt = buyAt.AddMonths(12);
                break;
            default:
                return BadRequest("Invalid package selection.");
        }

        var existingRecipe = await _context.Recipe
            .FirstOrDefaultAsync(r => r.UserID == userId && r.SubjectID == subjectId);

        if (existingRecipe != null)
        {
            // Update existing recipe
            existingRecipe.BuyAt = buyAt;
            existingRecipe.EndAt = endAt;
            existingRecipe.PricePackage_ID = packageId;
            existingRecipe.PricePackage_Type = selectedPackage;
            existingRecipe.Status = "Submitted"; // or keep it "Summited" if you prefer
        }
        else
        {
            // Create new recipe
            var recipe = new RecipeModel
            {
                UserID = userId,
                BuyAt = buyAt,
                EndAt = endAt,
                PricePackage_ID = packageId,
                PricePackage_Type = selectedPackage,
                Status = "Submitted",
                SubjectID = subjectId,
            };

            _context.Recipe.Add(recipe);
        }

        try
        {
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "MyRegistrations");
        }
        catch (Exception ex)
        {
            // Log the exception or handle it appropriately
            Console.WriteLine($"Error saving to database: {ex.Message}");
            return StatusCode(500, "Internal server error");
        }
    }


}
