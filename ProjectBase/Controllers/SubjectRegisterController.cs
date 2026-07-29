
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectBase.Helpers;
using ProjectBase.Models;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

[Authorize]
public class SubjectRegister : Controller
{
    private readonly DataContext _context;

    public SubjectRegister(DataContext context)
    {
        _context = context;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(int subjectId, int userId, int selectedPackage, int packageId)
    {
        if (!long.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var currentUserId))
        {
            return Challenge();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        await using var transaction = _context.Database.IsRelational()
            ? await _context.Database.BeginTransactionAsync()
            : null;

        var pricePackage = await _context.Price_package
            .FirstOrDefaultAsync(package =>
                package.ID == packageId &&
                package.SubjectID == subjectId);
        if (pricePackage == null)
        {
            return BadRequest("The selected package is invalid for this subject.");
        }

        selectedPackage = checked((int)pricePackage.PackageType);
        var buyAt = DateTime.UtcNow;
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
            .FirstOrDefaultAsync(r => r.UserID == currentUserId && r.SubjectID == subjectId);

        if (existingRecipe != null)
        {
            if (existingRecipe.Status == RegistrationStatuses.Registered)
            {
                return Conflict(new
                {
                    success = false,
                    message = "A paid registration cannot change package."
                });
            }

            // Update existing recipe
            existingRecipe.BuyAt = buyAt;
            existingRecipe.EndAt = endAt;
            existingRecipe.PricePackage_ID = packageId;
            existingRecipe.PricePackage_Type = selectedPackage;
            existingRecipe.Status = RegistrationStatuses.Submitted;
        }
        else
        {
            // Create new recipe
            var recipe = new RecipeModel
            {
                UserID = currentUserId,
                BuyAt = buyAt,
                EndAt = endAt,
                PricePackage_ID = packageId,
                PricePackage_Type = selectedPackage,
                Status = RegistrationStatuses.Submitted,
                SubjectID = subjectId,
            };

            _context.Recipe.Add(recipe);
        }

        try
        {
            await _context.SaveChangesAsync();
            if (transaction != null)
            {
                await transaction.CommitAsync();
            }
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
