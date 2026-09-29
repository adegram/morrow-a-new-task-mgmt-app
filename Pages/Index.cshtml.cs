using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Taskflow.Data;
using Taskflow.Models;

namespace Taskflow.Pages;

[Authorize]
public sealed class IndexModel(TaskStore store) : PageModel
{
    public List<TaskItem> Tasks { get; private set; } = [];
    public List<TaskItem> AllTasks { get; private set; } = [];
    public string ViewName { get; private set; } = "today";
    public string DisplayName => ViewName switch { "upcoming" => "Upcoming", "completed" => "Completed", "all" => "All tasks", _ => "Today" };
    public int OpenCount => AllTasks.Count(t => !t.IsCompleted);
    public int CompletedCount => AllTasks.Count(t => t.IsCompleted);
    public int DueTodayCount => AllTasks.Count(t => !t.IsCompleted && t.DueDate?.Date == DateTime.Today);
    public int OverdueCount => AllTasks.Count(t => !t.IsCompleted && t.DueDate?.Date < DateTime.Today);
    public int CompletionPercent => AllTasks.Count == 0 ? 0 : (int)Math.Round(CompletedCount * 100.0 / AllTasks.Count);
    public List<TaskItem> UpcomingTasks => AllTasks.Where(t => !t.IsCompleted && t.DueDate?.Date >= DateTime.Today).OrderBy(t => t.DueDate).Take(4).ToList();
    [TempData] public string? Notice { get; set; }

    public async Task OnGetAsync(string? view)
    {
        ViewName = ValidView(view);
        AllTasks = await store.GetAllAsync();
        Tasks = ViewName switch
        {
            "today" => AllTasks.Where(t => !t.IsCompleted && (t.DueDate?.Date <= DateTime.Today || t.DueDate is null)).ToList(),
            "upcoming" => AllTasks.Where(t => !t.IsCompleted && t.DueDate?.Date > DateTime.Today).ToList(),
            "completed" => AllTasks.Where(t => t.IsCompleted).ToList(),
            _ => AllTasks
        };
        ViewData["Title"] = DisplayName;
    }

    public async Task<IActionResult> OnPostCreateAsync(string title, string? notes, string category, string priority, DateTime? dueDate, string? view)
    {
        if (string.IsNullOrWhiteSpace(title) || title.Trim().Length > 180)
        {
            Notice = "Add a task title (up to 180 characters).";
            return RedirectToPage(new { view = ValidView(view) });
        }
        await store.AddAsync(title, notes, category, priority, dueDate);
        Notice = "Task added to your list.";
        return RedirectToPage(new { view = ValidView(view) });
    }

    public async Task<IActionResult> OnPostUpdateAsync(long id, string title, string? notes, string category, string priority, DateTime? dueDate, string? view)
    {
        if (id > 0 && !string.IsNullOrWhiteSpace(title) && title.Trim().Length <= 180)
        {
            await store.UpdateAsync(id, title, notes, category, priority, dueDate);
            Notice = "Your task has been updated.";
        }
        else Notice = "Please enter a task title of up to 180 characters.";
        return RedirectToPage(new { view = ValidView(view) });
    }

    public async Task<IActionResult> OnPostToggleAsync(long id, string? view)
    {
        if (id > 0) await store.ToggleAsync(id);
        return RedirectToPage(new { view = ValidView(view) });
    }

    public async Task<IActionResult> OnPostDeleteAsync(long id, string? view)
    {
        if (id > 0) { await store.DeleteAsync(id); Notice = "Task deleted."; }
        return RedirectToPage(new { view = ValidView(view) });
    }

    private static string ValidView(string? view) => view is "today" or "upcoming" or "all" or "completed" ? view : "today";
}
