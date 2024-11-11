using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddDbContext<TodoDb>(opt => opt.UseInMemoryDatabase("TodoList"));
var app = builder.Build();


app.UseMiddleware<ExternalModleHandlerMiddleware>();


app.MapGet("/", () => "Sample ToDo App");

app.MapGet("/todoitems", async (TodoDb db) =>
    await db.Todos.Select(x => new TodoItemDTO(x)).ToListAsync());

app.MapGet("/todoitems/{id}", async (int id, TodoDb db) =>
    await db.Todos.FindAsync(id)
        is Todo todo
            ? Results.Ok(new TodoItemDTO(todo))
            : Results.NotFound());

app.MapPost("/todoitems", async (TodoItemDTO todoItemDTO, TodoDb db) =>
{
    var todoItem = new Todo
    {
        IsComplete = todoItemDTO.IsComplete,
        Name = todoItemDTO.Name
    };

    db.Todos.Add(todoItem);
    await db.SaveChangesAsync();

    return Results.Created($"/todoitems/{todoItem.Id}", new TodoItemDTO(todoItem));
});


app.MapPut("/todoitems/{id}", async (int id, TodoItemDTO todoItemDTO, TodoDb db) =>
{
    var todo = await db.Todos.FindAsync(id);

    if (todo is null) return Results.NotFound();

    todo.Name = todoItemDTO.Name;
    todo.IsComplete = todoItemDTO.IsComplete;

    await db.SaveChangesAsync();

    return Results.NoContent();
});

app.MapDelete("/todoitems/{id}", async (int id, TodoDb db) =>
{
    if (await db.Todos.FindAsync(id) is Todo todo)
    {
        db.Todos.Remove(todo);
        await db.SaveChangesAsync();
        return Results.Ok(new TodoItemDTO(todo));
    }

    return Results.NotFound();
});

app.Run();

public class Todo
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public bool IsComplete { get; set; }
    public string? Secret { get; set; }
}

public class TodoItemDTO
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public bool IsComplete { get; set; }

    public TodoItemDTO() { }
    public TodoItemDTO(Todo todoItem) =>
    (Id, Name, IsComplete) = (todoItem.Id, todoItem.Name, todoItem.IsComplete);
}

public class TodoItemExternalModel
{
    public int Id { get; set; }
    public string? FullName { get; set; }
    public bool ComplitionStatus { get; set; }

    public TodoItemExternalModel(string fullName, bool complitionStatus) {
        FullName = fullName;
        ComplitionStatus = complitionStatus;
    }

}


class TodoDb : DbContext
{
    public TodoDb(DbContextOptions<TodoDb> options)
        : base(options) { }

    public DbSet<Todo> Todos => Set<Todo>();
}

 class ExternalModleHandlerMiddleware
{
    private readonly RequestDelegate _next;

    public ExternalModleHandlerMiddleware(RequestDelegate next)
    {
        _next = next;
    }

     public async Task InvokeAsync(HttpContext context, TodoDb db)
    {
        
        if (context.Request.Path == "/todoitems" && context.Request.Method == HttpMethods.Post)
        {
            // Let's check if recivied the external model. If so we will take care of the mapping 👌

            context.Request.EnableBuffering();


            var externalTodo = await context.Request.ReadFromJsonAsync<TodoItemExternalModel>();

            if (IsExternalModel(externalTodo))
            {
                // Map the external mode
                var todoItem = MapModel(externalTodo);


                db.Todos.Add(todoItem);
                await db.SaveChangesAsync();

                context.Response.StatusCode = StatusCodes.Status201Created;
                await context.Response.WriteAsJsonAsync(new TodoItemDTO(todoItem));
                return; 
            }
            context.Request.Body.Position = 0;

        }

        // If not an external model we continue business as usual: 
        await _next(context);
    }



    private Todo MapModel(TodoItemExternalModel todo)
    {
        return new Todo
        {
            Name = todo.FullName,
            IsComplete = todo.ComplitionStatus

        };
    }

    private bool IsExternalModel(TodoItemExternalModel? request)
    {
        if (request?.FullName == null)
        {
            return false;
        }
        return true;
    }
}

