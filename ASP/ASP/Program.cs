using System;
using System.Data;
using System.Data.SQLite;
using SQLiteExample;
using System.Threading;
var builder = WebApplication.CreateBuilder();
var app = builder.Build();
Sq met = new Sq();
app.Map("/a", async (context) =>
{
    if (context.Request.Cookies.ContainsKey("log"))
        context.Response.Redirect("/vhod");
    else
    {
        var form = context.Request.Form;
        Console.WriteLine("WORK");
        string n1 = form["n1"];
        string n2 = form["n2"];
        string vib = met.db_selector(n1, n2);
        if (vib == "true")
        {
            context.Response.Cookies.Append("log", n1);
            context.Response.Cookies.Append("pas", n2);
            await context.Response.WriteAsync($"<div><p>Login: {n1}</p><p>Thanks for reg</p></div>");
            met.createTable($"INSERT INTO Reg (login, password) VALUES (\"{n1}\",\"{n2}\")");
        }
        else if (vib == "Вы не ввели символы или написали логин или пароль раздельно")
        {
            await context.Response.WriteAsync($"<div><p>{ vib }</p></div>");
            await context.Response.SendFileAsync("html/about.html");
        }
        else if (vib == "Вы ввели слишком короткий логин или пароль, введите хотя-бы 6 символов")
        {
            await context.Response.WriteAsync($"<div><p>{ vib }</p></div>");
            await context.Response.SendFileAsync("html/about.html");
        }
        else if (vib == "Такой логин уже есть,выберите другой")
        {
            await context.Response.WriteAsync($"<div><p>{ vib }</p></div>");
            await context.Response.SendFileAsync("html/about.html");
        }
    }
});

app.Map("/vhod", async (context) =>
{
    if (context.Request.Cookies.ContainsKey("log"))
    {
        context.Response.Redirect("/main");
    }
    else
    {
        if (context.Request.Method == "GET")
        {
            await context.Response.SendFileAsync("html/vhod.html");
        }
        else if (context.Request.Method == "POST")
        {
            string logg;
            string pas;
            var form_vhod = context.Request.Form;
            logg = form_vhod["logg"];
            pas = form_vhod["pas"];
            bool vh = met.Vhod(logg, pas);
            if (vh == true)
            {
                context.Response.Cookies.Append("log", logg);
                context.Response.Cookies.Append("pas", pas);
                context.Response.Redirect("/main");
            }
            else
            {
                context.Response.Redirect("/vhod");
            }
        }
    }
});
app.Map("/del", async (context) =>
{
    string log = context.Request.Cookies["log"];
    string pas = context.Request.Form["pas"];
    string dl = met.Delete_acc(log, pas);
    if (dl == "no")
    {
        Console.WriteLine("Account delete Error");
        await context.Response.WriteAsync($"<div><p>Извините, не удалось удалить аккаунт попробуйте снова</p></div>");
    }
    else
    {
        met.createTable($"DELETE FROM Reg WHERE login = \'{ dl }\';");
        Console.WriteLine($"account { dl } delete");
    }
});
app.Map("/ismen", async (context) =>
{
    if (context.Request.Cookies.ContainsKey("log"))
    {
        if (context.Request.Method == "GET")
        {
            Console.WriteLine("a");
            await context.Response.SendFileAsync("html/smena.html");
        }
        else if (context.Request.Method == "POST")
        {
            var form_smen = context.Request.Form;
            string pas = form_smen["passw"];
            string log = context.Request.Cookies["log"];
            if (pas.Length > 5)
            {
                met.createTable($"UPDATE Reg SET password = \"{ pas }\" WHERE login = \"{ log }\";");
                context.Response.Redirect("/ismen");
            }
            else
                context.Response.Redirect("/ismen");
        }
    }
    else
    {
        context.Response.Redirect("/");
    }
});
app.Map("/", async (context) =>
{

    if (context.Request.Cookies.ContainsKey("log"))
    {
        context.Response.Redirect("/main");
    }
    else
        await context.Response.SendFileAsync("html/about.html");
});
app.Map("/main", async (context) =>
{
    if(context.Request.Cookies.ContainsKey("log"))
        await context.Response.SendFileAsync("html/htmlpage.html");
    else
        context.Response.Redirect("/");
});

app.Use(async (context, next) =>
{
    if (context.Request.Method == "GET")
    {
        context.Response.ContentType = "text/html; charset=utf-8";
    }
    await next.Invoke();
});

if (met.Connect("firstBase.db") == true)
{
    met.createTable("CREATE TABLE IF NOT EXISTS [Reg]([id] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL UNIQUE, [login] TEXT, [password] TEXT);");
    app.Run();
}